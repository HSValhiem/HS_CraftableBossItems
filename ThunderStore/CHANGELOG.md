## 0.1.4

- Fixed: the TrophyEikthyr "Station Level" setting was bound to the HardAntler section, so both recipes shared one entry. It now lives in "2 - TrophyEikthyr (Eikythr)".
- Fixed: refreshing the recipes (changing any enable setting, or a world start) added another copy of every enabled recipe. The mod now keeps its own recipe objects and replaces them.
- Fixed: turning a recipe off removed every recipe that makes that item, including other mods' recipes. Only this mod's recipe is removed now.
- Fixed: an unknown ingredient name was dropped silently, so the recipe got cheaper (or free). The recipe is now not registered and a warning names the unknown ingredient.
- Fixed: an ingredient listed twice in one recipe threw an error. The amounts are now added together, with a warning.
- Fixed: changing a recipe's crafting station did nothing until restart. Station and requirement changes now rebuild the recipe at once.
- Fixed: changing settings before a world was loaded (for example from the main menu or a server sync) could throw errors. Those changes now apply when the world loads.
- ServerSync minimum required version raised to 0.1.4: clients and servers must both run 0.1.4.
- Note: the TrophyEikthyr station level moved to its own key, so it resets to 3. Set it again under "2 - TrophyEikthyr (Eikythr)". The old shared "Station Level" key under "2 - HardAntler (Eikythr)" stays and keeps the HardAntler value.
- Built for Valheim 1.0.21 with the HS_ModStub build pipeline (net48, pinned ServerSync, no machine paths in the project).
- The Thunderstore page now links to https://github.com/HSValhiem/HS_CraftableBossItems.
- Added the MIT license.
