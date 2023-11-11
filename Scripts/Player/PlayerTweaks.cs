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

            public static readonly ConfigEntrySettings<float> SprintLongevity =
                new ConfigEntrySettings<float>("SprintLongevity", 12, 5);
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.SprintLongevity.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintLongevity.ConfigName, Configs.SprintLongevity.DefaultValue,
                Configs.SprintLongevity.ConfigDesc);
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        private static void ModifySprintLongevity(PlayerControllerB __instance)
        {
            __instance.sprintTime = Configs.SprintLongevity.Value;
        }
    }
}
