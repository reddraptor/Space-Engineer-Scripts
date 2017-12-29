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
        string bay01ProgramBlockName = "Programmable Block (BAY-01)";

        IMyProgrammableBlock bay01ProgramBlock;

        public Program()
        {
            bay01ProgramBlock = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName(bay01ProgramBlockName);
        }
        
        public void Main(string argument, UpdateType source)
        {
            if ((source & UpdateType.Antenna) != 0)
            {
                if (argument == "open bay01")
                {
                    bay01ProgramBlock.TryRun("depressurize");
                }
                else if (argument == "close bay01")
                {
                    bay01ProgramBlock.TryRun("pressurize");
                }
            }
        }
    }
}