# MikesTweaks
Mod for Lethal Company with multiple configs for tweaks to customize your game's experience. 

# Important! 
**If you're updating from before 1.9, backup your changed values in the config file, delete the config file, run the game again after updating to regenerate it and change the configs again to what they were before.**

**This is due to changes to some of the config values' names.**
# Target Game Version: v45

# Requirements
- It's a good idea for everyone that will join your lobby to have this mod installed as well and have the same version. Not having the mod and joining someone who does, might result in unexpected behaviour and bugs, but you are free to do so. Keep in mind that almost none of the features will work if you join someone's lobby that does not have my mod installed. I'm certain it's only the hotkey keybinds that are usable, but switching slots will not be synced to other players.
- You will need BepinEx to use this mod.

# Compatibility
- (Automatically Disabled) - means that regardless of what that config is set to, it will be automatically disabled, until the mod which caused the compatibility mode to start is disabled or removed (needs game restart)

- (Modifiable) - means that it can still be changed and it will take effect

- The mod auto detects and enters compatibility mode if it sees any of the following mods loaded:
    - LateGameUpgrades:
      - Stamina recharge values will not get applied (Automatically Disabled)
      - Jump stamina drain will not be applied (Automatically Disabled)
      
    - LethalThings:
      - Does not create extra slots from my mod (Automatically Disabled)
      - Item Slot keybinds still work, even for the reserved slots (Modifiable)
      
    - Any ReservedSlot mods:
      - Does not create extra slots from my mod (Automatically Disabled)
      - Item Slot keybinds still work, even for the reserved slots (Modifiable)

    - ReservedFlashlightSlot:
      - Disables flashlight keybind from my mod (Automatically Disabled)

    - ReservedWalkieSlot:
      - Disables the WalkieTalkie keybind from my mod (Automatically Disabled)

# These are the default tweaks that are adjustable from the config file:
## Info
  - For more detailed information on the config options check the config file.

  - Config File is located at (GAME_DIRECTORY/BepinEx/configs/mikes.lethalcompany.mikestweaks.cfg)

  - If you use the "Use Vanilla" game rules as a client you use the vanilla settings for that category regardless of the host's settings.

  - If you are the host and are using the vanilla "Use Vanilla" game rules, every client will also use vanilla settings for that category.

  - (Synced) means that the configs in that category will be synchronized from the host to every client that joins their lobby which has the same version of the mod.

  - (Individual) means that the configs in that category will not be synchronized from the host to every client that joins their lobby.

  - (Partial) - Some Configs might not be synced in that category.

  - (Networked) - Not Synced config but applies using the vanilla networking. Typically means even people without the mod will receive the changes.
  
## 1. Inventory slots (Synced)
  - Slots Amount: 6 (was 4)
## 2. Tool Item weights changes (Synced)
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
  - RadarBooster: 5 lb (was 18 lb)
  - SprayPaint: 1 lb (was 1 lb)
## 3. Tool Item Prices (Synced)
  - WalkieTalkie: 12 (was 12)
  - Flashlight: 15 (was 15)
  - Shovel: 30 (was 30)
  - LockPicker: 20 (was 20)
  - ProFlashlight: 25 (was 25)
  - StunGrenade: 30 (was 30)
  - Boombox: 60 (was 60)
  - TZPInhalant: 120 (was 120)
  - ZapGun: 400 (was 400)
  - Jetpack: 700 (was 700)
  - ExtensionLadder: 60 (was 60)
  - RadarBooster: 60 (was 60)
  - SprayPaint: 50 (was 50)
## 3. Player Keybinds (Individual)
  - Hotbar slots: 1-9 keyboard keys
  - Emotes: Y and U keys
  - Flashlight Keybind: F 
  - WalkieTalkie Keybind: R
## 4. Smart Functionality
  - Using the Flashlight hotkey will find the best flashlight you have in your inventory and cache it for later usage. It will try to find another best flashlight if the cached one is no longer in your inventory anymore, or if it's out of power, or if is a normal flashlight, meaning it will always try to find a pro-flashlight in your inventory if your cached one was a normal one. Priority is as follows: Pro-Flashlight > Normal Flashlight > Higher Battery Charge > Lower Battery Charge

  - Using the WalkieTalkie hotkey will prioritize using the highest battery charge walkie in your inventory that is turned on. It will not automatically turn on another walkie talkie even if it is present in your inventory and it has higher battery charge.
## 5. Player Sprint (Synced)
  - Sprint Stamina: 15 (was 11)
  - Stamina Drain from jump: 0.04 (was 0.08)
  - Default Sprint Speed value: 1.5 (was 1)
  - Max Sprint Speed Value: 3 (was 2.25)
  - Sprint Speed Increase: 1 (was 1)
  - Sprint Speed Decrease: 10 (was 10)
  - Stamina Recharge Rate: 5 (was 1)
  - Stamina Recharge Weight while walking: 9 (was 9)
  - Stamina Recharge Weight while standing still: 4 (was 4)
## 6. Moon Settings (Synced)
  - Moon costs can be changed, I've not set a custom cost on them by default, I left them at the vanilla defaults.
## 7. World Settings (Synced)
  - Time Speed: 0.5 (was 1)
## 8. Game Rules (Partial)
  - Allow Flashlight Keybind (allows/disallows using the quick-use Flashlight keybind for everyone): true (vanilla: false) (Synced)

  - Allow WalkieTalkie Keybind (allows/disallows using the quick-use Flashlight keybind for everyone): true (vanilla: false) (Synced)

  - Allow Hotbar Slot Keybinds (allows/disallows using the quick-use hotbar keybinds for everyone): true (vanilla: false) (Synced)

  - Allow Terminal Use by Clients (allows/disallows using the terminal as a client): true (vanilla: false) (Networked)

  - Use Vanilla Defaults for Moon Costs (allows you to use the vanilla settings for moon costs without changing all the individual values tied to moon costs, does not overwrite your values, only ingame values): false (vanilla: true) (Individual)

  - Use Vanilla Defaults for Sprinting (allows you to use the vanilla settings for sprinting without changing all the individual values tied to sprinting, does not overwrite your values, only ingame values): false (vanilla: true) (Individual)

  - Use Vanilla Defaults for Stamina (allows you to use the vanilla settings for stamina without changing all the individual values tied to stamina, does not overwrite your values, only ingame values): false (vanilla: true) (Individual)

  - Use Vanilla Defaults for Tool Item Weights (allows you to use the vanilla settings for the weight of tool items without changing all the individual values tied to them, does not overwrite your values, only ingame values): false (vanilla: true) (Individual)

  - Use Vanilla Defaults for Tool Item Prices (allows you to use the vanilla settings for the prices of tool items without changing all the individual values tied to them, does not overwrite your values, only ingame values): false (vanilla: true) (Synced)

# Network additions
 - When switching to another slot using the hotkeys it will properly sync which slot/item you're holding to the other people in the lobby.
 - Configs are synced from Host to Client when that Client joins the lobby
 - Your Config File will not be overwritten by the sync, only the values of those in memory when the game is running.
 - The Config Values will reset to what they are in your Config File when you go back to the main menu so when you decide to host it will use your own settings instead of the ones that were synced to you from the last lobby you had joined.

 # Known Issues
 - Going past 7 slots (i.e ExtraSlotsAmount = 3 in the config) is generally not a good idea as the slots will go off screen depending on your resolution. Will look into providing better support for this in the future.
 - When terminal use for clients is disabled, when you go in and out of the terminal as a host your currently equipped item's model will be hidden for everyone else but you, until you reselect that item.