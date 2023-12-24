using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using Unity.Netcode;
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
        public static void Awake(HUDManager __instance)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            if (MikesTweaks.Compatibility.ReservedSlotCoreCompat || MikesTweaks.Compatibility.LethalThingsCompat)
                return;

            InventoryTweaks.ChangeItemSlotsAmountUI();
        }
    }
}
