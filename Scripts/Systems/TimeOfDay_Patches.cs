using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.World;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Systems
{
    [HarmonyPatch(typeof(TimeOfDay))]
    public class TimeOfDay_Patches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void ChangeTimeSpeedMultiplier(TimeOfDay __instance)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            WorldTweaks.ReapplyConfigs(__instance);
        }
    }
}
