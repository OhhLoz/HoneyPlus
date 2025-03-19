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
    public const string PluginVersion = "6.1.0";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string PieceFileName = "pieces.json";
    private const string cookingConversionFileName = "conversions_cooking.json";
    private const string fermenterConversionFileName = "conversions_fermenter.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";
    private const string esTranslationFileName = "translation_ES.json";
    private const string deTranslationFileName = "translation_DE.json";
    private const string ptbrTranslationFileName = "translation_PT-BR.json";
    private const string koTranslationFileName = "translation_KO.json";
    private const string uaTranslationFileName = "translation_UA.json";
    private const string ruTranslationFileName = "translation_RU.json";

    private CustomLocalization Localization;
    private Assembly ModAssembly;
    private AssetBundle HoneyPlusAssetBundle;

    private ConfigEntry<bool> useOldRecipes;
    private ConfigEntry<bool> useMeadRecipes;
    private ConfigEntry<bool> useVanillaRecipeChanges;
    private ConfigEntry<bool> useVanillaRecipeAdditions;
    private ConfigEntry<bool> addJerkysCauldron;

    private GameObject HoneyAttach;

    private void Awake()
    {
      ModAssembly = typeof(HoneyPlus).Assembly;
      HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      Jotunn.Logger.LogInfo($"Loaded asset bundle: {HoneyPlusAssetBundle}");

      CreateConfigValues();

      ItemManager.OnItemsRegistered += OnItemsRegistered;

      if(!useOldRecipes.Value)
         AddCustomPieces();

      HoneyAttach = HoneyPlusAssetBundle.LoadAsset<GameObject>("HoneyAttach");

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
            // If user opts to disable vanilla recipe additions
            if (!useVanillaRecipeAdditions.Value && ((recipeConfig.Item == "Tar") || (recipeConfig.Item == "RoyalJelly")))
                continue;

            // If user opts to disable mead recipes
            if (!useMeadRecipes.Value && recipeConfig.Item.Contains("Mead"))
                continue;

            if (recipeConfig.Name != null)
            {
                if ((useOldRecipes.Value && recipeConfig.CraftingStation == "piece_apiary") || (addJerkysCauldron.Value && recipeConfig.Item.Contains("Jerky")))
                    recipeConfig.CraftingStation = "piece_cauldron";
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeConfig));
            }

            // Stops Tar/Royal Jelly being added as an item, only as a recipe above
            if ((recipeConfig.Item == "Tar") || (recipeConfig.Item == "RoyalJelly"))
                continue;
            CustomItem customItem = new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item), fixReference: true);
            ItemManager.Instance.AddItem(customItem);
            Jotunn.Logger.LogInfo("Loaded Item: " + recipeConfig.Item);
        }
    }

    private void AddCustomPieces()
    {
        List<PieceConfig> pieceConfigs = PieceConfig.ListFromJson(AssetUtils.LoadTextFromResources(PieceFileName, ModAssembly));

        foreach (PieceConfig config in pieceConfigs)
        {
            if (HoneyPlusAssetBundle.Contains(config.Name))
            {
                PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>(config.Name), "Hammer", fixReference: true));
                Jotunn.Logger.LogInfo("Loaded Piece: " + config.Name);
            }
        }
    }

    private void OnItemsRegistered()
    {
        try
        {
            if (useVanillaRecipeChanges.Value)
                ChangeRecipes();
            ChangeItems();
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

    private void ChangeItems()
    {
        GameObject honeyObj = ObjectDB.instance.m_items.Find(x => x.name == "Honey");
        HoneyAttach.name = "attach";
        HoneyAttach.transform.SetParent(honeyObj.transform);
        Jotunn.Logger.LogInfo("Changed Item: Honey");
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

        string ptbrTranslation = AssetUtils.LoadTextFromResources(ptbrTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Portuguese_Brazilian", ptbrTranslation);        
            
        string koTranslation = AssetUtils.LoadTextFromResources(koTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Korean", koTranslation);

        string uaTranslation = AssetUtils.LoadTextFromResources(uaTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Ukrainian", uaTranslation);

        string ruTranslation = AssetUtils.LoadTextFromResources(ruTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Russian", ruTranslation);
        }

    private void AddItemConversions()
    {
        // Carrot Cake, Hare Pie & Cooked Honeycomb
        List<CookingConversionConfig> cookingConfigs = CookingConversionConfig.ListFromJson(AssetUtils.LoadTextFromResources(cookingConversionFileName, ModAssembly));

        foreach (CookingConversionConfig config in cookingConfigs)
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(config));

        if(useMeadRecipes.Value)
        {
            // Meads
            List<FermenterConversionConfig> fermenterConfigs = FermenterConversionConfig.ListFromJson(AssetUtils.LoadTextFromResources(fermenterConversionFileName, ModAssembly));

            foreach (FermenterConversionConfig config in fermenterConfigs)
                ItemManager.Instance.AddItemConversion(new CustomItemConversion(config));
        }
    }

    private void CreateConfigValues()
    {
        Config.SaveOnConfigSet = true;

        useOldRecipes = Config.Bind("Main", "Use legacy recipes", false,
            new ConfigDescription("Set to true to add all recipes to the cauldron instead of the custom crafting station (apiary)", 
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        useVanillaRecipeAdditions = Config.Bind("Tweaks", "Add vanilla recipes", true,
            new ConfigDescription("Set to false to disable addition of vanilla recipes (Tar & Royal Jelly) to apiary/cauldron",
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        useVanillaRecipeChanges = Config.Bind("Tweaks", "Change vanilla recipes", true,
            new ConfigDescription("Set to false to disable changing of vanilla recipes (Wolf & Boar Jerky) to hand crafting",
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        addJerkysCauldron = Config.Bind("Tweaks", "Add jerkys to cauldron", false,
            new ConfigDescription("Set to true to add the new Jerkys (Neck, Deer & Lox) to the cauldron instead of the hand crafting",
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        useMeadRecipes = Config.Bind("Tweaks", "Use mead recipes", true,
            new ConfigDescription("Set to false to disable addition of new meads (Speed & Damage) to the apiary/cauldron",
            new AcceptableValueRange<bool>(false, true),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));
    }
  }
}