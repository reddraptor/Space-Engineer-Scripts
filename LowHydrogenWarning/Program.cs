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
        public List<string> nameHydrogenTanks = new List<string> { "Hydrogen Tank Small" };
        public string nameSoundBlock = "Sound Block";
        public string nameLCDPanel = "Corner LCD Top";
        public string nameWarningLight = "Warning Light";

        public float warningLevel = 0.95f;

        private LowHydrogenWarningSystem lowH2WarningSystem;

        public Program()
        {
            List<IMyGasTank> listH2Tanks = new List<IMyGasTank>();

            foreach (string nameH2Tank in nameHydrogenTanks)
            {
                listH2Tanks.Add((IMyGasTank)GridTerminalSystem.GetBlockWithName(nameH2Tank));
            }
            IMyInteriorLight warningLight = (IMyInteriorLight)GridTerminalSystem.GetBlockWithName(nameWarningLight);
            IMyTextPanel warningPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(nameLCDPanel);
            IMySoundBlock warningSoundBlock = (IMySoundBlock)GridTerminalSystem.GetBlockWithName(nameSoundBlock);

            lowH2WarningSystem = new LowHydrogenWarningSystem(listH2Tanks, warningLight, warningPanel, warningSoundBlock){ warningLevel = warningLevel };

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update100) != 0)
            {
                Echo(lowH2WarningSystem.Enabled.ToString());
                Echo(lowH2WarningSystem.GasTanksManager.FilledRatio.ToString());
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