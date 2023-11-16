using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MikesTweaks.Scripts.Systems
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDManager_Patches
    {
        [HarmonyPatch("Awake")]
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
            if (InventoryTweaks.Configs.ExtraItemSlotsAmount.Value == 0)
                return;

            // Prepare the arrays
            Image[] ItemSlotIconFrames = new Image[4 + InventoryTweaks.Configs.ExtraItemSlotsAmount.Value];
            ItemSlotIconFrames[0] = HUDManager.Instance.itemSlotIconFrames[0];
            ItemSlotIconFrames[1] = HUDManager.Instance.itemSlotIconFrames[1];
            ItemSlotIconFrames[2] = HUDManager.Instance.itemSlotIconFrames[2];
            ItemSlotIconFrames[3] = HUDManager.Instance.itemSlotIconFrames[3];

            Image[] ItemSlotIcons = new Image[4 + InventoryTweaks.Configs.ExtraItemSlotsAmount.Value];
            ItemSlotIcons[0] = HUDManager.Instance.itemSlotIcons[0];
            ItemSlotIcons[1] = HUDManager.Instance.itemSlotIcons[1];
            ItemSlotIcons[2] = HUDManager.Instance.itemSlotIcons[2];
            ItemSlotIcons[3] = HUDManager.Instance.itemSlotIcons[3];

            GameObject Slot4 = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/Inventory/Slot3");
            GameObject CurrentSlot = Slot4;

            // Spawn more UI slots.
            for (int i = 0; i < InventoryTweaks.Configs.ExtraItemSlotsAmount.Value; i++)
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
