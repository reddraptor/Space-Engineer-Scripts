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
        string nameTextPanel = "Text panel (AL-00)";
        IMyTextPanel textPanel;
        StatusReport statusReport1 = new StatusReport();
        StatusReport statusReport2 = new StatusReport();
        Reporter statusReporter = new Reporter();

        public class Reporter : StatusReporter
        {
            public void ReportTypical()
            {
                ReportItem("Typical Text.\nThis is very typical text that one would see when everything is working perfectly normally.", StatusReport.Type.TYPICAL);
            }

            public void ReportWarning()
            {
                ReportItem("A Warning.\nStay alert, something is up.", StatusReport.Type.WARNING);
            }

            public void ReportError()
            {
                ReportItem("An error!\nWell this is something completely unexpected. Better check things out.", StatusReport.Type.ERROR);
            }

            public void ReportAlert()
            {
                ReportItem("ALERT!\nShit has hit the fan!", StatusReport.Type.ALERT);
            }
        }

        public Program()
        {
            statusReporter.SetStatusReport(statusReport1);
            textPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(nameTextPanel);
        }

        public void Main(string argument)
        {
            statusReporter.ReportTypical();
            statusReporter.ReportWarning();

            statusReporter.SetStatusReport(statusReport2);

            statusReporter.ReportError();
            statusReporter.ReportAlert();

            Echo(statusReport1.RetrieveFullReportText());
            textPanel.WritePublicText(statusReport1.RetrieveFullReportText());
            Echo(statusReport2.RetrieveItemText(1));
            Echo(statusReport2.RetrieveItemType(1).ToString());
            Echo(statusReport2.RetrieveItemSource(1).ToString());
        }
    }
}