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
        public class Access : StatusReporter
        {
            List<IMyDoor> listOfDoors;
            Area[] area = new Area[2];

            public bool IsClosed
            {
                get
                {
                    if (!ValidDoorList()) return false;
                    foreach (IMyDoor door in listOfDoors)
                        if (!Valid(door) || door.Status !=  DoorStatus.Closed) return false;
                    return true;
                }
            }

            protected bool ValidDoorList()
            {
                if (listOfDoors == null)
                {
                    ReportItem("Access: listOfDoors == null!", StatusReport.Type.ERROR);
                    return false;
                }
                if (listOfDoors.Count < 1)
                {
                    ReportItem("Access: listOfDoors is empty!", StatusReport.Type.ERROR);
                    return false;
                }
                return true;
            }

            protected bool Valid(IMyDoor door)
            {
                if (door == null)
                {
                    ReportItem("Access: door == null!", StatusReport.Type.ERROR);
                    return false;
                }
                return true;
            }

            public bool IsSecured
            {
                get
                {
                    if (!ValidDoorList() || !IsClosed) return false;
                    foreach (IMyDoor door in listOfDoors)
                        if (!Valid(door) || door.Enabled) return false;
                    return true;
                }
            }

            public IMyDoor Door(int i)
            {
                if (!ValidDoorList()) return null;
                if (i < 0 || i >= listOfDoors.Count) return null;
                return listOfDoors[i];
            }

            public int DoorCount
            {
                get
                {
                    if (!ValidDoorList()) return -1;
                    return listOfDoors.Count;
                }
            }

            public Area Area(int i)
            {
                if (i < 0 || i >= area.Length) return null;
                return area[i];
            }

            public Access(Area area0, Area area1, List<IMyDoor> listOfDoors)
            {
                area[0] = area0;
                area[1] = area1;
                this.listOfDoors = listOfDoors;
                ValidDoorList();
            }

            public bool IsOpen()
            {
                if (!ValidDoorList()) return false; 
                foreach (IMyDoor door in listOfDoors)
                {
                    if (!Valid(door)) return false;
                    if (!door.Enabled) door.Enabled = true;
                    if (door.Status != DoorStatus.Open || door.Status != DoorStatus.Opening)
                    {
                        door.OpenDoor();
                    }
                }
                return true;
            }

            public bool Close()
            {
                if (!ValidDoorList()) return false;
                foreach (IMyDoor door in listOfDoors)
                {
                    if (!Valid(door)) return false;
                    door.Enabled = true;
                    door.CloseDoor();
                }
                return true;
            }

            public bool Disable()
            {
                if (!ValidDoorList()) return false;
                foreach (IMyDoor door in listOfDoors)
                {
                    if (!Valid(door)) return false;
                    door.Enabled = false;
                }
                return true;
            }
        }
    }
}
