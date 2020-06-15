﻿using System.Collections.Generic;
using UnityEngine;

namespace PrefsUGUI.Guis.Factories
{
    using Classes;
    using Guis.Preferences;
    using PrefsBase = Prefs.PrefsBase;

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public partial class PrefsCanvas : MonoBehaviour
    {
        public const string TopCategoryName = "";
        public const string TopHierarchyText = "hierarchy...";

        public RectTransform Panel
        {
            get { return this.links.Panel; }
        }

        [SerializeField]
        private Color topHierarchyColor = Color.gray;
        [SerializeField]
        private Color untopHierarchyColor = Color.white;
        [SerializeField]
        private CanvasLinks links = new CanvasLinks();
        [SerializeField]
        private PrefsGuiPrefabs prefabs = new PrefsGuiPrefabs();

        private PrefsGuiCreator creator = null;
        private CategoriesStruct structs = null;


        private void Awake()
        {
            this.creator = new PrefsGuiCreator(this);
            this.links.Content.gameObject.SetActive(false);

            var topContent = this.creator.CreateContent();
            this.structs = new CategoriesStruct(topContent, this.creator);
            this.OnGuiChanged(this.structs.current);

            this.links.Close.onClick.AddListener(this.OnClickedCloseButton);
            this.links.Discard.onClick.AddListener(this.OnClickedDiscardButton);
            this.links.Save.onClick.AddListener(this.OnClickedSaveButton);
        }

        private void OnValidate()
        {
            this.prefabs.OnValidate();
        }

        public GuiType AddPrefs<ValType, GuiType>(Prefs.PrefsValueBase<ValType> prefs)
            where GuiType : PrefsGuiBase, IPrefsGuiConnector<ValType, GuiType>
        {
            var category = this.structs.GetCategory(prefs.GuiHierarchy);
            var gui = this.creator.CreatePrefsGui<ValType, GuiType>(prefs, category);

            return gui;
        }

        public void RemovePrefs(string prefsSaveKey)
        {
            var categories = this.structs.categories;

            for (var i = 0; i < categories.Count; i++)
            {
                var dic = categories[i].Prefs;

                if (dic.ContainsKey(prefs) == true)
                {
                    var gui = dic[prefs].gameObject;

                    dic.Remove(prefs);
                    Destroy(gui);

                    return;
                }
            }
        }

        public void RemoveCategory(string fullHierarchyName)
            => this.structs.RemoveCategory(fullHierarchyName);

        public void ChangeGUI(Category nextCategory)
            => this.OnGuiChanged(this.structs.ChangeGUI(nextCategory));

        private void OnClickedDiscardButton()
        {
            foreach (var prefs in this.structs.Current.Prefs)
            {
                prefs.Key.Reload();
            }
        }

        private void OnClickedCloseButton() => this.OnGuiChanged(this.structs.ChangeGUI(this.structs.Current.Previous));

        private void OnClickedSaveButton()
        {
            Prefs.Save();
            gameObject.SetActive(false);
        }

        private void OnGuiChanged(Category category)
        {
            this.links.Scroll.content = category.Content;

            var isTop = this.SetHierarchy(category);
            this.SetButtonActive(isTop);
        }

        private bool SetHierarchy(Category category)
        {
            var hierarchy = category.CategoryName;
            var previous = category.Previous;
            while (previous != null)
            {
                hierarchy = previous.CategoryName + Prefs.HierarchySeparator + hierarchy;
                previous = previous.Previous;
            }
            var isTop = string.IsNullOrEmpty(hierarchy);

            this.links.Hierarchy.color = isTop == true ? this.topHierarchyColor : this.untopHierarchyColor;
            this.links.Hierarchy.fontStyle = isTop == true ? FontStyle.Italic : FontStyle.Normal;
            this.links.Hierarchy.text = isTop == true ?
                TopHierarchyText :
                hierarchy.TrimStart(Prefs.HierarchySeparator) + Prefs.HierarchySeparator;

            return isTop;
        }

        private void SetButtonActive(bool isTop)
        {
            if (isTop == true)
            {
                this.links.Close.gameObject.SetActive(false);
                this.links.Save.gameObject.SetActive(true);
            }
            else
            {
                this.links.Close.gameObject.SetActive(true);
                this.links.Save.gameObject.SetActive(false);
            }
        }

        private void Reset() => this.links.Reset(gameObject);
    }
}
