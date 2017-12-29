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
        string antennaName = "Antenna";

        IMyRadioAntenna antenna;

        public Program()
        {
            antenna = (IMyRadioAntenna)GridTerminalSystem.GetBlockWithName(antennaName);
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType source)
        {
            if ((source & UpdateType.Terminal) != 0)
            {
                string[] words = argument.Split(' ');

                if (words[0] == "transmit")
                {
                    antenna.TransmitMessage(argument.Substring("transmit ".Length));
                }
            }
        }
    }
}