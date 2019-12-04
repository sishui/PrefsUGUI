﻿using System;
using UnityEngine;

namespace PrefsUGUI
{
    using Guis.Prefs;

    [Serializable]
    public class PrefsColor : Prefs.PrefsExtends<Color, PrefsGuiColor>
    {
        public PrefsColor(string key, Color defaultValue = default(Color), GuiHierarchy hierarchy = null,
            string guiLabel = null, Action<Prefs.PrefsGuiBaseConnector<Color, PrefsGuiColor>> onCreatedGui = null)
            : base(key, defaultValue, hierarchy, guiLabel, onCreatedGui) { }

        protected override void OnCreatedGuiInternal(PrefsGuiColor gui)
        {
            gui.Initialize(this.GuiLabel, this.Get(), () => this.DefaultValue);
        }
    }
}
