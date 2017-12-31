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
        public class StatusReport
        {
            public enum Type { TYPICAL, WARNING, ALERT, ERROR }

            public int ReportCount
            {
                get { return itemList.Count; }
            }

            public int maxReports = 30;
            public int maxCharsPerLine = 40;

            private List<Item> itemList = new List<Item>();

            private class Item
            {
                public string reportText;
                public Type type;
                public object Source { get { return source; } }

                private object source;

                public Item(string reportText, Type type, object source)
                {
                    this.reportText = reportText;
                    this.type = type;
                    this.source = source;
                }
            }

            public void Clear()
            {
                itemList = new List<Item>();
            }

            public void AddItem(string reportText, Type type, object source = null)
            {
                reportText = InsertNewLines(reportText, maxCharsPerLine);
                if (itemList.Count >= maxReports) itemList.RemoveAt(0);
                itemList.Add(new Item(reportText, type, source));
            }

            string InsertNewLines(string text, int charsPerLine)
            {
                int spaceIndex;
                string newText = "";

                string[] lineArray = text.Split('\n');

                foreach (string line in lineArray)
                {
                    if (line.Length > charsPerLine)
                    {
                        spaceIndex = line.Substring(0, 40).LastIndexOf(' ');
                        newText += line.Substring(0, spaceIndex) + '\n' + InsertNewLines(line.Substring(spaceIndex + 1), maxCharsPerLine);
                    }
                    else newText += line + "\n";
                }

                return newText;
            }

            public string RetrieveFullReportText()
            {
                string text = "";

                foreach (Item item in itemList)
                {
                    if (item != null) text += StatusTypeText(item.type) + item.reportText + "\n";
                }

                return text;
            }

            public string RetrieveItemText(int index)
            {
                if (index >= 0 && index < ReportCount) return StatusTypeText(itemList[index].type) + itemList[index].reportText;
                else throw new Exception("RetrieveItemText(index): Index out of range.");
            }

            public Type RetrieveItemType(int index)
            {
                if (index >= 0 && index < ReportCount) return itemList[index].type;
                else throw new Exception("RetrieveItemType(index): Index out of range.");
            }

            public object RetrieveItemSource(int index)
            {
                if (index >= 0 && index < ReportCount) return itemList[index].Source;
                else throw new Exception("RetrieveItemSource(index): Index out of range.");
            }

            private string StatusTypeText(Type type)
            {
                switch (type)
                {
                    case Type.WARNING:
                        return "Warning:\n";
                    case Type.ERROR:
                        return "Error!:\n";
                    case Type.ALERT:
                        return "ALERT!:\n";
                    default:
                        return "";
                }
            }

        }
    }
}
