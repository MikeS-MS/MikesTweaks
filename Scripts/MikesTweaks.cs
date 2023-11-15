using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.Player;
using MikesTweaks.Scripts.World;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.UIElements.Internal;

namespace MikesTweaks.Scripts
{
    [BepInPlugin(GUID, Name, Version)]
    public class MikesTweaks : BaseUnityPlugin
    {
        public const string GUID = "mikes.lethalcompany.mikestweaks";
        public const string Name = "Mike's Tweaks";
        public const string Version = "1.4";
        public static ManualLogSource Log = null;
        public static MikesTweaks Instance { get; private set; } = null;

        public void LoadConfigs()
        {
            Config.Reload();
        }

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            InventoryTweaks.RegisterConfigs(Config);
            PlayerTweaks.RegisterConfigs(Config);
            WorldTweaks.RegisterConfigs(Config);
            Config.SaveOnConfigSet = false;

            Harmony.CreateAndPatchAll(typeof(NetworkManager_Patches));
            Harmony.CreateAndPatchAll(typeof(ConfigsRelated_Patches));
            Harmony.CreateAndPatchAll(typeof(PlayerControllerB_Patches));
            Harmony.CreateAndPatchAll(typeof(InventoryTweaks));
            Harmony.CreateAndPatchAll(typeof(WorldTweaks));

            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }
    }
}
