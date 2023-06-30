using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;
namespace HS_CraftableBossItems;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class HS_CraftableBossItems : BaseUnityPlugin
{
	public const string ModGUID = "hs.craftablebossitems";
	public const string ModName = "HS_CraftableBossItems";
    public const string ModVersion = "0.1.0";

	// Change MinimumRequiredVersion to the new version when changing config settings.
	public static ConfigSync ConfigSync = new(ModGUID)
	{
		DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = "0.1.0", ModRequired = true
	};

    #region Config Defines
    private static ConfigEntry<Toggle> _serverConfigLocked = null!;
    private static ConfigEntry<bool> _modEnabled = null!;

    public static ConfigEntry<CraftingTable> EikythrTable = null!;
	public static ConfigEntry<String> EikythrRequirements = null!;
	public static ConfigEntry<int> EikythrAmount = null!;
	public static ConfigEntry<int> EikythrLevel = null!;

    public static ConfigEntry<CraftingTable> TheElderTable = null!;
    public static ConfigEntry<String> TheElderRequirements = null!;
    public static ConfigEntry<int> TheElderAmount = null!;
    public static ConfigEntry<int> TheElderLevel = null!;

    public static ConfigEntry<CraftingTable> YagluthTable = null!;
    public static ConfigEntry<String> YagluthRequirements = null!;
    public static ConfigEntry<int> YagluthAmount = null!;
    public static ConfigEntry<int> YagluthLevel = null!;

    public static ConfigEntry<CraftingTable> DragonQueenTable = null!;
    public static ConfigEntry<String> DragonQueenRequirements = null!;
    public static ConfigEntry<int> DragonQueenAmount = null!;
    public static ConfigEntry<int> DragonQueenLevel = null!;

    public static ConfigEntry<CraftingTable> BonemassTable = null!;
    public static ConfigEntry<String> BonemassRequirements = null!;
    public static ConfigEntry<int> BonemassAmount = null!;
    public static ConfigEntry<int> BonemassLevel = null!;


    public static ConfigEntry<CraftingTable> TheQueenTable = null!;
    public static ConfigEntry<String> TheQueenRequirements = null!;
    public static ConfigEntry<int> TheQueenAmount = null!;
    public static ConfigEntry<int> TheQueenLevel = null!;
    #endregion

    #region Config Boilerplate

