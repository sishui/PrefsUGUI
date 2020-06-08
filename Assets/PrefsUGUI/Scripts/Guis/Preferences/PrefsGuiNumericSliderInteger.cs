﻿using System;
using UnityEngine;

namespace PrefsUGUI.Guis.Preferences
{
    [Serializable]
    public class PrefsGuiNumericSliderInteger : NumericSliderGuiBase<int, PrefsGuiNumericSliderInteger>
    {
        public override PrefsGuiNumericSliderInteger Component => this;

        protected override bool IsDecimalNumber => false;


        protected override bool IsDefaultValue()
            => this.GetValue() == this.defaultGetter();

        protected override void SetValueInternal(string value)
        {
            if (int.TryParse(value, out var v) == true)
            {
                this.SetValueInternal(v);
            }
        }

        protected override float GetValueAsFloat()
            => this.GetValue();

        protected override void SetValueAsFloat(float v)
            => this.SetValue(Mathf.RoundToInt(v));
    }
}