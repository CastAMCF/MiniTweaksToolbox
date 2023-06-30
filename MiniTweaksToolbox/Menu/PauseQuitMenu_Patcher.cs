using System.Collections.Generic;
using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(PauseQuitMenu))]
	[HarmonyPatch("Prepare")]
	public static class PauseQuitMenu_Patcher_Prepare
	{
		[HarmonyPrefix]
		public static void PauseQuitMenu_Prepare(PauseQuitMenu __instance, List<GameObject> ___items)
		{
			if (GameScript.Get().CurrentSceneType == SceneType.Garage)
			{
				SettingsHelper.CreateMTTSettingsButton(__instance, ___items);
			}
		}
	}

	[HarmonyPatch(typeof(PauseQuitMenu))]
	[HarmonyPatch("Update")]
	public static class PauseQuitMenu_Patcher_Update
	{
		[HarmonyPrefix]
		public static bool PauseQuitMenu_Update(List<GameObject> ___items, int ___selectedObjectIndex)
		{
			return !Main.enabled || (SettingsHelper.___isEnabled && ___items.Count > ___selectedObjectIndex);
		}
	}
}
