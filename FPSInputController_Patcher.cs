using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(FPSInputController))]
	[HarmonyPatch("Update")]
	public static class FPSInputController_Patcher_Update_Prefix
    {
		[HarmonyPrefix]
		public static void FPSInputController_Update_Prefix(ref float ___speedAdd)
        {
            switch (ModHelper.playerState)
            {
                case "walk":
                    ___speedAdd = Singleton<UpgradeSystem>.Instance.GetUpgradeValue("fast_movement");
                    break;
                case "running":
                    ___speedAdd = Singleton<UpgradeSystem>.Instance.GetUpgradeValue("fast_movement") * 2f;
                    break;
            }

        }
	}

    [HarmonyPatch(typeof(FPSInputController))]
    [HarmonyPatch("Update")]
    public static class FPSInputController_Patcher_Update_Postfix
    {
        [HarmonyPostfix]
        public static void FPSInputController_Update_Postfix(FPSInputController __instance)
        {
            if (Input.GetKeyDown(KeyCode.R) && (GameScript.Get().CurrentSceneType == SceneType.Parking || GameScript.Get().CurrentSceneType == SceneType.Salon || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage))
            {
                __instance.transform.position = ModHelper.playerPosition;
            }
        }
    }

    [HarmonyPatch(typeof(FPSInputController))]
    [HarmonyPatch("SetCharacterControllerPosition")]
    public static class FPSInputController_Patcher_SetCharacterControllerPosition_Postfix
    {
        [HarmonyPostfix]
        public static void FPSInputController_SetCharacterControllerPosition_Postfix(FPSInputController __instance)
        {
            ModHelper.playerPosition = __instance.transform.position;
        }
    }
}
