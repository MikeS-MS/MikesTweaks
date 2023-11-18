using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.World;

namespace MikesTweaks.Scripts.Items
{
    [HarmonyPatch(typeof(GrabbableObject))]
    public class GrabbableObject_Patches
    {
        [HarmonyPatch(nameof(GrabbableObject.Start))]
        [HarmonyPostfix]
        public static void ChangeTerminalItemWeights(GrabbableObject __instance)
        {
            if (__instance == null)
                return;

            int index = Array.FindIndex(InventoryTweaks.Configs.TerminalItemWeights,
                (ConfigEntrySettings<int> config) => config.ConfigName == __instance.itemProperties.name);

            if (index == -1) return;

            __instance.itemProperties.weight = (((float)InventoryTweaks.Configs.TerminalItemWeights[index].Value(WorldTweaks.Configs.UseVanillaTerminalItemWeights.Value())) / 100f) + 1f;

        }
    }
}
