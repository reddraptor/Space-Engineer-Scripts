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
        string[] nameAirVentsToGens = { "Air Vent (AL-00-GEN)" };
        string[] nameAirVentsToTanks = { "Air Vent (AL-00-TANK)" };
        string[] nameAirTanks = { "Oxygen tank (AL-00)" };
        string[] nameDoorsIn = { "Sliding Door (AL-00-A)" };
        string[] nameDoorsOut = { "Sliding Door (AL-00-B)" };
        string[] nameWarningLights = { "Interior Light (AL-00)" };
        string[] nameTextPanels = { "Text panel (AL-00)" };

        BlockNameConverter blockNameConverter;

        List<IMyAirVent> listAirVentsToGens = new List<IMyAirVent>();
        List<IMyAirVent> listAIrVentsToTanks = new List<IMyAirVent>();
        List<IMyGasTank> listAirTanks = new List<IMyGasTank>();
        List<IMyDoor> listDoorsIn = new List<IMyDoor>();
        List<IMyDoor> listDoorsOut = new List<IMyDoor>();
        IMyInteriorLight warningLight;
        IMyTextPanel textPanel;

        StatusReport statusReport = new StatusReport();
        GasTanksManager airTanksManager;
        PressurizedAreaController airLockController;

        AlertLight alertLight;
        AlertText alertText;

        AlertSystemManager alertSystemManager;
        

        public Program()
        {
            blockNameConverter = new BlockNameConverter(GridTerminalSystem);

            blockNameConverter.AppendBlocksFromCustomNames(nameAirVentsToGens, listAirVentsToGens);
            blockNameConverter.AppendBlocksFromCustomNames(nameAirVentsToTanks, listAIrVentsToTanks);
            blockNameConverter.AppendBlocksFromCustomNames(nameAirTanks, listAirTanks);
            blockNameConverter.AppendBlocksFromCustomNames(nameDoorsIn, listDoorsIn);
            blockNameConverter.AppendBlocksFromCustomNames(nameDoorsOut, listDoorsOut);

            airTanksManager = new GasTanksManager(listAirTanks);
            airTanksManager.SetStatusReport(statusReport);

            alertLight = new AlertLight(warningLight);
            alertText = new AlertText(textPanel, "default");
            
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked.
            // 
            // The method itself is required, but the argument above
            // can be removed if not needed.
        }
    }
}