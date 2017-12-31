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
        public class StatusReporter
        {
            private StatusReport statusReport;

            public void SetStatusReport(StatusReport statusReport)
            {
                this.statusReport = statusReport;
            }

            protected void ReportItem(string text, StatusReport.Type type = StatusReport.Type.TYPICAL)
            {
                if (statusReport == null) throw new Exception("No valid status report set, with SetStatusReport(statusReport), before ReportItem(text, type): " + this.ToString());
                statusReport.AddItem(text, type, this);
            }
        }
    }
}
