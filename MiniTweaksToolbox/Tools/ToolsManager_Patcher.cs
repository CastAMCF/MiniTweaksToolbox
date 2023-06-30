using Harmony12;

namespace MiniTweaksToolbox
{
    [HarmonyPatch(typeof(ToolsManager))]
    [HarmonyPatch("UseEngineCrane")]
    public static class ToolsManager_Patcher_UseEngineCrane_Prefix
    {
        [HarmonyPrefix]
        public static void ToolsManager_UseEngineCrane_Prefix(ref CarLoader carLoader)
        {
            carLoader = null;
        }
    }
}
