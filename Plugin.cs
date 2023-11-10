using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Player;
using UnityEngine.UIElements.Internal;

namespace MikesTweaks
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "mikes.lethalcompany.mikestweaks";
        public const string PluginName = "Mike's Tweaks";
        public const string PluginVersion = "1.0";
        public static ManualLogSource Log = null;

        private void Awake()
        {
            // Plugin startup logic
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginGUID} is loaded!");

            Harmony harmony = new Harmony(PluginGUID);
            InventoryTweaks.RegisterConfigs(Config);
            InventoryTweaks.RegisterPatches(harmony);
            PlayerTweaks.RegisterConfigs(Config);
            PlayerTweaks.RegisterPatches(harmony);
        }
    }
}
