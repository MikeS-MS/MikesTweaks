using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using UnityEngine;
using System.Reflection;
using BepInEx.Configuration;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MikesTweaks.Scripts.Inventory
{
    public class InventoryTweaks
    {

        private static class Configs
        {
            public static string InventoryTweaksSectionHeader => "InventoryTweaks";

            public static readonly ConfigEntrySettings<int> ExtraItemSlotsAmount = new ConfigEntrySettings<int>("ExtraItemSlots", 2, 0);

            public static string TerminalItemWeightsSectionHeader => "TerminalItemWeights";

            public static readonly Dictionary<string, ConfigEntrySettings<int>> TerminalItemWeights =
                new Dictionary<string, ConfigEntrySettings<int>>()
                {
                    {"WalkieTalkie", new ConfigEntrySettings<int>("WalkieTalkie", 0, 0)},
                    {"Flashlight", new ConfigEntrySettings<int>("Flashlight", 0, 0)},
                    {"Shovel", new ConfigEntrySettings<int>("Shovel", 5, 18)},
                    {"LockPicker", new ConfigEntrySettings<int>("LockPicker", 2, 15)},
                    {"ProFlashlight", new ConfigEntrySettings<int>("ProFlashlight", 0, 5)},
                    {"StunGrenade", new ConfigEntrySettings<int>("StunGrenade", 2, 5)},
                    {"Boombox", new ConfigEntrySettings<int>("Boombox", 5, 15)},
                    {"TZPInhalant", new ConfigEntrySettings<int>("TZPInhalant", 0, 0)},
                    {"ZapGun", new ConfigEntrySettings<int>("ZapGun", 4, 10)},
                    {"Jetpack", new ConfigEntrySettings<int>("Jetpack", 10, 50)},
                    {"ExtensionLadder", new ConfigEntrySettings<int>("ExtensionLadder", 0, 0)}
                };

        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.ExtraItemSlotsAmount.Entry = config.Bind(Configs.InventoryTweaksSectionHeader, Configs.ExtraItemSlotsAmount.ConfigName, Configs.ExtraItemSlotsAmount.DefaultValue, Configs.ExtraItemSlotsAmount.ConfigDesc);

            foreach (string key in Configs.TerminalItemWeights.Keys)
            {
                Configs.TerminalItemWeights[key].Entry = config.Bind(Configs.TerminalItemWeightsSectionHeader,
                    key, Configs.TerminalItemWeights[key].DefaultValue, Configs.TerminalItemWeights[key].ConfigDesc);
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        private static void ChangeItemSlotsAmount(PlayerControllerB __instance)
        {
            if (Configs.ExtraItemSlotsAmount.Value == 0)
                return;

            __instance.ItemSlots = new GrabbableObject[4 + Configs.ExtraItemSlotsAmount.Value];
        }

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        private static void ChangeItemSlotsAmountUI(HUDManager __instance)
        {
            if (Configs.ExtraItemSlotsAmount.Value == 0)
                return;

            // Prepare the arrays
            Image[] ItemSlotIconFrames = new Image[4 + Configs.ExtraItemSlotsAmount.Value];
            ItemSlotIconFrames[0] = HUDManager.Instance.itemSlotIconFrames[0];
            ItemSlotIconFrames[1] = HUDManager.Instance.itemSlotIconFrames[1];
            ItemSlotIconFrames[2] = HUDManager.Instance.itemSlotIconFrames[2];
            ItemSlotIconFrames[3] = HUDManager.Instance.itemSlotIconFrames[3];

            Image[] ItemSlotIcons = new Image[4 + Configs.ExtraItemSlotsAmount.Value];
            ItemSlotIcons[0] = HUDManager.Instance.itemSlotIcons[0];
            ItemSlotIcons[1] = HUDManager.Instance.itemSlotIcons[1];
            ItemSlotIcons[2] = HUDManager.Instance.itemSlotIcons[2];
            ItemSlotIcons[3] = HUDManager.Instance.itemSlotIcons[3];

            GameObject Inventory = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory");
            GameObject Slot4 = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory/Slot3");
            GameObject CurrentSlot = Slot4;

            // Spawn more UI slots.
            for (int i = 0; i < Configs.ExtraItemSlotsAmount.Value; i++)
            {
                GameObject NewSlot = Object.Instantiate(Slot4);
                NewSlot.name = $"Slot{ 3 + (i + 1) }";
                NewSlot.transform.parent = Inventory.transform;

                // Change locations.
                Vector3 localPosition = CurrentSlot.transform.localPosition;
                NewSlot.transform.SetLocalPositionAndRotation(
                                                            new Vector3(localPosition.x + 50, localPosition.y, localPosition.z), 
                                                            CurrentSlot.transform.localRotation);
                CurrentSlot = NewSlot;

                ItemSlotIconFrames[3 + (i + 1)] = NewSlot.GetComponent<Image>();
                ItemSlotIcons[3 + (i + 1)] = NewSlot.transform.GetChild(0).GetComponent<Image>(); ;
            }

            HUDManager.Instance.itemSlotIconFrames = ItemSlotIconFrames;
            HUDManager.Instance.itemSlotIcons = ItemSlotIcons;
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        private static void ChangeTerminalItemWeights(GrabbableObject __instance)
        {
            if (__instance == null)
                return;

            if (!Configs.TerminalItemWeights.TryGetValue(__instance.itemProperties.name, out var Entry))
                return;

            __instance.itemProperties.weight = (((float)Entry.Value) / 100f) + 1f;
        }
    }
}
