using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Networking
{
    public class CustomNetworking
    {
        public static CustomNetworking Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new CustomNetworking();
                return _Instance;
            }
        }

        private static CustomNetworking _Instance;
        private readonly List<string> Channels;

        public void RegisterChannel(string channel, CustomMessagingManager.HandleNamedMessageDelegate func)
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(channel, func);
            Channels.Add(channel);
        }

        public void UnregisterChannels()
        {
            Channels.Clear();
            return;
            foreach (var channel in Channels)
            {
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(channel);
            }

        }

        private CustomNetworking()
        {
            Channels = new List<string>();
        }
    }
}
