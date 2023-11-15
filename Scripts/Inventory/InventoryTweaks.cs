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
using MikesTweaks.Scripts.Networking;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Inventory
{
    public class InventoryTweaks
    {
        public static class Configs
        {
            public static string InventoryTweaksSectionHeader => "InventoryTweaks";

            public static ConfigEntrySettings<int> ExtraItemSlotsAmount = new ConfigEntrySettings<int>("ExtraItemSlots", 2, 0);

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
            writer.WriteValueSafe(Configs.ExtraItemSlotsAmount.Value);

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
            return ((Configs.ExtraItemSlotsAmount.Value + 4) - (slotID + 1)) > -1;
        }

        public static void ChangeItemSlotsAmount(PlayerControllerB __instance, bool force = false)
        {
            if (!force)
                if (Configs.ExtraItemSlotsAmount.Value == 0)
                    return;

            __instance.ItemSlots = new GrabbableObject[4 + Configs.ExtraItemSlotsAmount.Value];
        }

        public static void ReapplyConfigs()
        {
            GrabbableObject[] Items = GameObject.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);

            foreach (GrabbableObject item in Items)
            {
                ChangeTerminalItemWeights(item);
            }
        }

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        public static void ChangeItemSlotsAmountUI(HUDManager __instance)
        {
            GameObject Inventory = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory");
            List<string> SlotNamesToIgnore = new List<string>() { "Slot0", "Slot1", "Slot2", "Slot3" };
            for (int i = 0; i < Inventory.transform.childCount; i++)
            {
                Transform child = Inventory.transform.GetChild(i);
                if (SlotNamesToIgnore.Contains(child.gameObject.name))
                    continue;

                Object.Destroy(child.gameObject);
            }
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

            int index = Array.FindIndex(Configs.TerminalItemWeights,
                (ConfigEntrySettings<int> config) => config.ConfigName == __instance.itemProperties.name);

            if (index == -1) return;

            __instance.itemProperties.weight = (((float)Configs.TerminalItemWeights[index].Value) / 100f) + 1f;

        }
    }
}