    private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
		bool synchronizedSetting = true)
	{
		ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);
		SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
		syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
		return configEntry;
	}

	private ConfigEntry<T> config<T>(string group, string name, T value, string description,
		bool synchronizedSetting = true)
	{
		return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
	}

	private enum Toggle
	{
		On = 1,
		Off = 0
	}

    #endregion


    public void Awake()
	{
        // Genera Config Settings
		_serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
		ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
        _modEnabled = config("1 - General", "Mod Enabled", true, "");
        _modEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();


        // Eikythr
        EikythrTable = config("2 - HardAntler (Eikythr)", "Table", CraftingTable.Workbench, "Crafting station needed to construct HardAntler.");
        EikythrTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(EikythrTable.Value)).GetComponent<CraftingStation>();
        EikythrRequirements = config("2 - HardAntler (Eikythr)", "Requirements", "TrophyDeer:20,Resin:2", "The required items to construct HardAntler.");
        EikythrRequirements.SettingChanged += (_, _) => 
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null)
                recipe.m_resources = GetRequirements(EikythrRequirements);
        };
        EikythrAmount = config("2 - HardAntler (Eikythr)", "Amount", 1, "The amount of HardAntler created.");
        EikythrAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null)
                recipe.m_amount = EikythrAmount.Value;
        };
        EikythrLevel = config("2 - HardAntler (Eikythr)", "Station Level", 3, "Level of crafting station required to craft HardAntler.");
        EikythrLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null)
                recipe.m_minStationLevel = EikythrLevel.Value;
        };


        // TheElder
        TheElderTable = config("3 - CryptKey (TheElder)", "Table", CraftingTable.Forge, "Crafting station needed to construct CryptKey.");
        TheElderTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheElderTable.Value)).GetComponent<CraftingStation>();
        TheElderRequirements = config("3 - CryptKey (TheElder)", "Requirements", "AncientSeed:5,Bronze:20", "The required items to construct CryptKey.");
        TheElderRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null)
                recipe.m_resources = GetRequirements(TheElderRequirements);
        };
        TheElderAmount = config("3 - CryptKey (TheElder)", "Amount", 1, "The amount of CryptKey created.");
        TheElderAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null)
                recipe.m_amount = TheElderAmount.Value;
        };
        TheElderLevel = config("3 - CryptKey (TheElder)", "Station Level", 3, "Level of crafting station required to craft CryptKey.");
        TheElderLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null)
                recipe.m_minStationLevel = TheElderLevel.Value;
        };

        // Bonemass
        BonemassTable = config("4 - Wishbone (Bonemass)", "Table", CraftingTable.Forge, "Crafting station needed to construct Wishbone.");
        BonemassTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(BonemassTable.Value)).GetComponent<CraftingStation>();
        BonemassRequirements = config("4 - Wishbone (Bonemass)", "Requirements", "WitheredBone:15,Iron:2,TrophyDraugr:3,TrophyDraugrElite:1", "The required items to construct Wishbone.");
        BonemassRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null)
                recipe.m_resources = GetRequirements(BonemassRequirements);
        };
        BonemassAmount = config("4 - Wishbone (Bonemass)", "Amount", 1, "The amount of Wishbone created.");
        BonemassAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null)
                recipe.m_amount = BonemassAmount.Value;
        };
        BonemassLevel = config("4 - Wishbone (Bonemass)", "Station Level", 7, "Level of crafting station required to craft Wishbone.");
        BonemassLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null)
                recipe.m_minStationLevel = BonemassLevel.Value;
        };

        // Dragon Queen
        DragonQueenTable = config("5 - DragonTear (Moder)", "Table", CraftingTable.Workbench, "Crafting station needed to construct DragonTear.");
        DragonQueenTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(DragonQueenTable.Value)).GetComponent<CraftingStation>();
        DragonQueenRequirements = config("5 - DragonTear (Moder)", "Requirements", "DragonEgg:3,Crystal:10", "The required items to construct DragonTear.");
        DragonQueenRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null)
                recipe.m_resources = GetRequirements(DragonQueenRequirements);
        };
        DragonQueenAmount = config("5 - DragonTear (Moder)", "Amount", 5, "The amount of DragonTear created.");
        DragonQueenAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null)
                recipe.m_amount = DragonQueenAmount.Value;
        };
        DragonQueenLevel = config("5 - DragonTear (Moder)", "Station Level", 5, "Level of crafting station required to craft DragonTear.");
        DragonQueenLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null)
                recipe.m_minStationLevel = DragonQueenLevel.Value;
        };

        // Yagluth
        YagluthTable = config("6 - Torn spirit (Yagluth)", "Table", CraftingTable.ArtisanTable, "Crafting station needed to construct Torn spirit.");
        YagluthTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(YagluthTable.Value)).GetComponent<CraftingStation>();
        YagluthRequirements = config("6 - Yagluth", "Requirements", "GoblinTotem:10,TrophyGoblin:3,TrophyGoblinShaman:1,TrophyGoblinBrute:1", "The required items to construct Torn spirit.");
        YagluthRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null)
                recipe.m_resources = GetRequirements(YagluthRequirements);
        };
        YagluthAmount = config("6 - Yagluth", "Amount", 3, "The amount of Torn spirit created.");
        YagluthAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null)
                recipe.m_amount = YagluthAmount.Value;
        };
        YagluthLevel = config("6 - Yagluth", "Station Level", 1, "Level of crafting station required to craft Torn spirit.");
        YagluthLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null)
                recipe.m_minStationLevel = YagluthLevel.Value;
        };

        // The Queen
        TheQueenTable = config("7 - QueenDrop (TheQueen)", "Table", CraftingTable.BlackForge, "Crafting station needed to construct QueenDrop.");
        TheQueenTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheQueenTable.Value)).GetComponent<CraftingStation>();
        TheQueenRequirements = config("7 - QueenDrop (TheQueen)", "Requirements", "DvergrKeyFragment:9,Mandible:5,TrophySeeker:3,TrophySeekerBrute:1", "The required items to construct QueenDrop.");
        TheQueenRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null)
                recipe.m_resources = GetRequirements(TheQueenRequirements);
        };
        TheQueenAmount = config("7 - QueenDrop (TheQueen)", "Amount", 3, "The amount of QueenDrop created.");
        TheQueenAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null)
                recipe.m_amount = TheQueenAmount.Value;
        };
        TheQueenLevel = config("7 - QueenDrop (TheQueen)", "Station Level", 1, "Level of crafting station required to craft QueenDrop.");
        TheQueenLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null)
                recipe.m_minStationLevel = TheQueenLevel.Value;
        };

        // Harmony Setup
        Harmony harmony = new Harmony(ModGUID);
        harmony.Patch(AccessTools.Method(typeof(ZNet), nameof(ZNet.Start)),
            prefix: new HarmonyMethod(typeof(HS_CraftableBossItems), nameof(InvokeOnVanillaObjectsAvailable)) 
        );
    }

    // Harmony Patch
    private static void InvokeOnVanillaObjectsAvailable()
    {
        if (_modEnabled.Value)
        {
            // Add Recipes to List if Mod Enabled
            AddRecipe("HardAntler", EikythrTable.Value, EikythrAmount.Value, EikythrLevel.Value, EikythrRequirements.Value);
            AddRecipe("CryptKey", TheElderTable.Value, TheElderAmount.Value, TheElderLevel.Value, TheElderRequirements.Value);
            AddRecipe("YagluthDrop", YagluthTable.Value, YagluthAmount.Value, YagluthLevel.Value, YagluthRequirements.Value);
            AddRecipe("DragonTear", DragonQueenTable.Value, DragonQueenAmount.Value, DragonQueenLevel.Value, DragonQueenRequirements.Value);
            AddRecipe("Wishbone", BonemassTable.Value, BonemassAmount.Value, BonemassLevel.Value, BonemassRequirements.Value);
            AddRecipe("QueenDrop", TheQueenTable.Value, TheQueenAmount.Value, TheQueenLevel.Value, TheQueenRequirements.Value);
        }
        else
        {
            var mRecipes = ObjectDB.instance.m_recipes;
            if (mRecipes != null)
            {
                var recipesToRemove = mRecipes
                    .Where(recipe => recipe?.m_item != null &&
                                     (recipe.m_item.name == "HardAntler" ||
                                      recipe.m_item.name == "CryptKey" ||
                                      recipe.m_item.name == "YagluthDrop" ||
                                      recipe.m_item.name == "DragonTear" ||
                                      recipe.m_item.name == "Wishbone" ||
                                      recipe.m_item.name == "QueenDrop"))
                    .ToList();

                foreach (var recipe in recipesToRemove)
                {
                    mRecipes.Remove(recipe);
                }
            }
        }
    }

    // Helper Functions
    public static void AddRecipe(string prefabName, CraftingTable table, int amount, int level, string requirements)
    {
        GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
        Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
        recipe.name = "Recipe_" + prefabName;
        recipe.m_item = prefab.GetComponent<ItemDrop>();
        recipe.m_amount = amount;
        recipe.m_craftingStation = ZNetScene.instance.GetPrefab(GetInternalName(table)).GetComponent<CraftingStation>();
        recipe.m_minStationLevel = level;
        recipe.m_resources = SerializedRequirements.ToPieceReqs(ObjectDB.instance, new SerializedRequirements(requirements ?? ""), new SerializedRequirements(""));
        recipe.m_enabled = true;
        ObjectDB.instance.m_recipes.Add(Instantiate(recipe));
    }

    public static Recipe? GetRecipe(string name)
    {
        return ObjectDB.instance.m_recipes.FirstOrDefault(VARIABLE => VARIABLE.m_item != null && VARIABLE.m_item.name == name);
    }

    public static Piece.Requirement[] GetRequirements(ConfigEntry<string> configEntry)
    {
        return SerializedRequirements.ToPieceReqs(ObjectDB.instance, new SerializedRequirements(configEntry.Value ?? ""), new SerializedRequirements(""));
    }

    #region Ripped ItemManager Config
    public class InternalName : Attribute
    {
        public readonly string internalName;
        public InternalName(string internalName) => this.internalName = internalName;
    }

    public enum CraftingTable
    {
        //Disabled,
        //Inventory,
        [InternalName("piece_workbench")] Workbench,
        //[InternalName("piece_cauldron")] Cauldron,
        [InternalName("forge")] Forge,
        [InternalName("piece_artisanstation")] ArtisanTable,
        //[InternalName("piece_stonecutter")] StoneCutter,
        //[InternalName("piece_magetable")] MageTable,
        [InternalName("blackforge")] BlackForge
        //Custom
    }
    public static string GetInternalName<T>(T value) where T : struct => ((InternalName)typeof(T).GetMember(value.ToString())[0].GetCustomAttributes(typeof(InternalName)).First()).internalName;

    public struct Requirement
    {
        public string ItemName;
        public int Amount;
        public ConfigEntry<int>? AmountConfig;
    }
    public class SerializedRequirements
    {
        public readonly List<Requirement> Requirements;

        public SerializedRequirements(List<Requirement> requirements) => Requirements = requirements;

        public SerializedRequirements(string requirements)
        {
            Requirements = requirements.Split(',').Select(r =>
            {
                string[] parts = r.Split(':');
                return new Requirement { ItemName = parts[0], Amount = parts.Length > 1 && int.TryParse(parts[1], out int amount) ? amount : 1 };
            }).ToList();
        }

        public override string ToString()
        {
            return string.Join(",", Requirements.Select(r => $"{r.ItemName}:{r.Amount}"));
        }

        public static ItemDrop? FetchByName(ObjectDB objectDB, string name)
        {
            ItemDrop? item = objectDB.GetItemPrefab(name)?.GetComponent<ItemDrop>();
            return item;
        }

        public static Piece.Requirement[] ToPieceReqs(ObjectDB objectDB, SerializedRequirements craft, SerializedRequirements upgrade)
        {
            ItemDrop? ResItem(Requirement r) => FetchByName(objectDB, r.ItemName);

            Dictionary<string, Piece.Requirement?> resources =
                craft.Requirements.Where(r => r.ItemName != "").
                    ToDictionary(r => r.ItemName, r => ResItem(r) is { } item ?
                        new Piece.Requirement { m_amount = r.AmountConfig?.Value ?? r.Amount, m_resItem = item, m_amountPerLevel = 0 } : null);

            foreach (Requirement req in upgrade.Requirements.Where(r => r.ItemName != ""))
            {
                if ((!resources.TryGetValue(req.ItemName, out Piece.Requirement? requirement) || requirement == null) && ResItem(req) is { } item)
                {
                    requirement = resources[req.ItemName] = new Piece.Requirement { m_resItem = item, m_amount = 0 };
                }

                if (requirement != null)
                {
                    requirement.m_amountPerLevel = req.AmountConfig?.Value ?? req.Amount;
                }
            }

            return resources.Values.Where(v => v != null).ToArray()!;
        }
    }
    #endregion
}

