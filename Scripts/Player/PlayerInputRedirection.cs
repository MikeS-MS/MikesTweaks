using GameNetcodeStuff;
using MikesTweaks.Scripts.Input;
using MikesTweaks.Scripts.Inventory;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MikesTweaks.Scripts.Networking;
using MikesTweaks.Scripts.World;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

namespace MikesTweaks.Scripts.Player
{
    public class PlayerInputRedirection : MonoBehaviour, MikesTweaksPlayerInput.IHotbarActions, MikesTweaksPlayerInput.IEmotesActions, MikesTweaksPlayerInput.IActionsActions
    {
        private PlayerControllerB owner = null;
        private MikesTweaksPlayerInput input = null;
        private MethodInfo SwitchToSlotMethod = null;
        private WalkieTalkie WalkieTalkieToStop = null;
        private FlashlightItem FlashlightToStop = null;

        public void OnHotbar1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            RequestSlotChange(0);
        }

        public void OnHotbar2(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            RequestSlotChange(1);
        }

        public void OnHotbar3(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            RequestSlotChange(2);
        }

        public void OnHotbar4(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            RequestSlotChange(3);
        }

        public void OnHotbar5(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(4))
                return;

            RequestSlotChange(4);
        }

        public void OnHotbar6(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(5))
                return;

            RequestSlotChange(5);
        }

        public void OnHotbar7(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(6))
                return;

            RequestSlotChange(6);
        }

        public void OnHotbar8(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(7))
                return;

            RequestSlotChange(7);
        }

        public void OnHotbar9(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            if (!InventoryTweaks.HasEnoughSlots(8))
                return;

            RequestSlotChange(8);
        }

        public void OnEmote1(InputAction.CallbackContext context)
        {
            owner?.PerformEmote(context, 1);
        }

        public void OnEmote2(InputAction.CallbackContext context)
        {
            owner?.PerformEmote(context, 2);
        }

        public void OnFlashlightToggle(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;

            ToggleFlashlight();
        }

        public void OnWalkieTalkieSpeak(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                {
                    UseWalkieTalkie();
                    break;
                }
                case InputActionPhase.Canceled:
                {
                    StopUsingWalkieTalkie();
                    break;
                }
            }
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

        public void InitializeKeybinds()
        {
            owner = gameObject.GetComponent<PlayerControllerB>();
            input = new MikesTweaksPlayerInput();
            input.Hotbar.SetCallbacks(this);
            input.Emotes.SetCallbacks(this);
            input.Actions.SetCallbacks(this);
            input.Enable();

            SetupKeybinds();
        }

        public void SetupKeybinds()
        {
            input.Hotbar.Hotbar1.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[0].Value());

            input.Hotbar.Hotbar2.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[1].Value());

            input.Hotbar.Hotbar3.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[2].Value());

            input.Hotbar.Hotbar4.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[3].Value());

            input.Hotbar.Hotbar5.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[4].Value());

            input.Hotbar.Hotbar6.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[5].Value());

            input.Hotbar.Hotbar7.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[6].Value());

            input.Hotbar.Hotbar8.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[7].Value());

            input.Hotbar.Hotbar9.ChangeBinding(0).WithPath(PlayerTweaks.Configs.SlotKeybinds[8].Value());

            input.Emotes.Emote1.ChangeBinding(0).WithPath(PlayerTweaks.Configs.EmoteKeybinds[0].Value());

            input.Emotes.Emote2.ChangeBinding(0).WithPath(PlayerTweaks.Configs.EmoteKeybinds[1].Value());

            input.Actions.FlashlightToggle.ChangeBinding(0).WithPath(PlayerTweaks.Configs.FlashlightKeybind.Value());
            input.Actions.WalkieTalkieSpeak.ChangeBinding(0).WithPath(PlayerTweaks.Configs.WalkieTalkieKeybind.Value());
        }

        private void FindBestFlashlight(ref FlashlightItem BestFlashlight, ref List<FlashlightItem> Flashlights)
        {
            foreach (FlashlightItem flashlight in Flashlights)
            {
                if (BestFlashlight == null)
                {
                    BestFlashlight = flashlight;
                    continue;
                }

                if (flashlight.insertedBattery.charge > BestFlashlight.insertedBattery.charge && flashlight.insertedBattery.charge > 0f)
                    BestFlashlight = flashlight;
            }
        }

        private void UseSelectedFlashlight(ref FlashlightItem Flashlight, ref FieldInfo timeSinceSwitchingSlots)
        {
            bool pocketed = Flashlight.isPocketed;
            Flashlight.UseItemOnClient();
            timeSinceSwitchingSlots.SetValue(owner, 0f);
            FlashlightToStop = Flashlight;
            if (!pocketed)
                return;

            Flashlight.playerHeldBy.ChangeHelmetLight(Flashlight.flashlightTypeID, Flashlight.isBeingUsed);
            Flashlight.PocketItem();
        }

        private void ToggleFlashlight()
        {
            if (!NetworkManager.Singleton.IsServer && !ConfigsSynchronizer.ConfigsReceived)
                return;

            if (!WorldTweaks.Configs.AllowFlashlightKeybind.Value() || MikesTweaks.Compatibility.ReservedSlotsFlashlightCompat)
                return;

            bool canUseItem = PlayerTweaks.CanUseItem(owner);
            if (!canUseItem)
                return;

            FieldInfo timeSinceSwitchingSlots = typeof(PlayerControllerB)
                .GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance);

            if ((float)timeSinceSwitchingSlots.GetValue(owner) < 0.075f)
                return;

            if (FlashlightToStop)
            {
                if (FlashlightToStop.playerHeldBy == owner)
                {
                    if (FlashlightToStop.flashlightTypeID == 0 && FlashlightToStop.insertedBattery.charge > 0)
                    {
                        UseSelectedFlashlight(ref FlashlightToStop, ref timeSinceSwitchingSlots);
                        return;
                    }
                }
            }

            FlashlightItem BestFlashlight = null;
            List<FlashlightItem> ProFlashlights = new List<FlashlightItem>();
            List<FlashlightItem> NormalFlashlights = new List<FlashlightItem>();

            foreach (GrabbableObject item in owner.ItemSlots)
            {
                BestFlashlight = item as FlashlightItem;

                if (BestFlashlight == null)
                    continue;

                switch (BestFlashlight.flashlightTypeID)
                {
                    case 0:
                    {
                        ProFlashlights.Add(BestFlashlight);
                        break;
                    }
                    case 1:
                    {
                        NormalFlashlights.Add(BestFlashlight);
                        break;
                    }
                    default:
                        break;
                }
            }

            BestFlashlight = null;
            FindBestFlashlight(ref BestFlashlight, ref ProFlashlights);

            if (BestFlashlight != null)
            {
                UseSelectedFlashlight(ref BestFlashlight, ref timeSinceSwitchingSlots);
                return;
            }

            if (FlashlightToStop)
            {
                if (FlashlightToStop.playerHeldBy == owner)
                {
                    if (FlashlightToStop.flashlightTypeID == 1 && FlashlightToStop.insertedBattery.charge > 0)
                    {
                        UseSelectedFlashlight(ref FlashlightToStop, ref timeSinceSwitchingSlots);
                        return;
                    }
                }
            }

            FindBestFlashlight(ref BestFlashlight, ref NormalFlashlights);

            if (BestFlashlight != null)
            {
                UseSelectedFlashlight(ref BestFlashlight, ref timeSinceSwitchingSlots);
                return;
            }
        }

        private void UseWalkieTalkie()
        {
            if (!NetworkManager.Singleton.IsServer && !ConfigsSynchronizer.ConfigsReceived)
                return;

            if (!WorldTweaks.Configs.AllowWalkieTalkieKeybind.Value() || MikesTweaks.Compatibility.ReservedSlotsWalkieCompat)
                return;

            bool canUseItem = PlayerTweaks.CanUseItem(owner);
            if (!canUseItem)
                return;

            FieldInfo timeSinceSwitchingSlots = typeof(PlayerControllerB)
                .GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance);

            if ((float)timeSinceSwitchingSlots.GetValue(owner) < 0.075f)
                return;

            WalkieTalkie BestWalkieTalkie = null;
            List<WalkieTalkie> WalkieTalkieList = new List<WalkieTalkie>();
            foreach (GrabbableObject item in owner.ItemSlots)
            {
                BestWalkieTalkie = item as WalkieTalkie;

                if (BestWalkieTalkie == null)
                    continue;
                WalkieTalkieList.Add(BestWalkieTalkie);
            }

            BestWalkieTalkie = null;
            foreach (WalkieTalkie walkie in WalkieTalkieList)
            {
                if (BestWalkieTalkie == null)
                {
                    BestWalkieTalkie = walkie;
                    continue;
                }

                if (!walkie.isBeingUsed)
                    continue;

                if (walkie.insertedBattery.charge <= BestWalkieTalkie.insertedBattery.charge)
                    continue;

                BestWalkieTalkie = walkie;
            }

            if (BestWalkieTalkie == null)
                return;

            BestWalkieTalkie.UseItemOnClient();
            timeSinceSwitchingSlots.SetValue(owner, 0f);
            WalkieTalkieToStop = BestWalkieTalkie;
        }

        private void StopUsingWalkieTalkie()
        {
            if (!WalkieTalkieToStop) return;
            if (WalkieTalkieToStop.playerHeldBy != owner) return;

            WalkieTalkieToStop.UseItemOnClient(false);
        }

        private void Awake()
        {
            owner = gameObject.GetComponent<PlayerControllerB>();
            SwitchToSlotMethod = typeof(PlayerControllerB).GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void RequestSlotChange(int slot)
        {
            if (!PlayerTweaks.CanSwitchSlot(owner) || !WorldTweaks.Configs.AllowHotbarKeybinds.Value())
                return;

            SwitchToSlot(slot);
            typeof(PlayerControllerB).GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(owner, 0f);
            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;
            FastBufferWriter writer = new FastBufferWriter(sizeof(int), Allocator.Temp);
            writer.WriteValueSafe(slot);
            manager.SendNamedMessage(PlayerTweaks.PlayerSwitchSlotRequestChannel, 0, writer, NetworkDelivery.Reliable);
        }

        public void SwitchToSlot(int slot)
        {
            // because the dev sets them here
            ShipBuildModeManager.Instance.CancelBuildMode();
            int backup = owner.currentItemSlot;
            owner.playerBodyAnimator.SetBool("GrabValidated", false);
            object[] args = { slot, null };
            SwitchToSlotMethod.Invoke(owner, args);
        }

    }
}
