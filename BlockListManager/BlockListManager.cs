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
        /// <summary>
        /// Block List Manager 1.0
        /// </summary>
        public class BlockListManager
        {
            IMyGridTerminalSystem gridTerminalSystem;

            public BlockListManager( IMyGridTerminalSystem gridTerminalSystem)
            {
                this.gridTerminalSystem = gridTerminalSystem;
            }

            /// <summary>
            /// Appends blocks, from the manager's grid, with the given names and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock</typeparam>
            /// <param name="blockNames">Array of block custom names</param>
            /// <param name="blockList">List of blocks of given type</param>
            public void AppendBlocksWithNames<BlockType>(string[] blockNames, List<BlockType> blockList) where BlockType : IMyTerminalBlock
            {
                BlockType block;

                foreach (string name in blockNames)
                {
                    if ((block = (BlockType)gridTerminalSystem.GetBlockWithName(name)) == null) throw new Exception("Block with name '" + name + "' does not exist in this grid.");
                    blockList.Add(block);
                }
            }

            /// <summary>
            /// Appends blocks, from the manager's grid, from groups with the given names and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock</typeparam>
            /// <param name="groupNames">Array of group names</param>
            /// <param name="blockList">List of blocks of given type</param>
            public void AppendBlocksFromGroupsWithNames<BlockType>(string[] groupNames, List<BlockType> blockList) where BlockType : class, IMyTerminalBlock
            {
                IMyBlockGroup blockGroup;
                List<BlockType> blockSubList = new List<BlockType>();

                foreach (string name in groupNames)
                {
                    if ((blockGroup = gridTerminalSystem.GetBlockGroupWithName(name)) == null) throw new Exception("Block group with name '" + name + "' does not exist in this grid.");
                    blockGroup.GetBlocksOfType(blockSubList);
                    blockList.AddRange(blockSubList);
                }
            }
        }
    }
}
