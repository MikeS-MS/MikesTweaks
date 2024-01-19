using UnityEngine.InputSystem;
using LethalCompanyInputUtils.Api;
using MikesTweaks.Scripts.Player;

namespace LethalAutocomplete
{
    internal class Keybinds : LcInputActions
    {
        public InputAction FlashlightAction => Asset["FlashlightToggle"];
        public InputAction WalkieTalkieAction => Asset["WalkieTalkieSpeak"];

        public InputAction HotBarAction(int slot)
        {
            return Asset[$"Hotbar{slot}"];
        }

        public InputAction Emote(int slot)
        {
            return Asset[$"Emote{slot}"];
        }


        public override void CreateInputActions(in InputActionMapBuilder builder)
        {
            base.CreateInputActions(builder);

            for (int i = 0; i < PlayerTweaks.Configs.SlotKeybinds.Length; i++)
            {
                builder.NewActionBinding()
                    .WithActionId($"Hotbar{i+1}")
                    .WithActionType(InputActionType.Button)
                    .WithKbmPath(PlayerTweaks.Configs.SlotKeybinds[i].Value())
                    .WithBindingName($"Hotbar{i+1} Key")
                    .Finish();
            }

            builder.NewActionBinding()
                .WithActionId($"FlashlightToggle")
                .WithActionType(InputActionType.Button)
                .WithKbmPath(PlayerTweaks.Configs.FlashlightKeybind.Value())
                .WithBindingName($"Toggle Flashlight")
                .Finish();

            builder.NewActionBinding()
                .WithActionId($"WalkieTalkieSpeak")
                .WithActionType(InputActionType.Button)
                .WithKbmPath(PlayerTweaks.Configs.WalkieTalkieKeybind.Value())
                .WithBindingName($"Walkie Talkie Speak")
                .Finish();

            for (int i = 0; i < PlayerTweaks.Configs.EmoteKeybinds.Length; i++)
            {
                builder.NewActionBinding()
                    .WithActionId($"Emote{i + 1}")
                    .WithActionType(InputActionType.Button)
                    .WithKbmPath(PlayerTweaks.Configs.SlotKeybinds[i].Value())
                    .WithBindingName($"Emote{i + 1} Key")
                    .Finish();
            }
        }
    }
}