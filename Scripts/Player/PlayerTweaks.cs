using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using GameNetcodeStuff;
using BepInEx.Configuration;
using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Input;
using Mono.Cecil.Cil;
using UnityEngine.InputSystem;
using Object = System.Object;
using OpCodes = System.Reflection.Emit.OpCodes;
using System.Reflection.Emit;
using Dissonance.Integrations.Unity_NFGO;
using MikesTweaks.Scripts.Networking;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Player
{

    public class PlayerTweaks
    {
        public static class Configs
        {
            public static string PlayerTweaksSectionHeader => "PlayerTweaks";

            public static ConfigEntrySettings<float> SprintLongevity =
                new ConfigEntrySettings<float>("SprintLongevity", 12, 5);

            public static ConfigEntrySettings<float> DefaultSprintMultiplier =
                new ConfigEntrySettings<float>("DefaultSprintMultiplier", 1.5f, 1f);

            public static ConfigEntrySettings<float> SprintMultiplierIncrease =
                new ConfigEntrySettings<float>("SprintMultiplierIncrease", 1f, 1f);

            public static ConfigEntrySettings<float> SprintMultiplierDecrease =
                new ConfigEntrySettings<float>("SprintMultiplierDecrease", 10f, 10f);

            public static ConfigEntrySettings<float> MaxSprintMultiplier =
                new ConfigEntrySettings<float>("MaxSprintMultiplier", 3f, 2.25f);

            public static ConfigEntrySettings<float> JumpStaminaDrain =
                new ConfigEntrySettings<float>("JumpStaminaDrain", 0.04f, 0.08f);


            public static string KeybindsSectionHeader => "Keybinds";

            public static List<ConfigEntrySettings<string>> SlotKeybinds = new List<ConfigEntrySettings<string>>()
            {
                new ConfigEntrySettings<string>("Slot1", "<Keyboard>/1", ""),
                new ConfigEntrySettings<string>("Slot2", "<Keyboard>/2", ""),
                new ConfigEntrySettings<string>("Slot3", "<Keyboard>/3", ""),
                new ConfigEntrySettings<string>("Slot4", "<Keyboard>/4", ""),
                new ConfigEntrySettings<string>("Slot5", "<Keyboard>/5", ""),
                new ConfigEntrySettings<string>("Slot6", "<Keyboard>/6", ""),
                new ConfigEntrySettings<string>("Slot7", "<Keyboard>/7", ""),
                new ConfigEntrySettings<string>("Slot8", "<Keyboard>/8", ""),
                new ConfigEntrySettings<string>("Slot9", "<Keyboard>/9", "")
            };

            public static List<ConfigEntrySettings<string>> EmoteKeybinds = new List<ConfigEntrySettings<string>>()
            {
                new ConfigEntrySettings<string>("Emote1", "<Keyboard>/y", "<Keyboard>/1"),
                new ConfigEntrySettings<string>("Emote2", "<Keyboard>/u", "<Keyboard>/2")
            };
        }

        public static PlayerControllerB LocalPlayerController = null;

        public static void RegisterConfigs(ConfigFile config)
        {
            Configs.SprintLongevity.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.SprintLongevity.ConfigName, Configs.SprintLongevity.DefaultValue,
                Configs.SprintLongevity.ConfigDesc);

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

            Configs.JumpStaminaDrain.Entry = config.Bind(Configs.PlayerTweaksSectionHeader,
                Configs.JumpStaminaDrain.ConfigName, Configs.JumpStaminaDrain.DefaultValue,
                Configs.JumpStaminaDrain.ConfigDesc);

            foreach (ConfigEntrySettings<string> slot in Configs.SlotKeybinds)
                slot.Entry = config.Bind(Configs.KeybindsSectionHeader, slot.ConfigName, slot.DefaultValue, slot.ConfigDesc);

            foreach (ConfigEntrySettings<string> emote in Configs.EmoteKeybinds)
                emote.Entry = config.Bind(Configs.KeybindsSectionHeader, emote.ConfigName, emote.DefaultValue, emote.ConfigDesc);

            ConfigsSynchronizer.OnConfigsChangedDelegate += () => ReapplyConfigs(LocalPlayerController, true, true);
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => sizeof(float) * 6);
        }

        public static bool IsLocallyControlled(PlayerControllerB player)
        {
            return player.playerUsername == GameNetworkManager.Instance.username;
        }

        public static bool CanSwitchSlot(PlayerControllerB player)
        {
            Type playerType = typeof(PlayerControllerB);
            bool throwingObject = (bool)playerType.GetField("throwingObject", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(player);
            float timeSinceSwitchingSlots = (float)playerType.GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(player);

            // The dev's mess
            if ((!player.IsOwner || !player.isPlayerControlled || player.IsServer && !player.isHostPlayerObject) &&
                !player.isTestingPlayer || timeSinceSwitchingSlots < 0.30000001192092896 ||
                player.isGrabbingObjectAnimation || player.inSpecialInteractAnimation || throwingObject ||
                player.isTypingChat || player.twoHanded || player.activatingItem || player.jetpackControls ||
                player.disablingJetpackControls)
                return false;

            return true;
        }

        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.SprintLongevity.Value);
            writer.WriteValueSafe(Configs.DefaultSprintMultiplier.Value);
            writer.WriteValueSafe(Configs.SprintMultiplierIncrease.Value);
            writer.WriteValueSafe(Configs.SprintMultiplierDecrease.Value);
            writer.WriteValueSafe(Configs.MaxSprintMultiplier.Value);
            writer.WriteValueSafe(Configs.JumpStaminaDrain.Value);

            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValue(out float Value);
            Configs.SprintLongevity.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.DefaultSprintMultiplier.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.SprintMultiplierIncrease.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.SprintMultiplierDecrease.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.MaxSprintMultiplier.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.JumpStaminaDrain.Entry.Value = Value;

            return payload;
        }

        public static void ReapplyConfigs(PlayerControllerB player, bool force = false, bool updateHud = false)
        {
            player.sprintTime = Configs.SprintLongevity.Value;
            InventoryTweaks.ChangeItemSlotsAmount(player, force);

            if (updateHud)
                InventoryTweaks.ChangeItemSlotsAmountUI(HUDManager.Instance);
        }
    }

    public class PlayerInputRedirection : MonoBehaviour, PlayerHotbarInput.IHotbarActions
    {
        private PlayerControllerB owner = null;
        private PlayerHotbarInput input = null;
        private MethodInfo SwitchToSlotMethod = null;
        
        public void Awake()
        {
            owner = gameObject.GetComponent<PlayerControllerB>();
            SwitchToSlotMethod = typeof(PlayerControllerB).GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        //public void Start()
        //{
        //    if (PlayerTweaks.ConfigsChanged)
        //    {
        //        PlayerTweaks.ReapplyConfigs(owner, true, true);
        //    }
        //}
        
        public void SwitchSlot_Server(int slot)
        {
            if (!PlayerTweaks.CanSwitchSlot(owner))
                return;

            ShipBuildModeManager.Instance.CancelBuildMode();
            owner.playerBodyAnimator.SetBool("GrabValidated", false);
            SwitchToSlot(slot);
            SwitchSlot_Client(slot);
        }

        public void SwitchSlot_Client(int slot)
        {
            SwitchToSlot(slot);
        }

        public void OnHotbar1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            SwitchSlot_Server(0);
        }

        public void OnHotbar2(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            SwitchSlot_Server(1);
        }

        public void OnHotbar3(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            SwitchSlot_Server(2);
        }

        public void OnHotbar4(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            SwitchSlot_Server(3);
        }

        public void OnHotbar5(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(4))
                return;

            SwitchSlot_Server(4);
        }

        public void OnHotbar6(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(5))
                return;

            SwitchSlot_Server(5);
        }

        public void OnHotbar7(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(6))
                return;

            SwitchSlot_Server(6);
        }

        public void OnHotbar8(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(7))
                return;

            SwitchSlot_Server(7);
        }

        public void OnHotbar9(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(8))
                return;

            SwitchSlot_Server(8);
        }

        public void OnEnable()
        {
            input?.Enable();
        }

        public void OnDisable()
        {
            input?.Disable();
        }

        public void Destroy()
        {
            input?.Dispose();
        }

        public void BindHotbarSlots()
        {
            owner = gameObject.GetComponent<PlayerControllerB>();
            input = new PlayerHotbarInput();
            input.Hotbar.SetCallbacks(this);
            input.Enable();

            input.Hotbar.Hotbar1.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar1.AddBinding(PlayerTweaks.Configs.SlotKeybinds[0].Value);

            input.Hotbar.Hotbar2.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar2.AddBinding(PlayerTweaks.Configs.SlotKeybinds[1].Value);

            input.Hotbar.Hotbar3.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar3.AddBinding(PlayerTweaks.Configs.SlotKeybinds[2].Value);

            input.Hotbar.Hotbar4.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar4.AddBinding(PlayerTweaks.Configs.SlotKeybinds[3].Value);

            input.Hotbar.Hotbar5.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar5.AddBinding(PlayerTweaks.Configs.SlotKeybinds[4].Value);

            input.Hotbar.Hotbar6.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar6.AddBinding(PlayerTweaks.Configs.SlotKeybinds[5].Value);

            input.Hotbar.Hotbar7.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar7.AddBinding(PlayerTweaks.Configs.SlotKeybinds[6].Value);

            input.Hotbar.Hotbar8.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar8.AddBinding(PlayerTweaks.Configs.SlotKeybinds[7].Value);

            input.Hotbar.Hotbar9.ChangeBinding(0).Erase();
            input.Hotbar.Hotbar9.AddBinding(PlayerTweaks.Configs.SlotKeybinds[8].Value);
        }

        private void SwitchToSlot(int slot)
        {
            object[] args = { slot, null };
            SwitchToSlotMethod?.Invoke(owner, args);
        }
    }

    // Transpilers for PlayerControllerB
    [HarmonyPatch(typeof(PlayerControllerB))]
    public static class PlayerControllerB_Patches
    {
        private static PlayerInputRedirection inputRedirection = null;

        public static void SetupKeybinds(PlayerControllerB __instance)
        {
            if (!PlayerTweaks.IsLocallyControlled(__instance))
                return;

            inputRedirection = __instance.gameObject.GetComponent<PlayerInputRedirection>();
            inputRedirection.BindHotbarSlots();
            __instance.playerActions.Movement.Emote1.ChangeBinding(0).Erase();
            __instance.playerActions.Movement.Emote1.AddBinding(PlayerTweaks.Configs.EmoteKeybinds[0].Value);
            __instance.playerActions.Movement.Emote2.ChangeBinding(0).Erase();
            __instance.playerActions.Movement.Emote2.AddBinding(PlayerTweaks.Configs.EmoteKeybinds[1].Value);
        }

        // Change the arbitrary values inside the Update() method to use the values of modifiable variables.
        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsAsList = new List<CodeInstruction>(instructions);
            ModifySprintMultiplierValues(ref instructionsAsList);

            // Insert a method call to bind my actions.
            for (int i = 0; i < instructionsAsList.Count; i++)
            {
                var instruction = instructionsAsList[i];

                if (instruction.opcode != OpCodes.Call)
                    continue;

                if (instruction.operand as MethodInfo != typeof(PlayerControllerB).GetMethod("ConnectClientToPlayerObject"))
                    continue;

                instructionsAsList.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                instructionsAsList.Insert(i + 2, CodeInstruction.Call(typeof(PlayerControllerB_Patches), nameof(SetupKeybinds)));
                break;
            }
            return instructionsAsList.AsEnumerable();
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

                toListInstructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), "get_Value");
                toListInstructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.JumpStaminaDrain)));
                break;
            }

            return toListInstructions.AsEnumerable();
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(PlayerControllerB __instance)
        {
            GameObject playerGameObject = __instance.gameObject;
            playerGameObject.AddComponent<PlayerInputRedirection>();
            PlayerTweaks.ReapplyConfigs(__instance);
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
                instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), "get_Value");
                instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.MaxSprintMultiplier)));
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
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), "get_Value");
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.SprintMultiplierIncrease)));
                        patchedSprintMultiIncrease = true;
                        continue;
                    }
                    if (!patchedDefaultSprintMultiplier && Math.Abs((float)instruction.operand - DefaultSprintValue) < 0.1)
                    {
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), "get_Value");
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.DefaultSprintMultiplier)));
                        patchedDefaultSprintMultiplier = true;
                        continue;
                    }
                    if (!patchedSprintMultiDecrease && Math.Abs((float)instruction.operand - SprintMultiDecreaseValue) < 0.1)
                    {
                        instructions[i] = CodeInstruction.Call(typeof(ConfigEntrySettings<float>), "get_Value");
                        instructions.Insert(i, CodeInstruction.LoadField(typeof(PlayerTweaks.Configs), nameof(PlayerTweaks.Configs.SprintMultiplierDecrease)));
                        patchedSprintMultiDecrease = true;
                        continue;
                    }
                }
            }
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
