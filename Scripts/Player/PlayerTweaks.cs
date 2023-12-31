﻿using System;
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
using MikesTweaks.Scripts.Systems;
using MikesTweaks.Scripts.World;
using Unity.Collections;
using Unity.Netcode;
using static UnityEngine.UI.GridLayoutGroup;
using MikesTweaks.Scripts.Configs;

namespace MikesTweaks.Scripts.Player
{

    public class PlayerTweaks
    {
        public static class Configs
        {
            public static string PlayerTweaksSectionHeader => "PlayerTweaks";

            public static ConfigEntrySettings<float> MaxStamina =
                new ConfigEntrySettings<float>("MaxStamina", 15f, 11f, "This is the maximum amount of time you can run.\nThe higher the number, the longer you can run for.");

            public static ConfigEntrySettings<float> DefaultSprintSpeed =
                new ConfigEntrySettings<float>("DefaultSprintSpeed", 1.5f, 1f, "This is the floor of your sprint speed.\nEvery frame your sprint speed decreases when you don't run and this is as low as it can go.");

            public static ConfigEntrySettings<float> SprintSpeedIncreasePerFrame =
                new ConfigEntrySettings<float>("SprintSpeedIncreasePerFrame", 1f, 1f, "The higher this value is, the faster your sprint speed reaches the maximum sprint speed.\nYour sprint speed increments every frame you're running.");

            public static ConfigEntrySettings<float> SprintSpeedDecreasePerFrame =
                new ConfigEntrySettings<float>("SprintSpeedDecreasePerFrame", 10f, 10f, "The higher this value is, the faster your sprint speed goes to the default sprint speed.\nYour sprint speed decrements every frame you're not running.");

            public static ConfigEntrySettings<float> MaxSprintSpeed =
                new ConfigEntrySettings<float>("MaxSprintSpeed", 3f, 2.25f, "This is your sprint speed ceiling.\nEvery frame your sprint speed increases and this is how high it can go.");

            public static ConfigEntrySettings<float> StaminaRechargePerFrame =
                new ConfigEntrySettings<float>("StaminaRechargePerFrame", 5f, 1f, "The bigger number this is the faster your stamina recharges.");

            public static ConfigEntrySettings<float> StaminaWeightWhileWalking =
                new ConfigEntrySettings<float>("StaminaWeightWhileWalking", 9f, 9f, "The bigger number this is, the slower your stamina recharges while walking.");

            public static ConfigEntrySettings<float> StaminaWeightWhileStandingStill =
                new ConfigEntrySettings<float>("StaminaWeightWhileStandingStill", 4f, 4f, "The bigger number this is, the slower your stamina recharges while standing still");

            public static ConfigEntrySettings<float> JumpStaminaDrain =
                new ConfigEntrySettings<float>("JumpStaminaDrain", 0.04f, 0.08f, "The lower this amount is, the less stamina jumping drains.\n");


            public static string KeybindsSectionHeader => "Keybinds";

            public static ConfigEntrySettings<string>[] SlotKeybinds = new ConfigEntrySettings<string>[]
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

            public static ConfigEntrySettings<string>[] EmoteKeybinds = new ConfigEntrySettings<string>[]
            {
                new ConfigEntrySettings<string>("Emote1", "<Keyboard>/y", "<Keyboard>/1"),
                new ConfigEntrySettings<string>("Emote2", "<Keyboard>/u", "<Keyboard>/2")
            };

            public static ConfigEntrySettings<string> FlashlightKeybind =
                new ConfigEntrySettings<string>("Flashlight", "<Keyboard>/f", "");

            public static ConfigEntrySettings<string> WalkieTalkieKeybind =
                new ConfigEntrySettings<string>("WalkieTalkieKeybind", "<Keyboard>/r", "");
        }

        public static PlayerControllerB LocalPlayerController => GameNetworkManager.Instance.localPlayerController;
        public static string PlayerSwitchSlotChannel => "PlayerChangeSlot";
        public static string PlayerSwitchSlotRequestChannel => "PlayerChangeSlotRequest";

