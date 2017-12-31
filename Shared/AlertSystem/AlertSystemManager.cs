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
        /// Alert System Manager v1.0
        /// </summary>
        public class AlertSystemManager<keyType>
        {
            private Dictionary<keyType, Alert> dictAlerts;

            public Dictionary<keyType, Alert> Alerts
            {
                get { return dictAlerts; }
            }

            public AlertSystemManager()
            {
                dictAlerts = new Dictionary<keyType, Alert>();
            }

            public AlertSystemManager(Dictionary<keyType, Alert> dictionaryAlerts)
            {
                dictAlerts = dictionaryAlerts;
            }

            public void DisableAlerts()
            {
                foreach (Alert alert in dictAlerts.Values)
                {
                    if (alert.Enabled) alert.Enabled = false;
                }
            }
        }
    }
}

