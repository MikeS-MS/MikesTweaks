using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Configs;
using MikesTweaks.Scripts.Environment;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Items;
using MikesTweaks.Scripts.Player;
using MikesTweaks.Scripts.Systems;
using MikesTweaks.Scripts.World;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Internal;

namespace MikesTweaks.Scripts
{
    [BepInPlugin(GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MikesTweaks : BaseUnityPlugin
    {
        public const string GUID = "mikes.lethalcompany.mikestweaks";

        public static ManualLogSource Log = null;
        public static MikesTweaks Instance { get; private set; } = null;

        public static class Compatibility
        {
            public static bool ReservedSlotsCompat = false;
        }

        public void BindConfig<T>(ref ConfigEntrySettings<T> config, string SectionName)
        {
            config.Entry = Config.Bind(SectionName, config.ConfigName, config.DefaultValue, config.ConfigDesc);
        }

        public void LoadConfigs()
        {
            Config.Reload();
        }

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            WorldTweaks.RegisterConfigs(Config);
            PlayerTweaks.RegisterConfigs(Config);
            InventoryTweaks.RegisterConfigs(Config);
            Config.SaveOnConfigSet = false;
            
            Harmony.CreateAndPatchAll(typeof(MenuManager_Patches));
            Harmony.CreateAndPatchAll(typeof(IngamePlayerSettings_Patches));
            Harmony.CreateAndPatchAll(typeof(HUDManager_Patches));
            Harmony.CreateAndPatchAll(typeof(NetworkManager_Patches));
            Harmony.CreateAndPatchAll(typeof(StartOfRound_Patches));
            Harmony.CreateAndPatchAll(typeof(TimeOfDay_Patches));
            Harmony.CreateAndPatchAll(typeof(InteractTrigger_Patches));
            Harmony.CreateAndPatchAll(typeof(Terminal_Patches));
            Harmony.CreateAndPatchAll(typeof(PlayerControllerB_Patches));
            Harmony.CreateAndPatchAll(typeof(GrabbableObject_Patches));

            CheckCompatibilities();

            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        private void CheckCompatibilities()
        {
            Compatibility.ReservedSlotsCompat = IsModPresent("ReservedItemSlotCore");
            
            if (Compatibility.ReservedSlotsCompat)
                Log.LogInfo("Found: ReservedItemSlotCore");
        }

        public static bool IsModPresent(string name)
        {
            foreach (var pluginInfo in Chainloader.PluginInfos)
            {
                if (name == pluginInfo.Value.Metadata.Name)
                    return true;
            }

            return false;
        }
    }
}
