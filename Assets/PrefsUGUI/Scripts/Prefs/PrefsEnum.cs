﻿using System;

namespace PrefsUGUI
{
    using Guis.Prefs;

    [Serializable]
    public class PrefsEnum<T> : Prefs.PrefsGuiBase<int, PrefsGuiEnum> where T : Enum
    {
        public PrefsEnum(
            string key, T defaultValue = default, GuiHierarchy hierarchy = null,
            string guiLabel = null, Action<Prefs.PrefsGuiBase<int, PrefsGuiEnum>> onCreatedGui = null
        )
            : base(key, Convert.ToInt32(defaultValue), hierarchy, guiLabel, onCreatedGui)
        {
        }

        protected override void OnCreatedGuiInternal(PrefsGuiEnum gui)
            => gui.Initialize<T>(this.GuiLabel, this.Get(), this.GetDefaultValue);
    }
}
