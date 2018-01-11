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
    partial class Program : MyGridProgram
    {
        public string[] hangerDoorGroupNames = { "Hangar Door (BAY-01)" };
        public string[] exteriorDoorNames = { };
        public string[] interiorDoorNames = { "Sliding Door (BAY-01)", "Sliding Door (CM03-RED)" };
        public string[] airVentToO2GenNames = { "Air Vent (BAY-01-GEN)" };
        public string[] airVentToO2TankNames = { "Air Vent (BAY-01-TANK)" };
        public string[] O2TankNames = { "Oxygen Tank (BAY-01)" };
        public string[] nameWarningLightsGroups = { "Warning Lights (BAY-01)" };
        public string[] nameTextPanels = { "LCD Panel (BAY-01) (1)" };
        public string[] nameWarningSounds = { "Sound Block (Bay-01)" };

        BlockNameConverter blockNameConverter;

        List<IMyAirVent> listAirVentsToGens = new List<IMyAirVent>();
        List<IMyAirVent> listAIrVentsToTanks = new List<IMyAirVent>();
        List<IMyGasTank> listAirTanks = new List<IMyGasTank>();
        List<IMyDoor> listDoorsIn = new List<IMyDoor>();
        List<IMyDoor> listDoorsOut = new List<IMyDoor>();
        List<IMyInteriorLight> listWarningLights = new List<IMyInteriorLight>();
        List<IMyTextPanel> listTextPanels = new List<IMyTextPanel>();
        List<IMySoundBlock> listWarningSounds = new List<IMySoundBlock>();

        StatusReport statusReport = new StatusReport();
        GasTanksManager airTanksManager;
        PressurizedAreaController airLockController;

        public Program()
        {
            blockNameConverter = new BlockNameConverter(GridTerminalSystem);

            blockNameConverter.AppendBlocksFromCustomNames(airVentToO2GenNames, listAirVentsToGens);
            blockNameConverter.AppendBlocksFromCustomNames(airVentToO2TankNames, listAIrVentsToTanks);
            blockNameConverter.AppendBlocksFromCustomNames(O2TankNames, listAirTanks);
            blockNameConverter.AppendBlocksFromCustomNames(exteriorDoorNames, listDoorsIn);
            blockNameConverter.AppendBlocksFromCustomNames(interiorDoorNames, listDoorsOut);
            blockNameConverter.AppendBlocksFromGroupNames(hangerDoorGroupNames, listDoorsOut);
            blockNameConverter.AppendBlocksFromGroupNames(nameWarningLightsGroups, listWarningLights);
            blockNameConverter.AppendBlocksFromCustomNames(nameTextPanels, listTextPanels);
            blockNameConverter.AppendBlocksFromCustomNames(nameWarningSounds, listWarningSounds);

            airTanksManager = new GasTanksManager(listAirTanks);
            airTanksManager.SetStatusReport(statusReport);
            statusReport.maxCharsPerLine = 40;

            airLockController = new PressurizedAreaController(listDoorsOut, listDoorsIn, listAIrVentsToTanks, listAirVentsToGens, airTanksManager, AlertDictionary(), statusReport);
            
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument == "depressurize")
            {
                airLockController.Depressurize();
            }
            else if (argument == "pressurize")
            {
                airLockController.Pressurize();
            }
            else if (argument == "lockdown")
            {
                airLockController.LockDown();
            }

            if (updateSource.HasFlag(UpdateType.Update100))
            {
                airLockController.CheckStatus();
                Echo(statusReport.RetrieveFullReportText());
                foreach (IMyTextPanel panel in listTextPanels)
                {
                    panel.WritePublicText(statusReport.RetrieveFullReportText(), true);
                }
                statusReport.Clear();
            }
        }

        Dictionary<PressurizedAreaController.AlertStatus, Alert> AlertDictionary()
        {
            Dictionary<PressurizedAreaController.AlertStatus, Alert> alertDictionary = new Dictionary<PressurizedAreaController.AlertStatus, Alert>();

            List<AlertObject>[] listAlertObjects= new List<AlertObject>[PressurizedAreaController.AlertStatusCount];

            for (PressurizedAreaController.AlertStatus alertStatus = 0; (int)alertStatus < PressurizedAreaController.AlertStatusCount; alertStatus++)
            {
                listAlertObjects[(int)alertStatus] = new List<AlertObject>();
            }

            for (int indexLight = 0; indexLight < listWarningLights.Count; indexLight++)
            {
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.NONE].Add(new AlertLight(listWarningLights[indexLight], Color.White, false));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.CYCLING].Add(new AlertLight(listWarningLights[indexLight], Color.Orange, true));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.DEPRESSURIZED].Add(new AlertLight(listWarningLights[indexLight], Color.Red, false));
            }

            for (int indexText = 0; indexText < listTextPanels.Count; indexText++)
            {
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.NONE].Add(new AlertText(listTextPanels[indexText], "Area Pressurized.\n", 1f, Color.White, Color.Blue));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.CYCLING].Add(new AlertText(listTextPanels[indexText], "Securing Area!\n", 1f, Color.Orange, Color.Black));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.DEPRESSURIZED].Add(new AlertText(listTextPanels[indexText], "Area\nDepressurized!\n", 1f, Color.Cyan, Color.Red));
            }

            for (int indexSound = 0; indexSound < listWarningSounds.Count; indexSound++)
            {
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.NONE].Add(new AlertSound(null));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.CYCLING].Add(new AlertSound(listWarningSounds[indexSound]));
                listAlertObjects[(int)PressurizedAreaController.AlertStatus.DEPRESSURIZED].Add(new AlertSound(null));
            }

            for (PressurizedAreaController.AlertStatus alertStatus = 0; (int)alertStatus < PressurizedAreaController.AlertStatusCount; alertStatus++)
            {
                alertDictionary.Add(alertStatus, new Alert(listAlertObjects[(int)alertStatus]));
            }

            return alertDictionary;
        }
    }
}