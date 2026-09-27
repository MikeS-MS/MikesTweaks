using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Reflection.Emit;
using UnityEngine.InputSystem;
using BepInEx.Logging;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.World;
using Unity.Netcode;
using MikesTweaks.Scripts.Configs;

namespace MikesTweaks.Scripts.Player
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public static class PlayerControllerB_Patches
    {
        private static PlayerInputRedirection inputRedirection = null;

        public static void SetupKeybinds(PlayerControllerB player)
        {
            if (!PlayerTweaks.IsLocallyControlled(player))
                return;

            inputRedirection = player.gameObject.GetComponent<PlayerInputRedirection>();
            inputRedirection.InitializeKeybinds();
        }

        private static bool IsLdarg0(CodeInstruction instruction) => instruction.opcode == OpCodes.Ldarg_0;

        private static bool IsLdloc(CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Ldloc
                || instruction.opcode == OpCodes.Ldloc_S
                || instruction.opcode == OpCodes.Ldloc_0
                || instruction.opcode == OpCodes.Ldloc_1
                || instruction.opcode == OpCodes.Ldloc_2
                || instruction.opcode == OpCodes.Ldloc_3;
        }

        private static bool IsSprintMeterField(CodeInstruction instruction, OpCode opcode)
        {
            return instruction.opcode == opcode && instruction.operand is FieldInfo field && field.Name == nameof(PlayerControllerB.sprintMeter);
        }

        private static bool TryReadFloat(CodeInstruction instruction, out float value)
        {
            value = 0f;
            if (instruction.opcode != OpCodes.Ldc_R4 || !(instruction.operand is float constant))
                return false;

            value = constant;
            return true;
        }

        private static void MoveMarks(CodeInstruction from, CodeInstruction to)
        {
            if (from.labels != null && from.labels.Count > 0)
            {
                to.labels.AddRange(from.labels);
                from.labels.Clear();
            }

            if (from.blocks != null && from.blocks.Count > 0)
            {
                to.blocks.AddRange(from.blocks);
                from.blocks.Clear();
            }
        }

        // The recharge sites are Mathf.Clamp assignments into sprintMeter.
        // Hindered walking multiplies by 0.5, standing still adds 4, walking adds 9.
        // The float multiplied in is whatever local the game uses for that factor, not local 0.
        private static bool TryGetStaminaRechargeReplacement(List<CodeInstruction> instructions, int end, out int start, out CodeInstruction localLoad, out MethodInfo replacement)
        {
            start = -1;
            localLoad = null;
            replacement = null;

            int windowStart = Math.Max(0, end - 30);
            for (int i = end - 3; i >= windowStart; i--)
            {
                if (IsLdarg0(instructions[i]) && IsLdarg0(instructions[i + 1]) && IsSprintMeterField(instructions[i + 2], OpCodes.Ldfld))
                {
                    start = i;
                    break;
                }
            }

            if (start < 0)
                return false;

            bool sawClamp = false;
            float? matchedConstant = null;
            for (int i = start; i <= end; i++)
            {
                CodeInstruction instruction = instructions[i];
                if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo method && method.Name == nameof(Mathf.Clamp))
                    sawClamp = true;

                if (IsLdloc(instruction))
                    localLoad = instruction;

                if (TryReadFloat(instruction, out float constant))
                {
                    if (Math.Abs(constant - 0.5f) <= 0.01f || Math.Abs(constant - 4f) <= 0.01f || Math.Abs(constant - 9f) <= 0.01f)
                    {
                        if (matchedConstant != null)
                            return false;

                        matchedConstant = constant;
                    }
                }
            }

            if (!sawClamp || localLoad == null || matchedConstant == null)
                return false;

            string helperName;
            if (Math.Abs(matchedConstant.Value - 0.5f) <= 0.01f)
                helperName = nameof(PlayerTweaks.StaminaRechargeMovementHinderedWalking);
            else if (Math.Abs(matchedConstant.Value - 4f) <= 0.01f)
                helperName = nameof(PlayerTweaks.StaminaRechargeMovementNotHinderedNotWalking);
            else
                helperName = nameof(PlayerTweaks.StaminaRechargeMovementNotHinderedWalking);

            replacement = AccessTools.Method(typeof(PlayerTweaks), helperName, new[] { typeof(PlayerControllerB), typeof(float) });
            return replacement != null;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(PlayerControllerB __instance)
        {
            GameObject playerGameObject = __instance.gameObject;
            playerGameObject.AddComponent<PlayerInputRedirection>();

            if (!NetworkManager.Singleton.IsServer)
                return;

            PlayerTweaks.ReapplyConfigs(__instance);
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(PlayerControllerB __instance)
        {
            PlayerTweaks.RegisterSwitchSlotMessage();
        }

        private static void ModifySprintMultiplierValues(ref List<CodeInstruction> instructions)
        {
            float MaxSprintValue = 2.25f;
            float SprintMultiIncreaseValue = 1f;
            float DefaultSprintValue = 1f;
            float SprintMultiDecreaseValue = 10f;

            int indexOfMaxSprintMultiplier = -1;
            bool patchedSprintMultiIncrease = false;
            bool patchedDefaultSprintMultiplier = false;
            bool patchedSprintMultiDecrease = false;

            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (instruction.opcode != OpCodes.Ldc_R4)
                    continue;

                if (Math.Abs((float)instruction.operand - MaxSprintValue) > 0.1)
                    continue;

                indexOfMaxSprintMultiplier = i;
                instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                instructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                instructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                instructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaSprintSpeedValues)));
                instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.MaxSprintSpeed)));
                break;
            }

            if (indexOfMaxSprintMultiplier == -1)
                return;

            for (int i = indexOfMaxSprintMultiplier; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (instruction.opcode == OpCodes.Ldc_R4)
                {
                    if (patchedDefaultSprintMultiplier && patchedSprintMultiDecrease && patchedSprintMultiIncrease)
                        break;

                    if (!patchedSprintMultiIncrease && Math.Abs((float)instruction.operand - SprintMultiIncreaseValue) < 0.1)
                    {
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                        instructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                        instructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaSprintSpeedValues)));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.SprintSpeedIncreasePerFrame)));
                        patchedSprintMultiIncrease = true;
                        continue;
                    }
                    if (!patchedDefaultSprintMultiplier && Math.Abs((float)instruction.operand - DefaultSprintValue) < 0.1)
                    {
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                        instructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                        instructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaSprintSpeedValues)));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.DefaultSprintSpeed)));
                        patchedDefaultSprintMultiplier = true;
                        continue;
                    }
                    if (!patchedSprintMultiDecrease && Math.Abs((float)instruction.operand - SprintMultiDecreaseValue) < 0.1)
                    {
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                        instructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                        instructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaSprintSpeedValues)));
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.SprintSpeedDecreasePerFrame)));
                        patchedSprintMultiDecrease = true;
                        continue;
                    }
                }
            }
        }

        // Change the arbitrary values inside the Update() method to use the values of modifiable variables.
        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsAsList = new List<CodeInstruction>(instructions);
            ModifySprintMultiplierValues(ref instructionsAsList);
            return instructionsAsList.AsEnumerable();
        }

        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void AddHotkeys(PlayerControllerB __instance)
        {
            SetupKeybinds(__instance);
        }

        [HarmonyPatch("LateUpdate")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> LateUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (MikesTweaks.Compatibility.LateGameUpgradesCompat)
                return instructions;

            List<CodeInstruction> instructionsToList = new List<CodeInstruction>(instructions);
            int patched = 0;
            for (int end = 0; end < instructionsToList.Count; end++)
            {
                if (!IsSprintMeterField(instructionsToList[end], OpCodes.Stfld))
                    continue;

                if (!TryGetStaminaRechargeReplacement(instructionsToList, end, out int start, out CodeInstruction localLoad, out MethodInfo replacement))
                    continue;

                CodeInstruction storeTarget = new CodeInstruction(OpCodes.Ldarg_0);
                for (int i = start; i <= end; i++)
                    MoveMarks(instructionsToList[i], storeTarget);

                List<CodeInstruction> rewritten = new List<CodeInstruction>
                {
                    storeTarget,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(localLoad.opcode, localLoad.operand),
                    new CodeInstruction(OpCodes.Call, replacement),
                    CodeInstruction.StoreField(typeof(PlayerControllerB), nameof(PlayerControllerB.sprintMeter))
                };

                instructionsToList.RemoveRange(start, end - start + 1);
                instructionsToList.InsertRange(start, rewritten);
                end = start + rewritten.Count - 1;
                patched++;
            }

            if (patched != 3)
                MikesTweaks.Log?.LogWarning($"LateUpdate stamina recharge patch matched {patched} sites (expected 3). Unmatched sites keep vanilla behavior.");

            return instructionsToList.AsEnumerable();
        }

        [HarmonyPatch("Jump_performed")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ModifyJumpDrain(IEnumerable<CodeInstruction> instructions)
        {
            if (MikesTweaks.Compatibility.LateGameUpgradesCompat)
                return instructions;

            float JumpDrainValue = 0.08f;

            List<CodeInstruction> toListInstructions = new List<CodeInstruction>(instructions);
            for (int i = 0; i < toListInstructions.Count; i++)
            {
                var instruction = toListInstructions[i];
                if (instruction.opcode != OpCodes.Ldc_R4)
                    continue;

                if (Math.Abs((float)instruction.operand - JumpDrainValue) > 0.01f)
                    continue;

                toListInstructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                toListInstructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                toListInstructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                toListInstructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaStaminaValues)));
                toListInstructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.JumpStaminaDrain)));
                break;
            }

            return toListInstructions.AsEnumerable();
        }

        [HarmonyPatch("Emote1_performed")]
        [HarmonyPrefix]
        private static bool Emote1_performed()
        {
            return false;
        }

        [HarmonyPatch("Emote2_performed")]
        [HarmonyPrefix]
        private static bool Emote2_performed()
        {
            return false;
        }

        [HarmonyPatch("SendNewPlayerValuesClientRpc")]
        [HarmonyPostfix]
        private static void ConnectClientToPlayerObject(PlayerControllerB __instance)
        {
            WorldTweaks.MakeTerminalUnusableForAnyoneButHost();
        }

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable(PlayerControllerB __instance)
        {
            if (!PlayerTweaks.IsLocallyControlled(__instance))
                return;

            inputRedirection?.OnEnable();
        }

        [HarmonyPatch("OnDisable")]
        [HarmonyPostfix]
        private static void OnDisable(PlayerControllerB __instance)
        {
            if (!PlayerTweaks.IsLocallyControlled(__instance))
                return;
            inputRedirection?.OnDisable();
        }

        [HarmonyPatch("OnDestroy")]
        [HarmonyPrefix]
        public static void OnDestroy(PlayerControllerB __instance)
        {
            if (!PlayerTweaks.IsLocallyControlled(__instance))
                return;
            inputRedirection?.Destroy();
            inputRedirection = null;
        }
    }
}
