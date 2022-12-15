using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoneyPlus
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
  internal class HoneyPlus : BaseUnityPlugin
  {
    public const string PluginGUID = "OhhLoz-HoneyPlus";
    public const string PluginName = "HoneyPlus";
    public const string PluginVersion = "3.0.1";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";

    private static readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, PluginGUID);

    private CustomLocalization Localization;

    private void Awake()
    {
      AddCustomItems();
      AddRecipes();
      AddLocalizations();
    }

    private void AddCustomItems()
    {
      Assembly ModAssembly = typeof(HoneyPlus).Assembly;
      AssetBundle HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      Jotunn.Logger.LogInfo($"Loaded asset bundle: {HoneyPlusAssetBundle}");

      string RecipePath = Path.Combine(ModPath, RecipeFileName);
      List<RecipeConfig> recipeConfigs = RecipeConfig.ListFromJson(AssetUtils.LoadText(RecipePath));
      Jotunn.Logger.LogInfo("Loaded recipes list");

        foreach (RecipeConfig recipeConfig in recipeConfigs)
        {
            if (HoneyPlusAssetBundle.Contains(recipeConfig.Item))
            {
                CustomItem customItem = new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item), false);
                ItemManager.Instance.AddItem(customItem);
                Jotunn.Logger.LogInfo("Loaded Item: " + recipeConfig.Item);
            }
        }

        HoneyPlusAssetBundle.Unload(false);
    }

    private void AddRecipes()
    {
        string RecipePath = Path.Combine(ModPath, RecipeFileName);
        ItemManager.Instance.AddRecipesFromJson(RecipePath);
        Jotunn.Logger.LogInfo("Loaded all recipes");
    }
    private void AddLocalizations()
    {
        Localization = new CustomLocalization();
        LocalizationManager.Instance.AddLocalization(Localization);

        string enTranslationsPath = Path.Combine(ModPath, enTranslationFileName);
        string enTranslation = AssetUtils.LoadText(enTranslationsPath);
        Localization.AddJsonFile("English", enTranslation);

        string cnTranslationsPath = Path.Combine(ModPath, cnTranslationFileName);
        string cnTranslation = AssetUtils.LoadText(cnTranslationsPath);
        Localization.AddJsonFile("Chinese", cnTranslation);
    }
  }
}