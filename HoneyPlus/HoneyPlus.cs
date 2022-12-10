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
    public const string PluginVersion = "2.0.9";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string enTranslationFileName = "translation_EN.json";
    private const string cnTranslationFileName = "translation_CN.json";

    private static readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, PluginGUID);

    private CustomLocalization Localization;

    private void Awake()
    {
      AddCustomItems();
      AddTranslations();
    }

    private void AddCustomItems()
    {
      string RecipePath = Path.Combine(ModPath, RecipeFileName);
      Assembly ModAssembly = typeof(HoneyPlus).Assembly;
      AssetBundle HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      List<ItemConfig> itemConfigs = ItemConfig.ListFromJson(AssetUtils.LoadText(RecipePath));

      foreach(ItemConfig itemConfig in itemConfigs)
      {
        if (HoneyPlusAssetBundle.Contains(itemConfig.Name))
        {
          CustomItem customItem = new CustomItem(HoneyPlusAssetBundle, itemConfig.Name, false, itemConfig);
          ItemManager.Instance.AddItem(customItem);
          Jotunn.Logger.LogInfo("Loaded Item: " + itemConfig.Name);
        }
      }

      HoneyPlusAssetBundle.Unload(false);
    }
    private void AddTranslations()
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

    internal static class HoneyPlusLogger
    {
      public static void LogMessage(object data) => Jotunn.Logger.LogMessage(data);
      public static void LogDebug(object data) => Jotunn.Logger.LogDebug(data);
    }
  }
}