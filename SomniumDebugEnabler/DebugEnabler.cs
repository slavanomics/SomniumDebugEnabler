using System;
using MelonLoader;
using Game;
using SLua;
using HarmonyLib;

namespace SomniumDebugEnabler
{

    [HarmonyPatch(typeof(ScriptManager), "Load")]
    class Patch2
    {
        public static void Postfix()
        {
            DebugEnabler.logger.Msg(LuaState.main["USER_NAME"]);
        }
    }

    [HarmonyPatch(typeof(LuaState), "setObject", new Type[] {
    typeof (string), typeof (object), typeof (bool), typeof (bool)
  })]
    class Patch
    {
        public static void Prefix(LuaState __instance, string key, ref object v)
        {
            if (key == "UNITY_EDITOR")
            {
                v = DebugEnabler.EDITOR.Value;
            }
            if (key == "UNITY_STANDALONE_WIN")
            {
                v = DebugEnabler.STANDALONE_WIN.Value;
            }
            if (key == "UNITY_PS4")
            {
                v = DebugEnabler.PS4.Value;
            }
            if (key == "UNITY_XBOXONE")
            {
                v = DebugEnabler.XBOXONE.Value;
            }
            if (key == "UNITY_SWITCH")
            {
                v = DebugEnabler.SWITCH.Value;
            }
            if (key == "BUILD_RELEASE")
            {
                v = DebugEnabler.BUILD_RELEASE.Value;
            }
            if (key == "BUILD_REGION")
            {
                v = DebugEnabler.BUILD_REGION.Value;
            }
            if (key == "USER_NAME")
            {
                v = DebugEnabler.USERNAME.Value;
            }
        }

    }
    public class DebugEnabler : MelonMod
    {
        public static MelonLogger.Instance logger;
        public static MelonPreferences_Category overCat;
        public static MelonPreferences_Entry<int> EDITOR;
        public static MelonPreferences_Entry<int> STANDALONE_WIN;
        public static MelonPreferences_Entry<int> PS4;
        public static MelonPreferences_Entry<int> XBOXONE;
        public static MelonPreferences_Entry<int> SWITCH;
        public static MelonPreferences_Entry<int> BUILD_RELEASE;
        public static MelonPreferences_Entry<string> BUILD_REGION;
        public static MelonPreferences_Entry<string> USERNAME;

        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("Mod Loaded.");
            logger = LoggerInstance;
            overCat = MelonPreferences.CreateCategory("Overrides");
            EDITOR = overCat.CreateEntry("UNITY_EDITOR", 0);
            STANDALONE_WIN = overCat.CreateEntry("UNITY_STANDALONE_WIN", 1);
            PS4 = overCat.CreateEntry("UNITY_PS4", 0);
            XBOXONE = overCat.CreateEntry("UNITY_XBOXONE", 0);
            SWITCH = overCat.CreateEntry("UNITY_SWITCH", 0);
            BUILD_RELEASE = overCat.CreateEntry("BUILD_RELEASE", 0);
            BUILD_REGION = overCat.CreateEntry("BUILD_REGION", "BUILD_WORLDWIDE");
            USERNAME = overCat.CreateEntry("USER_NAME", "gonzo");

        }
    }
}