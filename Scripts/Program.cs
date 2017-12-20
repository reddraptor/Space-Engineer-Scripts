using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using VRage.Game;

namespace SEScript.AirLockController
{
    class Program : MyGridProgram
    {
        string innerDoorName = "Sliding Door (AL01-IN)";
        string outerDoorName = "Sliding Door (AL01-OUT)";
        string airVentName = "Air Vent (AL01)";
        string timerName = "Timer Block (AL01)";

        IMyTerminalBlock innerDoorBlock;
        IMyTerminalBlock outerDoorBlock;
        IMyTerminalBlock airVentBlock;
        IMyTerminalBlock timerBlock;

        public Program() 
        {
            innerDoorBlock = (IMyDoor)GridTerminalSystem.GetBlockWithName(innerDoorName);
            outerDoorBlock = (IMyDoor)GridTerminalSystem.GetBlockWithName(outerDoorName);
            airVentBlock = (IMyAirVent)GridTerminalSystem.GetBlockWithName(airVentName);
            timerBlock = GridTerminalSystem.GetBlockWithName(timerName);
        }



        public void Save()
        {

            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.

        }



        public void Main(string argument)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked.
            // 
            // The method itself is required, but the argument above
            // can be removed if not needed.

            if (argument == "depressurize")
            {
                //Power on inner door
                //Close inner door
                //Delay
                //Power off inner door
                //Air vent depressurize
                //Power on outer door
                //Open outer door
            }
            else if (argument == "pressurize")
            {
                //Power on outer door
                //Close outer door
                //Delay
                //Power off outer door
                //Air vent pressurize
                //Power on inner door
                //Open inner door

            }
            else if (argument == "lockdown") { }
            else
            {
                Echo(
                    "Invalid Argument." + "\n" +
                    "Valid arguments: 'depressurize', 'pressurize', 'lockdown'"
                    );
            }

        }


    }
}
