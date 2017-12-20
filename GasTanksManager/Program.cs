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
        string[] nameArrayGasTanks = { "Oxygen Tank", "Oxygen Tank 2" };

        GasTanksManager gasTanksManager;
        StatusReport statusReport = new StatusReport();

        public Program()
        {
            IMyTerminalBlock block;

            gasTanksManager = new GasTanksManager();
            gasTanksManager.SetStatusReport(statusReport);

            foreach (string nameGasTank in nameArrayGasTanks)
            {
                block = GridTerminalSystem.GetBlockWithName(nameGasTank);
                if (block != null & block is IMyGasTank) gasTanksManager.Add((IMyGasTank)block);
            }

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update100) != 0)
            {
                Echo(statusReport.RetrieveFullReportText());
                statusReport.Clear();
                Echo(gasTanksManager.FilledRatio.ToString());
            }
        }
    }
}