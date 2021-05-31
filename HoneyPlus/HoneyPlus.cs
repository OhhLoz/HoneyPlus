using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace HoneyPlus
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
  internal class HoneyPlus : BaseUnityPlugin
  {
    public const string PluginGUID = "HoneyPlusJotunn";
    public const string PluginName = "HoneyPlusJotunn";
    public const string PluginVersion = "0.0.1";
    internal static readonly string RecipePath = Path.Combine(BepInEx.Paths.PluginPath, "HoneyPlus", "recipes.json");

    private void Awake()
    {
      AddCustomItems();
      AddRecipes();
    }

    private static void AddRecipes()
    {
      HoneyPlusLogger.LogMessage("Loading Recipes");
      ItemManager.Instance.AddRecipesFromJson(Path.Combine("HoneyPlus", "recipes.json"));
    }

    private static void AddCustomItems()
    {
      AssetBundle HoneyPlusAssetBundle = AssetUtils.LoadAssetBundleFromResources("honeyplusassets", typeof(HoneyPlus).Assembly);

      List<RecipeConfig> recipeConfigs = RecipeConfig.ListFromJson(AssetUtils.LoadText(RecipePath));

      foreach (RecipeConfig recipeConfig in recipeConfigs)
      {
        if (HoneyPlusAssetBundle.Contains(recipeConfig.Item))
        {
          HoneyPlusLogger.LogMessage($"Loading prefab for {recipeConfig.Item}");
          GameObject prefab = HoneyPlusAssetBundle.LoadAsset<GameObject>(recipeConfig.Item);
          CustomItem customItem = new CustomItem(prefab, true);
          ItemManager.Instance.AddItem(customItem);
        }
      }
    }

    internal static class HoneyPlusLogger
    {
      public static void LogMessage(object data) => Jotunn.Logger.LogMessage(data);
      public static void LogDebug(object data) => Jotunn.Logger.LogDebug(data);
    }
  }
}