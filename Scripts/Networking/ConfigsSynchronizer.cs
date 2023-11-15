using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using HarmonyLib;
using MikesTweaks.Scripts.Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace MikesTweaks.Scripts.Networking
{
    public class ConfigsSynchronizer
    {
        public static Action OnConfigsChangedDelegate;

        public static ConfigsSynchronizer Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ConfigsSynchronizer();
                return _Instance;
            }
        }

        private static string SyncChannelName => "ConfigsSync";
        private static string RequestChannelName => "RequestConfigsSync";
        private static ConfigsSynchronizer _Instance = null;
        private List<Func<int>> ConfigSizeGetters = new List<Func<int>>();
        private List<Func<FastBufferReader, FastBufferReader>> ConfigSetters = new List<Func<FastBufferReader, FastBufferReader>>();
        private List<Func<FastBufferWriter, FastBufferWriter>> ConfigGetters = new List<Func<FastBufferWriter, FastBufferWriter>>();

        public void AddConfigSizeGetter(Func<int> SizeGetter)
        {
            ConfigSizeGetters.Add(SizeGetter);
        }            

        public void AddConfigSetter(Func<FastBufferReader, FastBufferReader> Setter)
        {
            ConfigSetters.Add(Setter);
        }        

        public void AddConfigGetter(Func<FastBufferWriter, FastBufferWriter> Getter)
        {
            ConfigGetters.Add(Getter);
        }

        public void RegisterMessages()
        {
            //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(SyncChannelName, ReceiveConfigs);
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(RequestChannelName, ReceiveRequestConfigs);
        }

        public void RequestConfigs()
        {
            FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;

            manager.SendNamedMessage(RequestChannelName, 0, writer, NetworkDelivery.Reliable);
        }

        public void ReceiveRequestConfigs(ulong senderID, FastBufferReader payload)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            SendConfigs(senderID);
        }

        public void ReceiveConfigs(ulong senderID, FastBufferReader payload)
        {
            if (NetworkManager.Singleton.IsServer)
                return;

            foreach (var setter in ConfigSetters)
            {
                payload = setter.Invoke(payload);
            }

            OnConfigsChangedDelegate?.Invoke();
        }

        private ConfigsSynchronizer()
        {

        }

        private void SendConfigs(ulong clientID)
        {
            if (clientID == NetworkManager.Singleton.LocalClientId)
                return;

            int size = 0;
            foreach (var configSizeGetter in ConfigSizeGetters)
            {
                size += configSizeGetter.Invoke();
            }

            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;
            FastBufferWriter writer = new FastBufferWriter(size, Allocator.Temp);

            foreach (var getter in ConfigGetters)
            {
                writer = getter.Invoke(writer);
            }

            manager.SendNamedMessage(SyncChannelName, clientID, writer, NetworkDelivery.Reliable);
        }
    }

    [HarmonyPatch(typeof(NetworkManager))]
    public static class NetworkManager_Patches
    {
        [HarmonyPatch("StartHost")]
        [HarmonyPostfix]
        private static void StartHost_Post(GameNetworkManager __instance)
        {
            ConfigsSynchronizer.Instance.RegisterMessages();
        }

        [HarmonyPatch("StartClient")]
        [HarmonyPostfix]
        private static void StartClient_Post(GameNetworkManager __instance)
        {
            ConfigsSynchronizer.Instance.RegisterMessages();
        }
    }


    [HarmonyPatch(typeof(StartOfRound))]
    public static class ConfigsRelated_Patches
    {
        [HarmonyPatch(typeof(StartOfRound))]
        [HarmonyPatch("OnPlayerConnectedClientRpc")]
        [HarmonyPostfix]
        private static void OnPlayerConnectedClientRpc(StartOfRound __instance, ulong clientId, int assignedPlayerObjectId)
        {
            PlayerTweaks.LocalPlayerController = __instance.allPlayerScripts[assignedPlayerObjectId];

            if (NetworkManager.Singleton.IsServer)
                return;

            ConfigsSynchronizer.Instance.RequestConfigs();
        }

        [HarmonyPatch(typeof(MenuManager))]
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void MenuManager_Start(MenuManager __instance)
        {
            MikesTweaks.Instance.LoadConfigs();
        }
    }
}
