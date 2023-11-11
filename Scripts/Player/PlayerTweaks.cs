using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameNetcodeStuff;
using MikesTweaks.Scripts.Inventory;
using System.Reflection;
using UnityEngine;
using System.Numerics;
using System.Reflection.Emit;
using Vector3 = UnityEngine.Vector3;

namespace MikesTweaks.Scripts.Player
{
    [HarmonyPatch]
    public class PlayerTweaks
    {
        public static class Configs
        {
            public static string PlayerTweaksSectionHeader => "PlayerTweaks";

            public static readonly ConfigEntrySettings<float> SprintLongevity =
                new ConfigEntrySettings<float>("SprintLongevity", 12, 5);

            public static readonly ConfigEntrySettings<float> DefaultSprintMultiplier =
                new ConfigEntrySettings<float>("DefaultSprintMultiplier", 1.5f, 1f);

            public static readonly ConfigEntrySettings<float> SprintMultiplierIncrease =
                new ConfigEntrySettings<float>("SprintMultiplierIncrease", 1f, 1f);

            public static readonly ConfigEntrySettings<float> SprintMultiplierDecrease =
                new ConfigEntrySettings<float>("SprintMultiplierDecrease", 10f, 10f);

            public static readonly ConfigEntrySettings<float> MaxSprintMultiplier =
                new ConfigEntrySettings<float>("MaxSprintMultiplier", 3f, 2.25f);

            public static readonly ConfigEntrySettings<float> JumpStaminaDrain =
                new ConfigEntrySettings<float>("JumpStaminaDrain", 0.04f, 0.08f);
        }

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.SprintLongevity.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintLongevity.ConfigName, Configs.SprintLongevity.DefaultValue,
                Configs.SprintLongevity.ConfigDesc);

            Configs.JumpStaminaDrain.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.JumpStaminaDrain.ConfigName, Configs.JumpStaminaDrain.DefaultValue,
                Configs.JumpStaminaDrain.ConfigDesc);

            Configs.DefaultSprintMultiplier.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.DefaultSprintMultiplier.ConfigName, Configs.DefaultSprintMultiplier.DefaultValue,
                Configs.DefaultSprintMultiplier.ConfigDesc);

            Configs.SprintMultiplierIncrease.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintMultiplierIncrease.ConfigName, Configs.SprintMultiplierIncrease.DefaultValue,
                Configs.SprintMultiplierIncrease.ConfigDesc);

            Configs.SprintMultiplierDecrease.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintMultiplierDecrease.ConfigName, Configs.SprintMultiplierDecrease.DefaultValue,
                Configs.SprintMultiplierDecrease.ConfigDesc);

            Configs.MaxSprintMultiplier.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.MaxSprintMultiplier.ConfigName, Configs.MaxSprintMultiplier.DefaultValue,
                Configs.MaxSprintMultiplier.ConfigDesc);
        }

    }

    // Transpilers for PlayerControllerB
    [HarmonyPatch(typeof(PlayerControllerB))]
    public static class PlayerControllerB_Patches
    {
        // Change the arbitrary values inside the Update() method to use the values of modifiable variables.
        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ModifySprintMultiplierValues(IEnumerable<CodeInstruction> instructions)
        {
            float MaxSprintValue = 2.25f;
            float SprintMultiIncreaseValue = 1f;
            float DefaultSprintValue = 1f;
            float SprintMultiDecreaseValue = 10f;

            int indexOfMaxSprintMultiplier = -1;
            bool patchedSprintMultiIncrease = false;
            bool patchedDefaultSprintMultiplier = false;
            bool patchedSprintMultiDecrease = false;

            List<CodeInstruction> toListInstructions = new List<CodeInstruction>(instructions);
            for (int i = 0; i < toListInstructions.Count; i++)
            {
                var instruction = toListInstructions[i];
                if (instruction.opcode != OpCodes.Ldc_R4)
                    continue;

                if (Math.Abs((float)instruction.operand - MaxSprintValue) > 0.1)
                    continue;

                indexOfMaxSprintMultiplier = i;
                toListInstructions[i].operand = PlayerTweaks.Configs.MaxSprintMultiplier.Value;
                break;
            }

            if (indexOfMaxSprintMultiplier == -1)
                return toListInstructions.AsEnumerable();

            for (int i = indexOfMaxSprintMultiplier; i < toListInstructions.Count; i++)
            {
                var instruction = toListInstructions[i];
                if (instruction.opcode == OpCodes.Ldc_R4)
                {
                    if (patchedDefaultSprintMultiplier && patchedSprintMultiDecrease && patchedSprintMultiIncrease)
                        break;

                    if (!patchedSprintMultiIncrease && Math.Abs((float)instruction.operand - SprintMultiIncreaseValue) < 0.1)
                    {
                        toListInstructions[i].operand = PlayerTweaks.Configs.SprintMultiplierIncrease.Value;
                        patchedSprintMultiIncrease = true;
                        continue;
                    }
                    if (!patchedDefaultSprintMultiplier && Math.Abs((float)instruction.operand - DefaultSprintValue) < 0.1)
                    {
                        toListInstructions[i].operand = PlayerTweaks.Configs.DefaultSprintMultiplier.Value;
                        patchedDefaultSprintMultiplier = true;
                        continue;
                    }
                    if (!patchedSprintMultiDecrease && Math.Abs((float)instruction.operand - SprintMultiDecreaseValue) < 0.1)
                    {
                        toListInstructions[i].operand = PlayerTweaks.Configs.SprintMultiplierDecrease.Value;
                        patchedSprintMultiDecrease = true;
                        continue;
                    }
                }
            }

            return toListInstructions.AsEnumerable();
        }

        [HarmonyPatch("Jump_performed")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ModifyJumpDrain(IEnumerable<CodeInstruction> instructions)
        {
            float JumpDrainValue = 0.08f;

            List<CodeInstruction> toListInstructions = new List<CodeInstruction>(instructions);
            for (int i = 0; i < toListInstructions.Count; i++)
            {
                var instruction = toListInstructions[i];
                if (instruction.opcode != OpCodes.Ldc_R4)
                    continue;

                if (Math.Abs((float)instruction.operand - JumpDrainValue) > 0.01f)
                    continue;

                toListInstructions[i].operand = PlayerTweaks.Configs.JumpStaminaDrain.Value;
                break;
            }

            return toListInstructions.AsEnumerable();
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void ModifySprintLongevity(PlayerControllerB __instance)
        {
            __instance.sprintTime = PlayerTweaks.Configs.SprintLongevity.Value;
        }
    }
}
