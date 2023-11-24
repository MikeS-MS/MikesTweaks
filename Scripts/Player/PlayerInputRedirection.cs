using GameNetcodeStuff;
using MikesTweaks.Scripts.Input;
using MikesTweaks.Scripts.Inventory;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

        private void ToggleFlashlight()
        {
            if (!WorldTweaks.Configs.AllowFlashlightKeybind.Value() || MikesTweaks.Compatibility.ReservedSlotsCompat)
                return;

            bool canUseItem = PlayerTweaks.CanUseItem(owner);
            if (!canUseItem)
                return;

            FieldInfo timeSinceSwitchingSlots = typeof(PlayerControllerB)
                .GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance);

            if ((float)timeSinceSwitchingSlots.GetValue(owner) < 0.075f)
                return;

            /* TODO: 
             * Prioritize Pro flashlights over normal flashlights
             * Ignore laser pointers
             * Prioritize flashlights with batteries
             */
            foreach (GrabbableObject item in owner.ItemSlots)
            {
                FlashlightItem flashlight = item as FlashlightItem;

                if (flashlight == null)
                    continue;

                bool pocketed = flashlight.isPocketed;
                flashlight.UseItemOnClient();
                timeSinceSwitchingSlots.SetValue(owner, 0f);
                if (pocketed)
                    flashlight.PocketItem();
                return;
            }
        }

        private void UseWalkieTalkie()
        {
            if (!WorldTweaks.Configs.AllowWalkieTalkieKeybind.Value() || MikesTweaks.Compatibility.ReservedSlotsCompat)
                return;

            bool canUseItem = PlayerTweaks.CanUseItem(owner);
            if (!canUseItem)
                return;

            FieldInfo timeSinceSwitchingSlots = typeof(PlayerControllerB)
                .GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance);

            if ((float)timeSinceSwitchingSlots.GetValue(owner) < 0.075f)
                return;

            // TODO: Prioritize WalkieTalkies with batteries
            foreach (GrabbableObject item in owner.ItemSlots)
            {
                WalkieTalkieToStop = item as WalkieTalkie;

                if (WalkieTalkieToStop == null)
                    continue;

                WalkieTalkieToStop.UseItemOnClient();
                timeSinceSwitchingSlots.SetValue(owner, 0f);
                return;
            }
        }

        private void StopUsingWalkieTalkie()
        {
            WalkieTalkieToStop?.UseItemOnClient(false);
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
            CustomMessagingManager manager = NetworkManager.Singleton.CustomMessagingManager;
            FastBufferWriter writer = new FastBufferWriter(sizeof(int), Allocator.Temp);
            writer.WriteValueSafe(slot);
            manager.SendNamedMessage(PlayerTweaks.PlayerSwitchSlotRequestChannel, 0, writer, NetworkDelivery.Reliable);
            typeof(PlayerControllerB).GetField("timeSinceSwitchingSlots", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(owner, 0f);
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
