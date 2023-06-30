using Harmony12;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(PieMenuController))]
	[HarmonyPatch("CheckSelectedOption")]
	public static class PieMenuController_Patcher_CheckSelectedOption
    {
        [HarmonyPrefix]
        public static void PieMenuController_CheckSelectedOption(PieMenuController __instance, ref List<string> ___NameList, int ___CurrOption, ref bool ___IsPieMenuOpen, ref bool ___RunPrevMenuWhenClose, RectTransform ___container)
		{
            string text = ___NameList[___CurrOption];

			if (text == "engine_add")
			{
                if (GameScript.Get().EngineStandIsEmpty())
                {
                    if (GroupInventory.Get().GetGroupInventory("engine-all").Count == 0)
                    {
                        __instance.IsEnabledPieMenu = false;

                        if (GameSettings.ConsoleMode && UIManager.Get().GetCurrentIODescription() == UIHelper.PieMenu)
                        {
                            UIManager.Get().SetIODescription(string.Empty, UIHelper.None);
                        }

                        ___IsPieMenuOpen = false;
                        ___RunPrevMenuWhenClose = false;

                        LeanTween.cancel(___container.gameObject);
                        SoundManager.Get().PlaySFXOneShot("PiemenuClose");

                        if (GameSettings.ConsoleMode)
                        {
                            Time.timeScale = 0.3f;
                        }

                        LeanTween.scale(___container, new Vector3(0.001f, 0.001f, 1f), 0.3f).setEase(LeanTweenType.easeInQuad).setIgnoreTimeScale(true).setOnComplete(new Action(__instance.OnCompleteCloseAnimation));

                        ___NameList[___CurrOption] = "car_engine_add";

                        GameScript.Get().CanOpenPieMenu = false;

                        UIManager.Get().IsWindow("EngineCarSelectAskWindow");
                    }
                }
            }
        }
	}
}
