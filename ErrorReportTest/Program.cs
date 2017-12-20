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

        StatusReport statusReport1 = new StatusReport();
        StatusReport statusReport2 = new StatusReport();
        Reporter statusReporter = new Reporter();

        public class Reporter : StatusReporter
        {
            public void ReportTypical()
            {
                ReportItem("Typical Text.", StatusReport.Type.TYPICAL);
            }

            public void ReportWarning()
            {
                ReportItem("A Warning.", StatusReport.Type.WARNING);
            }

            public void ReportError()
            {
                ReportItem("An error!", StatusReport.Type.ERROR);
            }

            public void ReportAlert()
            {
                ReportItem("ALERT!", StatusReport.Type.ALERT);
            }
        }

        public Program()
        {
            statusReporter.SetStatusReport(statusReport1);
        }

        public void Main(string argument)
        {
            statusReporter.ReportTypical();
            statusReporter.ReportWarning();

            statusReporter.SetStatusReport(statusReport2);

            statusReporter.ReportError();
            statusReporter.ReportAlert();

            Echo(statusReport1.RetrieveFullReportText());
            Echo(statusReport2.RetrieveItemText(1));
            Echo(statusReport2.RetrieveItemType(1).ToString());
            Echo(statusReport2.RetrieveItemSource(1).ToString());
        }
    }
}