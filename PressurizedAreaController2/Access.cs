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
        public class Access
        {
            List<IMyDoor> listOfDoors;
            Area[] area = new Area[2];

            public class NoDoorsException : Exception
            {
                public NoDoorsException() : base("Access: No doors in list.") { }
            }

            public bool IsClosed
            {
                get
                {
                    ValidateDoorList();
                    foreach (IMyDoor door in listOfDoors)
                    {
                        Validate(door);
                        if (door.Status != DoorStatus.Closed) return false;
                    }
                    return true;
                }
            }

            protected void ValidateDoorList()
            {
                if (listOfDoors == null) throw new NullReferenceException("Access: Door list is null.");
                if (listOfDoors.Count < 1) throw new NoDoorsException();
            }

            protected void Validate(IMyDoor door)
            {
                if (door == null) throw new NullReferenceException("Access: A door has null value.");
            }

            public bool IsSecured
            {
                get
                {
                    ValidateDoorList();
                    foreach (IMyDoor door in listOfDoors)
                    {
                        Validate(door);
                        if (door.Status != DoorStatus.Closed || door.Enabled) return false;
                    }
                    return true;
                }
            }

            public IMyDoor Door(int i)
            {
                ValidateDoorList();
                if (i < 0 || i >= listOfDoors.Count) return null;
                return listOfDoors[i];
            }

            public int DoorCount
            {
                get
                {
                    ValidateDoorList();
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
                ValidateDoorList();
            }

            public bool Open()
            {
                ValidateDoorList();
                foreach (IMyDoor door in listOfDoors)
                {
                    Validate(door);
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
                ValidateDoorList();
                foreach (IMyDoor door in listOfDoors)
                {
                    Validate(door);
                    door.Enabled = true;
                    door.CloseDoor();
                }
                return true;
            }

            public bool Disable()
            {
                ValidateDoorList();
                foreach (IMyDoor door in listOfDoors)
                {
                    Validate(door);
                    door.Enabled = false;
                }
                return true;
            }
        }
    }
}
