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
    partial class Program
    {
        public class NameFindAndReplace
        {
            IMyGridTerminalSystem gridTerminalSystem;
            
            public Exception NullGridException
            {
                get { return new Exception("NameFindAndReplace: Grid terminal system is null."); }
            }

            public Exception BlockNotFoundException
            {
                get { return new Exception("NameFindAndReplace: Block not found."); }
            }

            public Exception GroupNotFoundException
            {
                get { return new Exception("NameFindAndReplace: Group not found."); }
            }
                
            public NameFindAndReplace(IMyGridTerminalSystem gridTerminalSystem)
            {
                if (gridTerminalSystem == null) throw NullGridException;
                this.gridTerminalSystem = gridTerminalSystem;
            }
            
            public void ReplaceInAll(string target, string replacement)
            {
                List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();
                gridTerminalSystem.GetBlocks(blockList);
                Replace(target, replacement, blockList);
            }

            public void Replace(string target, string replacement, string blockName)
            {
                Replace(target, replacement, gridTerminalSystem.GetBlockWithName(blockName));
            }

            public void Replace(string target, string replacement, IMyTerminalBlock block)
            {
                if (block == null) throw BlockNotFoundException;
                block.CustomName = block.CustomName.Replace(target, replacement);
            }

            public void Replace(string target, string replacement, List<IMyTerminalBlock> blockList)
            {
                foreach (IMyTerminalBlock block in blockList)
                    Replace(target, replacement, block);
            }

            public void ReplaceInGroup(string target, string replacement, IMyBlockGroup blockGroup)
            {
                if (blockGroup == null) throw GroupNotFoundException;
                List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();
                blockGroup.GetBlocks(blockList);
                Replace(target, replacement, blockList);
            }

            public void ReplaceInGroup(string target, string replacement, string blockGroupName)
            {
                ReplaceInGroup(target, replacement, gridTerminalSystem.GetBlockGroupWithName(blockGroupName));
            }

            public void AppendToAll(string suffix)
            {
                List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();
                gridTerminalSystem.GetBlocks(blockList);
                Append(suffix, blockList);
            }

            public void Append(string suffix, List<IMyTerminalBlock> blockList)
            {
                foreach (IMyTerminalBlock block in blockList)
                    Append(suffix, block);
            }

            public void Append(string suffix, string blockName)
            {
                Append(suffix, gridTerminalSystem.GetBlockWithName(blockName));
            }

            public void Append(string suffix, IMyTerminalBlock block)
            {
                if (block == null) throw BlockNotFoundException;
                block.CustomName = block.CustomName + suffix;
            }

            public void AppendInGroup(string suffix, string blockGroupName)
            {
                AppendInGroup(suffix, gridTerminalSystem.GetBlockGroupWithName(blockGroupName));
            }

            public void AppendInGroup (string suffix, IMyBlockGroup blockGroup)
            {
                if (blockGroup == null) throw GroupNotFoundException;
                List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();
                blockGroup.GetBlocks(blockList);
                Append(suffix, blockList);
            }
        }
    }
}
