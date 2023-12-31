# Version 2.0.2
**Added**
 - Compatibility mode for LateGameUpgrades that automatically disables my stamina recharge values and jump stamina drain value

**Changed**
 - Fixed a bug where using flashlight while it was on screen would toggle both the pocketed light and the light coming from the flashlight itself
 - Fixed a bug where the walkie talkie button would not work
 - Fixed a bug where jump stamina drain would never be applied
 - Fixed typo in config description for the walkie-talkie use game rule

# Version 2.0.1
**Changed**
 - Fixed styling issue in CHANGELOG

# Version 2.0 - The Smart Update
**Added**
 - Smart Flashlight Use with the hotkey, which caches the best flashlight and uses it until it cannot anymore (either no charge left, not in inventory or doesn't exist). The priority is as follows Pro-Flashlight > Normal Flashlight then Higher Battery Charge > Lower Battery Charge
 - Smart Walkie Use with the hotkey, which uses the highest battery WalkieTalkie that is tured on. It will not automatically turn on walkies that are turned off even if they are a better candidate - Compatibility for the LethalThings mod, that disables my way of adding inventory slots. The only mod that will be adding inventory slots will be LethalThings with the Belt item

**Changed**
 - Fixed a bug where pressing the hotkeys on clients will change the current slot of the host - ReservedSlot mods now have separate compatibility options, if the flashlight one is detected, only my flashlight hotkey will be disabled, if the walkie one is detected my walkie hotkey will be disabled, if any of them are detected, the inventory slots will not extend regardless to what you set them as in the config

 **Removed**
 - The option that allowed you to bind mouse buttons to push-to-talk, due to it being integrated into the game

# Version 1.9.2
**Changed**
 - Fixed wrong vanilla value for `GlobalTimeSpeedMultiplier`, was 1, now it's 1.4 which is the correct vanilla value - Fixed missing README entry for the new item that was introduced in v45

# Version 1.9.1
## Important! Backup your changed values in the config file, delete the config file, run the game again to regenerate it and change the configs again to what they were before
**Changed**
 - Fixed incorrect internal names causing prices to not apply to given items - Fixed the `UseVanillaToolPriceValues` not appearing in configs

# Version 1.9
## Important! Backup your changed values in the config file, delete the config file, run the game again to regenerate it and change the configs again to what they were before
**Added**
 - A way to change the price for all items that can be picked up in your inventory - A Game rule that sets it so that all prices are vanilla and not what you set them as

**Changed**
 - Fixed a bug where emotes are still bound to 1 and 2 as well as whatever is set in the config file, introduced in Lethal Company v45

# Version 1.8.3
**Changed**
 - Fixed incorrect vanilla value for your max stamina when using `UseVanillaStaminaValues`, it was 5 before this update, but the actual vanilla default value is 11

# Version 1.8.2
**Changed**
 - Fixed a bug where Terminal Item Weights would not be synced to the clients
 - Fixed a bug where when syncing configs it wouldn't sync any due to buffer size issues

# Version 1.8.1
**Changed**
 - Fixed a bug when `UseVanillaMoonCosts` was set to `true`

# Version 1.8
**Added**
 - A way to change the cost of traveling to every moon, even ones added by mods if they implemented them the vanilla way - A Game Rule that uses the vanilla values for the cost of traveling to moons

# Version 1.7
**Added**
 - A configurable keybind to use walkie talkies (if present in inventory, will pick the first one left to right)
 - A Game Rule to allow the usage of the walkie talkie keybind
 
# Version 1.6.5
**Changed**
 - Fixed a description typo, no need to update to this if you have 1.6.4

# Version 1.6.4
**Changed**
 - Fixed items becoming desynced by staying in place for some clients while they were actually in someone else's invetory

# Version 1.6.3
**Added**
 - Compatibility with the ReservedSlot mods which disables extra inventory slots and flashlight toggle, which is automatically enabled if the mod detects that any of the ReservedSlot mods are loaded

# Version 1.6.2
**Changed**
 - Fixed keybinds not working under certain conditions (hopefully)
 - Fixed Menu opening when exiting the Terminal

# Version 1.6.1
## IMPORTANT!
 - I recommend you update to this version as soon as possible if you use this option and want it to work properly
**Changed**
- Fixed terminal becoming accessible to clients with the `AllowClientsToUseTerminal` Game Rule set to false after the host gets teleported inside or outside the facility

# Version 1.6
**Added**
 - A Game Rule config to disallow people who join your lobby from using the hotbar keybinds
 - A Game Rule config to disallow people from accessing the terminal
 - A Game Rule config to use vanilla defaults for all Sprint related options without having to manually change each one to the vanilla default (does not overwrite your stored values, only in-game values use the vanilla ones instead)
 - A Game Rule config to use vanilla defaults for all Stamina related options without having to manually change each one to the vanilla default (does not overwrite your stored values, only in-game values use the vanilla ones instead)
 - A Game Rule config to use vanilla defaults for all Terminal Item Weight options without having to manually change each one to the vanilla default (does not overwrite your stored values, only in-game values use the vanilla ones instead)
 - If you have any of the vanilla override game rules set to true, when you join someone's lobby you will use the vanilla default settings for that category (stamina/sprint speed/item weights), instead of use the synced ones from the host

# Version 1.5
## IMPORTANT!
 - I've renamed a few configs so it's best to delete the config file you have and let it regenerate by launching the game and change the configs back to your liking
**Added**
 - BepinEx as dependency to be installed
 - A config that allows you to bind the first available flashlight in your inventory to a key
 - A config that allows you to disallow people from using the flashlight keybind if they join your lobby
 - Configs that allow you to modify Stamina Recharging

# Version 1.4.3
**Changed**
 - Fixed link in README.md to link to the correct place

# Version 1.4.2
**Added**
 - Switching between item slots will now properly sync to other players with the mod. This cannot work for people without the mod due to the original game's code
 - The Push To Talk key can be bound to mouse buttons
 
# Version 1.4
**Added**
 - Configs are now synced from Host to Client when someone joins your lobby
 - Your config values in memory load from your config file when you go back to the Main Menu so that when you host you have your own settings applied

# Version 1.3
**Added**
 - A way to bind hotbar slots (bound by default to 1-9 keys)
 - A way to bind emotes (bound by default to Y and U keys)