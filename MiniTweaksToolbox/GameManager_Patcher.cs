using Harmony12;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(GameManager))]
	[HarmonyPatch("ButtonAccept")]
	public static class GameManager_Patcher_ButtonAccept
	{
		[HarmonyPostfix]
		public static void GameManager_ButtonAccept(GameManager __instance, ref NewHash hash)
		{
			string type = hash.GetFromKey("Type") as string;
            string from = hash.GetFromKey("From") as string;

            switch (type)
            {
                case "SellPerCondition":
                    {
                        __instance.StartCoroutine(ModHelper.SellPerCondition((hash.GetFromKey("Slider") as Slider).value));
                        break;
                    }
                case "SellAll":
                    {
                        __instance.StartCoroutine(ModHelper.ClearInventory());
                        break;
                    }
                case "MoveItem":
                    {
                        if (from.Contains("ItemsExchange"))
                        {
                            switch (GameScript.Get().CurrentSceneType)
                            {
                                case SceneType.Junkyard:
                                    if (ModHelper.toogleXray)
                                    {
                                        ModHelper.toogleXray = !ModHelper.toogleXray;
                                        ModHelper.ShedJunkXray();
                                    }
                                    else if (ModHelper.toogleXrayCar)
                                    {
                                        ModHelper.toogleXrayCar = !ModHelper.toogleXrayCar;
                                        ModHelper.ShedJunkXrayCar();
                                    }
                                    else if (ModHelper.toogleXraySpecial)
                                    {
                                        ModHelper.toogleXraySpecial = !ModHelper.toogleXraySpecial;
                                        ModHelper.ShedJunkXraySpecial("specialMap");
                                    }
                                    break;

                                case SceneType.Shed:
                                    if (ModHelper.toogleXray)
                                    {
                                        ModHelper.toogleXray = !ModHelper.toogleXray;
                                        ModHelper.ShedJunkXray();
                                    }
                                    else if (ModHelper.toogleXrayCar)
                                    {
                                        ModHelper.toogleXrayCar = !ModHelper.toogleXrayCar;
                                        ModHelper.ShedJunkXrayCar();
                                    }
                                    else if (ModHelper.toogleXraySpecial)
                                    {
                                        ModHelper.toogleXraySpecial = !ModHelper.toogleXraySpecial;
                                        ModHelper.ShedJunkXraySpecial("specialCase");
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;
                    }
            }
        }
	}
}
