using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Player;
using MikesTweaks.Scripts.World;
using UnityEngine.UIElements.Internal;

namespace MikesTweaks.Scripts
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "mikes.lethalcompany.mikestweaks";
        public const string PluginName = "Mike's Tweaks";
        public const string PluginVersion = "1.2.2";
        public static ManualLogSource Log = null;

        private void Awake()
        {
            // Plugin startup logic
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginGUID} is loaded!");
            
            InventoryTweaks.RegisterConfigs(Config);
            PlayerTweaks.RegisterConfigs(Config);
            WorldTweaks.RegisterConfigs(Config);

            Harmony.CreateAndPatchAll(typeof(PlayerControllerB_Patches));
            Harmony.CreateAndPatchAll(typeof(InventoryTweaks));
            Harmony.CreateAndPatchAll(typeof(WorldTweaks));
        }
    }
}
