using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.World;

namespace MikesTweaks.Scripts.Items
{
    [HarmonyPatch(typeof(Terminal))]
    public class Terminal_Patches
    {
        [HarmonyPatch("Start")]
        public static void Start(Terminal __instance)
        {
            
        }
    }
}
