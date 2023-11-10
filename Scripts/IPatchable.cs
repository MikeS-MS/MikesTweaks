using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;

namespace MikesTweaks.Scripts
{
    public interface IPatchable
    {
        public static void RegisterPatches(Harmony harmony)
        {
            
        }

        public static void RegisterConfigs(ConfigFile config)
        {

        }
    }
}
