using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Pressurized Area Controller v0.01
        /// Requires: GasTankManager, AlertSystemManager, StatusReport, StatusReporter
        /// </summary>
        public class PressurizedAreaController : StatusReporter
        {
            public enum PressurizedAreaStatus { PRESSURIZED, DEPRESSURIZING, DEPRESSURIZED, PRESSURIZING, SECURING }

            protected List<IMyDoor> listExteriorDoors;
            protected List<IMyDoor> listInteriorDoors;
            protected List<IMyAirVent> listVentsToO2Tanks;
            protected List<IMyAirVent> listVentsToO2Gens;

            protected GasTanksManager o2TanksManager;
            protected AlertSystemManager alertSystemManager;

            protected PressurizedAreaStatus status = PressurizedAreaStatus.DEPRESSURIZED;

            /// <summary>
            /// Acceptable nearness to target pressures to complete a cycle.
            /// </summary>
            public float pressureTolerence = 0.01f;

            public PressurizedAreaStatus Status
            {
               get { return status; }
            }

            public bool AreaDoorsSecured
            { 
                get
                {
                    if (listInteriorDoors == null) ReportItem("Not a valid (INT) door list. Setup valid (INT) door list and recompile.", StatusReport.Type.ERROR);
                    else if (listExteriorDoors == null) ReportItem("Not a valid (EXT) door list. Setup valid (EXT) door list and recompile.", StatusReport.Type.ERROR);

                    foreach (IMyDoor door in listInteriorDoors)
                    {
                        if (door == null || !door.IsFunctional) ReportItem("Missing or nonfunctional (INT) door. Area may not be securable. Check doors and recompile.", StatusReport.Type.WARNING);
                        else if (door.Status != DoorStatus.Closed || door.Enabled) return false;
                    }

                    foreach (IMyDoor door in listExteriorDoors)
                    {
                        if (door == null || !door.IsFunctional) ReportItem("Missing or nonfunctional (EXT) door. Area may not be securable. Check doors and recompile.", StatusReport.Type.WARNING);
                        else if (door.Status != DoorStatus.Closed || door.Enabled) return false;
                    }
                    
                    return true;
                }
            }

            /// <summary>
            /// Returns the average oxygen level reported by all enabled vents.
            /// </summary>
            public float AreaOxygenLevel()
            {
                float o2LevelSum = 0;
                float ventCount = 0;

                if (listVentsToO2Gens == null)
                {
                    ReportItem("Not a valid air vent (GENS) list. Setup valid air vent (GENS) list and recompile.", StatusReport.Type.ERROR);
                    return 0;
                }
                else if (listVentsToO2Tanks == null)
                {
                    ReportItem("Not a valid air vent (TANKS) list. Setup valid air vent (TANKS) list and recompile.", StatusReport.Type.ERROR);
                    return 0;
                }
                else
                {
                    foreach (IMyAirVent airVent in listVentsToO2Gens)
                    {
                        if (airVent == null || !airVent.IsFunctional)
                            ReportItem("Missing or nonfunctional (GENS) air vent. Not included in area oxygen calculations.", StatusReport.Type.WARNING);
                        else if (!airVent.Enabled)
                            ReportItem("(GENS) air vent disabled. Not included in area oxygen calculations.", StatusReport.Type.WARNING);
                        else
                        {
                            o2LevelSum += airVent.GetOxygenLevel();
                            ventCount += 1;
                        }

                    }

                    foreach (IMyAirVent airVent in listVentsToO2Tanks)
                    {
                        if (airVent == null || !airVent.IsFunctional)
                            ReportItem("Missing or nonfunctional (TANKS) air vent. Not included in area oxygen calculations.", StatusReport.Type.WARNING);
                        else if (!airVent.Enabled)
                            ReportItem("(TANKS) air vent disabled. Not included in area oxygen calculations.", StatusReport.Type.WARNING);
                        else
                        {
                            o2LevelSum += airVent.GetOxygenLevel();
                            ventCount += 1;
                        }
                    }

                    return o2LevelSum / ventCount;
                }
            }

            /// <summary>
            /// PressurizedAreaController constructor.
            /// </summary>
            /// <param name="listExteriorDoors">Doors to non-pressurized area.</param>
            /// <param name="listInteriorDoors">Doors to pressurized area.</param>
            /// <param name="listVentsToO2Tanks">Vents to oxygen storage tanks.</param>
            /// <param name="listVentsToO2Gens">Vents to oxygen generators.</param>
            /// <param name="o2TanksManager">Oxygen tank manager.</param>
            /// <param name="alertSystemManager">Alert system manager.</param>
            public PressurizedAreaController(
                List<IMyDoor> listExteriorDoors, List<IMyDoor> listInteriorDoors,
                List<IMyAirVent> listVentsToO2Tanks, List<IMyAirVent> listVentsToO2Gens,
                GasTanksManager o2TanksManager = null, AlertSystemManager alertSystemManager = null, StatusReport statusReport = null)
            {
                this.listExteriorDoors = listExteriorDoors;
                this.listInteriorDoors = listInteriorDoors;
                this.listVentsToO2Tanks = listVentsToO2Tanks;
                this.listVentsToO2Gens = listVentsToO2Gens;
                this.o2TanksManager = o2TanksManager;
                this.alertSystemManager = alertSystemManager;
                SetStatusReport(statusReport);
                Pressurize();
            }

            public void Pressurize()
            {
                if (status == PressurizedAreaStatus.PRESSURIZED || status == PressurizedAreaStatus.PRESSURIZING) return;
                status = PressurizedAreaStatus.PRESSURIZING;
                CheckStatus();
            }

            public void Depressurize()

            {
                if (status == PressurizedAreaStatus.DEPRESSURIZED || status == PressurizedAreaStatus.DEPRESSURIZING) return;
                status = PressurizedAreaStatus.DEPRESSURIZING;
                EnableVents(listVentsToO2Gens, false);
                CheckStatus();
            }
            
            /// <summary>
            /// Many functions of the area controller take multiply ticks to complete, like waiting for doors to open/close or venting rooms. Each run of the programmable block needs to
            /// CheckStatus() to complete theses functions.
            /// </summary>
            public void CheckStatus() {

                if (status == PressurizedAreaStatus.DEPRESSURIZING)
                {
                    ReportItem("Securing Doors... ");
                    if (SecureDoors(listInteriorDoors) && SecureDoors(listExteriorDoors))
                    {
                        float areaO2Level = AreaOxygenLevel();

                        ReportItem("Secured. \nDepressurizing... " + areaO2Level * 100 + "%\n");
                        SetDepressurize(listVentsToO2Tanks);

                        if (areaO2Level <= 0f + pressureTolerence) // Oxygen level is near zero.
                        {
                            GoToDepressurizedStatus();
                        }
                        else if (o2TanksManager == null)
                        {
                            ReportItem("No O2 tanks manager, venting to non-pressurized environement!", StatusReport.Type.WARNING);
                            GoToDepressurizedStatus();
                        }
                        else if (o2TanksManager.FilledRatio == 1f) // 02 tanks are full
                        {
                            ReportItem("Tanks full, venting to non-pressurized environment!", StatusReport.Type.WARNING);
                            GoToDepressurizedStatus();
                        }
                    }
                }
                else if (status == PressurizedAreaStatus.PRESSURIZING)
                {
                    ReportItem("Securing Doors... ");
                    if (SecureDoors(listInteriorDoors) && SecureDoors(listExteriorDoors))
                    {
                        float areaO2Level = AreaOxygenLevel();

                        ReportItem("Secured. \nPressurizing... " + areaO2Level * 100 + "%\n");
                        SetDepressurize(listVentsToO2Tanks, false);

                        if (areaO2Level >= 1f - pressureTolerence) // Oxygen level is near 100%.
                        {
                            GoToPressurizedStatus();
                        }
                        else if (o2TanksManager == null)
                        {
                            ReportItem("No 02 tanks manager, opening to pressurized environment.", StatusReport.Type.WARNING);
                                GoToPressurizedStatus();
                        }
                        else if (o2TanksManager.FilledRatio == 0f) // 02 tanks are empty
                        {
                            ReportItem("Tanks are empty, opening to pressurized environment.", StatusReport.Type.WARNING);
                            GoToPressurizedStatus();
                        }
                    }
                }
            }

            public void LockDown()
            {
                throw new Exception("Not Implemented.");
            }


            protected void GoToPressurizedStatus()
            {
                status = PressurizedAreaStatus.PRESSURIZED;
                EnableVents(listVentsToO2Gens);
                OpenDoors(listInteriorDoors);
                ReportItem("All vents enabled. Opening interior doors. Pressurized.");
            }

            protected void GoToDepressurizedStatus()
            {
                status = PressurizedAreaStatus.DEPRESSURIZED;
                OpenDoors(listExteriorDoors);
                ReportItem("Opening exterior doors. Depressurized.");
            }

            protected bool SecureDoors(List<IMyDoor> listDoors)
            {
                if (listDoors == null)
                {
                    ReportItem("Not a valid door list. Doors can not be secured. Setup valid door lists and recompile.", StatusReport.Type.ERROR);
                    return false;
                }
                else
                {
                    bool doorsSecured = true;

                    foreach (IMyDoor door in listDoors)
                    {
                        if (door == null || !door.IsFunctional)
                        {
                            ReportItem("Missing or nonfunctioning door. Doors can not be secured. Check doors and recompile.", StatusReport.Type.ERROR);
                            return false;
                        } 
                        else if (door.Status == DoorStatus.Closed) door.Enabled = false;
                        else
                        {
                            if (!door.Enabled) door.Enabled = true;
                            door.CloseDoor();
                            doorsSecured = false;
                        }
                    }
                    return doorsSecured;
                }
            }

            protected void OpenDoors(List<IMyDoor> listDoors, bool setToOpen = true)
            {
                if (listDoors == null)
                {
                    ReportItem("Not a valid door list. Doors inoperable. Setup valid door lists and recompile.", StatusReport.Type.ERROR);
                    return;
                }

                foreach (IMyDoor door in listDoors)
                {
                    if (door == null || !door.IsFunctional)
                        ReportItem("Missing or nonfunctioning door. Door inoperable.", StatusReport.Type.WARNING);
                    else if (setToOpen)
                    {
                        door.Enabled = true;
                        door.OpenDoor();
                    }
                    else if (!setToOpen)
                    {
                        door.Enabled = true;
                        door.CloseDoor();
                    }
                }
            }

            protected void EnableVents(List<IMyAirVent> listAirVents, bool setToEnable = true)
            {
                if (listAirVents == null)
                {
                    ReportItem("Not a valid air vent list. Air vents inoperable. Setup valid air vent lists and recompile.", StatusReport.Type.ERROR);
                    return;
                }

                foreach (IMyAirVent airVent in listAirVents)
                {
                    if (airVent == null || !airVent.IsFunctional)
                        ReportItem("Missing or nonfunctioning air vent. Vent inoperable.", StatusReport.Type.WARNING);
                    else
                        airVent.Enabled = setToEnable;
                }
            }

            protected void SetDepressurize(List<IMyAirVent> listAirVents, bool setToDepressurize = true)
            {
                if (listAirVents == null)
                {
                    ReportItem("Not a valid air vent list. Air vents inoperable. Setup valid air vent lists and recompile.", StatusReport.Type.ERROR);
                    return;
                }

                foreach (IMyAirVent airVent in listAirVents)
                {
                    if (airVent == null || !airVent.IsFunctional)
                        ReportItem("Missing or nonfunctioning air vent. Vent inoperable.", StatusReport.Type.WARNING);
                    else
                    {
                        airVent.Enabled = true;
                        airVent.Depressurize = setToDepressurize;
                    }
                }
            }
        }
    }
}