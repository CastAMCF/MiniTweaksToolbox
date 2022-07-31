using Harmony12;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(GameManager))]
	[HarmonyPatch("ButtonAccept")]
	public static class GameManager_Patcher_ButtonAccept
	{
		[HarmonyPrefix]
		public static void GameManager_ButtonAccept(GameManager __instance, ref NewHash hash)
		{
			string text = hash.GetFromKey("Type") as string;

            if (text.Equals("SellPerCondition"))
            {
				__instance.StartCoroutine(ModHelper.SellPerCondition((hash.GetFromKey("Slider") as Slider).value));
			}
		}
	}
}
