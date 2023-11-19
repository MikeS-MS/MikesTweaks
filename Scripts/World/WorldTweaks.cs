using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using MikesTweaks.Scripts.Configs;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Networking;
using Unity.Netcode;

namespace MikesTweaks.Scripts.World
{
    public class WorldTweaks
    {
        public class Configs
        {
            public static string GameRulesSectionHeader => "GameRules";


            public static ConfigEntrySettings<float> GlobalTimeSpeedMulti =
                new ConfigEntrySettings<float>("GlobalTimeSpeedMultiplier", 0.5f, 1f, "Think of this as a percentage, the lower it is, the slower the time goes by, the higher it is, the faster time passes.");

            public static ConfigEntrySettings<bool> AllowHotbarKeybinds =
                new ConfigEntrySettings<bool>("AllowHotbarKeybinds", true, false,
                    "Set this to false if you don't want people who join your lobby to be able to use the hotbar keybinds and to true if you want them to be able to.");

            public static ConfigEntrySettings<bool> AllowFlashlightKeybind =
                new ConfigEntrySettings<bool>("AllowFlashlightKeybind", true, false,
                    "Set this to false if you don't want people who join your lobby to be able to use the flashlight keybind and to true if you want them to be able to.");

            public static ConfigEntrySettings<bool> AllowClientsToUseTerminal =
                new ConfigEntrySettings<bool>("AllowClientsToUseTerminal", false, true,
                    "Set this to false if you don't want people who join your lobby to be able to use the terminal and to true if you want them to be able to.\nYou probably want to set this to true if you're hosting a lobby with people you know and trust.");

            public static ConfigEntrySettings<bool> UseVanillaSprintSpeedValues =
                new ConfigEntrySettings<bool>("UseVanillaSprintSpeedValues", false, true,
                    "Set this to true if you want to use all the vanilla values tied to sprinting.\nStamina drain related configs not included.");

            public static ConfigEntrySettings<bool> UseVanillaStaminaValues =
                new ConfigEntrySettings<bool>("UseVanillaStaminaValues", false, true,
                    "Set this to true if you want to use all the vanilla values tied to stamina.\nSprint speed related configs not included.");

            public static ConfigEntrySettings<bool> UseVanillaTerminalItemWeights =
                new ConfigEntrySettings<bool>("UseVanillaTerminalItemWeights", false, true,
                    "Set this to true if you want to use all the vanilla values for the weight of every terminal item.");
        }

        public static Terminal terminalInstance = null;
        public static InteractTrigger TerminalInteractTriggerInstance = null;
        private static MethodInfo terminalTriggerInUseRPC = typeof(InteractTrigger)
            .GetMethod("UpdateUsedByPlayerServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void RegisterConfigs(ConfigFile config)
        {
            MikesTweaks.Instance.BindConfig(ref Configs.GlobalTimeSpeedMulti, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.AllowHotbarKeybinds, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.AllowFlashlightKeybind, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.AllowClientsToUseTerminal, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.UseVanillaSprintSpeedValues, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.UseVanillaStaminaValues, Configs.GameRulesSectionHeader);
            MikesTweaks.Instance.BindConfig(ref Configs.UseVanillaTerminalItemWeights, Configs.GameRulesSectionHeader);

            ConfigsSynchronizer.OnConfigsChangedDelegate += () => ReapplyConfigs(TimeOfDay.Instance);
            ConfigsSynchronizer.Instance.AddConfigGetter(WriteConfigsToWriter);
            ConfigsSynchronizer.Instance.AddConfigSetter(ReadConfigChanges);
            ConfigsSynchronizer.Instance.AddConfigSizeGetter(() => sizeof(float) + (2 * sizeof(bool)));
        }

        public static FastBufferWriter WriteConfigsToWriter(FastBufferWriter writer)
        {
            writer.WriteValueSafe(Configs.GlobalTimeSpeedMulti.Value());
            writer.WriteValueSafe(Configs.AllowFlashlightKeybind.Value());
            writer.WriteValueSafe(Configs.AllowHotbarKeybinds.Value());

            return writer;
        }

        public static FastBufferReader ReadConfigChanges(FastBufferReader payload)
        {
            payload.ReadValue(out float Value);
            Configs.GlobalTimeSpeedMulti.Entry.Value = Value;

            payload.ReadValue(out bool BoolValue);
            Configs.AllowFlashlightKeybind.Entry.Value = BoolValue;

            payload.ReadValue(out BoolValue);
            Configs.AllowHotbarKeybinds.Entry.Value = BoolValue;

            return payload;
        }

        public static void ReapplyConfigs(TimeOfDay timeOfDay)
        {
            timeOfDay.globalTimeSpeedMultiplier = Configs.GlobalTimeSpeedMulti.Value();
        }

        public static bool CanInteractWithTerminal(InteractTrigger __instance)
        {
            if (!NetworkManager.Singleton.IsServer)
                return false;

            if (WorldTweaks.Configs.AllowClientsToUseTerminal.Value())
                return false;

            return WorldTweaks.TerminalInteractTriggerInstance == __instance;

        }

        public static void MakeTerminalUnusableForAnyoneButHost()
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (Configs.AllowClientsToUseTerminal.Value())
                return;

            object[] args = { 0 };
            terminalTriggerInUseRPC.Invoke(TerminalInteractTriggerInstance, args);
        }

        public static void ResetValues(InteractTrigger trigger)
        {
            if (trigger.hidePlayerItem && StartOfRound.Instance.allPlayerScripts[0].currentlyHeldObjectServer != null)
            {
                StartOfRound.Instance.allPlayerScripts[0].currentlyHeldObjectServer.EnableItemMeshes(true);
                typeof(InteractTrigger).GetField("playerUsingId", BindingFlags.NonPublic| BindingFlags.Instance).SetValue(trigger, -1);
            }
            StartOfRound.Instance.allPlayerScripts[0].currentTriggerInAnimationWith = null;
            trigger.isPlayingSpecialAnimation = false;
        }
    }
}
