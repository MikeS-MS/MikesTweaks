using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Configs;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.World;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Items
{
    [HarmonyPatch(typeof(GrabbableObject))]
    public class GrabbableObject_Patches
    {
        [HarmonyPatch(nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        public static void ChangeTerminalItemWeights(GrabbableObject __instance)
        {
            if (MikesTweaks.DebugMode)
                MikesTweaks.Log.LogInfo($"{__instance.itemProperties.name}: {__instance.itemProperties.weight}");

            if (!NetworkManager.Singleton.IsServer && !ConfigsSynchronizer.ConfigsReceived)
                return;

            InventoryTweaks.ModifyItemWeight(__instance);
        }
    }
}
