﻿using BepInEx;
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
using MikesTweaks.Scripts.Utilities;

namespace MikesTweaks.Scripts.Inventory
{
    public class InventoryTweaks
    {
        public static class Configs
        {
            public static string InventoryTweaksSectionHeader => "InventoryTweaks";

            public static ConfigEntrySettings<int> ExtraItemSlotsAmount = new ConfigEntrySettings<int>("ExtraItemSlots", 2, 0, "This increases how many slots you have.\n0 Slots means you have the default 4 from the vanilla game, if you increase this number you get additional slots in addition to the original 4.");

            public static string TerminalItemProperties => "TerminalItemProperties";

            public static ConfigEntrySettings<int>[] ToolItemWeights =
                new ConfigEntrySettings<int>[]
                {
                    new ConfigEntrySettings<int>("WalkieTalkieWeight", 0, 0),
                    new ConfigEntrySettings<int>("FlashlightWeight", 0, 0),
                    new ConfigEntrySettings<int>("ShovelWeight", 5, 18),
                    new ConfigEntrySettings<int>("LockPickerWeight", 2, 15),
                    new ConfigEntrySettings<int>("ProFlashlightWeight", 0, 5),
                    new ConfigEntrySettings<int>("StunGrenadeWeight", 2, 5),
                    new ConfigEntrySettings<int>("BoomboxWeight", 5, 15),
                    new ConfigEntrySettings<int>("TZPInhalantWeight", 0, 0),
                    new ConfigEntrySettings<int>("ZapGunWeight", 4, 10),
                    new ConfigEntrySettings<int>("JetpackWeight", 10, 50),
                    new ConfigEntrySettings<int>("ExtensionLadderWeight", 0, 0),
                    new ConfigEntrySettings<int>("RadarBoosterWeight", 5, 18),
                    new ConfigEntrySettings<int>("SprayPaintWeight", 1, 1)
                };

            public static ConfigEntrySettings<int>[] ToolItemPrices =
                new ConfigEntrySettings<int>[]
                {
                    new ConfigEntrySettings<int>("WalkieTalkiePrice", 12, 12),
                    new ConfigEntrySettings<int>("FlashlightPrice", 15, 15),
                    new ConfigEntrySettings<int>("ShovelPrice", 30, 30),
                    new ConfigEntrySettings<int>("LockPickerPrice", 20, 20),
                    new ConfigEntrySettings<int>("ProFlashlightPrice", 25, 25),
                    new ConfigEntrySettings<int>("StunGrenadePrice", 30, 30),
                    new ConfigEntrySettings<int>("BoomboxPrice", 60, 60),
                    new ConfigEntrySettings<int>("TZPInhalantPrice", 120, 120),
                    new ConfigEntrySettings<int>("ZapGunPrice", 400, 400),
                    new ConfigEntrySettings<int>("JetpackPrice", 700, 700),
                    new ConfigEntrySettings<int>("ExtensionLadderPrice", 60, 60),
                    new ConfigEntrySettings<int>("RadarBoosterPrice", 60, 60),
                    new ConfigEntrySettings<int>("SprayPaintPrice", 50, 50)
                };
        }

        public static void RegisterConfigs()
        {
            MikesTweaks.Instance.BindConfig(ref Configs.ExtraItemSlotsAmount, Configs.InventoryTweaksSectionHeader);

            for (int i = 0; i < Configs.ToolItemWeights.Length; i++)
            {
                MikesTweaks.Instance.BindConfig(ref Configs.ToolItemWeights[i], Configs.TerminalItemProperties);
                MikesTweaks.Instance.BindConfig(ref Configs.ToolItemPrices[i], Configs.TerminalItemProperties);
            }

            ConfigsSynchronizer.OnConfigsChangedDelegate += () =>
            {
                ReapplyConfigs();
                ApplyItemPrices(WorldTweaks.TerminalInstance);
            };
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => sizeof(int) + sizeof(int) * Configs.ToolItemWeights.Length + sizeof(int) * Configs.ToolItemPrices.Length);
        }

        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.ExtraItemSlotsAmount.Value());
            foreach (var item in Configs.ToolItemWeights)
            {
                writer.WriteValueSafe(item.Value(WorldTweaks.Configs.UseVanillaToolItemWeights.Value()));
            }
            foreach (var item in Configs.ToolItemPrices)
            {
                writer.WriteValueSafe(item.Value(WorldTweaks.Configs.UseVanillaToolItemPrices.Value()));
            }
            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValueSafe(out int Value);
            Configs.ExtraItemSlotsAmount.Entry.Value = Value;

            foreach (var item in Configs.ToolItemWeights)
            {
                payload.ReadValueSafe(out Value);
                item.Entry.Value = Value;
            }

            foreach (var item in Configs.ToolItemPrices)
            {
                payload.ReadValueSafe(out Value);
                item.Entry.Value = Value;
            }
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
            GrabbableObject[] Items = Resources.FindObjectsOfTypeAll<GrabbableObject>();

            foreach (GrabbableObject item in Items)
            {
                ModifyItemWeight(item);
            }

            ApplyItemPrices(WorldTweaks.TerminalInstance);
        }

        public static void ApplyItemPrices(Terminal terminal)
        {
            if (!terminal)
                return;

            TerminalKeyword Buy = Array.Find(terminal.terminalNodes.allKeywords, (TerminalKeyword keyword) => keyword.name.ToLower() == "buy");

            if (Buy == null)
                return;

            bool useVanillaPrices = WorldTweaks.Configs.UseVanillaToolItemPrices.Value();

            foreach (var buyItem in Buy.compatibleNouns)
            {
                foreach (var item in Configs.ToolItemPrices)
                {
                    string buyItemName = buyItem.noun.name.ToLower();
                    StringUtils.RemoveChar(ref buyItemName, ' ');
                    if (!item.ConfigName.ToLower().Contains(buyItemName))
                        continue;

                    buyItem.result.itemCost = item.Value(useVanillaPrices);
                    terminal.buyableItemsList[buyItem.result.buyItemIndex].creditsWorth = item.Value(useVanillaPrices);
                    foreach (var confirmAction in buyItem.result.terminalOptions)
                    {
                        if (confirmAction.noun.name.ToLower().Contains("deny"))
                            continue;

                        confirmAction.result.itemCost = item.Value(useVanillaPrices);
                        break;
                    }
                    break;
                }
            }
        }

        public static void ModifyItemWeight(GrabbableObject item)
        {
            if (item == null)
                return;

            string itemName = item.itemProperties.name.ToLower();
            StringUtils.RemoveChar(ref itemName, ' ');
            int index = Array.FindIndex(InventoryTweaks.Configs.ToolItemWeights,
                (ConfigEntrySettings<int> config) => config.ConfigName.ToLower().Contains(itemName));

            if (index == -1) return;

            item.itemProperties.weight = (((float)InventoryTweaks.Configs.ToolItemWeights[index].Value(WorldTweaks.Configs.UseVanillaToolItemWeights.Value())) / 100f) + 1f;
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
