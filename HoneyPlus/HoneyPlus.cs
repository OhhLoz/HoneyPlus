using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace HoneyPlus
{
    [BepInPlugin(modGuid, modName, modVer)]
    [BepInProcess("valheim.exe")]
    public class HoneyPlus : BaseUnityPlugin
    {
        public const string modAuthor = "Loz";
        public const string modGuid = "Loz." + modName;
        private const string modName = "HoneyPlus";
        private const string modVer = "1.0.0";
        internal static HoneyPlus Instance { get; private set; }

        private readonly Harmony harmony = new Harmony(modGuid);

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        class Jump_Patch
        {
            static void Prefix(ref float ___m_jumpForce)
            {
                Debug.Log($"Jump force: {___m_jumpForce}");
                ___m_jumpForce = 15;
                Debug.Log($"Modified jump force: {___m_jumpForce}");
            }
        }
    }
}
