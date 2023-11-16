using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

        public static byte[] ToBytes(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        public static object ToObject(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return bf.Deserialize(ms);
        }


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
            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;

            byte[] versionAsBytes = ToBytes(MikesTweaks.Version);
            FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(versionAsBytes), Allocator.Temp);

            using (writer)
            {
                writer.WriteValueSafe(versionAsBytes);
                manager.SendNamedMessage(RequestChannelName, 0, writer, NetworkDelivery.Reliable);
            }
        }

        public void ReceiveRequestConfigs(ulong senderID, FastBufferReader payload)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (payload.Length <= 8)
            {
                //StartOfRound.Instance.KickPlayer((int)senderID);
                return;
            }

            payload.ReadValueSafe(out byte[] versionAsBytes);

            if ((string)ToObject(versionAsBytes) != MikesTweaks.Version)
            {
                //StartOfRound.Instance.KickPlayer((int)senderID);
                return;
            }

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
}
