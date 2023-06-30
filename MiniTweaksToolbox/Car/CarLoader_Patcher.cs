using Harmony12;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(CarLoader))]
	[HarmonyPatch("UseEngineCrane")]
	public static class CarLoader_Patcher_UseEngineCrane_Prefix
    {
		[HarmonyPrefix]
		public static void ToolsMoveManager_UseEngineCrane_Prefix(CarLoader __instance)
		{
            __instance.StartCoroutine(ModHelper.UseEngineCrane(__instance));
        }
	}
}
