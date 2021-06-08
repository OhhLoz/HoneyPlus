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
    public const string PluginVersion = "2.0.1";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string TranslationsFileName = "translations.json";

    private static readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, PluginGUID);

    private void Awake()
    {
      AddCustomItems();
      AddTranslations();
    }

    private static void AddCustomItems()
    {
      string RecipePath = Path.Combine(ModPath, RecipeFileName);
      Assembly ModAssembly = typeof(HoneyPlus).Assembly;
      AssetBundle HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, ModAssembly);
      List<ItemConfig> itemConfigs = ItemConfig.ListFromJson(AssetUtils.LoadText(RecipePath));

      foreach(ItemConfig itemConfig in itemConfigs)
      {
        if (HoneyPlusAssetBundle.Contains(itemConfig.Name))
        {
          GameObject prefab = HoneyPlusAssetBundle.LoadAsset<GameObject>(itemConfig.Name);
          CustomItem customItem = new CustomItem(prefab, true, itemConfig);
          ItemManager.Instance.AddItem(customItem);
        }
      }
    }
    private static void AddTranslations()
    {
            string translationsPath = Path.Combine(ModPath, TranslationsFileName);
            string translationsToString = AssetUtils.LoadText(translationsPath);
            LocalizationManager.Instance.AddJson("English", translationsToString);
            LocalizationManager.Instance.AddJson("Swedish", translationsToString);
            LocalizationManager.Instance.AddJson("Simplified Chinese", translationsToString);
            LocalizationManager.Instance.AddJson("Japanese", translationsToString);
            LocalizationManager.Instance.AddJson("Polish", translationsToString);
            LocalizationManager.Instance.AddJson("French", translationsToString);
            LocalizationManager.Instance.AddJson("German", translationsToString);
            LocalizationManager.Instance.AddJson("Russian", translationsToString);
            LocalizationManager.Instance.AddJson("Turkish", translationsToString);
            LocalizationManager.Instance.AddJson("Brazilian Portugese", translationsToString);
        }

        internal static class HoneyPlusLogger
    {
      public static void LogMessage(object data) => Jotunn.Logger.LogMessage(data);
      public static void LogDebug(object data) => Jotunn.Logger.LogDebug(data);
    }
  }
}