using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.World;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Environment
{
    [HarmonyPatch(typeof(InteractTrigger))]
    public class InteractTrigger_Patches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(InteractTrigger __instance)
        {
            Terminal terminal = __instance.gameObject.GetComponent<Terminal>();
            if (terminal == null)
                return;

            WorldTweaks.TerminalInstance = terminal;
            WorldTweaks.TerminalInteractTriggerInstance = __instance;
        }

        [HarmonyPatch("UpdateUsedByPlayerClientRpc")]
        [HarmonyPostfix]
        private static void Interact(InteractTrigger __instance)
        {
            if (!WorldTweaks.CanInteractWithTerminal(__instance))
                return;

            WorldTweaks.ResetValues(__instance);
        }

        [HarmonyPatch("StopUsingClientRpc")]
        [HarmonyPostfix]
        private static void StopUsingClientRpc(InteractTrigger __instance)
        {
            if (!WorldTweaks.CanInteractWithTerminal(__instance))
                return;

            WorldTweaks.MakeTerminalUnusableForAnyoneButHost();
            WorldTweaks.ResetValues(__instance);
        }
    }
}
