﻿using System;
using UnityEngine;

namespace PrefsUGUI
{
    using Guis.Prefs;

    [Serializable]
    public class PrefsIntSlider : Prefs.PrefsGuiBase<int, PrefsGuiNumericSliderInteger>
    {
        [SerializeField]
        protected int min = 0;
        [SerializeField]
        protected int max = 0;


        public PrefsIntSlider(
            string key, int defaultValue = default, GuiHierarchy hierarchy = null,
            string guiLabel = null, Action<Prefs.PrefsGuiBase<int, PrefsGuiNumericSliderInteger>> onCreatedGui = null
        )
            : base(key, defaultValue, hierarchy, guiLabel, onCreatedGui)
        {
        }

        public PrefsIntSlider(
            string key, int minValue, int maxValue, int defaultValue = default, GuiHierarchy hierarchy = null,
            string guiLabel = null, Action<Prefs.PrefsGuiBase<int, PrefsGuiNumericSliderInteger>> onCreatedGui = null
        )
            : this(key, defaultValue, hierarchy, guiLabel, onCreatedGui)
        {
            this.min = minValue;
            this.max = maxValue;
        }

        protected override void OnCreatedGuiInternal(PrefsGuiNumericSliderInteger gui)
        {
            if (this.min == this.max)
            {
                gui.Initialize(this.GuiLabel, this.Get(), this.GetDefaultValue);
            }
            else
            {
                gui.Initialize(this.GuiLabel, this.Get(), this.min, this.max, this.GetDefaultValue);
            }
        }
    }
}
