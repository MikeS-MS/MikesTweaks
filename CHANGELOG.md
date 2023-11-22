# Version 1.6.5
**Changed**
 - Fixed a description typo, no need to update to this if you have 1.6.4

# Version 1.6.4
**Changed**
 - Fixed items becoming desynced by staying in place for some clients while they were actually in someone else's invetory.

# Version 1.6.3
**Added**
 - Compatibility with the ReservedSlot mods which disables extra inventory slots and flashlight toggle, which is automatically enabled if the mod detects that any of the ReservedSlot mods are loaded.

# Version 1.6.2
**Changed**
 - Fixed keybinds not working under certain conditions (hopefully)
 - Fixed Menu opening when exiting the Terminal

# Version 1.6.1
## IMPORTANT!
 - I recommend you update to this version as soon as possible if you use this option and want it to work properly.

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
 - I've renamed a few configs so it's best to delete the config file you have and let it regenerate by launching the game and change the configs back to your liking.

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