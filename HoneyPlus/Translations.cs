using System;
using System.Collections.Generic;

namespace HoneyPlus
{
  [Serializable]
  public class HoneyPlusTranslation
  {
    public string NameToken;
    public string NameValue;
    public string DescriptionToken;
    public string Description;
  }
  [Serializable]
  public class HoneyPlusTranslations
  {
    public Dictionary<string, HoneyPlusTranslation> translations = new Dictionary<string, HoneyPlusTranslation>();
  }
}