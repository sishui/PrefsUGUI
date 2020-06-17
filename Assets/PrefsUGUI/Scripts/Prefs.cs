﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace PrefsUGUI
{
    using Guis;
    using Guis.Preferences;
    using XmlStorage;
    using XmlStorageConsts = XmlStorage.Systems.XmlStorageConsts;

    /// <summary>
    /// Central part of PrefsUGUI.
    /// </summary>
    public static partial class Prefs
    {
        public const char HierarchySeparator = '/';

        public static string AggregationName { get; private set; } = "";
        public static string FileName { get; private set; } = "";

        private static PrefsGuis PrefsGuis = null;
        private static ConcurrentBag<Action> StorageValueSetters = new ConcurrentBag<Action>();
        private static ConcurrentDictionary<string, Action> AddPrefsCache = new ConcurrentDictionary<string, Action>();
        private static ConcurrentQueue<string> AddPrefsCacheOrders = new ConcurrentQueue<string>();
        private static ConcurrentDictionary<Guid, Action> RemovePrefsCache = new ConcurrentDictionary<Guid, Action>();
        private static ConcurrentDictionary<Guid, Action> RemoveGuiHierarchyCache = new ConcurrentDictionary<Guid, Action>();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            PrefsGuis = UnityEngine.Object.FindObjectOfType<PrefsGuis>();
            if (PrefsGuis == null)
            {
                var prefab = (GameObject)Resources.Load(PrefsGuis.PrefsGuisPrefabName, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }

                PrefsGuis = UnityEngine.Object.Instantiate(prefab).GetComponent<PrefsGuis>();
            }

            var parameters = UnityEngine.Object.FindObjectOfType<PrefsParameters>();
            AggregationName = parameters == null ? PrefsParameters.DefaultNameGetter() : parameters.AggregationName;
            FileName = parameters == null ? PrefsParameters.DefaultNameGetter() : parameters.FileName;

            void ExecuteCachingActions()
            {
                ExecuteAndClear(AddPrefsCache, AddPrefsCacheOrders);
                ExecuteAndClear(RemovePrefsCache);
                ExecuteAndClear(RemoveGuiHierarchyCache);
            }
            PrefsGuis.SetCachingActionsExecutor(ExecuteCachingActions);
        }

        public static void Save()
        {
            var current = Storage.CurrentAggregationName;

            Storage.ChangeAggregation(AggregationName);
            Storage.CurrentAggregation.FileName = FileName + XmlStorageConsts.Extension;


            foreach (var setter in StorageValueSetters)
            {
                setter?.Invoke();
            }

            Storage.ChangeAggregation(current);
            Storage.Save();
        }

        public static void ShowGUI()
        {
            if (PrefsGuis != null)
            {
                PrefsGuis.ShowGUI();
            }
        }

        public static bool IsShowing()
            => PrefsGuis != null && PrefsGuis.IsShowing;

        public static void SetCanvasSize(float width, float height)
        {
            if (PrefsGuis != null)
            {
                PrefsGuis.SetCanvasSize(width, height);
            }
        }

        public static void RemoveGuiHierarchy(Guid hierarchyId)
        {
            void RemoveGuiHierarchy() => PrefsGuis.RemoveCategory(ref hierarchyId);
            RemoveGuiHierarchyCache[hierarchyId] = RemoveGuiHierarchy;
        }

        private static void AddPrefs<ValType, GuiType>(PrefsValueBase<ValType> prefs, Action<GuiType> onCreated)
            where GuiType : PrefsGuiBase, IPrefsGuiConnector<ValType, GuiType>
        {
            void AddPrefs() => PrefsGuis.AddPrefs(prefs, onCreated);
            AddPrefsCache[prefs.SaveKey] = AddPrefs;
            AddPrefsCacheOrders.Enqueue(prefs.SaveKey);
        }

        private static void RemovePrefs(Guid prefsId)
        {
            void RemovePrefs() => PrefsGuis.RemovePrefs(ref prefsId);
            RemovePrefsCache[prefsId] = RemovePrefs;
        }

        private static void ExecuteAndClear<T>(ConcurrentDictionary<T, Action> dictionary)
        {
            foreach(var pair in dictionary)
            {
                pair.Value?.Invoke();
            }
            dictionary.Clear();
        }

        private static void ExecuteAndClear<T>(ConcurrentDictionary<T, Action> dictionary, ConcurrentQueue<T> orders)
        {
            while (orders.TryDequeue(out var index) == true)
            {
                if (dictionary.TryRemove(index, out var action) == true)
                {
                    action?.Invoke();
                }
            }
            dictionary.Clear();
        }
    }
}
