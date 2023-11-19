using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using UnityEngine;
using System.Reflection;
using BepInEx.Configuration;
using MikesTweaks.Scripts.Items;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.World;
using Unity.Netcode;
using MikesTweaks.Scripts.Configs;

namespace MikesTweaks.Scripts.Inventory
{
    public class InventoryTweaks
    {
        public static class Configs
        {
            public static string InventoryTweaksSectionHeader => "InventoryTweaks";

            public static ConfigEntrySettings<int> ExtraItemSlotsAmount = new ConfigEntrySettings<int>("ExtraItemSlots", 2, 0, "This increases how many slots you have.\n0 Slots means you have the default 4 from the vanilla game, if you increase this number you get additional slots in addition to the original 4.");

            public static string TerminalItemWeightsSectionHeader => "TerminalItemWeights";

            public static ConfigEntrySettings<int>[] TerminalItemWeights =
                new ConfigEntrySettings<int>[]
                {
                    new ConfigEntrySettings<int>("WalkieTalkie", 0, 0),
                    new ConfigEntrySettings<int>("Flashlight", 0, 0),
                    new ConfigEntrySettings<int>("Shovel", 5, 18),
                    new ConfigEntrySettings<int>("LockPicker", 2, 15),
                    new ConfigEntrySettings<int>("ProFlashlight", 0, 5),
                    new ConfigEntrySettings<int>("StunGrenade", 2, 5),
                    new ConfigEntrySettings<int>("Boombox", 5, 15),
                    new ConfigEntrySettings<int>("TZPInhalant", 0, 0),
                    new ConfigEntrySettings<int>("ZapGun", 4, 10),
                    new ConfigEntrySettings<int>("Jetpack", 10, 50),
                    new ConfigEntrySettings<int>("ExtensionLadder", 0, 0),
                    new ConfigEntrySettings<int>("RadarBooster", 5, 18)
                };
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            MikesTweaks.Instance.BindConfig(ref Configs.ExtraItemSlotsAmount, Configs.InventoryTweaksSectionHeader);

            for (int i = 0; i < Configs.TerminalItemWeights.Length; i++)
                MikesTweaks.Instance.BindConfig(ref Configs.TerminalItemWeights[i], Configs.TerminalItemWeightsSectionHeader);

            ConfigsSynchronizer.OnConfigsChangedDelegate += ReapplyConfigs;
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => sizeof(int));
        }

        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.ExtraItemSlotsAmount.Value());

            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValue(out int Value);
            Configs.ExtraItemSlotsAmount.Entry.Value = Value;

            return payload;
        }

        public static bool HasEnoughSlots(int slotID)
        {
            return ((Configs.ExtraItemSlotsAmount.Value() + 4) - (slotID + 1)) > -1;
        }

        public static void ChangeItemSlotsAmount(PlayerControllerB __instance, bool force = false)
        {
            if (!force)
                if (Configs.ExtraItemSlotsAmount.Value() == 0)
                    return;

            __instance.ItemSlots = new GrabbableObject[4 + Configs.ExtraItemSlotsAmount.Value()];
        }

        public static void ReapplyConfigs()
        {
            GrabbableObject[] Items = GameObject.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);

            foreach (GrabbableObject item in Items)
            {
                GrabbableObject_Patches.ChangeTerminalItemWeights(item);
            }
        }
    }
}
