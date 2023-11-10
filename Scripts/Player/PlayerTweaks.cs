using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using MikesTweaks.Scripts.Inventory;
using System.Reflection;

namespace MikesTweaks.Scripts.Player
{
    internal class PlayerTweaks
    {
        private static class Configs
        {
            public static string PlayerTweaksSectionHeader => "PlayerTweaks";

            public static float SprintLongevityDefaultValue => 12f;
            public static string SprintLongevityConfigName => "SprintLongevity";
            public static string SprintLongevityConfigDesc => "Higher Values increase the time allowed to sprint\nVanilla Default: 5f";
            public static ConfigEntry<float> SprintLongevityValue = null;
        }

        public static void RegisterPatches(Harmony harmony)
        {
            MethodInfo PlayerControllerB_Awake = AccessTools.Method(typeof(PlayerControllerB), "Awake");
            MethodInfo PlayerControllerB_PostAwakeMethod = AccessTools.Method(typeof(PlayerTweaks), "ModifySprintLongevity");

            harmony.Patch(PlayerControllerB_Awake, null, new HarmonyMethod(PlayerControllerB_PostAwakeMethod));
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.SprintLongevityValue = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintLongevityConfigName, Configs.SprintLongevityDefaultValue,
                Configs.SprintLongevityConfigDesc);
        }

        private static void ModifySprintLongevity(PlayerControllerB __instance)
        {
            __instance.sprintTime = Configs.SprintLongevityValue.Value;
        }
    }
}
