﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace PrefsUGUI.Guis.Preferences
{
    [Serializable]
    public abstract class NumericSliderGuiBase<ValType, GuiType> : NumericGuiBase<ValType, GuiType>
        where GuiType : PrefsInputGuiBase<ValType, GuiType>
    {
        public float MinValue => this.slider.minValue;
        public float MaxValue => this.slider.maxValue;

        [SerializeField]
        protected Slider slider = null;

        protected bool inited = false;


        protected override void Awake()
        {
            base.Awake();
            this.slider.onValueChanged.AddListener(this.OnSliderChanged);
        }

        protected override void Reset()
        {
            base.Reset();
            this.slider = this.GetComponentInChildren<Slider>();
        }

        public override void Initialize(string label, ValType initialValue, Func<ValType> defaultGetter)
        {
            var val = this.GetValueAsFloat();
            this.Initialize(label, initialValue, 0f, val + val, defaultGetter);
        }

        public virtual void Initialize(string label, ValType initialValue, float minValue, float maxValue, Func<ValType> defaultGetter)
        {
            this.InitializeSlider(minValue, maxValue);
            base.Initialize(label, initialValue, defaultGetter);

            this.inited = true;
        }

        protected virtual void InitializeSlider(float min, float max)
        {
            this.slider.wholeNumbers = !this.IsDecimalNumber;
            this.slider.minValue = (min > max ? max : min);
            this.slider.maxValue = (min < max ? max : min);
        }

        protected override void SetFields()
        {
            base.SetFields();
            this.slider.value = this.GetValueAsFloat();
        }

        protected virtual void OnSliderChanged(float v)
        {
            if(this.inited == false)
            {
                return;
            }

            this.SetValueAsFloat(v);
            this.FireOnValueChanged();
        }

        protected abstract float GetValueAsFloat();
        protected abstract void SetValueAsFloat(float v);
    }
}