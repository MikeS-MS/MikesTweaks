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
        private class ItemConfigEntry
        {
            public ItemConfigEntry(int defaultWeightCost, int vanillaWeightCost)
            {
                DefaultWeightCost = defaultWeightCost;
                VanillaWeightCost = vanillaWeightCost;
            }

            private readonly int VanillaWeightCost = 0;
            public int DefaultWeightCost { get; private set; }
            public string ConfigDesc => $"Vanilla Default: {VanillaWeightCost} lb";
            public ConfigEntry<int> WeightCost;

        }

        private static class Configs
        {
            public static string InventoryTweaksSectionHeader => "InventoryTweaks";

            public static string ExtraItemSlotsAmountConfigName => "ExtraItemSlots";
            public static string ExtraItemSlotsAmountConfigDesc => "Vanilla Default: 0 slots";
            public static ConfigEntry<int> ExtraItemSlotsAmount { get; set; }

            public static string TerminalItemWeightsSectionHeader => "TerminalItemWeights";

            public static readonly Dictionary<string, ItemConfigEntry> TerminalItemWeights = new Dictionary<string, ItemConfigEntry>() {
                {"WalkieTalkie", new ItemConfigEntry(0, 0)},
                {"Flashlight", new ItemConfigEntry(0, 0)},
                {"Shovel", new ItemConfigEntry(5, 18)},
                {"LockPicker", new ItemConfigEntry(2, 15)},
                {"ProFlashlight", new ItemConfigEntry(0, 5)},
                {"StunGrenade", new ItemConfigEntry(2, 5)},
                {"Boombox", new ItemConfigEntry(5, 15)},
                {"TZPInhalant", new ItemConfigEntry(0, 0)},
                {"ZapGun", new ItemConfigEntry(4, 10)},
                {"Jetpack", new ItemConfigEntry(10, 50)},
                {"ExtensionLadder", new ItemConfigEntry(0, 0)}
            };

        }

        public static void RegisterPatches(Harmony harmony) 
        {
            MethodInfo PlayerControllerB_Awake = AccessTools.Method(typeof(PlayerControllerB), "Awake");
            MethodInfo PlayerControllerB_PostAwakeMethod = AccessTools.Method(typeof(InventoryTweaks), "ChangeItemSlotsAmount");            
            
            MethodInfo HUDManager_Awake = AccessTools.Method(typeof(HUDManager), "Awake");
            MethodInfo HUDManager_PostAwakeMethod = AccessTools.Method(typeof(InventoryTweaks), "ChangeItemSlotsAmountUI");

            MethodInfo GrabbableObject_Start = AccessTools.Method(typeof(GrabbableObject), "Start");
            MethodInfo GrabbableObject_PostStartMethod = AccessTools.Method(typeof(InventoryTweaks), "ChangeTerminalItemWeights");

            harmony.Patch(PlayerControllerB_Awake, null, new HarmonyMethod(PlayerControllerB_PostAwakeMethod));
            harmony.Patch(HUDManager_Awake, null, new HarmonyMethod(HUDManager_PostAwakeMethod));
            harmony.Patch(GrabbableObject_Start, null, new HarmonyMethod(GrabbableObject_PostStartMethod));
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.ExtraItemSlotsAmount = config.Bind(Configs.InventoryTweaksSectionHeader, Configs.ExtraItemSlotsAmountConfigName, 2, Configs.ExtraItemSlotsAmountConfigDesc);

            foreach (string key in Configs.TerminalItemWeights.Keys)
            {
                Configs.TerminalItemWeights[key].WeightCost = config.Bind(Configs.TerminalItemWeightsSectionHeader,
                    key, Configs.TerminalItemWeights[key].DefaultWeightCost, Configs.TerminalItemWeights[key].ConfigDesc);
            }
        }

        private static void ChangeItemSlotsAmount(PlayerControllerB __instance)
        {
            __instance.ItemSlots = new GrabbableObject[4 + Configs.ExtraItemSlotsAmount.Value];
        }

        private static void ChangeItemSlotsAmountUI(HUDManager __instance)
        {
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

        private static void ChangeTerminalItemWeights(GrabbableObject __instance)
        {
            if (__instance == null)
                return;

            if (!Configs.TerminalItemWeights.TryGetValue(__instance.itemProperties.name, out var Entry))
                return;

            __instance.itemProperties.weight = (((float)Entry.WeightCost.Value) / 100f) + 1f;
        }
    }
}
