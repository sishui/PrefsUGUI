﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrefsUGUI
{
    using Guis;
    using Guis.Factories;
    using Guis.Factories.Classes;
    using Guis.Preferences;
    using static Prefs;

    [Serializable]
    public class RemovableGuiHierarchy : GuiHierarchy
    {
        public event Action OnRemoved = delegate { };

        public override HierarchyType HierarchyType => HierarchyType.Removable;


        public RemovableGuiHierarchy(
            string hierarchyName, Action onRemoved = null, int sortOrder = DefaultSortOrder, GuiHierarchy parent = null
        )
            : base(hierarchyName, sortOrder, parent)
        {
            if(onRemoved != null)
            {
                this.OnRemoved += onRemoved;
            }
        }
        
        protected override void Regist()
            => AddGuiHierarchy<PrefsGuiRemovableButton>(this, this.OnCreatedGuiButton);

        protected virtual void OnCreatedGuiButton(PrefsCanvas canvas, Category category, PrefsGuiRemovableButton gui)
        {
            void onButtonClicked() => canvas.ChangeGUI(category);
            void FireOnRemoved() => this.OnRemoved?.Invoke();
            gui.Initialize(this.HierarchyName, onButtonClicked, FireOnRemoved);
        }

        protected override void Dispose(bool disposing)
        {
            this.OnRemoved = null;
            base.Dispose(disposing);
        }
    }
}
