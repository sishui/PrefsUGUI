﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PrefsUGUI
{
    using Guis.Prefs;

    [Serializable]
    public class PrefsVector4 : Prefs.PrefsParam<Vector4, PrefsGuiVector4>
    {
        public PrefsVector4(string key, Vector4 defaultValue = default(Vector4), string guiHierarchy = "", string guiLabel = "")
            : base(key, defaultValue, guiHierarchy, guiLabel) { }

        protected override void OnCreatedGuiInternal(PrefsGuiVector4 gui)
        {
            gui.Initialize(this.GuiLabel, this.Get(), () => this.DefaultValue);
        }
    }
}