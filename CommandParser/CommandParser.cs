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
        public class CommandParser
        {
            /// <summary>
            /// Thrown when there is unpaired quotation marks.
            /// </summary>
            public class MissingQuotationException : Exception
            {
                public MissingQuotationException() : base("CommandParser: Missing quotation mark.") { }
            } 

            List<string> lastParseResult;

            public List<string> LastParseResult
            {
                get { return lastParseResult; }
            }

            /// <summary>
            /// Takes a string, such as an argument from programmable block, and parses it into string tokens based on spaces and quotation marks.
            /// </summary>
            /// <param name="command">String to parse.</param>
            /// <returns>List of tokens as Strings.</returns>
            /// <exception cref="MissingQuotationException"></exception>
            public List<string> GetTokensFrom(string command)
            {
                StringBuilder sbToken = new StringBuilder(command.Length);
                String sToken;
                List<string> tokenList = new List<string>();
                bool openQuote = false;
                char[] charArray = command.ToCharArray();

                for (int i = 0; i < charArray.Length; i++)
                {
                    if (charArray[i] == '"' || charArray[i] == '\'')
                    {
                        if (openQuote && sbToken.Length == 0) tokenList.Add("");
                        openQuote = !openQuote;
                    }

                    else if (charArray[i] == ' ')
                    {
                        if (!openQuote)
                        {
                            sToken = sbToken.ToString();
                            if (sToken != "") tokenList.Add(sToken);
                            sbToken.Clear();
                        }

                        else
                            sbToken.Append(' ');
                    }

                    else
                        sbToken.Append(charArray[i]);
                }

                if (openQuote) throw new MissingQuotationException();
                sToken = sbToken.ToString();
                if (sToken != "") tokenList.Add(sToken);

                return lastParseResult = tokenList;
            }

            public string LastCommand()
            {
                string command = "";

                foreach (string token in lastParseResult)
                {
                    command += "'" + token + "' ";
                }

                return command;
            }
        }
    }
}
