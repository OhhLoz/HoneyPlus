using System;
using System.Collections.Generic;

namespace HoneyPlus
{
    [Serializable]
    public class RecipeRequirement
    {
        public string item;
        public int amount;
    }

    [Serializable]
    public class CustomRecipe
    {
        public string name;
        public string item;
        public int amount;
        public string craftingStation;
        public int minStationLevel;
        public bool enabled;
        public string repairStation;
        public List<RecipeRequirement> resources = new List<RecipeRequirement>();
    }

    [Serializable]
    public class Recipes
    {
        public List<CustomRecipe> recipes = new List<CustomRecipe>();
    }
}