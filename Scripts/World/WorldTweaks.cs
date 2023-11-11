using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;

namespace MikesTweaks.Scripts.World
{
    public class WorldTweaks
    {
        private class Configs
        {
            public static string WorldTweaksSectionHeader => "WorldTweaks";

            public static readonly ConfigEntrySettings<float> GlobalTimeSpeedMulti =
                new ConfigEntrySettings<float>("GlobalTimeSpeedMultiplier", 0.5f, 1f);
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.GlobalTimeSpeedMulti.Entry = config.Bind(Configs.WorldTweaksSectionHeader,
                Configs.GlobalTimeSpeedMulti.ConfigName, Configs.GlobalTimeSpeedMulti.DefaultValue,
                Configs.GlobalTimeSpeedMulti.ConfigDesc);
        }

        [HarmonyPatch(typeof(TimeOfDay), "Start")]
        [HarmonyPostfix]
        private static void ChangeTimeSpeedMultiplier(TimeOfDay __instance)
        {
            __instance.globalTimeSpeedMultiplier = Configs.GlobalTimeSpeedMulti.Value;
        }
    }
}
