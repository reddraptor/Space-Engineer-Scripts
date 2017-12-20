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
        /// Alert Class 1.1
        /// </summary>
        public class Alert
        {
            protected List<AlertObject> listAlertObjects;
            private bool enabled = false;

            public List<AlertObject> AlertObjects
            {
                get { return listAlertObjects; }
            }

            public Alert()
            {
                listAlertObjects = new List<AlertObject>();
            }

            public Alert(List<AlertObject> listAlertObjects)
            {
                this.listAlertObjects = listAlertObjects;
            }

            public bool Enabled
            {
                get { return enabled; }
                set
                {
                    if (value)
                    {
                        enabled = true;
                        SetAlertObjects(true);
                    }
                    else
                    {
                        enabled = false;
                        SetAlertObjects(false);

                    }
                }
            }

            private void SetAlertObjects(bool enabled)
            {
                foreach (AlertObject alertObject in listAlertObjects)
                {
                    alertObject.Enabled = enabled;
                }
            }
        }

        public abstract class AlertObject
        {
            private bool enabled = false;

            public bool Enabled
            {
                get { return enabled; }
                set
                {
                    if (value)
                    {
                        enabled = true;
                        Enable();
                    }
                    else
                    {
                        enabled = false;
                        Disable();
                    }
                }
            }

            protected abstract void Enable();

            protected abstract void Disable();
        }

        public class SoundAlert : AlertObject
        {
            private IMySoundBlock soundBlock;

            public SoundAlert(IMySoundBlock soundBlock)
            {
                this.soundBlock = soundBlock;
            }

            protected override void Enable()
            {
                if (soundBlock != null)
                {
                    soundBlock.Enabled = true;
                    soundBlock.Play();
                }
            }

            protected override void Disable()
            {
                if (soundBlock != null) soundBlock.Stop();
            }
        }

        public class TextAlert : AlertObject
        {
            private string alertText;
            private string normalText;
            private bool normalShowText;
            private IMyTextPanel textPanelBlock;

            public TextAlert(IMyTextPanel textPanelBlock, string alertText)
            {
                this.textPanelBlock = textPanelBlock;
                this.alertText = alertText;
            }

            protected override void Enable()
            {
                if (textPanelBlock != null)
                {
                    textPanelBlock.Enabled = true;
                    normalShowText = textPanelBlock.ShowText;
                    normalText = textPanelBlock.GetPublicText();
                    textPanelBlock.ShowPublicTextOnScreen();
                    textPanelBlock.WritePublicText(alertText);
                }
            }

            protected override void Disable()
            {
                if (textPanelBlock != null)
                {
                    if (!normalShowText) textPanelBlock.ShowTextureOnScreen();
                    textPanelBlock.WritePublicText(normalText);
                }
            }
        }

        public class LightAlert : AlertObject
        {
            private IMyInteriorLight lightBlock;

            public LightAlert(IMyInteriorLight lightBlock)
            {
                this.lightBlock = lightBlock;
            }

            protected override void Disable()
            {
                if (lightBlock != null) lightBlock.Enabled = false;
            }

            protected override void Enable()
            {
                if (lightBlock != null) lightBlock.Enabled = true;
            }
        }
    }
}
