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
    /// <summary>
    /// SE 'Hercules-H' Ship Script
    /// Features: Low Hydrogen Warning System, Transmit Commands over Antenna
    /// Requirements: LowHydrogenWarningSystem, AlertSystem, GasTanksManager
    /// </summary>
    partial class Program : MyGridProgram
    {
        public string nameHydrogenTank = "Hydrogen Tank Small";
        public string nameLCDPanel = "Corner LCD Top";
        public string nameWarningLight = "Warning Light";
        public string nameAntenna = "Antenna (HH-02)";

        public float h2WarningLevel = 0.25f;

        private LowHydrogenWarningSystem lowH2WarningSystem;
        private IMyRadioAntenna antenna;

        public Program()
        {
            List<IMyGasTank> listH2Tanks = new List<IMyGasTank> { (IMyGasTank)GridTerminalSystem.GetBlockWithName(nameHydrogenTank) };
            IMyInteriorLight warningLight = (IMyInteriorLight)GridTerminalSystem.GetBlockWithName(nameWarningLight);
            IMyTextPanel warningPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(nameLCDPanel);

            lowH2WarningSystem = new LowHydrogenWarningSystem(listH2Tanks, warningLight, warningPanel, null){ warningLevel = h2WarningLevel };
            antenna = (IMyRadioAntenna)GridTerminalSystem.GetBlockWithName(nameAntenna);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update100) != 0)
            {
                Echo("Hydrogen Level: " + lowH2WarningSystem.GasTanksManager.FilledRatio * 100 + "%");
                lowH2WarningSystem.CheckLevels();
            }
            else if ((updateSource & UpdateType.Terminal) != 0 ||
                (updateSource & UpdateType.Trigger) != 0)
            {
                string[] words = argument.Split(' ');

                if (argument == "test 1")
                { 
                    lowH2WarningSystem.Enabled = true;
                }
                else if (argument == "test 0")
                {
                    lowH2WarningSystem.Enabled = false;
                }
                else if (words[0] == "transmit")
                {
                    string message = argument.Substring("transmit ".Length);
                    Echo("Transmitting: " + message);
                    antenna.TransmitMessage(message);
                }
            }
        }
    }
}