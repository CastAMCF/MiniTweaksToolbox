using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(ShedManager))]
	[HarmonyPatch("Update")]
	public static class ShedManager_Patcher_Update
    {
		[HarmonyPostfix]
		public static void ShedManager_Update(GameObject ___ShedRoot)
		{
			ModHelper.xrayScene = ___ShedRoot.transform;

            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.V).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Shed)
                    {
                        ModHelper.toogleXrayCar = !ModHelper.toogleXrayCar;
                        ModHelper.ShedJunkXrayCar();
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.X).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Shed)
                    {
                        ModHelper.toogleXray = !ModHelper.toogleXray;
                        ModHelper.ShedJunkXray();
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.Z).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Shed)
                    {
                        ModHelper.toogleXraySpecial = !ModHelper.toogleXraySpecial;
                        ModHelper.ShedJunkXraySpecial("specialCase");
                    }
                    break;

                default:
                    break;
            }

        }
	}

    [HarmonyPatch(typeof(ShedManager))]
    [HarmonyPatch("Start")]
    public static class ShedManager_Patcher_Start_Postfix
    {
        [HarmonyPostfix]
        public static void ShedManager_Start_Postfix(Transform ___player)
        {
            ModHelper.playerPosition = ___player.position;
            ModHelper.GetAllOwnedCars();
        }
    }
}
