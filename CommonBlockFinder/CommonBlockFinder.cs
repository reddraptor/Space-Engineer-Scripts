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
        public class CommonBlockFinder
        {
            List<BlockType> FindCommonBlocks<BlockType>(List<IMyBlockGroup> blockGroupList) where BlockType : class, IMyTerminalBlock
            {
                if (blockGroupList == null || blockGroupList.Count < 1) return null;
                
                List<BlockType> blockList0 = new List<BlockType>();
                blockGroupList[0].GetBlocksOfType(blockList0);

                if (blockGroupList.Count < 2) return blockList0;

                blockGroupList.RemoveAt(0);
                List<BlockType> blockList1 = FindCommonBlocks<BlockType>(blockGroupList);

                List<BlockType> commonBlockList = new List<BlockType>();

                foreach (BlockType block in blockList0)
                {
                    foreach (BlockType comparerBlock in blockList1)
                    {
                        if (block == comparerBlock) commonBlockList.Add(block);
                    }
                }

                return commonBlockList;
            }
        }
    }
}
