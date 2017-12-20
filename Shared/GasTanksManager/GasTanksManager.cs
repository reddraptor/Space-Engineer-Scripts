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
        /// Gas Tanks Manager 0.02
        /// Requires: StatusReporter, StatusReport
        /// </summary>
        public class GasTanksManager : StatusReporter
        {
            private List<IMyGasTank> gasTankList;

            public float TotalCapacity
            {
                get
                {
                    float totalCapacity = 0;

                    foreach (IMyGasTank gasTank in gasTankList)
                    {
                        if (gasTank == null || !gasTank.IsFunctional) ReportItem("Missing or nonfunctional gas tank. Not included in total capacity.", StatusReport.Type.WARNING);
                        else totalCapacity += gasTank.Capacity;
                    }

                    return totalCapacity;
                }
            }

            public float FilledRatio
            {
                get
                {
                    float totalGasLevel = 0;
                    float totalCapacity = 0;

                    foreach (IMyGasTank gasTank in gasTankList)
                    {
                        if (gasTank == null || !gasTank.IsFunctional) ReportItem("Missing or nonfunctional gas tank. Not included in gas level calculation.", StatusReport.Type.WARNING);
                        else
                        {
                            totalGasLevel += gasTank.FilledRatio * gasTank.Capacity;
                            totalCapacity += gasTank.Capacity;
                        }
                    }

                    return totalGasLevel / totalCapacity;
                }
            }

            public GasTanksManager()
            {
                gasTankList = new List<IMyGasTank>();
            }

            public GasTanksManager(IMyGasTank gasTank)
            {
                gasTankList = new List<IMyGasTank>();
                Add(gasTank);
            }

            public GasTanksManager(List<IMyGasTank> gasTankList)
            {
                this.gasTankList = new List<IMyGasTank>();
                Add(gasTankList);
            }

            public void Add(IMyGasTank gasTank)
            {
                gasTankList.Add(gasTank);
            }

            public void Add(List<IMyGasTank> gasTankList)
            {
                foreach (IMyGasTank gasTank in gasTankList)
                {
                    this.gasTankList.Add(gasTank);
                }
            }

            public void Remove(IMyGasTank gasTank)
            {
                gasTankList.Remove(gasTank);
            }

            public void Remove(List<IMyGasTank> gasTankList)
            {
                foreach (IMyGasTank gasTank in gasTankList)
                {
                    this.gasTankList.Remove(gasTank);
                }
            }
        }
    }
}
