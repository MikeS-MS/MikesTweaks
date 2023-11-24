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
using MikesTweaks.Scripts.Player;

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

        public static void RegisterConfigs()
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
            return ((PlayerTweaks.LocalPlayerController.ItemSlots.Length) - (slotID + 1)) > -1;
        }

        public static void ChangeItemSlotsAmount(PlayerControllerB __instance, bool force = false)
        {
            if (!force)
                if (Configs.ExtraItemSlotsAmount.Value() == 0)
                    return;

            List<GrabbableObject> objects = new List<GrabbableObject>(__instance.ItemSlots);
            __instance.ItemSlots = new GrabbableObject[4 + Configs.ExtraItemSlotsAmount.Value()];
            for (int i = 0; i < objects.Count; i++) 
            {
                __instance.ItemSlots[i] = objects[i];
            }
        }

        public static void ReapplyConfigs()
        {
            GrabbableObject[] Items = GameObject.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);

            foreach (GrabbableObject item in Items)
            {
                ModifyItemWeight(item);
            }
        }

        public static void ModifyItemWeight(GrabbableObject item)
        {
            if (item == null)
                return;

            string itemName = item.itemProperties.name;
            int index = Array.FindIndex(InventoryTweaks.Configs.TerminalItemWeights,
                (ConfigEntrySettings<int> config) => config.ConfigName == itemName);

            if (index == -1) return;

            item.itemProperties.weight = (((float)InventoryTweaks.Configs.TerminalItemWeights[index].Value(WorldTweaks.Configs.UseVanillaTerminalItemWeights.Value())) / 100f) + 1f;
        }

        public static void ChangeItemSlotsAmountUI()
        {
            if (Configs.ExtraItemSlotsAmount.Value() == 0)
                return;

            GameObject Inventory = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory");
            List<string> SlotNamesToIgnore = new List<string>() { "Slot0", "Slot1", "Slot2", "Slot3" };
            for (int i = 0; i < Inventory.transform.childCount; i++)
            {
                Transform child = Inventory.transform.GetChild(i);
                if (SlotNamesToIgnore.Contains(child.gameObject.name))
                    continue;

                Object.Destroy(child.gameObject);
            }

            // Prepare the arrays
            Image[] ItemSlotIconFrames = new Image[4 + InventoryTweaks.Configs.ExtraItemSlotsAmount.Value()];
            ItemSlotIconFrames[0] = HUDManager.Instance.itemSlotIconFrames[0];
            ItemSlotIconFrames[1] = HUDManager.Instance.itemSlotIconFrames[1];
            ItemSlotIconFrames[2] = HUDManager.Instance.itemSlotIconFrames[2];
            ItemSlotIconFrames[3] = HUDManager.Instance.itemSlotIconFrames[3];

            Image[] ItemSlotIcons = new Image[4 + InventoryTweaks.Configs.ExtraItemSlotsAmount.Value()];
            ItemSlotIcons[0] = HUDManager.Instance.itemSlotIcons[0];
            ItemSlotIcons[1] = HUDManager.Instance.itemSlotIcons[1];
            ItemSlotIcons[2] = HUDManager.Instance.itemSlotIcons[2];
            ItemSlotIcons[3] = HUDManager.Instance.itemSlotIcons[3];

            GameObject Slot4 = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory/Slot3");
            GameObject CurrentSlot = Slot4;

            // Spawn more UI slots.
            for (int i = 0; i < InventoryTweaks.Configs.ExtraItemSlotsAmount.Value(); i++)
            {
                GameObject NewSlot = Object.Instantiate(Slot4);
                NewSlot.name = $"Slot{3 + (i + 1)}";
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
    }
}
