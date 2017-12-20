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
        public string nameSoundBlock = "Sound Block";
        public List<string> nameLightBlocks = new List<string>{ "Warning Light 1", "Warning Light 2" };
        public string nameLCDPanel = ("LCD Panel");

        public string warningMessage = "Alert!";
        public string warningMessage2 = "Danger!";

        private IMySoundBlock soundBlock;
        private List<IMyInteriorLight> lightBlocks;
        private IMyTextPanel lcdPanel;

        private AlertObject bigBadAlertSound;
        private AlertObject bigBadAlertLight;
        private AlertObject biggerBadderAlertLight;
        private AlertObject bigBadAlertMessage;
        private AlertObject biggerBadderAlertMessage;
        private Alert bigBadAlert;
        private Alert biggerBadderAlert;

        private AlertSystemManager alertSystemManager;

        public Program()
        {
            soundBlock = (IMySoundBlock)GridTerminalSystem.GetBlockWithName(nameSoundBlock);
            lightBlocks = new List<IMyInteriorLight>
            {
                (IMyInteriorLight)GridTerminalSystem.GetBlockWithName(nameLightBlocks[0]),
                (IMyInteriorLight)GridTerminalSystem.GetBlockWithName(nameLightBlocks[1])
            };
            lcdPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(nameLCDPanel);

            bigBadAlertLight = new LightAlert(lightBlocks[0]);
            biggerBadderAlertLight = new LightAlert(lightBlocks[1]);
            bigBadAlertSound = new SoundAlert(soundBlock);
            bigBadAlertMessage = new TextAlert(lcdPanel, warningMessage);
            biggerBadderAlertMessage = new TextAlert(lcdPanel, warningMessage2);
            bigBadAlert = new Alert(new List<AlertObject> { bigBadAlertLight, bigBadAlertMessage, bigBadAlertSound });
            biggerBadderAlert = new Alert(new List<AlertObject> { biggerBadderAlertLight, bigBadAlertLight, biggerBadderAlertMessage, bigBadAlertSound });

            alertSystemManager = new AlertSystemManager(new Dictionary<int, Alert> { { 0, bigBadAlert }, { 1, biggerBadderAlert } });

            //Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Trigger) != 0)
            {
                if (alertSystemManager.Alerts[0].Enabled)
                {
                    alertSystemManager.Alerts[0].Enabled = false;
                    alertSystemManager.Alerts[1].Enabled = true;
                }
                else if (alertSystemManager.Alerts[1].Enabled)
                {
                    alertSystemManager.Alerts[1].Enabled = false;
                }
                else
                {
                    alertSystemManager.Alerts[0].Enabled = true;
                }
            }
            else if ((updateSource & UpdateType.Terminal) != 0)
            {
                if (argument == "alerts 0")
                {
                    alertSystemManager.DisableAlerts();
                }
            }
        }
    }
}