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
            public static bool LateGameUpgradesCompat = false;
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
            CheckCompatibilities();
            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll(typeof(MenuManager_Patches));
            //harmony.PatchAll(typeof(IngamePlayerSettings_Patches));
            harmony.PatchAll(typeof(HUDManager_Patches));
            harmony.PatchAll(typeof(NetworkManager_Patches));
            harmony.PatchAll(typeof(StartOfRound_Patches));
            harmony.PatchAll(typeof(TimeOfDay_Patches));
            harmony.PatchAll(typeof(InteractTrigger_Patches));
            harmony.PatchAll(typeof(Terminal_Patches));
            harmony.PatchAll(typeof(PlayerControllerB_Patches));
            harmony.PatchAll(typeof(GrabbableObject_Patches));


            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        private void CheckCompatibilities()
        {
            Compatibility.ReservedSlotCoreCompat = IsModPresent("FlipMods.ReservedItemSlotCore");
            Compatibility.ReservedSlotsWalkieCompat = IsModPresent("FlipMods.ReservedWalkieSlot");
            Compatibility.ReservedSlotsFlashlightCompat = IsModPresent("FlipMods.ReservedFlashlightSlot");
            Compatibility.LethalThingsCompat = IsModPresent("evaisa.lethalthings");
            Compatibility.LateGameUpgradesCompat = IsModPresent("com.malco.lethalcompany.moreshipupgrades");
        }

        public static bool IsModPresent(string name)
        {
            foreach (var pluginInfo in Chainloader.PluginInfos)
            {
                if (name == pluginInfo.Value.Metadata.GUID)
                {
                    Log.LogInfo($"Found: {pluginInfo.Value.Metadata.Name}");
                    return true;
                }
            }

            return false;
        }
    }
}
