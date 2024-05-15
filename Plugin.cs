using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HS;
using ServerSync;
using UnityEngine;

namespace HS_CraftableBossItems;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	// Change MinimumRequiredVersion to the new version when changing Config settings.
	public static ConfigSync ConfigSync = new(MyPluginInfo.PLUGIN_GUID)
	{
		DisplayName = MyPluginInfo.PLUGIN_NAME, CurrentVersion = MyPluginInfo.PLUGIN_VERSION, MinimumRequiredVersion = "0.1.1", ModRequired = true
	};

    #region Config Defines
    private static ConfigEntry<Toggle> _serverConfigLocked = null!;
    private static ConfigEntry<bool> _modEnabled = null!;

    private static ConfigEntry<bool> _overrideVersionCheck = null!;

    public static ConfigEntry<bool> EikythrDropEnabled = null!;
    public static ConfigEntry<CraftingTable> EikythrTable = null!;
	public static ConfigEntry<String> EikythrRequirements = null!;
	public static ConfigEntry<int> EikythrAmount = null!;
	public static ConfigEntry<int> EikythrLevel = null!;

    public static ConfigEntry<bool> EikythrTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> EikythrTrophyTable = null!;
    public static ConfigEntry<String> EikythrTrophyRequirements = null!;
    public static ConfigEntry<int> EikythrTrophyAmount = null!;
    public static ConfigEntry<int> EikythrTrophyLevel = null!;


    public static ConfigEntry<bool> TheElderDropEnabled = null!;
    public static ConfigEntry<CraftingTable> TheElderTable = null!;
    public static ConfigEntry<String> TheElderRequirements = null!;
    public static ConfigEntry<int> TheElderAmount = null!;
    public static ConfigEntry<int> TheElderLevel = null!;

    public static ConfigEntry<bool> TheElderTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> TheElderTrophyTable = null!;
    public static ConfigEntry<String> TheElderTrophyRequirements = null!;
    public static ConfigEntry<int> TheElderTrophyAmount = null!;
    public static ConfigEntry<int> TheElderTrophyLevel = null!;


    public static ConfigEntry<bool> YagluthDropEnabled = null!;
    public static ConfigEntry<CraftingTable> YagluthTable = null!;
    public static ConfigEntry<String> YagluthRequirements = null!;
    public static ConfigEntry<int> YagluthAmount = null!;
    public static ConfigEntry<int> YagluthLevel = null!;

    public static ConfigEntry<bool> YagluthTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> YagluthTrophyTable = null!;
    public static ConfigEntry<String> YagluthTrophyRequirements = null!;
    public static ConfigEntry<int> YagluthTrophyAmount = null!;
    public static ConfigEntry<int> YagluthTrophyLevel = null!;


    public static ConfigEntry<bool> DragonQueenDropEnabled = null!;
    public static ConfigEntry<CraftingTable> DragonQueenTable = null!;
    public static ConfigEntry<String> DragonQueenRequirements = null!;
    public static ConfigEntry<int> DragonQueenAmount = null!;
    public static ConfigEntry<int> DragonQueenLevel = null!;

    public static ConfigEntry<bool> DragonQueenTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> DragonQueenTrophyTable = null!;
    public static ConfigEntry<String> DragonQueenTrophyRequirements = null!;
    public static ConfigEntry<int> DragonQueenTrophyAmount = null!;
    public static ConfigEntry<int> DragonQueenTrophyLevel = null!;


    public static ConfigEntry<bool> BonemassDropEnabled = null!;
    public static ConfigEntry<CraftingTable> BonemassTable = null!;
    public static ConfigEntry<String> BonemassRequirements = null!;
    public static ConfigEntry<int> BonemassAmount = null!;
    public static ConfigEntry<int> BonemassLevel = null!;

    public static ConfigEntry<bool> BonemassTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> BonemassTrophyTable = null!;
    public static ConfigEntry<String> BonemassTrophyRequirements = null!;
    public static ConfigEntry<int> BonemassTrophyAmount = null!;
    public static ConfigEntry<int> BonemassTrophyLevel = null!;


    public static ConfigEntry<bool> TheQueenDropEnabled = null!;
    public static ConfigEntry<CraftingTable> TheQueenTable = null!;
    public static ConfigEntry<String> TheQueenRequirements = null!;
    public static ConfigEntry<int> TheQueenAmount = null!;
    public static ConfigEntry<int> TheQueenLevel = null!;

    public static ConfigEntry<bool> TheQueenTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> TheQueenTrophyTable = null!;
    public static ConfigEntry<String> TheQueenTrophyRequirements = null!;
    public static ConfigEntry<int> TheQueenTrophyAmount = null!;
    public static ConfigEntry<int> TheQueenTrophyLevel = null!;


    public static ConfigEntry<bool> FaderDropEnabled = null!;
    public static ConfigEntry<CraftingTable> FaderTable = null!;
    public static ConfigEntry<String> FaderRequirements = null!;
    public static ConfigEntry<int> FaderAmount = null!;
    public static ConfigEntry<int> FaderLevel = null!;

    public static ConfigEntry<bool> FaderTrophyEnabled = null!;
    public static ConfigEntry<CraftingTable> FaderTrophyTable = null!;
    public static ConfigEntry<String> FaderTrophyRequirements = null!;
    public static ConfigEntry<int> FaderTrophyAmount = null!;
    public static ConfigEntry<int> FaderTrophyLevel = null!;
    #endregion

    #region Config Boilerplate

    private ConfigEntry<T> Config<T>(string group, string name, T value, ConfigDescription description,
		bool synchronizedSetting = true)
	{
		ConfigEntry<T> configEntry = base.Config.Bind(group, name, value, description);
		SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
		syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
		return configEntry;
	}

	private ConfigEntry<T> Config<T>(string group, string name, T value, string description,
		bool synchronizedSetting = true)
	{
		return Config(group, name, value, new ConfigDescription(description), synchronizedSetting);
	}

	private enum Toggle
	{
		On = 1,
		Off = 0
	}

    #endregion

    public static readonly ManualLogSource HS_Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_NAME);

    public void Awake()
	{
        #region Config Setup

        // General Config Settings
        _serverConfigLocked = Config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
		ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
        _modEnabled = Config("1 - General", "Mod Enabled", true, "");
        _modEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();

        _overrideVersionCheck = Config("1 - General", "Override Version Check", true,
            new ConfigDescription("Set to True to override the Valheim version check and allow the mod to start even if an incorrect Valheim version is detected.",
                null, new ConfigurationManagerAttributes { Browsable = false }));

        // Eikythr Drop
        EikythrDropEnabled = Config("2 - HardAntler (Eikythr)", "Boss Drop Enabled", true, " Enable Crafting of HardAntler");
        EikythrDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        EikythrTable = Config("2 - HardAntler (Eikythr)", "Table", CraftingTable.Workbench, "Crafting station needed to construct HardAntler.");
        EikythrTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(EikythrTable.Value)).GetComponent<CraftingStation>();
        EikythrRequirements = Config("2 - HardAntler (Eikythr)", "Requirements", "TrophyDeer:20,Resin:2", "The required items to construct HardAntler.");
        EikythrRequirements.SettingChanged += (_, _) => 
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null) recipe.m_resources = GetRequirements(EikythrRequirements);
        };
        EikythrAmount = Config("2 - HardAntler (Eikythr)", "Amount", 1, "The amount of HardAntler created.");
        EikythrAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null) recipe.m_amount = EikythrAmount.Value;
        };
        EikythrLevel = Config("2 - HardAntler (Eikythr)", "Station Level", 3, "Level of crafting station required to craft HardAntler.");
        EikythrLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("HardAntler");
            if (recipe != null) recipe.m_minStationLevel = EikythrLevel.Value;
        };

        // Eikythr Trophy
        EikythrTrophyEnabled = Config("2 - TrophyEikthyr (Eikythr)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyEikthyr");
        EikythrTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        EikythrTrophyTable = Config("2 - TrophyEikthyr (Eikythr)", "Table", CraftingTable.Workbench, "Crafting station needed to construct TrophyEikthyr.");
        EikythrTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(EikythrTrophyTable.Value)).GetComponent<CraftingStation>();
        EikythrTrophyRequirements = Config("2 - TrophyEikthyr (Eikythr)", "Requirements", "TrophyDeer:20,Resin:2", "The required items to construct TrophyEikthyr.");
        EikythrTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyEikthyr");
            if (recipe != null) recipe.m_resources = GetRequirements(EikythrTrophyRequirements);
        };
        EikythrTrophyAmount = Config("2 - TrophyEikthyr (Eikythr)", "Amount", 1, "The amount of TrophyEikthyr created.");
        EikythrTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyEikthyr");
            if (recipe != null) recipe.m_amount = EikythrTrophyAmount.Value;
        };
        EikythrTrophyLevel = Config("2 - HardAntler (Eikythr)", "Station Level", 3, "Level of crafting station required to craft TrophyEikthyr.");
        EikythrTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyEikthyr");
            if (recipe != null) recipe.m_minStationLevel = EikythrTrophyLevel.Value;
        };


        // TheElder Drop
        TheElderDropEnabled = Config("3 - CryptKey (TheElder)", "Boss Drop Enabled", true, " Enable Crafting of CryptKey");
        TheElderDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        TheElderTable = Config("3 - CryptKey (TheElder)", "Table", CraftingTable.Forge, "Crafting station needed to construct CryptKey.");
        TheElderTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheElderTable.Value)).GetComponent<CraftingStation>();
        TheElderRequirements = Config("3 - CryptKey (TheElder)", "Requirements", "AncientSeed:5,Bronze:20", "The required items to construct CryptKey.");
        TheElderRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null) recipe.m_resources = GetRequirements(TheElderRequirements);
        };
        TheElderAmount = Config("3 - CryptKey (TheElder)", "Amount", 1, "The amount of CryptKey created.");
        TheElderAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null) recipe.m_amount = TheElderAmount.Value;
        };
        TheElderLevel = Config("3 - CryptKey (TheElder)", "Station Level", 3, "Level of crafting station required to craft CryptKey.");
        TheElderLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("CryptKey");
            if (recipe != null) recipe.m_minStationLevel = TheElderLevel.Value;
        };

        // TheElder Trophy
        TheElderTrophyEnabled = Config("3 - TrophyTheElder (TheElder)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyTheElder");
        TheElderTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        TheElderTrophyTable = Config("3 - TrophyTheElder (TheElder)", "Table", CraftingTable.Forge, "Crafting station needed to construct TrophyTheElder.");
        TheElderTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheElderTrophyTable.Value)).GetComponent<CraftingStation>();
        TheElderTrophyRequirements = Config("3 - TrophyTheElder (TheElder)", "Requirements", "AncientSeed:5,Bronze:20", "The required items to construct TrophyTheElder.");
        TheElderTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyTheElder");
            if (recipe != null) recipe.m_resources = GetRequirements(TheElderTrophyRequirements);
        };
        TheElderTrophyAmount = Config("3 - TrophyTheElder (TheElder)", "Amount", 1, "The amount of TrophyTheElder created.");
        TheElderTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyTheElder");
            if (recipe != null) recipe.m_amount = TheElderTrophyAmount.Value;
        };
        TheElderTrophyLevel = Config("3 - TrophyTheElder (TheElder)", "Station Level", 3, "Level of crafting station required to craft TrophyTheElder.");
        TheElderTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyTheElder");
            if (recipe != null) recipe.m_minStationLevel = TheElderTrophyLevel.Value;
        };


        // Bonemass Drop
        BonemassDropEnabled = Config("4 - Wishbone (Bonemass)", "Boss Drop Enabled", true, " Enable Crafting of Wishbone");
        BonemassDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        BonemassTable = Config("4 - Wishbone (Bonemass)", "Table", CraftingTable.Forge, "Crafting station needed to construct Wishbone.");
        BonemassTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(BonemassTable.Value)).GetComponent<CraftingStation>();
        BonemassRequirements = Config("4 - Wishbone (Bonemass)", "Requirements", "WitheredBone:15,Iron:2,TrophyDraugr:3,TrophyDraugrElite:1", "The required items to construct Wishbone.");
        BonemassRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null) recipe.m_resources = GetRequirements(BonemassRequirements);
        };
        BonemassAmount = Config("4 - Wishbone (Bonemass)", "Amount", 1, "The amount of Wishbone created.");
        BonemassAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null) recipe.m_amount = BonemassAmount.Value;
        };
        BonemassLevel = Config("4 - Wishbone (Bonemass)", "Station Level", 7, "Level of crafting station required to craft Wishbone.");
        BonemassLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("Wishbone");
            if (recipe != null) recipe.m_minStationLevel = BonemassLevel.Value;
        };

        // Bonemass Trophy
        BonemassTrophyEnabled = Config("4 - TrophyBonemass (Bonemass)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyBonemass");
        BonemassTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        BonemassTrophyTable = Config("4 - TrophyBonemass (Bonemass)", "Table", CraftingTable.Forge, "Crafting station needed to construct TrophyBonemass.");
        BonemassTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(BonemassTrophyTable.Value)).GetComponent<CraftingStation>();
        BonemassTrophyRequirements = Config("4 - TrophyBonemass (Bonemass)", "Requirements", "WitheredBone:15,Iron:2,TrophyDraugr:3,TrophyDraugrElite:1", "The required items to construct TrophyBonemass.");
        BonemassTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyBonemass");
            if (recipe != null) recipe.m_resources = GetRequirements(BonemassTrophyRequirements);
        };
        BonemassTrophyAmount = Config("4 - TrophyBonemass (Bonemass)", "Amount", 1, "The amount of TrophyBonemass created.");
        BonemassTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyBonemass");
            if (recipe != null) recipe.m_amount = BonemassTrophyAmount.Value;
        };
        BonemassTrophyLevel = Config("4 - TrophyBonemass (Bonemass)", "Station Level", 7, "Level of crafting station required to craft TrophyBonemass.");
        BonemassTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyBonemass");
            if (recipe != null) recipe.m_minStationLevel = BonemassTrophyLevel.Value;
        };


        // Dragon Queen Drop
        DragonQueenDropEnabled = Config("5 - DragonTear (Moder)", "Boss Drop Enabled", true, " Enable Crafting of DragonTear");
        DragonQueenDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        DragonQueenTable = Config("5 - DragonTear (Moder)", "Table", CraftingTable.Workbench, "Crafting station needed to construct DragonTear.");
        DragonQueenTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(DragonQueenTable.Value)).GetComponent<CraftingStation>();
        DragonQueenRequirements = Config("5 - DragonTear (Moder)", "Requirements", "DragonEgg:3,Crystal:10", "The required items to construct DragonTear.");
        DragonQueenRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null) recipe.m_resources = GetRequirements(DragonQueenRequirements);
        };
        DragonQueenAmount = Config("5 - DragonTear (Moder)", "Amount", 5, "The amount of DragonTear created.");
        DragonQueenAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null) recipe.m_amount = DragonQueenAmount.Value;
        };
        DragonQueenLevel = Config("5 - DragonTear (Moder)", "Station Level", 5, "Level of crafting station required to craft DragonTear.");
        DragonQueenLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("DragonTear");
            if (recipe != null) recipe.m_minStationLevel = DragonQueenLevel.Value;
        };

        // Dragon Queen Trophy
        DragonQueenTrophyEnabled = Config("5 - TrophyDragonQueen (Moder)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyDragonQueen");
        DragonQueenTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        DragonQueenTrophyTable = Config("5 - TrophyDragonQueen (Moder)", "Table", CraftingTable.Workbench, "Crafting station needed to construct TrophyDragonQueen.");
        DragonQueenTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(DragonQueenTrophyTable.Value)).GetComponent<CraftingStation>();
        DragonQueenTrophyRequirements = Config("5 - TrophyDragonQueen (Moder)", "Requirements", "DragonEgg:3,Crystal:10", "The required items to construct TrophyDragonQueen.");
        DragonQueenTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyDragonQueen");
            if (recipe != null) recipe.m_resources = GetRequirements(DragonQueenTrophyRequirements);
        };
        DragonQueenTrophyAmount = Config("5 - TrophyDragonQueen (Moder)", "Amount", 1, "The amount of TrophyDragonQueen created.");
        DragonQueenTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyDragonQueen");
            if (recipe != null) recipe.m_amount = DragonQueenTrophyAmount.Value;
        };
        DragonQueenTrophyLevel = Config("5 - TrophyDragonQueen (Moder)", "Station Level", 5, "Level of crafting station required to craft TrophyDragonQueen.");
        DragonQueenTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyDragonQueen");
            if (recipe != null) recipe.m_minStationLevel = DragonQueenTrophyLevel.Value;
        };


        // Yagluth Drop
        YagluthDropEnabled = Config("6 - Torn spirit (Yagluth)", "Boss Drop Enabled", true, " Enable Crafting of Torn spirit");
        YagluthDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        YagluthTable = Config("6 - Torn spirit (Yagluth)", "Table", CraftingTable.ArtisanTable, "Crafting station needed to construct Torn spirit.");
        YagluthTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(YagluthTable.Value)).GetComponent<CraftingStation>();
        YagluthRequirements = Config("6 - Torn spirit (Yagluth)", "Requirements", "GoblinTotem:10,TrophyGoblin:3,TrophyGoblinShaman:1,TrophyGoblinBrute:1", "The required items to construct Torn spirit.");
        YagluthRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null) recipe.m_resources = GetRequirements(YagluthRequirements);
        };
        YagluthAmount = Config("6 - Torn spirit (Yagluth)", "Amount", 3, "The amount of Torn spirit created.");
        YagluthAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null) recipe.m_amount = YagluthAmount.Value;
        };
        YagluthLevel = Config("6 - Torn spirit (Yagluth)", "Station Level", 1, "Level of crafting station required to craft Torn spirit.");
        YagluthLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("YagluthDrop");
            if (recipe != null) recipe.m_minStationLevel = YagluthLevel.Value;
        };

        // Yagluth Trophy
        YagluthTrophyEnabled = Config("6 - TrophyGoblinKing (Yagluth)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyGoblinKing");
        YagluthTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        YagluthTrophyTable = Config("6 - TrophyGoblinKing (Yagluth)", "Table", CraftingTable.ArtisanTable, "Crafting station needed to construct TrophyGoblinKing.");
        YagluthTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(YagluthTrophyTable.Value)).GetComponent<CraftingStation>();
        YagluthTrophyRequirements = Config("6 - TrophyGoblinKing (Yagluth)", "Requirements", "GoblinTotem:10,TrophyGoblin:3,TrophyGoblinShaman:1,TrophyGoblinBrute:1", "The required items to construct TrophyGoblinKing.");
        YagluthTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyGoblinKing");
            if (recipe != null) recipe.m_resources = GetRequirements(YagluthTrophyRequirements);
        };
        YagluthTrophyAmount = Config("6 - TrophyGoblinKing (Yagluth)", "Amount", 1, "The amount of TrophyGoblinKing created.");
        YagluthTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyGoblinKing");
            if (recipe != null) recipe.m_amount = YagluthTrophyAmount.Value;
        };
        YagluthTrophyLevel = Config("6 - TrophyGoblinKing (Yagluth)", "Station Level", 1, "Level of crafting station required to craft TrophyGoblinKing.");
        YagluthTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyGoblinKing");
            if (recipe != null) recipe.m_minStationLevel = YagluthTrophyLevel.Value;
        };


        // The Queen Drop
        TheQueenDropEnabled = Config("7 - QueenDrop (TheQueen)", "Boss Drop Enabled", true, " Enable Crafting of QueenDrop");
        TheQueenDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        TheQueenTable = Config("7 - QueenDrop (TheQueen)", "Table", CraftingTable.BlackForge, "Crafting station needed to construct QueenDrop.");
        TheQueenTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheQueenTable.Value)).GetComponent<CraftingStation>();
        TheQueenRequirements = Config("7 - QueenDrop (TheQueen)", "Requirements", "DvergrKeyFragment:9,Mandible:5,TrophySeeker:3,TrophySeekerBrute:1", "The required items to construct QueenDrop.");
        TheQueenRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null) recipe.m_resources = GetRequirements(TheQueenRequirements);
        };
        TheQueenAmount = Config("7 - QueenDrop (TheQueen)", "Amount", 3, "The amount of QueenDrop created.");
        TheQueenAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null) recipe.m_amount = TheQueenAmount.Value;
        };
        TheQueenLevel = Config("7 - QueenDrop (TheQueen)", "Station Level", 1, "Level of crafting station required to craft QueenDrop.");
        TheQueenLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("QueenDrop");
            if (recipe != null) recipe.m_minStationLevel = TheQueenLevel.Value;
        };

        // The Queen Trophy
        TheQueenTrophyEnabled = Config("7 - TrophySeekerQueen (TheQueen)", "Boss Trophy Enabled", true, " Enable Crafting of TrophySeekerQueen");
        TheQueenTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        TheQueenTrophyTable = Config("7 - TrophySeekerQueen (TheQueen)", "Table", CraftingTable.BlackForge, "Crafting station needed to construct TrophySeekerQueen.");
        TheQueenTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(TheQueenTrophyTable.Value)).GetComponent<CraftingStation>();
        TheQueenTrophyRequirements = Config("7 - TrophySeekerQueen (TheQueen)", "Requirements", "DvergrKeyFragment:9,Mandible:5,TrophySeeker:3,TrophySeekerBrute:1", "The required items to construct TrophySeekerQueen.");
        TheQueenTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophySeekerQueen");
            if (recipe != null) recipe.m_resources = GetRequirements(TheQueenTrophyRequirements);
        };
        TheQueenTrophyAmount = Config("7 - TrophySeekerQueen (TheQueen)", "Amount", 1, "The amount of TrophySeekerQueen created.");
        TheQueenTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophySeekerQueen");
            if (recipe != null) recipe.m_amount = TheQueenTrophyAmount.Value;
        };
        TheQueenTrophyLevel = Config("7 - TrophySeekerQueen (TheQueen)", "Station Level", 1, "Level of crafting station required to craft TrophySeekerQueen.");
        TheQueenTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophySeekerQueen");
            if (recipe != null) recipe.m_minStationLevel = TheQueenTrophyLevel.Value;
        };


        // Fader Drop
        FaderDropEnabled = Config("8 - FaderDrop (Fader)", "Boss Drop Enabled", true, " Enable Crafting of FaderDrop");
        FaderDropEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        FaderTable = Config("8 - FaderDrop (Fader)", "Table", CraftingTable.BlackForge, "Crafting station needed to construct FaderDrop.");
        FaderTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(FaderTable.Value)).GetComponent<CraftingStation>();
        FaderRequirements = Config("8 - FaderDrop (Fader)", "Requirements", "BellFragment:3,CelestialFeather:5,TrophyAsksvin:3,TrophyMorgen:1", "The required items to construct FaderDrop.");
        FaderRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("FaderDrop");
            if (recipe != null) recipe.m_resources = GetRequirements(FaderRequirements);
        };
        FaderAmount = Config("8 - FaderDrop (Fader)", "Amount", 5, "The amount of FaderDrop created.");
        FaderAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("FaderDrop");
            if (recipe != null) recipe.m_amount = FaderAmount.Value;
        };
        FaderLevel = Config("8 - FaderDrop (Fader)", "Station Level", 2, "Level of crafting station required to craft FaderDrop.");
        FaderLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("FaderDrop");
            if (recipe != null) recipe.m_minStationLevel = FaderLevel.Value;
        };

        // Fader Trophy
        FaderTrophyEnabled = Config("8 - TrophyFader (Fader)", "Boss Trophy Enabled", true, " Enable Crafting of TrophyFader");
        FaderTrophyEnabled.SettingChanged += (_, _) => InvokeOnVanillaObjectsAvailable();
        FaderTrophyTable = Config("8 - TrophyFader (Fader)", "Table", CraftingTable.BlackForge, "Crafting station needed to construct TrophyFader.");
        FaderTrophyTable.SettingChanged += (_, _) => ZNetScene.instance.GetPrefab(GetInternalName(FaderTrophyTable.Value)).GetComponent<CraftingStation>();
        FaderTrophyRequirements = Config("8 - TrophyFader (Fader)", "Requirements", "BellFragment:3,CelestialFeather:5,TrophyAsksvin:3,TrophyMorgen:1", "The required items to construct TrophyFader.");
        FaderTrophyRequirements.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyFader");
            if (recipe != null) recipe.m_resources = GetRequirements(FaderTrophyRequirements);
        };
        FaderTrophyAmount = Config("8 - TrophyFader (Fader)", "Amount", 1, "The amount of TrophyFader created.");
        FaderTrophyAmount.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyFader");
            if (recipe != null) recipe.m_amount = FaderTrophyAmount.Value;
        };
        FaderTrophyLevel = Config("8 - TrophyFader (Fader)", "Station Level", 2, "Level of crafting station required to craft TrophyFader.");
        FaderTrophyLevel.SettingChanged += (_, _) =>
        {
            var recipe = GetRecipe("TrophyFader");
            if (recipe != null) recipe.m_minStationLevel = FaderTrophyLevel.Value;
        };

        #endregion

        // Check if Plugin was Built for Current Version of Valheim
        if (!VersionChecker.Check(HS_Logger, Info, _overrideVersionCheck.Value, base.Config)) return;

        // Harmony Setup
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.Patch(AccessTools.Method(typeof(ZNet), nameof(ZNet.Start)),
            prefix: new HarmonyMethod(typeof(Plugin), nameof(Plugin.InvokeOnVanillaObjectsAvailable)) 
        );
    }

    // Harmony Patch
    public static void InvokeOnVanillaObjectsAvailable()
    {
#if DEBUG
        ObjectDB.instance.m_items
            .Select(gameObject => gameObject.GetComponent<ItemDrop>())
            .ToList()
            .ForEach(component => HS_Logger.LogWarning(component.gameObject.name));
#endif

        if (_modEnabled.Value)
        {
            // Add Recipes to List if Mod Enabled and Recipe is Enabled
            if (EikythrDropEnabled.Value) AddRecipe("HardAntler", EikythrTable.Value, EikythrAmount.Value, EikythrLevel.Value, EikythrRequirements.Value);
            else RemoveRecipe("HardAntler");
            if (TheElderDropEnabled.Value) AddRecipe("CryptKey", TheElderTable.Value, TheElderAmount.Value, TheElderLevel.Value, TheElderRequirements.Value);
            else RemoveRecipe("CryptKey");
            if (YagluthDropEnabled.Value) AddRecipe("YagluthDrop", YagluthTable.Value, YagluthAmount.Value, YagluthLevel.Value, YagluthRequirements.Value);
            else RemoveRecipe("YagluthDrop");
            if (DragonQueenDropEnabled.Value) AddRecipe("DragonTear", DragonQueenTable.Value, DragonQueenAmount.Value, DragonQueenLevel.Value, DragonQueenRequirements.Value);
            else RemoveRecipe("DragonTear");
            if (BonemassDropEnabled.Value) AddRecipe("Wishbone", BonemassTable.Value, BonemassAmount.Value, BonemassLevel.Value, BonemassRequirements.Value);
            else RemoveRecipe("Wishbone");
            if (TheQueenDropEnabled.Value) AddRecipe("QueenDrop", TheQueenTable.Value, TheQueenAmount.Value, TheQueenLevel.Value, TheQueenRequirements.Value);
            else RemoveRecipe("QueenDrop");
            if (FaderDropEnabled.Value) AddRecipe("FaderDrop", FaderTable.Value, FaderAmount.Value, FaderLevel.Value, FaderRequirements.Value);
            else RemoveRecipe("FaderDrop");

            if (FaderTrophyEnabled.Value) AddRecipe("TrophyFader", FaderTrophyTable.Value, FaderTrophyAmount.Value, FaderTrophyLevel.Value, FaderTrophyRequirements.Value);
            else RemoveRecipe("TrophyFader");
            if (TheQueenTrophyEnabled.Value) AddRecipe("TrophySeekerQueen", TheQueenTrophyTable.Value, TheQueenTrophyAmount.Value, TheQueenTrophyLevel.Value, TheQueenTrophyRequirements.Value);
            else RemoveRecipe("TrophySeekerQueen");
            if (YagluthTrophyEnabled.Value) AddRecipe("TrophyGoblinKing", YagluthTrophyTable.Value, YagluthTrophyAmount.Value, YagluthTrophyLevel.Value, YagluthTrophyRequirements.Value);
            else RemoveRecipe("TrophyGoblinKing");
            if (DragonQueenTrophyEnabled.Value) AddRecipe("TrophyDragonQueen", DragonQueenTrophyTable.Value, DragonQueenTrophyAmount.Value, DragonQueenTrophyLevel.Value, DragonQueenTrophyRequirements.Value);
            else RemoveRecipe("TrophyDragonQueen");
            if (BonemassTrophyEnabled.Value) AddRecipe("TrophyBonemass", BonemassTrophyTable.Value, BonemassTrophyAmount.Value, BonemassTrophyLevel.Value, BonemassTrophyRequirements.Value);
            else RemoveRecipe("TrophyBonemass");
            if (TheElderTrophyEnabled.Value) AddRecipe("TrophyTheElder", TheElderTrophyTable.Value, TheElderTrophyAmount.Value, TheElderTrophyLevel.Value, TheElderTrophyRequirements.Value);
            else RemoveRecipe("TrophyTheElder");
            if (EikythrTrophyEnabled.Value) AddRecipe("TrophyEikthyr", EikythrTrophyTable.Value, EikythrTrophyAmount.Value, EikythrTrophyLevel.Value, EikythrTrophyRequirements.Value);
            else RemoveRecipe("TrophyEikthyr");
        }
        else
        {
            // Remove all Recipes when Mod is Disabled
            RemoveRecipe("HardAntler");
            RemoveRecipe("CryptKey");
            RemoveRecipe("YagluthDrop");
            RemoveRecipe("DragonTear");
            RemoveRecipe("Wishbone");
            RemoveRecipe("QueenDrop");
            RemoveRecipe("FaderDrop");
            RemoveRecipe("TrophyFader");
            RemoveRecipe("TrophySeekerQueen");
            RemoveRecipe("TrophyGoblinKing");
            RemoveRecipe("TrophyDragonQueen");
            RemoveRecipe("TrophyBonemass");
            RemoveRecipe("TrophyTheElder");
            RemoveRecipe("TrophyEikthyr");
        }
    }

    // Helper Functions
    public static void AddRecipe(string prefabName, CraftingTable table, int amount, int level, string requirements)
    {
        var prefab = ZNetScene.instance.GetPrefab(prefabName);
        var recipe = ScriptableObject.CreateInstance<Recipe>();
        recipe.name = "Recipe_" + prefabName;
        recipe.m_item = prefab.GetComponent<ItemDrop>();
        recipe.m_amount = amount;
        recipe.m_craftingStation = ZNetScene.instance.GetPrefab(GetInternalName(table)).GetComponent<CraftingStation>();
        recipe.m_minStationLevel = level;
        recipe.m_resources = SerializedRequirements.ToPieceReqs(ObjectDB.instance, new SerializedRequirements(requirements ?? ""), new SerializedRequirements(""));
        recipe.m_enabled = true;
        ObjectDB.instance.m_recipes.Add(Instantiate(recipe));
    }

    public static void RemoveRecipe(string name) => ObjectDB.instance.m_recipes?.RemoveAll(recipe => recipe?.m_item != null && recipe.m_item.name == name);

    public static Recipe? GetRecipe(string name) => ObjectDB.instance.m_recipes.FirstOrDefault(recipe => recipe.m_item != null && recipe.m_item.name == name);

    public static Piece.Requirement[] GetRequirements(ConfigEntry<string> configEntry) =>
        SerializedRequirements.ToPieceReqs(ObjectDB.instance, new SerializedRequirements(configEntry.Value ?? ""), new SerializedRequirements(""));

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
        [InternalName("piece_cauldron")] Cauldron,
        [InternalName("forge")] Forge,
        [InternalName("piece_artisanstation")] ArtisanTable,
        [InternalName("piece_stonecutter")] StoneCutter,
        [InternalName("piece_magetable")] MageTable,
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
            var resources =
                craft.Requirements.Where(r => r.ItemName != "").
                    ToDictionary(r => r.ItemName, r => ResItem(r) is { } item ?
                        new Piece.Requirement { m_amount = r.AmountConfig?.Value ?? r.Amount, m_resItem = item, m_amountPerLevel = 0 } : null);

            foreach (var req in upgrade.Requirements.Where(r => r.ItemName != ""))
            {
                if ((!resources.TryGetValue(req.ItemName, out var requirement) || requirement == null) && ResItem(req) is { } item)
                {
                    requirement = resources[req.ItemName] = new Piece.Requirement { m_resItem = item, m_amount = 0 };
                }

                if (requirement != null)
                {
                    requirement.m_amountPerLevel = req.AmountConfig?.Value ?? req.Amount;
                }
            }

            return resources.Values.Where(v => v != null).ToArray()!;

            ItemDrop? ResItem(Requirement r) => FetchByName(objectDB, r.ItemName);
        }
    }
    #endregion
}

