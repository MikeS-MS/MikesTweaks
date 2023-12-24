using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Networking;

namespace MikesTweaks.Scripts.Systems
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManager_Patches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void MenuManager_Start(MenuManager __instance)
        {
            MikesTweaks.Instance.LoadConfigs();
            CustomNetworking.Instance.UnregisterChannels();
        }
    }
}
