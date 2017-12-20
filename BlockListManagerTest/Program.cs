﻿using Sandbox.Game.EntityComponents;
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
        string[] doorNames = { "Sliding Door (CM03-RED)", "Sliding Door (BAY-01)" };
        string[] doorGroupNames = { "Hanger Door (BAY-01)", "Hanger Door (TEST)" };

        List<IMyDoor> doorList = new List<IMyDoor>();
        BlockListManager blockListManager;

        public void Main(string argument)
        {
            blockListManager = new BlockListManager(GridTerminalSystem);

            blockListManager.AppendBlocksWithNames(doorNames, doorList);

            foreach (IMyDoor door in doorList)
            {
                Echo(door.CustomName);
            }

            blockListManager.AppendBlocksFromGroupsWithNames(doorGroupNames, doorList);
            foreach (IMyDoor door in doorList)
            {
                Echo(door.CustomName);

            }
        }
    }
}