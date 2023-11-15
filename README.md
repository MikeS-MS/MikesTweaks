# MikesTweaks
Mod for Lethal Company with multiple tweaks to make the game better

# Requirements
- It's a good idea for everyone that will join your lobby to have this mod installed as well and have the same version. Not having the mod and joining someone who does might result in unexpected behaviour and bugs.
- You will need BepinEx to use this mod, which you can check how to get and install from [here](https://www.example.com) (this message will stay here until BepinEx for Lethal Company has been released on the Thunderstore so I can add it as a dependency and it automatically downloads)

# These are the default tweaks that are adjustable from the config file:
  - Config File is located at (GAME_DIRECTORY/BepinEx/configs/mikes.lethalcompany.mikestweaks.cfg)
  - (Synced) means that the configs in that category will be synchronized from the host to every client that joins their lobby.
  - (Individual) means that the configs in that category will not be synchronized from the host to every client that joins their lobby.
## 1. Inventory slots (Synced)
  - Slots Amount: 6 (was 4)
## 2. Item weights changes (Synced)
  - WalkieTalkie: 0 lb (was 0 lb)
  - Flashlight: 0 lb (was 0 lb)
  - Shovel: 5 lb (was 18 lb)
  - LockPicker: 2 lb (was 15 lb)
  - ProFlashlight: 0 lb (was 5 lb)
  - StunGrenade: 2 lb (was 5 lb)
  - Boombox: 5 lb (was 15 lb)
  - TZPInhalant: 0 lb (was 0 lb)
  - ZapGun: 4 lb (was 10 lb)
  - Jetpack: 10 lb (was 50 lb)
  - ExtensionLadder: 0 lb (was 0 lb)
  - Radar Booster: 5 lb (was 18 lb)
## 3. Player Keybinds (Individual)
  - Hotbar slots: 1-9 keyboard keys
  - Emotes: Y and U keys
## 4. Player Sprint (Synced)
  - Sprint Longevity: 12 (was 5)
  - Stamina Drain from jump: 0.04 (was 0.08)
  - Default Sprint Multiplier value: 1.5 (was 1)
  - Max Sprint Multiplier Value: 3 (was 2.25)
  - Sprint Multiplier Increase: 1 (was 1)
  - Sprint Multiplier Decrease: 10 (was 10)
## 5. World Time Speed (Synced)
  - Speed: 0.5 (was 1)

# Network additions
 - When switching to another slot using the hotkeys it will properly sync which slot/item you're holding to the other people in the lobby.
 - Configs are synced from Host to Client when that Client joins the lobby
 - Your Config File will not be overwritten by the sync, only the values of those in memory when the game is running.
 - The Config Values will reset to what they are in your Config File when you go back to the main menu so when you decide to host it will use your own settings instead of the ones that were synced to you from the last lobby you had joined.

 # Known Issues
 - Going past 7 slots (i.e ExtraSlotsAmount = 3 in the config) is generally not a good idea as the slots will go off screen depending on your resolution. Will look into providing better support for this in the future.