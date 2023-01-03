using BepInEx;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace HoneyPlus
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
  internal class HoneyPlus : BaseUnityPlugin
  {
    public const string PluginGUID = "OhhLoz-HoneyPlus";
    public const string PluginName = "HoneyPlus";
    public const string PluginVersion = "4.0.0";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string ConversionFileName = "conversions.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";
    private const string esTranslationFileName = "translation_ES.json";
    private const string deTranslationFileName = "translation_DE.json";

    private CustomLocalization Localization;
    private Assembly ModAssembly;
    private AssetBundle HoneyPlusAssetBundle;

    private ConfigEntry<bool> useOldRecipes;

    private void Awake()
    {
      ModAssembly = typeof(HoneyPlus).Assembly;
      HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      Jotunn.Logger.LogInfo($"Loaded asset bundle: {HoneyPlusAssetBundle}");

      ItemManager.OnItemsRegistered += OnItemsRegistered;
      CreateConfigValues();
      if(!useOldRecipes.Value)
         AddCustomPieces();
      AddCustomItems();
      AddItemConversions();
      AddLocalizations();

      HoneyPlusAssetBundle.Unload(false);
    }

    private void AddCustomItems()
    {
        List<RecipeConfig> recipeConfigs = RecipeConfig.ListFromJson(AssetUtils.LoadTextFromResources(RecipeFileName, ModAssembly));
        Jotunn.Logger.LogInfo("Loaded recipes list");

        foreach (RecipeConfig recipeConfig in recipeConfigs)
        {
            if (HoneyPlusAssetBundle.Contains(recipeConfig.Item))
            {
                CustomItem customItem = new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item), true);
                if (useOldRecipes.Value && recipeConfig.CraftingStation == "piece_apiary")
                    recipeConfig.CraftingStation = "piece_cauldron";
                ItemManager.Instance.AddItem(customItem);
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeConfig));
                Jotunn.Logger.LogInfo("Loaded Item: " + recipeConfig.Item);
            }
        }
    }

    private void AddCustomPieces()
    {
        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("piece_apiary"), "Hammer", fixReference: true));
        Jotunn.Logger.LogInfo("Loaded Piece: Apiary");
        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary_ext1"), "Hammer", fixReference: true));
        Jotunn.Logger.LogInfo("Loaded Piece: Bee Smoker");
        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary_ext2"), "Hammer", fixReference: true));
        Jotunn.Logger.LogInfo("Loaded Piece: Beekeepers Toolbox");
        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary_ext3"), "Hammer", fixReference: true));
        Jotunn.Logger.LogInfo("Loaded Piece: Bottling Table");
        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary_ext4"), "Hammer", fixReference: true));
        Jotunn.Logger.LogInfo("Loaded Piece: Galdr's Blessing");
    }

    private void OnItemsRegistered()
    {
        try
        {
            ChangeRecipes();
        }
        catch (Exception e)
        {
            Jotunn.Logger.LogInfo($"Error OnItemsRegistered : {e.Message}");
        }
        finally
        {
            PrefabManager.OnPrefabsRegistered -= OnItemsRegistered;
        }
    }

    private void ChangeRecipes()
    {
        foreach (Recipe fetchedRecipe in ObjectDB.instance.m_recipes)
        {
            if (fetchedRecipe.name == "Recipe_BoarJerky")
            {
                fetchedRecipe.m_craftingStation = null;
                Jotunn.Logger.LogInfo("Changed Recipe: BoarJerky");
            }
            else if (fetchedRecipe.name == "Recipe_WolfJerky")
            {
                fetchedRecipe.m_craftingStation = null;
                Jotunn.Logger.LogInfo("Changed Recipe: WolfJerky");
            }
        }
    }
    
    private void AddLocalizations()
    {
        Localization = new CustomLocalization();
        LocalizationManager.Instance.AddLocalization(Localization);

        string enTranslation = AssetUtils.LoadTextFromResources(enTranslationFileName, ModAssembly);
        Localization.AddJsonFile("English", enTranslation);

        string cnTranslation = AssetUtils.LoadTextFromResources(cnTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Chinese", cnTranslation);        
            
        string deTranslation = AssetUtils.LoadTextFromResources(deTranslationFileName, ModAssembly);
        Localization.AddJsonFile("German", deTranslation);

        string esTranslation = AssetUtils.LoadTextFromResources(esTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Spanish", esTranslation);
    }

    private void AddItemConversions()
    {
        List<CookingConversionConfig> conversionConfigs = CookingConversionConfig.ListFromJson(AssetUtils.LoadTextFromResources(ConversionFileName, ModAssembly));
        ItemManager.Instance.AddItem(new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>("HoneyDessertPie"), true));
        ItemManager.Instance.AddItem(new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>("HoneyHarePie"), true));

        foreach (CookingConversionConfig config in conversionConfigs)
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(config));
    }

    private void CreateConfigValues()
    {
        Config.SaveOnConfigSet = true;

        useOldRecipes = Config.Bind("Client config", "Use legacy recipes", false,
            new ConfigDescription("Set to true to add all recipes to the cauldron instead of the custom crafting station", 
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
    }
  }
}