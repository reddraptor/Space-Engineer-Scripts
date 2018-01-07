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
        public class Area : StatusReporter
        {
            List<Access> listOfAccessPoints;
            List<IMyAirVent> listOfStorageAirVents;
            List<IMyAirVent> listOfSourceAirVents;
            GasTanksManager storageTanksManager;

            public Area(List<Access> accessPoints, List<IMyAirVent> sourceAirVents, List<IMyAirVent> storageAirVents = null, GasTanksManager storageTanksManager = null, StatusReport statusReport = null)
            {
                listOfAccessPoints = accessPoints;
                AccessPointsSetStatusReport(statusReport);
                listOfStorageAirVents = storageAirVents;
                listOfSourceAirVents = sourceAirVents;
                this.storageTanksManager = storageTanksManager;
                storageTanksManager.SetStatusReport(statusReport);
            }

            private void AccessPointsSetStatusReport(StatusReport statusReport)
            {
                if (listOfAccessPoints == null) return;
                foreach (Access access in listOfAccessPoints)
                {
                    access.SetStatusReport(statusReport);
                }
            }

            protected bool ValidAccessList()
            {
                if (listOfAccessPoints == null)
                {
                    ReportItem("Area: listOfAccessPoints == null!", StatusReport.Type.ERROR);
                    return false;
                }
                else if (listOfAccessPoints.Count < 1)
                {
                    ReportItem("Area: listOfAccessPoints is empty", StatusReport.Type.ERROR);
                    return false;
                }
                else return true;
            }

            protected bool ValidAccess(int index)
            {
                if (index < 0 || index >= listOfAccessPoints.Count)
                {
                    ReportItem("Area: access index out of range.", StatusReport.Type.ERROR);
                    return false;
                }
                else if (listOfAccessPoints[index] == null)
                {
                    ReportItem("Area: access == null!", StatusReport.Type.ERROR);
                    return false;
                }
                else return true;
            }

            public int AccessPointCount
            {
                get
                {
                    if (!ValidAccessList()) return -1;
                    return listOfAccessPoints.Count;
                }
            }

            public Access AccessPoint(int i)
            {
                if (!ValidAccessList() || !ValidAccess(i)) return null;
                return listOfAccessPoints[i];
            }

            public int StorageAirVentCount
            {
                get
                {
                    if (listOfSourceAirVents == null) return -1;
                    return listOfStorageAirVents.Count;
                }
            }

            public IMyAirVent StorageAirVent(int i)
            {
                if (listOfStorageAirVents == null) return null;
                if (i < 0 || i >= listOfStorageAirVents.Count) return null;
                return listOfStorageAirVents[i];
            }

            public int SourceAirVentCount
            {
                get
                {
                    if (listOfSourceAirVents == null) return -1;
                    return listOfSourceAirVents.Count;
                }
            }

            public IMyAirVent SourceAirVent(int i)
            {
                if (listOfSourceAirVents == null) return null;
                if (i < 0 || i >= listOfSourceAirVents.Count) return null;
                return listOfSourceAirVents[i];
            }

            public GasTanksManager StorageTanksManager
            {
                get { return storageTanksManager; }
            }
            
        }
    }
}