        public static void RegisterConfigs()
        {
            MikesTweaks.Instance.BindConfig(ref Configs.MaxStamina, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.DefaultSprintSpeed, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.SprintSpeedIncreasePerFrame, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.SprintSpeedDecreasePerFrame, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.MaxSprintSpeed, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.StaminaRechargePerFrame, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.StaminaWeightWhileWalking, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.StaminaWeightWhileStandingStill, Configs.PlayerTweaksSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.JumpStaminaDrain, Configs.PlayerTweaksSectionHeader);
            
            MikesTweaks.Instance.BindConfig(ref Configs.FlashlightKeybind, Configs.KeybindsSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.WalkieTalkieKeybind, Configs.KeybindsSectionHeader);
            for (int i = 0; i < Configs.SlotKeybinds.Length; i++)
                MikesTweaks.Instance.BindConfig(ref Configs.SlotKeybinds[i], Configs.KeybindsSectionHeader);
            for (int i = 0; i < Configs.EmoteKeybinds.Length; i++)
                MikesTweaks.Instance.BindConfig(ref Configs.EmoteKeybinds[i], Configs.KeybindsSectionHeader);

            ConfigsSynchronizer.OnConfigsChangedDelegate += () => ReapplyConfigs(LocalPlayerController, true, true, true);
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => (sizeof(float) * 9));
        }
        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.DefaultSprintSpeed.Value(WorldTweaks.Configs.UseVanillaSprintSpeedValues.Value()));
            writer.WriteValueSafe(Configs.SprintSpeedIncreasePerFrame.Value(WorldTweaks.Configs.UseVanillaSprintSpeedValues.Value()));
            writer.WriteValueSafe(Configs.SprintSpeedDecreasePerFrame.Value(WorldTweaks.Configs.UseVanillaSprintSpeedValues.Value()));
            writer.WriteValueSafe(Configs.MaxSprintSpeed.Value(WorldTweaks.Configs.UseVanillaSprintSpeedValues.Value()));

            writer.WriteValueSafe(Configs.MaxStamina.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()));
            writer.WriteValueSafe(Configs.StaminaRechargePerFrame.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()));
            writer.WriteValueSafe(Configs.StaminaWeightWhileWalking.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()));
            writer.WriteValueSafe(Configs.StaminaWeightWhileStandingStill.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()));
            writer.WriteValueSafe(Configs.JumpStaminaDrain.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()));

            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValue(out float Value);
            Configs.DefaultSprintSpeed.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.SprintSpeedIncreasePerFrame.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.SprintSpeedDecreasePerFrame.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.MaxSprintSpeed.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.MaxStamina.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.StaminaRechargePerFrame.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.StaminaWeightWhileWalking.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.StaminaWeightWhileStandingStill.Entry.Value = Value;

            payload.ReadValue(out Value);
            Configs.JumpStaminaDrain.Entry.Value = Value;

            return payload;
        }

        public static void ReapplyConfigs(PlayerControllerB player, bool applyToAllPlayers = false, bool force = false, bool updateHud = false)
        {
            float VanillaSprintTime = player.sprintTime;
            int VanillaInventorySlots = player.ItemSlots.Length;
            player.sprintTime = Configs.MaxStamina.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value());

            if (MikesTweaks.Compatibility.ReservedSlotCoreCompat || MikesTweaks.Compatibility.LethalThingsCompat)
                return;

            if (!applyToAllPlayers)
            {
                InventoryTweaks.ChangeItemSlotsAmount(player, force);
            }
            else
            {
                foreach (PlayerControllerB playerScript in StartOfRound.Instance.allPlayerScripts)
                {
                    InventoryTweaks.ChangeItemSlotsAmount(playerScript, force);
                }
            }

            if (updateHud)
                InventoryTweaks.ChangeItemSlotsAmountUI();


            if (!MikesTweaks.DebugMode)
                return;

            MikesTweaks.Log.LogInfo($"InventorySlots: {VanillaInventorySlots}");
            MikesTweaks.Log.LogInfo($"MaxStamina: {VanillaSprintTime}");
        }

        public static void RegisterSwitchSlotMessage()
        {
            CustomNetworking.Instance.RegisterChannel(PlayerSwitchSlotChannel, ReceiveSwitchSlot);
            CustomNetworking.Instance.RegisterChannel(PlayerSwitchSlotRequestChannel, ReceiveSwitchSlotRequest);
        }

        public static void SwitchSlot_Server(int slot, ulong clientIDOfChangedSlot)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            // Send the message
            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;
            FastBufferWriter writer = new FastBufferWriter(sizeof(int) + sizeof(ulong), Allocator.Temp);
            writer.WriteValueSafe(slot);
            writer.WriteValueSafe(clientIDOfChangedSlot);
            manager.SendNamedMessageToAll(PlayerSwitchSlotChannel, writer, NetworkDelivery.Reliable);
        }

        public static void ReceiveSwitchSlotRequest(ulong senderID, FastBufferReader payload)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            payload.ReadValueSafe(out int slot);
            SwitchSlot_Server(slot, senderID);
        }

        public static void ReceiveSwitchSlot(ulong senderID, FastBufferReader payload)
        {
            payload.ReadValueSafe(out int slot);
            payload.ReadValueSafe(out ulong clientID);

            foreach (PlayerControllerB playerController in StartOfRound.Instance.allPlayerScripts)
            {
                if (playerController.actualClientId != clientID)
                    continue;

                playerController.gameObject.GetComponent<PlayerInputRedirection>().SwitchToSlot(slot);
                break;
            }
        }

        public static float StaminaRechargeMovementHinderedWalking(PlayerControllerB player, float num2)
        {
            // TODO: Add a config for this.
            return Mathf.Clamp(player.sprintMeter - Time.deltaTime / player.sprintTime * num2 * 0.5f, 0f, 1f);;
        }

        public static float StaminaRechargeMovementNotHinderedWalking(PlayerControllerB player, float num2)
        {
            return Mathf.Clamp(player.sprintMeter + (Time.deltaTime * Configs.StaminaRechargePerFrame.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value())) / (player.sprintTime + Configs.StaminaWeightWhileWalking.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value())) * num2, 0f, 1f);
        }

        public static float StaminaRechargeMovementNotHinderedNotWalking(PlayerControllerB player, float num2)
        {
            return Mathf.Clamp(player.sprintMeter + (Time.deltaTime * Configs.StaminaRechargePerFrame.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value()))/ (player.sprintTime + Configs.StaminaWeightWhileStandingStill.Value(WorldTweaks.Configs.UseVanillaStaminaValues.Value())) * num2, 0f, 1f);
        }

        public static bool IsLocallyControlled(PlayerControllerB player)
        {
            return player == GameNetworkManager.Instance.localPlayerController;
        }

        public static bool CanSwitchSlot(PlayerControllerB player)
        {
            Type playerType = typeof(PlayerControllerB);
            bool throwingObject = (bool)playerType.GetField("throwingObject", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(player);
            float timeSinceSwitchingSlots = (float)playerType.GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(player);

            // The dev's mess
            if ((!player.IsOwner || !player.isPlayerControlled || player.IsServer && !player.isHostPlayerObject) &&
                !player.isTestingPlayer || timeSinceSwitchingSlots < 0.3f ||
                player.isGrabbingObjectAnimation || player.inSpecialInteractAnimation || throwingObject ||
                player.isTypingChat || player.twoHanded || player.activatingItem || player.jetpackControls ||
                player.disablingJetpackControls)
                return false;

            return true;
        }

        public static bool CanUseItem(PlayerControllerB player)
        {
            bool usableInSpecialAnimations = true;
            if (player.currentlyHeldObjectServer != null)
                usableInSpecialAnimations = player.currentlyHeldObjectServer.itemProperties.usableInSpecialAnimations;

            return (player.IsOwner && player.isPlayerControlled && (!player.IsServer || player.isHostPlayerObject) || player.isTestingPlayer)  && !player.quickMenuManager.isMenuOpen && !player.isPlayerDead && (usableInSpecialAnimations || !player.isGrabbingObjectAnimation && !player.inTerminalMenu && !player.isTypingChat && (!player.inSpecialInteractAnimation || player.inShockingMinigame));
        }
    }
    
}
