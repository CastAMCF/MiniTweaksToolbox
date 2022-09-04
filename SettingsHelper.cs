using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
	internal static class SettingsHelper
	{
		public static void CreateMTTSettingsButton(PauseQuitMenu __instance, List<GameObject> ___items)
		{
			Transform group = __instance.transform.Find("Content/Buttons");
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/ButtonsWindowButton"), group);
			Action backToMenu = delegate ()
			{
				SettingsHelper.___isEnabled = false;
				SettingsHelper.ClearMenu(__instance, ___items);
				SettingsHelper.CreateMTTSettingsButton(__instance, ___items);
				SettingsHelper.ShowOriginalMenu(__instance, ___items, group);
				SettingsHelper.AddAll(group, ___items);
				SettingsHelper.___isEnabled = true;
			};
			Action action = delegate ()
			{
				SettingsHelper.___isEnabled = false;
				SettingsHelper.ClearMenu(__instance, ___items);
				SettingsHelper.ShowSettingsMenu(__instance, ___items, group, backToMenu);
				SettingsHelper.___isEnabled = true;
			};
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"RunAction",
				"Action",
				action
			});
			gameObject.transform.Find("Text").GetComponent<Text>().text = "MiniTweaksToolbox Mod Settings";
			gameObject.GetComponent<ButtonsWindowMenuItem>().SetParent(__instance);
			gameObject.GetComponent<ButtonAction>().Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

		private static void ShowSettingsMenu(PauseQuitMenu __instance, List<GameObject> ___items, Transform group, Action backToMenu)
		{
			GameObject prefab = Resources.Load<GameObject>("UI/ButtonsWindowButton");
			GameObject prefab2 = Resources.Load<GameObject>("UI/CheckListItem");
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Tunned Parts", new Func<bool>(Settings.ToggleTunnedPartsSetting), Settings.tunnedParts);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Group Parts", new Func<bool>(Settings.ToggleGroupPartsSetting), Settings.groupParts);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Custom License Plates", new Func<bool>(Settings.ToggleCustomLicensePlatesSetting), Settings.customLPN);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Undiscovered Parts", new Func<bool>(Settings.ToggleUncheckedPartsSetting), Settings.uncheckedParts);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Inventory Check", new Func<bool>(Settings.ToggleInvCheckSetting), Settings.invCheck);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Painted Parts", new Func<bool>(Settings.TogglePaintPartsSetting), Settings.paintParts);
			SettingsHelper.CreateSetting(__instance, group, prefab2, "Auto Select", new Func<bool>(Settings.ToggleAutoSelectSetting), Settings.autoSelect);
            SettingsHelper.CreateSetting(__instance, group, prefab2, "No Oil Drain", new Func<bool>(Settings.ToggleNoOilDrainSetting), Settings.noOilDrain);
            SettingsHelper.CreateUpdatingSetting(__instance, group, prefab2, "Quality: ", Settings.quality.ToString(), new Func<object>(Settings.ToggleItemQualitySetting), Settings.itemQuality);
            SettingsHelper.CreateBackButton(__instance, group, prefab, backToMenu);
			SettingsHelper.AddAll(group, ___items);
		}

		private static void CreateSetting(PauseQuitMenu __instance, Transform group, GameObject prefab, string text, Func<bool> func, bool setting)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
			gameObject.transform.Find("Text").GetComponent<Text>().text = text;
			gameObject.transform.Find("ImgON").gameObject.SetActive(setting);
			gameObject.transform.Find("ImgOFF").gameObject.SetActive(!setting);
			Action action = delegate ()
			{
				bool flag = func();
                gameObject.transform.Find("ImgON").gameObject.SetActive(flag);
				gameObject.transform.Find("ImgOFF").gameObject.SetActive(!flag);
			};
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"RunAction",
				"Action",
				action
			});
			gameObject.AddComponent<ButtonSound>();
			ButtonAction buttonAction = gameObject.AddComponent<ButtonAction>();
			buttonAction.interactable = true;
			gameObject.AddComponent<EventTrigger>();
			gameObject.AddComponent<ChecklistWindowMenuItem>().SetParent(__instance);
			buttonAction.Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

        private static void CreateUpdatingSetting(PauseQuitMenu __instance, Transform group, GameObject prefab, string text, string changer, Func<object> func, bool setting)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
            gameObject.transform.Find("Text").GetComponent<Text>().text = text + changer;
            gameObject.transform.Find("ImgON").gameObject.SetActive(setting);
            gameObject.transform.Find("ImgOFF").gameObject.SetActive(!setting);
            Action action = delegate ()
            {
                object Func = func();
				bool flag = (bool)Func.GetType().GetProperty("flag").GetValue(Func, null);
                string Changer = (string)Func.GetType().GetProperty("changer").GetValue(Func, null);

                gameObject.transform.Find("Text").GetComponent<Text>().text = text + Changer;
                gameObject.transform.Find("ImgON").gameObject.SetActive(flag);
                gameObject.transform.Find("ImgOFF").gameObject.SetActive(!flag);
            };
            NewHash hash = new NewHash(new object[]
            {
                "WindowType",
                "QuitPauseMenu",
                "Type",
                "RunAction",
                "Action",
                action
            });
            gameObject.AddComponent<ButtonSound>();
            ButtonAction buttonAction = gameObject.AddComponent<ButtonAction>();
            buttonAction.interactable = true;
            gameObject.AddComponent<EventTrigger>();
            gameObject.AddComponent<ChecklistWindowMenuItem>().SetParent(__instance);
            buttonAction.Set(hash);
            LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
        }

        private static void CreateBackButton(PauseQuitMenu __instance, Transform group, GameObject prefab, Action action)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"RunAction",
				"Action",
				action
			});
			gameObject.transform.Find("Text").GetComponent<Text>().text = "Back";
			gameObject.GetComponent<ButtonsWindowMenuItem>().SetParent(__instance);
			gameObject.GetComponent<ButtonAction>().Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

		private static void ShowOriginalMenu(PauseQuitMenu __instance, List<GameObject> ___items, Transform group)
		{
			GameObject prefab = Resources.Load<GameObject>("UI/ButtonsWindowButton");
			SettingsHelper.CreateSaveAndQuitButton(__instance, group, prefab);
			SettingsHelper.CreateQuitButton(__instance, group, prefab);
			SettingsHelper.CreateContinueButton(__instance, group, prefab);
		}

		private static void AddAll(Transform group, List<GameObject> ___items)
		{
			IEnumerator enumerator = group.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					if (transform != null && transform.gameObject != null)
					{
						___items.Add(transform.gameObject);
					}
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
		}

		private static void CreateSaveAndQuitButton(PauseQuitMenu __instance, Transform group, GameObject prefab)
		{
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"ChangeScene",
				"NewScene",
				"Menu",
				"SceneType",
				SceneType.Menu
			});
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
			gameObject.transform.Find("Text").GetComponent<Text>().text = Localization.Instance.Get("GUI_Pause_QuitMenuSaveButton");
			gameObject.GetComponent<ButtonsWindowMenuItem>().SetParent(__instance);
			gameObject.GetComponent<ButtonAction>().Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

		private static void CreateQuitButton(PauseQuitMenu __instance, Transform group, GameObject prefab)
		{
			Action action = delegate ()
			{
				__instance.StartCoroutine(GameManager.Get().SelectSceneToLoad("Menu", SceneType.Menu, true, false));
			};
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"RunAction",
				"Action",
				action
			});
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
			gameObject.transform.Find("Text").GetComponent<Text>().text = Localization.Instance.Get("GUI_Pause_QuitMenuButton");
			gameObject.GetComponent<ButtonsWindowMenuItem>().SetParent(__instance);
			gameObject.GetComponent<ButtonAction>().Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

		private static void CreateContinueButton(PauseQuitMenu __instance, Transform group, GameObject prefab)
		{
			Action action = delegate ()
			{
				Debug.Log("Continue game");
				UIManager.Get().Hide("PauseQuitMenu");
			};
			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"QuitPauseMenu",
				"Type",
				"RunAction",
				"Action",
				action
			});
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group);
			gameObject.transform.Find("Text").GetComponent<Text>().text = Localization.Instance.Get("GUI_Pause_ContinueButton");
			gameObject.GetComponent<ButtonsWindowMenuItem>().SetParent(__instance);
			gameObject.GetComponent<ButtonAction>().Set(hash);
			LayoutRebuilder.ForceRebuildLayoutImmediate(group as RectTransform);
		}

		private static void ClearMenu(PauseQuitMenu __instance, List<GameObject> ___items)
		{
			Transform transform = __instance.transform.Find("Content/Buttons");
			IEnumerator enumerator = transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform2 = (Transform)obj;
					if (transform2 != null && transform2.gameObject != null)
					{
						UnityEngine.Object.Destroy(transform2.gameObject);
					}
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
			transform.DetachChildren();
			___items.Clear();
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
		}

		public static bool ___isEnabled = true;
	}
}
