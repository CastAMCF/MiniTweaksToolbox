using Harmony12;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(CarSelectAskWindow))]
	[HarmonyPatch("Prepare")]
	public static class CarSelectAskWindow_Patcher_Prepare
    {
		[HarmonyPostfix]
		public static void CarSelectAskWindow_Prepare(CarSelectAskWindow __instance, SceneType sceneType)
		{
            if (sceneType == SceneType.Garage)
            {
                GameObject original = Resources.Load<GameObject>("UI/SelectCarButton");
                CarLoader[] array = UnityEngine.Object.FindObjectsOfType<CarLoader>();
                IEnumerator enumerator = __instance.transform.Find("Scroll View/Viewport/Content").GetEnumerator();

                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        Transform transform = (Transform)obj;
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }

                array.ToList().ForEach(carLoader =>
                {
                    if (carLoader.GetRoot())
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(original, __instance.transform.Find("Scroll View/Viewport/Content"));
                        string carNameWithSuffix = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(carLoader.carToLoad, carLoader.ConfigVersion);
                        gameObject.transform.Find("CarImage").GetComponent<Image>().sprite = Singleton<GameInventory>.Instance.GetThumb(carLoader.carToLoad + "-" + carLoader.carToLoad);
                        gameObject.transform.Find("TextCarName").GetComponent<Text>().text = carNameWithSuffix;
                        gameObject.transform.Find("CarColor").gameObject.SetActive(true);
                        gameObject.transform.Find("CarColor").GetComponent<Image>().color = carLoader.color;
                        UIManager.Get().SetRibbon(gameObject.transform, carLoader.carToLoad);

                        Action action = delegate ()
                        {
                            NewInventoryItem newInventoryItem = new NewInventoryItem(carLoader.GetEngine().name, true);
                            newInventoryItem.extraParameters.Add("Carloader", carLoader);

                            GameScript.Get().SetEngineOnEngineStand(newInventoryItem);
                            __instance.Hide();
                            UIManager.Get().Hide("CarSelectAskWindow");
                        };

                        NewHash hash = new NewHash(new object[]
                        {
                            "WindowType",
                            "CarSelectAskWindow",
                            "Type",
                            "RunAction",
                            "Action",
                            action
                        });

                        gameObject.GetComponent<ButtonAction>().Set(hash);
                    }
                });
            }
        }
	}

    [HarmonyPatch(typeof(CarSelectAskWindow))]
    [HarmonyPatch("Hide")]
    public static class CarSelectAskWindow_Patcher_Hide
    {
        [HarmonyPostfix]
        public static void CarSelectAskWindow_Hide()
        {
            if (UIManager.Get().transform.Find("CarSelectAskWindow/Text").GetComponent<TextLocalize>().GetComponent<Text>().text == string.Empty)
            {
                GameScript.Get().CanOpenPieMenu = true;
                GameMode.Get().SetCurrentMode(gameMode.Garage);
            }
        }
    }
}
