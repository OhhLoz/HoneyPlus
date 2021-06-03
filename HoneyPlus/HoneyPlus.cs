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
    public const string PluginVersion = "2.0.0";

    private const string AssetBundleName = "honeyplusassets";
    private const string RecipeFileName = "recipes.json";
    private const string I18NFileName = "i18n.json";

    private static readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, PluginGUID);

    private void Awake()
    {
      AddCustomItems();
      Addi18n();
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
    private static void Addi18n()
    {
      string i18nPath = Path.Combine(ModPath, I18NFileName);
      string i18nJsonString = AssetUtils.LoadText(i18nPath);
      HoneyPlusTranslations honeyPlusTranslations = SimpleJson.SimpleJson.DeserializeObject<HoneyPlusTranslations>(i18nJsonString);

      foreach(KeyValuePair<string, HoneyPlusTranslation> translation in honeyPlusTranslations.translations)
      {
        LocalizationManager.Instance.AddToken(translation.Value.NameToken, translation.Value.NameValue, false);
        LocalizationManager.Instance.AddToken(translation.Value.DescriptionToken, translation.Value.Description, false);
      }
    }

    internal static class HoneyPlusLogger
    {
      public static void LogMessage(object data) => Jotunn.Logger.LogMessage(data);
      public static void LogDebug(object data) => Jotunn.Logger.LogDebug(data);
    }
  }
}