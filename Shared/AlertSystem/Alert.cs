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

        public class AlertSound : AlertObject
        {
            private IMySoundBlock soundBlock;

            public AlertSound(IMySoundBlock soundBlock)
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

        public class AlertText : AlertObject
        {
            public string alertText;
            public float fontSize;
            public Color fontColor;
            public Color bgColor;

            private IMyTextPanel textPanelBlock;

            private bool normalShowText;
            private string normalText;
            private float normalFontSize;
            private Color normalFontColor;
            private Color normalBgColor;

            public AlertText(IMyTextPanel textPanelBlock)
            {
                this.textPanelBlock = textPanelBlock;
                alertText = "Alert!";
                fontSize = 2f;
                fontColor = Color.Cyan;
                bgColor = Color.Red;
            }

            public AlertText(IMyTextPanel textPanelBlock, string alertText, float fontSize, Color fontColor, Color bgColor)
            {
                this.textPanelBlock = textPanelBlock;
                this.alertText = alertText;
                this.fontSize = fontSize;
                this.bgColor = bgColor;
                this.fontColor = fontColor;
            }

            protected override void Enable()
            {
                if (textPanelBlock != null)
                {
                    normalShowText = textPanelBlock.ShowText;
                    normalText = textPanelBlock.GetPublicText();
                    normalFontSize = textPanelBlock.FontSize;
                    normalFontColor = textPanelBlock.FontColor;
                    normalBgColor = textPanelBlock.BackgroundColor;

                    textPanelBlock.WritePublicText(alertText);
                    textPanelBlock.FontSize = fontSize;
                    textPanelBlock.FontColor = fontColor;
                    textPanelBlock.BackgroundColor = bgColor;

                    textPanelBlock.Enabled = true;
                    textPanelBlock.ShowPublicTextOnScreen();
                }
            }

            protected override void Disable()
            {
                if (textPanelBlock != null)
                {
                    if (!normalShowText) textPanelBlock.ShowTextureOnScreen();

                    textPanelBlock.WritePublicText(normalText);
                    textPanelBlock.FontSize = normalFontSize;
                    textPanelBlock.FontColor = normalFontColor;
                    textPanelBlock.BackgroundColor = normalBgColor;
                }
            }
        }

        public class AlertLight : AlertObject
        {
            public bool blink;
            public Color color;
            public float blinkIntervalSeconds = 0.5f;
            public float blinkLength = 50f;
            public float blinkOffset = 0f;

            private IMyInteriorLight lightBlock;
            private bool normalEnabled;
            private float normalBlinkInterval;
            private float normalBlinkOffset;
            private float normalBlinkLength;
            private Color normalColor;

            public AlertLight(IMyInteriorLight lightBlock)
            {
                this.lightBlock = lightBlock;
                color = Color.Red;
                blink = true;
            }

            public AlertLight(IMyInteriorLight lightBlock, Color color, bool blink)
            {
                this.lightBlock = lightBlock;
                this.color = color;
                this.blink = blink;
            }

            protected override void Disable()
            {
                if (lightBlock != null)
                {
                    if (!normalEnabled) lightBlock.Enabled = false;
                    lightBlock.Color = normalColor;
                    lightBlock.BlinkIntervalSeconds = normalBlinkInterval;
                    lightBlock.BlinkLength = normalBlinkLength;
                    lightBlock.BlinkOffset = normalBlinkOffset;
                }
            }

            protected override void Enable()
            {
                if (lightBlock != null)
                {
                    normalEnabled = lightBlock.Enabled;
                    normalColor = lightBlock.Color;
                    normalBlinkInterval = lightBlock.BlinkIntervalSeconds;
                    normalBlinkLength = lightBlock.BlinkLength;
                    normalBlinkOffset = lightBlock.BlinkOffset;

                    if (!lightBlock.Enabled) lightBlock.Enabled = true;
                    lightBlock.Color = color;
                    if (blink)
                    {
                        lightBlock.BlinkIntervalSeconds = blinkIntervalSeconds;
                        lightBlock.BlinkLength = blinkLength;
                        lightBlock.BlinkOffset = blinkOffset;
                    }
                }
            }
        }
    }
}
