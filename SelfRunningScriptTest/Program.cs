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
        public string nameLight = "Corner Light";

        private IMyInteriorLight blockLight;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            IMyTerminalBlock block = GridTerminalSystem.GetBlockWithName(nameLight);
            if (block is IMyInteriorLight) blockLight = (IMyInteriorLight)block;

            blockLight.Enabled = !blockLight.Enabled;
            Echo(updateSource.ToString());
        }
    }
}