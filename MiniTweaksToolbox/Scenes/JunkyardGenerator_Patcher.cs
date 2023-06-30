using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(JunkyardGenerator))]
	[HarmonyPatch("Update")]
	public static class JunkyardGenerator_Patcher_Update
    {
		[HarmonyPostfix]
		public static void JunkyardGenerator_Update(Transform ___Junkyard)
		{
            ModHelper.xrayScene = ___Junkyard;

            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.V).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Junkyard)
                    {
                        ModHelper.toogleXrayCar = !ModHelper.toogleXrayCar;
                        ModHelper.ShedJunkXrayCar();
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.X).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Junkyard)
                    {
                        ModHelper.toogleXray = !ModHelper.toogleXray;
                        ModHelper.ShedJunkXray();
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyCode.Z).ToString()):
                    if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Junkyard)
                    {
                        ModHelper.toogleXraySpecial = !ModHelper.toogleXraySpecial;
                        ModHelper.ShedJunkXraySpecial("specialMap");
                    }
                    break;

                default:
                    break;
            }

        }
	}

    [HarmonyPatch(typeof(JunkyardGenerator))]
    [HarmonyPatch("Start")]
    public static class JunkyardGenerator_Patcher_Start_Postfix
    {
        [HarmonyPostfix]
        public static void JunkyardGenerator_Start_Postfix()
        {
            ModHelper.GetAllOwnedCars();
        }
    }
}
