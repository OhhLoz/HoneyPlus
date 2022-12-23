using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.IO;
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
    public const string PluginVersion = "3.2.0";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";
    private const string esTranslationFileName = "translation_ES.json";
    private const string deTranslationFileName = "translation_DE.json";

    private CustomLocalization Localization;
    private Assembly ModAssembly;
    private AssetBundle HoneyPlusAssetBundle;

    private void Awake()
    {
      ModAssembly = typeof(HoneyPlus).Assembly;
      HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      Jotunn.Logger.LogInfo($"Loaded asset bundle: {HoneyPlusAssetBundle}");
      ItemManager.OnItemsRegistered += OnItemsRegistered;
      //AddCustomPieces();
      AddCustomItems();
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
                // if legacy config then recipeConfig.craftingstation = "piece_cauldron"
                CustomItem customItem = new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item), true);
                ItemManager.Instance.AddItem(customItem);
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeConfig));
                Jotunn.Logger.LogInfo("Loaded Item: " + recipeConfig.Item);
            }
        }
    }

    //private void AddCustomPieces()
    //{
    //        //PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary"), "Hammer", fixReference: true));
    //        //PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle.LoadAsset<GameObject>("apiary_ext1"), "Hammer", fixReference: true));
    //        PieceConfig apiaryConfig = new PieceConfig();
    //        apiaryConfig.Name = "$custom_piece_apiary";
    //        apiaryConfig.Description = "$custom_piece_apiary_description";
    //        apiaryConfig.CraftingStation = "piece_workbench";
    //        apiaryConfig.Category = "Crafting";
    //        apiaryConfig.PieceTable = "Hammer";
    //        apiaryConfig.Icon = HoneyPlusAssetBundle.LoadAsset<Sprite>("apiary");
    //        apiaryConfig.AddRequirement(new RequirementConfig("QueenBee", 1, 0, true));
    //        apiaryConfig.AddRequirement(new RequirementConfig("Wood", 15, 0, true));
    //        apiaryConfig.AddRequirement(new RequirementConfig("Honey", 6, 0, true));

    //        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle, "apiary", fixReference: true, apiaryConfig));

    //        PieceConfig apiaryExt1Config = new PieceConfig();
    //        apiaryExt1Config.Name = "$custom_piece_apiary_ext1";
    //        apiaryExt1Config.Description = "$custom_piece_apiary $item_upgrade";
    //        apiaryExt1Config.CraftingStation = "piece_workbench";
    //        apiaryExt1Config.Category = "Crafting";
    //        apiaryExt1Config.PieceTable = "Hammer";
    //        apiaryExt1Config.Icon = HoneyPlusAssetBundle.LoadAsset<Sprite>("apiary_ext1");
    //        apiaryExt1Config.ExtendStation = "$custom_piece_apiary";
    //        apiaryExt1Config.AddRequirement(new RequirementConfig("Copper", 5, 0, true));
    //        apiaryExt1Config.AddRequirement(new RequirementConfig("Coal", 5, 0, true));
    //        apiaryExt1Config.AddRequirement(new RequirementConfig("LeatherScraps", 5, 0, true));

    //        PieceManager.Instance.AddPiece(new CustomPiece(HoneyPlusAssetBundle, "apiary_ext1", fixReference: true, apiaryExt1Config));
    //}
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
  }
}