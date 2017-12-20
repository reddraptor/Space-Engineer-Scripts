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
    /// Features: Low Hydrogen Warning System
    /// Requirements: LowHydrogenWarningSystem, AlertSystem, GasTanksManager
    /// </summary>
    partial class Program : MyGridProgram
    {
        public string nameHydrogenTank = "Hydrogen Tank Small";
        public string nameLCDPanel = "Corner LCD Top";
        public string nameWarningLight = "Warning Light";

        public float h2WarningLevel = 0.25f;

        private LowHydrogenWarningSystem lowH2WarningSystem;

        public Program()
        {
            List<IMyGasTank> listH2Tanks = new List<IMyGasTank> { (IMyGasTank)GridTerminalSystem.GetBlockWithName(nameHydrogenTank) };
            IMyInteriorLight warningLight = (IMyInteriorLight)GridTerminalSystem.GetBlockWithName(nameWarningLight);
            IMyTextPanel warningPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(nameLCDPanel);

            lowH2WarningSystem = new LowHydrogenWarningSystem(listH2Tanks, warningLight, warningPanel, null){ warningLevel = h2WarningLevel };

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update100) != 0)
            {
                Echo("Hydrogen Level: " + lowH2WarningSystem.GasTanksManager.FilledRatio * 100 + "%");
                lowH2WarningSystem.CheckLevels();
            }
            else if ((updateSource & UpdateType.Terminal) != 0)
            {
                if (argument == "test 1")
                { 
                    lowH2WarningSystem.Enabled = true;
                }
                else if (argument == "test 0")
                {
                    lowH2WarningSystem.Enabled = false;
                }
            }
        }
    }
}