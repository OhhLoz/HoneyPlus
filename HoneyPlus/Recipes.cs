using System;
using System.Collections.Generic;
using Jotunn.Configs;

namespace HoneyPlus
{
  [Serializable]
  public class HoneyPlusRecipeRequirement
  {
    public string item;
    public int amount;
  }

  [Serializable]
  public class HoneyPlusRecipeConfig : RecipeConfig
  {
    public string NameValue;
    public string DescriptionToken;
    public string Description;
  }

  [Serializable]
  public class HoneyPlusRecipeConfigs
  {
    public List<HoneyPlusRecipeConfig> recipes = new List<HoneyPlusRecipeConfig>();
  }
}