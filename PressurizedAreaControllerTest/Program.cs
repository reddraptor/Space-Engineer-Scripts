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
        string[] hangerDoorGroupNames = {"Hanger Doors"};
        string[] exteriorDoorNames = {"Sliding Door (EXT)"};
        string[] interiorDoorNames = {"Door (INT)"};
        string[] airVentToO2GenNames = {"Air Vent (GEN)"};
        string[] airVentToO2TankNames = {"Air Vent (TANK)"};
        string[] O2TankNames = {"Oxygen Tank"};

        BlockNameConverter blockListManager;
        StatusReport statusReport = new StatusReport();

        List<IMyDoor> exteriorDoorList = new List<IMyDoor>();
        List<IMyDoor> interiorDoorList = new List<IMyDoor>();
        List<IMyAirVent> airVentToO2GenList = new List<IMyAirVent>();
        List<IMyAirVent> airVentToO2TankList = new List<IMyAirVent>();
        List<IMyGasTank> o2TankList = new List<IMyGasTank>();

        PressurizedAreaController hangerPressureController;
        GasTanksManager gasTanksManager;

        public Program()
        {
            blockListManager = new BlockNameConverter(GridTerminalSystem);

            blockListManager.AppendBlocksFromCustomNames(exteriorDoorNames, exteriorDoorList);
            blockListManager.AppendBlocksFromGroupNames(hangerDoorGroupNames, exteriorDoorList);
            blockListManager.AppendBlocksFromCustomNames(interiorDoorNames, interiorDoorList);
            blockListManager.AppendBlocksFromCustomNames(airVentToO2GenNames, airVentToO2GenList);
            blockListManager.AppendBlocksFromCustomNames(airVentToO2TankNames, airVentToO2TankList);
            blockListManager.AppendBlocksFromCustomNames(O2TankNames, o2TankList);

            gasTanksManager = new GasTanksManager(o2TankList);
            gasTanksManager.SetStatusReport(statusReport);
            hangerPressureController = new PressurizedAreaController(exteriorDoorList, interiorDoorList, airVentToO2TankList, airVentToO2GenList, gasTanksManager, null, statusReport);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument == "depressurize")
            {
                hangerPressureController.Depressurize();
            }
            else if (argument == "pressurize")
            {
                hangerPressureController.Pressurize();
            }
            else if (argument == "lockdown") {
                hangerPressureController.LockDown();
            }

            if ((updateSource & UpdateType.Update100) != 0)
            {
                hangerPressureController.CheckStatus();
                Echo(statusReport.RetrieveFullReportText());
                statusReport.Clear();
            }
        }
    }
}