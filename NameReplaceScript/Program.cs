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
        NameFindAndReplace nameChanger;
        CommandParser commandParser;

        public Program()
        {
            nameChanger = new NameFindAndReplace(GridTerminalSystem);
            commandParser = new CommandParser();
        }


        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource.HasFlag(UpdateType.Terminal))
            {
                List<string> tokenList;

                try
                {
                    tokenList = commandParser.GetTokensFrom(argument);
                }
                catch (CommandParser.MissingQuotationException)
                {
                    Echo("Error. Missing a quotation mark.");
                    return;
                }
                catch (Exception) { throw; }
                
                Echo(commandParser.LastCommand());

                try
                {
                    switch (tokenList[0])
                    {
                        case "replace":
                            switch (tokenList[1])
                            {
                                case "one":
                                    nameChanger.Replace(tokenList[3], tokenList[4], tokenList[2]);
                                    break;
                                case "all":
                                    nameChanger.ReplaceInAll(tokenList[2], tokenList[3]);
                                    break;
                                case "group":
                                    nameChanger.ReplaceInGroup(tokenList[3], tokenList[4], tokenList[2]);
                                    break;
                                default:
                                    Echo("Unknown Command. Use: replace (one/all/group) {(param0),...}");
                                    return;
                            }
                            break;

                        case "append":
                            switch (tokenList[1])
                            {
                                case "one":
                                    nameChanger.Append(tokenList[3], tokenList[2]);
                                    break;
                                case "all":
                                    nameChanger.AppendToAll(tokenList[2]);
                                    break;
                                case "group":
                                    nameChanger.AppendInGroup(tokenList[3], tokenList[2]);
                                    break;
                                default:
                                    Echo("Unknown Command. Use: append (one/all/group) {(param0),...}");
                                    return;
                            }
                            break;

                        default:
                            Echo("Unknown Command. Use: (replace/append) (one/all/group) {(param0),...}");
                            return;
                    }

                }
                catch (NameFindAndReplace.BlockNotFoundException)
                {
                    Echo("Block '" + tokenList[2] + "' not found.");
                }
                catch (NameFindAndReplace.GroupNotFoundException)
                {
                    Echo("Group '" + tokenList[2] + "' not found.");
                }
                catch (Exception) { throw; }
            }
        }
    }
}