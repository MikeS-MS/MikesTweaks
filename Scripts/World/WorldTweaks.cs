using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Networking;
using Unity.Netcode;

namespace MikesTweaks.Scripts.World
{
    public class WorldTweaks
    {
        private class Configs
        {
            public static string WorldTweaksSectionHeader => "WorldTweaks";

            public static ConfigEntrySettings<float> GlobalTimeSpeedMulti =
                new ConfigEntrySettings<float>("GlobalTimeSpeedMultiplier", 0.5f, 1f);
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.GlobalTimeSpeedMulti.Entry = config.Bind(Configs.WorldTweaksSectionHeader,
                Configs.GlobalTimeSpeedMulti.ConfigName, Configs.GlobalTimeSpeedMulti.DefaultValue,
                Configs.GlobalTimeSpeedMulti.ConfigDesc);

            ConfigsSynchronizer.OnConfigsChangedDelegate += () => ReapplyConfigs(TimeOfDay.Instance);
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => sizeof(float));
        }

        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.GlobalTimeSpeedMulti.Value);

            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValue(out float Value);
            Configs.GlobalTimeSpeedMulti.Entry.Value = Value;

            return payload;
        }

        public static void ReapplyConfigs(TimeOfDay timeOfDay)
        {
            timeOfDay.globalTimeSpeedMultiplier = Configs.GlobalTimeSpeedMulti.Value;
        }

        [HarmonyPatch(typeof(TimeOfDay), "Start")]
        [HarmonyPostfix]
        private static void ChangeTimeSpeedMultiplier(TimeOfDay __instance)
        {
            ReapplyConfigs(__instance);
        }
    }
}
