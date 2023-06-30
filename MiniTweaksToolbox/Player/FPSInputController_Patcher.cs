using Harmony12;
using UnityEngine;
using static ModKit.UI;

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
            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Reset Postion").Key).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI)
                    {
                        switch (GameScript.Get().CurrentSceneType)
                        {
                            case SceneType.Parking:
                            case SceneType.Salon:
                            case SceneType.Junkyard:
                            case SceneType.Shed:
                            case SceneType.Garage:

                                __instance.transform.position = ModHelper.playerPosition;
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
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
