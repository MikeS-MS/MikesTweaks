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

        private static bool InsertStaminaRechargeMovementHinderedWalking(ref List<CodeInstruction> instructions, CodeInstruction instruction, int i, ref List<int> IndexesToRemove)
        {
            if (instruction.opcode != OpCodes.Ldc_R4)
                return false;

            if (Math.Abs((float)instruction.operand - 0.5f) > 0.01f)
                return false;

            instructions[i - 7] = new CodeInstruction(OpCodes.Ldloc_0);
            instructions[i - 6] = CodeInstruction.Call(typeof(PlayerTweaks), nameof(PlayerTweaks.StaminaRechargeMovementHinderedWalking));
            instructions[i - 5] = CodeInstruction.StoreField(typeof(PlayerControllerB), nameof(PlayerControllerB.sprintMeter));

            for (int j = i - 4; j <= i + 6; j++)
                IndexesToRemove.Add(j);

            return true;
        }

        private static bool InsertStaminaRechargeMovementNotHinderedWalking(ref List<CodeInstruction> instructions, CodeInstruction instruction, int i, ref List<int> IndexesToRemove)
        {
            if (instruction.opcode != OpCodes.Ldc_R4)
                return false;

            if (Math.Abs((float)instruction.operand - 9f) > 0.01f)
                return false;

            instructions[i - 4] = new CodeInstruction(OpCodes.Ldloc_0);
            instructions[i - 3] = CodeInstruction.Call(typeof(PlayerTweaks), nameof(PlayerTweaks.StaminaRechargeMovementNotHinderedWalking));
            instructions[i - 2] = CodeInstruction.StoreField(typeof(PlayerControllerB), nameof(PlayerControllerB.sprintMeter));

            for (int j = i - 1; j <= i + 9; j++)
                IndexesToRemove.Add(j);

            return true;
        }

        private static bool InsertStaminaRechargeMovementNotHinderedNotWalking(ref List<CodeInstruction> instructions, CodeInstruction instruction, int i, ref List<int> IndexesToRemove)
        {
            if (instruction.opcode != OpCodes.Ldc_R4)
                return false;

            if (Math.Abs((float)instruction.operand - 4f) > 0.01f)
                return false;

            instructions[i - 4] = new CodeInstruction(OpCodes.Ldloc_0);
            instructions[i - 3] = CodeInstruction.Call(typeof(PlayerTweaks), nameof(PlayerTweaks.StaminaRechargeMovementNotHinderedNotWalking));
            instructions[i - 2] = CodeInstruction.StoreField(typeof(PlayerControllerB), nameof(PlayerControllerB.sprintMeter));

            for (int j = i - 1; j <= i + 9; j++)
                IndexesToRemove.Add(j);

            return true;
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
            bool HinderedWalkingDone = false;
            bool NotHinderedWalkingDone = false;
            bool NotHinderedNotWalkingDone = false;

            List<int> IndexesToRemove = new List<int>();
            List<CodeInstruction> instructionsToList = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructionsToList.Count; i++)
            {
                var instruction = instructionsToList[i];

                if (!HinderedWalkingDone)
                    HinderedWalkingDone = InsertStaminaRechargeMovementHinderedWalking(ref instructionsToList, instruction, i, ref IndexesToRemove);
                if (!NotHinderedNotWalkingDone)
                    NotHinderedNotWalkingDone = InsertStaminaRechargeMovementNotHinderedNotWalking(ref instructionsToList, instruction, i, ref IndexesToRemove);
                if (!NotHinderedWalkingDone)
                    NotHinderedWalkingDone = InsertStaminaRechargeMovementNotHinderedWalking(ref instructionsToList, instruction, i, ref IndexesToRemove);

                if (HinderedWalkingDone && NotHinderedNotWalkingDone && NotHinderedWalkingDone)
                    break;
            }

            IndexesToRemove.Sort();
            for (int i = IndexesToRemove.Count - 1; i >= 0; i--)
                instructionsToList.RemoveAt(IndexesToRemove[i]);

            return instructionsToList.AsEnumerable();
        }

        [HarmonyPatch("Jump_performed")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ModifyJumpDrain(IEnumerable<CodeInstruction> instructions)
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

                toListInstructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), nameof(ConfigEntrySettings<float>.Value));
                toListInstructions.Insert(i, CodeInstruction.Call(typeof(ConfigEntrySettings<bool>), nameof(ConfigEntrySettings<bool>.Value)));
                toListInstructions.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_0));
                toListInstructions.Insert(i, CodeInstruction.LoadField(typeof(WorldTweaks.Configs), nameof(WorldTweaks.Configs.UseVanillaStaminaValues)));
                toListInstructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.JumpStaminaDrain)));
                break;
            }

            return instructions.AsEnumerable();
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
