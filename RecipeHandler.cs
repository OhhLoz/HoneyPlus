using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
using ValheimLib;
using ValheimLib.ODB;
using LitJson;

namespace HoneyPlus
{
    public static class RecipeHandler
    {
        public static ValheimLib.ODB.CustomRecipe CustomRecipe;
        public static ValheimLib.ODB.CustomItem CustomItem;
        public static Recipes Recipes;
        public static AssetBundle assetBundle;
        public static readonly Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

        public static void Init()
        {
            Debug.Log("[HoneyPlus] Initialising");

            FetchAssetBundle();
            AddCustomRecipes();
            AddCustomItems();
        }

        private static void FetchAssetBundle()
        {
            Recipes = LoadJsonFile<Recipes>("recipes.json");
            assetBundle = LoadAssetBundle("honeyplusassets");
            if (Recipes != null && assetBundle != null)
            {
                foreach (var recipe in Recipes.recipes)
                {
                    if (assetBundle.Contains(recipe.item))
                    {
                        Debug.Log("[HoneyPlus] Found Recipe: " + recipe.item);
                        var prefab = assetBundle.LoadAsset<GameObject>("Assets/Prefabs/" + recipe.item + ".prefab");
                        Prefabs.Add(recipe.item, prefab);
                    }
                }
            }
  
            assetBundle?.Unload(false);
        }

        private static void AddCustomItems()
        {
            foreach (var prefab in Prefabs.Values)
            {
                Debug.Log("[HoneyPlus] Adding Prefab: " + prefab.name);
                var itemDrop = prefab.GetComponent<ItemDrop>();
                if (itemDrop != null)
                {
                    CustomItem = new CustomItem(prefab, true);
                    ObjectDBHelper.Add(CustomItem);
                }
            }
        }

        private static void AddCustomRecipes()
        {
            foreach (var recipe in Recipes.recipes)
            {
                var tempRecipe = ScriptableObject.CreateInstance<Recipe>();
                var isPrefabFound = Prefabs.TryGetValue(recipe.item, out GameObject currPrefab);

                if (isPrefabFound)
                {
                    tempRecipe.m_item = currPrefab.GetComponent<ItemDrop>();
                    tempRecipe.m_amount = recipe.amount;

                    tempRecipe.m_craftingStation = Mock<CraftingStation>.Create(recipe.craftingStation);

                    var resourceRequirements = new List<Piece.Requirement>();

                    foreach (var resource in recipe.resources)
                    {
                        resourceRequirements.Add(MockRequirement.Create(resource.item, resource.amount));
                    }

                    tempRecipe.m_resources = resourceRequirements.ToArray();

                    Language.AddToken(recipe.name, recipe.nameValue);
                    Language.AddToken(recipe.descriptionToken, recipe.description);
                    CustomRecipe = new ValheimLib.ODB.CustomRecipe(tempRecipe, true, true);
                    ObjectDBHelper.Add(CustomRecipe);
                    Debug.Log("[HoneyPlus] Added Recipe: " + recipe.item);
                }
            }
        }

        private static T LoadJsonFile<T>(string filename) where T : class
        {
            var jsonFileName = GetAssetPath(filename);
            if (!string.IsNullOrEmpty(jsonFileName))
            {
                var jsonFile = File.ReadAllText(jsonFileName);
                return JsonMapper.ToObject<T>(jsonFile);
            }

            return null;
        }

        private static AssetBundle LoadAssetBundle(string filename)
        {
            var assetBundlePath = GetAssetPath(filename);
            if (!string.IsNullOrEmpty(assetBundlePath))
            {
                return AssetBundle.LoadFromFile(assetBundlePath);
            }

            return null;
        }

        private static string GetAssetPath(string assetName)
        {
            var assetFileName = Path.Combine(Paths.PluginPath, "HoneyPlus", assetName);
            if (!File.Exists(assetFileName))
            {
                Assembly assembly = typeof(HoneyPlus).Assembly;
                assetFileName = Path.Combine(Path.GetDirectoryName(assembly.Location), assetName);
                if (!File.Exists(assetFileName))
                {
                    Debug.LogError($"Could not find asset ({assetName})");
                    return null;
                }
            }

            return assetFileName;
        }
    }
}
