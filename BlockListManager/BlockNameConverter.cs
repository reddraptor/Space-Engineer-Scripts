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
        public class BlockNameConverter
        {
            IMyGridTerminalSystem gridTerminalSystem;

            public BlockNameConverter( IMyGridTerminalSystem gridTerminalSystem)
            {
                this.gridTerminalSystem = gridTerminalSystem;
            }

            /// <summary>
            /// Appends blocks, from the grid terminal system, with the given names and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock</typeparam>
            /// <param name="blockNames">Array of block custom names</param>
            /// <param name="blockList">List of blocks of given type</param>
            public void AppendBlocksFromCustomNames<BlockType>(string[] blockNames, List<BlockType> blockList) where BlockType : IMyTerminalBlock
            {
                foreach (string name in blockNames)
                {
                    AppendBlockFromCustomName(name, blockList);
                }
            }

            /// <summary>
            /// Appends block, from the grid, with the given name and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock.</typeparam>
            /// <param name="blockName">string containing custom name of block.</param>
            /// <param name="blockList">List of blocks of given type.</param>
            public void AppendBlockFromCustomName<BlockType>(string blockName, List<BlockType> blockList) where BlockType : IMyTerminalBlock
            {
                BlockType block;

                if ((block = (BlockType)gridTerminalSystem.GetBlockWithName(blockName)) == null) throw new Exception("Block with name '" + blockName + "' does not exist in this grid.");
                blockList.Add(block);
            }

            /// <summary>
            /// Appends blocks, from the grid, from groups with the given names and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock</typeparam>
            /// <param name="groupNames">Array of group names</param>
            /// <param name="blockList">List of blocks of given type</param>
            public void AppendBlocksFromGroupNames<BlockType>(string[] groupNames, List<BlockType> blockList) where BlockType : class, IMyTerminalBlock
            {
                foreach (string name in groupNames)
                {
                    AppendBlocksFromGroupName(name, blockList);
                }
            }

            /// <summary>
            /// Appends blocks, from the grid, from group with the given name and type; to the end of a typed block list.
            /// </summary>
            /// <typeparam name="BlockType">Block type that is a subtype of IMyTerminalBlock</typeparam>
            /// <param name="groupName">String containing the group name.</param>
            /// <param name="blockList">List of blocks of given type.</param>
            public void AppendBlocksFromGroupName<BlockType>(string groupName, List<BlockType> blockList) where BlockType : class, IMyTerminalBlock
            {
                IMyBlockGroup blockGroup;
                List<BlockType> blockSubList = new List<BlockType>();

                if ((blockGroup = gridTerminalSystem.GetBlockGroupWithName(groupName)) == null) throw new Exception("Block group with name '" + groupName + "' does not exist in this grid.");
                blockGroup.GetBlocksOfType(blockSubList);
                blockList.AddRange(blockSubList);
            }
        }
    }
}
