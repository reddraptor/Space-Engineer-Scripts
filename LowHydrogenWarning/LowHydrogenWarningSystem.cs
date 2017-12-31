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
        /// Low Hydrogen Warning Alert System 1.0
        /// Requirements: AlertSystem, GasTanksManager
        /// </summary>
        public class LowHydrogenWarningSystem : Alert
        {
            /// <summary>
            /// // ratio of full capacity to raise warning
            /// </summary>
            public float warningLevel = 0.25f; 

            private GasTanksManager gasTanksManager;

            public GasTanksManager GasTanksManager
            {
                get { return gasTanksManager; }
            }

            public LowHydrogenWarningSystem(List<IMyGasTank> h2TankBlocks, IMyInteriorLight warningLightBlock, IMyTextPanel warningLCDBlock, IMySoundBlock warningSoundBlock)
            {
                gasTanksManager = new GasTanksManager(h2TankBlocks);
                
                AlertLight warningLight = new AlertLight(warningLightBlock);
                AlertLight warningLCD = new AlertText(warningLCDBlock, "Low H2!");
                AlertLight warningSound = new AlertSound(warningSoundBlock);

                listAlertObjects = new List<AlertLight> { warningLight, warningLCD, warningSound };
            }

            public void CheckLevels()
            {
                if (Enabled == false && gasTanksManager.FilledRatio <= warningLevel)
                    Enabled = true;
                else if (Enabled == true && gasTanksManager.FilledRatio > warningLevel) 
                    Enabled = false;
            }
        }


    }
}
