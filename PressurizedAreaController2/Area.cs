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
        public class Area
        {
            string tagName;
            List<Access> listOfAccessPoints;
            List<IMyAirVent> listOfStorageAirVents;
            List<IMyAirVent> listOfSourceAirVents;
            GasTanksManager storageTanksManager;
            AlertSystemManager<int> alertSystemManager;

            public Area(List<Access> accessPoints, List<IMyAirVent> sourceAirVents, List<IMyAirVent> storageAirVents = null,
                GasTanksManager storageTanksManager = null, AlertSystemManager<int> alertSystemManager = null)
            {
                listOfAccessPoints = accessPoints;
                listOfStorageAirVents = storageAirVents;
                listOfSourceAirVents = sourceAirVents;
                this.storageTanksManager = storageTanksManager;
                this.alertSystemManager = alertSystemManager;
            }

            public class NoAccessPointsException : Exception
            {
                public NoAccessPointsException() : base("Area: No access points in list.") { } 
            }

            protected void ValidateAccessList()
            {
                if (listOfAccessPoints == null) throw new NullReferenceException("Area: Access point list has null value.");
                if (listOfAccessPoints.Count < 1) throw new NoAccessPointsException();
            }

            protected void ValidateAccess(int index)
            {
                if (listOfAccessPoints[index] == null) throw new NullReferenceException("Area: Access point had null value.");
            }

            public int AccessPointCount
            {
                get
                {
                    ValidateAccessList();
                    return listOfAccessPoints.Count;
                }
            }

            public Access AccessPoint(int i)
            {
                ValidateAccessList(); ValidateAccess(i);
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

            public AlertSystemManager<int> AlertSystemManager
            {
                get { return alertSystemManager; }
            }
        }
    }
}
