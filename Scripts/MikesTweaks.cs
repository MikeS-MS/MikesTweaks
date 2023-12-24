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
using MikesTweaks.Scripts.Moons;
using MikesTweaks.Scripts.Networking;
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
        public const bool DebugMode = false;

        public static ManualLogSource Log = null;
        public static MikesTweaks Instance { get; private set; } = null;

        public static class Compatibility
        {
            public static bool ReservedSlotCoreCompat = false;
            public static bool ReservedSlotsWalkieCompat = false;
            public static bool ReservedSlotsFlashlightCompat = false;
            public static bool LethalThingsCompat = false;
        }

        public void BindConfig<T>(ref ConfigEntrySettings<T> config, string SectionName)
        {
            config.Entry = Config.Bind(SectionName, config.ConfigName, config.DefaultValue, config.ConfigDesc);
        }

        public void LoadConfigs()
        {
            Config.Reload();
            ConfigsSynchronizer.ConfigsReceived = false;
        }

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            WorldTweaks.RegisterConfigs();
            MoonTweaks.RegisterConfigs();
            PlayerTweaks.RegisterConfigs();
            InventoryTweaks.RegisterConfigs();
            Config.SaveOnConfigSet = false;
            
            Harmony.CreateAndPatchAll(typeof(MenuManager_Patches));
            //Harmony.CreateAndPatchAll(typeof(IngamePlayerSettings_Patches));
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
            Compatibility.ReservedSlotCoreCompat = IsModPresent("FlipMods.ReservedItemSlotCore");

            if (Compatibility.ReservedSlotCoreCompat)
                Log.LogInfo("Found: ReservedItemSlotCore");

            Compatibility.ReservedSlotsWalkieCompat = IsModPresent("FlipMods.ReservedWalkieSlot");
            
            if (Compatibility.ReservedSlotsWalkieCompat)
                Log.LogInfo("Found: ReservedWalkieSlot");

            Compatibility.ReservedSlotsFlashlightCompat = IsModPresent("FlipMods.ReservedFlashlightSlot");

            if (Compatibility.ReservedSlotsFlashlightCompat)
                Log.LogInfo("Found: ReservedFlashlightSlot");

            Compatibility.LethalThingsCompat = IsModPresent("evaisa.lethalthings");

            if (Compatibility.LethalThingsCompat)
                Log.LogInfo("Found: LethalThings");
        }

        public static bool IsModPresent(string name)
        {
            foreach (var pluginInfo in Chainloader.PluginInfos)
            {
                if (name == pluginInfo.Value.Metadata.GUID)
                    return true;
            }

            return false;
        }
    }
}
