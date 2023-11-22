using HarmonyLib;
using MikesTweaks.Scripts.Inventory;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.Player;
using Unity.Netcode;

namespace MikesTweaks.Scripts.Systems;

[HarmonyPatch(typeof(StartOfRound))]
public static class StartOfRound_Patches
{
    [HarmonyPatch("OnPlayerConnectedClientRpc")]
    [HarmonyPostfix]
    private static void OnPlayerConnectedClientRpc(StartOfRound __instance, ulong clientId, int assignedPlayerObjectId)
    {
        //PlayerTweaks.LocalPlayerController = __instance.allPlayerScripts[assignedPlayerObjectId];

        if (NetworkManager.Singleton.IsServer)
            return;

        ConfigsSynchronizer.Instance.RequestConfigs();
    }
}