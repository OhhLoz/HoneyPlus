using BepInEx;

namespace HoneyPlus
{
    [BepInPlugin(modGuid, modName, modVer)]
    public class HoneyPlus : BaseUnityPlugin
    {
        public const string modGuid = "Loz." + modName;
        private const string modName = "HoneyPlus";
        private const string modVer = "1.0.0";
    }
}
