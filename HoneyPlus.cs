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
    [BepInDependency(ValheimLib.ValheimLib.ModGuid)]
    [BepInPlugin(modGuid, modName, modVer)]
    [BepInProcess("valheim.exe")]
    public class HoneyPlus : BaseUnityPlugin
    {
        public const string modAuthor = "Loz";
        public const string modGuid = "Loz." + modName;
        private const string modName = "HoneyPlus";
        private const string modVer = "1.0.6";

        internal static HoneyPlus Instance { get; private set; }

        void Awake()
        {
            Debug.Log("[HoneyPlus] Awake()");

            RecipeHandler.Init();

            Instance = this;
        }
        private void OnDestroy()
        {
            Debug.Log("[HoneyPlus] OnDestroy() Called");
            foreach (var prefab in RecipeHandler.Prefabs.Values)
            {
                Destroy(prefab);
            }
            RecipeHandler.Prefabs.Clear();
        }
    }
}
