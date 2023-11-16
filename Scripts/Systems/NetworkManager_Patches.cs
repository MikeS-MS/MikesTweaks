using HarmonyLib;
using MikesTweaks.Scripts.Networking;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Systems;

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