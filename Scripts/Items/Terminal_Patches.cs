using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Moons;
using MikesTweaks.Scripts.World;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Items
{
    [HarmonyPatch(typeof(Terminal))]
    public class Terminal_Patches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(Terminal __instance)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            MoonTweaks.ReapplyConfigs(__instance);
        }
    }
}
