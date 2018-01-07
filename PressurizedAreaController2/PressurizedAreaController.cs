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
            [Flags]public enum PressurizedAreaStatus {  NONE            = 0,
                                                        PRESSURIZE      = 1 << 0,
                                                        CYCLING         = 1 << 1,
                                                        SECURING        = 1 << 2 }

            public static int AlertStatusCount = 3;
            public enum AlertStatus { NONE, CYCLING, DEPRESSURIZED }

            protected Area area;
            protected AlertSystemManager<AlertStatus> alertSystemManager;

            protected PressurizedAreaStatus status = PressurizedAreaStatus.NONE;

            /// <summary>
            /// Acceptable nearness to target pressures to complete a cycle.
            /// </summary>
            public float pressureTolerence = 0.01f;

            public PressurizedAreaStatus Status
            {
               get { return status; }
            }

            public bool AreaSecured
            { 
                get
                {
                    if (area == null)
                    {
                        ReportItem("Area is null. Ensure controller has a valid area and recompile.", StatusReport.Type.ERROR);
                        return false;
                    }

                    for (int i = 0; i < area.AccessPointCount; i++)
                    {
                        if (area.AccessPoint(i) == null)
                        {
                            ReportItem("AccessPoint is null. Ensure area has valid access points and recompile.", StatusReport.Type.ERROR);
                            return false;
                        }
                        if (!area.AccessPoint(i).IsSecured) return false;
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

                if (area == null)
                {
                    ReportItem("Area is null. Ensure controller has a valid area and recompile.", StatusReport.Type.ERROR);
                    return -1f;
                }

                for (int i = 0; i < area.SourceAirVentCount; i++)
                {
                    IMyAirVent sourceAirVent = area.SourceAirVent(i);

                    if (ValidateO2Level(sourceAirVent))
                    {
                        o2LevelSum += sourceAirVent.GetOxygenLevel();
                        ventCount += 1; 
                    }
                }

                for (int i = 0; i < area.StorageAirVentCount; i++)
                {
                    IMyAirVent storageAirVent = area.StorageAirVent(i);

                    if (ValidateO2Level(storageAirVent))
                    {
                        o2LevelSum += storageAirVent.GetOxygenLevel();
                        ventCount += 1;
                    }
                }

                return o2LevelSum / ventCount;
            }

            protected bool ValidateO2Level(IMyAirVent airVent)
            {
                if (airVent == null)
                {
                    ReportItem("Source air vent is null. Can not report oxygen level.", StatusReport.Type.ERROR);
                    return false;
                }
                else if (!airVent.IsFunctional)
                {
                    ReportItem("Source air vent nonfunctional. Can not report oxygen level.", StatusReport.Type.ERROR);
                    return false;
                }
                else if (!airVent.Enabled)
                {
                    ReportItem("Source air vent disabled. Can not report oxygen level.", StatusReport.Type.ERROR);
                    return false;
                }
                else return true;
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
            public PressurizedAreaController(Area area, Dictionary<AlertStatus, Alert> alertDictionary = null, StatusReport statusReport = null)
            {
                this.area = area;
                if (alertDictionary != null) alertSystemManager = new AlertSystemManager<AlertStatus>(alertDictionary);
                SetStatusReport(statusReport);
                Pressurize();
            }

            public bool OpenAccess(int index)
            {

            }

            public void Pressurize()
            {
                if (status.HasFlag(PressurizedAreaStatus.PRESSURIZE)) return;
                status |= (PressurizedAreaStatus.PRESSURIZE | PressurizedAreaStatus.CYCLING | PressurizedAreaStatus.SECURING);
                if (alertSystemManager != null)
                {
                    alertSystemManager.DisableAlerts();
                    alertSystemManager.Alerts[AlertStatus.CYCLING].Enabled = true;
                }
                CheckStatus();
            }

            public void Depressurize()

            {
                if (!status.HasFlag(PressurizedAreaStatus.PRESSURIZE)) return;
                status &= ~PressurizedAreaStatus.PRESSURIZE;
                status |= (PressurizedAreaStatus.CYCLING | PressurizedAreaStatus.SECURING);
                if (alertSystemManager != null)
                {
                    alertSystemManager.DisableAlerts();
                    alertSystemManager.Alerts[AlertStatus.CYCLING].Enabled = true;
                }
                EnableVents(listVentsToO2Gens, false);
                CheckStatus();
            }
            
            /// <summary>
            /// Many functions of the area controller take multiply ticks to complete, like waiting for doors to open/close or venting rooms. Each run of the programmable block needs to
            /// CheckStatus() to complete theses functions.
            /// </summary>
            public void CheckStatus() {

                if (!status.HasFlag(PressurizedAreaStatus.PRESSURIZE) && status.HasFlag(PressurizedAreaStatus.CYCLING))
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
                else if (status.HasFlag(PressurizedAreaStatus.PRESSURIZE | PressurizedAreaStatus.CYCLING))
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
                status &= ~PressurizedAreaStatus.CYCLING;
                if (alertSystemManager != null)
                {
                    alertSystemManager.DisableAlerts();
                    alertSystemManager.Alerts[AlertStatus.NONE].Enabled = true;
                }
                EnableVents(listVentsToO2Gens);
                OpenDoors(listInteriorDoors);
                ReportItem("All vents enabled. Opening interior doors. Pressurized.");
            }

            protected void GoToDepressurizedStatus()
            {
                status &= ~PressurizedAreaStatus.CYCLING;
                if (alertSystemManager != null)
                {
                    alertSystemManager.DisableAlerts();
                    alertSystemManager.Alerts[AlertStatus.DEPRESSURIZED].Enabled = true;
                }
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