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
    public const string PluginVersion = "3.0.6";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";

    private static readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, PluginGUID);

    private CustomLocalization Localization;
    private Assembly ModAssembly;

    private void Awake()
    {
      ModAssembly = typeof(HoneyPlus).Assembly;
      AddCustomItems();
      AddLocalizations();
    }

    private void AddCustomItems()
    {
        AssetBundle HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
        Jotunn.Logger.LogInfo($"Loaded asset bundle: {HoneyPlusAssetBundle}");

        List<RecipeConfig> recipeConfigs = RecipeConfig.ListFromJson(AssetUtils.LoadTextFromResources(RecipeFileName, ModAssembly));
        Jotunn.Logger.LogInfo("Loaded recipes list");

        foreach (RecipeConfig recipeConfig in recipeConfigs)
        {
            if (HoneyPlusAssetBundle.Contains(recipeConfig.Item))
            {
                CustomItem customItem = new CustomItem(HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item), false);
                ItemManager.Instance.AddItem(customItem);
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeConfig));
                Jotunn.Logger.LogInfo("Loaded Item: " + recipeConfig.Item);
            }
        }

        HoneyPlusAssetBundle.Unload(false);
    }

    private void AddLocalizations()
    {
        Localization = new CustomLocalization();
        LocalizationManager.Instance.AddLocalization(Localization);

        string enTranslation = AssetUtils.LoadTextFromResources(enTranslationFileName, ModAssembly);
        Localization.AddJsonFile("English", enTranslation);

        string cnTranslation = AssetUtils.LoadTextFromResources(cnTranslationFileName, ModAssembly);
        Localization.AddJsonFile("Chinese", cnTranslation);
    }
  }
}