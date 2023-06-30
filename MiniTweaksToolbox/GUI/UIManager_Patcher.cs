using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Harmony12;
using UnityEngine;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
    [HarmonyPatch(typeof(UIManager))]
    [HarmonyPatch("IsWindow")]
    public static class UIManager_Patcher_IsWindow_Prefix
    {
        [HarmonyPrefix]
        public static void UIManager_IsWindow_Prefix(UIManager __instance, ref string windowName, ref UIGridManager ___carSelectAskWindowGridManager)
        {
            if (windowName.Equals("EngineCarSelectAskWindow"))
            {
                Transform transform = __instance.transform.Find("CarSelectAskWindow");
                if (transform != null)
                {
                    transform.GetComponent<CarSelectAskWindow>().Show(SceneType.Garage);
                    ___carSelectAskWindowGridManager = transform.GetComponent<UIGridManager>();
                    __instance.transform.Find("CarSelectAskWindow/Text").GetComponent<TextLocalize>().GetComponent<Text>().text = string.Empty;
                }
                SoundManager.Get().PlaySFX("MenuOpen");
            }
        }
    }

    [HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("prepareChooseItemsMenu")]
	public static class UIManager_Patcher_prepareChooseItemsMenu
	{
		[HarmonyPrefix]
		public static void UIManager_prepareChooseItemsMenu(UIManager __instance, ref string windowName, ref string type, ref string[] ID)
		{
			Main.mod.Logger.Log(windowName);
			Main.mod.Logger.Log(type);

			try
			{
				if (type.Equals("item") && !(windowName.Equals("NewChooseItemsMenuPaintshopPart") || windowName.Equals("NewChooseEngineMenu")))
				{
					if (Inventory.Get().MakeInventoryMount(ID[0]) == 0)
					{
						GameScript.Get().SelectPartToMount("999", 0f, Color.black, true, 0);
						__instance.ShowInfoWindow(Localization.Instance.Get("GUI_NieMaPrzedmiotowDoZalozenia"));

						type = "none";
					}
					else if (Main.Settings.autoSelect)
					{
                        NewInventoryItem newInventoryItem = (from i in Inventory.Get().GetItems(ID[0])
                                                             orderby Singleton<GameInventory>.Instance.GetItemProperty(i.ID).Price descending
                                                             orderby i.Condition descending
                                                             select i).ToList()[0];

                        if (Singleton<GameInventory>.Instance.GetItemProperty(newInventoryItem.ID).IsBody)
                            GameManager.Get().SelectedItemToMount = newInventoryItem;

                        CarLoader carLoader = GameScript.Get().GetIOMouseOverCarLoader2();

                        if (carLoader == null && !GameScript.Get().EngineStandIsEmpty())
							carLoader = GameScript.Get().GetGroupOnEngineStand().ItemList[0].extraParameters.GetHashTable()["Carloader"] as CarLoader;

                        if (Main.Settings.originalParts)
						{
							if (GameScript.Get().GetPartMouseOver() != null)
							{
								if (GameScript.Get().GetPartMouseOver().GetCondition() > 0.30)
								{
									if (Inventory.Get().GetItems(ID[0]).Where(x => x.Condition == GameScript.Get().GetPartMouseOver().GetCondition()).Any())
									{
										newInventoryItem = Inventory.Get().GetItems(ID[0]).FirstOrDefault(x => x.Condition == GameScript.Get().GetPartMouseOver().GetCondition());
									}
								}
							}
							else
							{
                                if (GameScript.Get().GetIOMouseOverCarLoader().condition > 0.30)
                                {
                                    if (Inventory.Get().GetItems(ID[0]).Where(x => x.Condition == GameScript.Get().GetIOMouseOverCarLoader().condition).Any())
                                    {
                                        newInventoryItem = Inventory.Get().GetItems(ID[0]).FirstOrDefault(x => x.Condition == GameScript.Get().GetIOMouseOverCarLoader().condition);
                                    }
                                }
                            }


                            if (carLoader != null)
							{
                                int carLoaderID = CarLoaderPlaces.Get().GetCarLoaderId(carLoader);

								if (OrderGenerator.Get().GetJobForCarLoader(carLoaderID) != null)
								{
									Job job = OrderGenerator.Get().GetJobForCarLoader(carLoaderID);

									if (GameScript.Get().GetPartMouseOver() != null)
									{
                                        if (job.jobParts.Any(jobPart => jobPart.partList.Any(part => part.Contains(GameScript.Get().GetPartMouseOver().GetIDWithTuned()))))
                                            if (GameScript.Get().GetPartMouseOver().GetCondition() > job.globalCondition)
											{
												if (Inventory.Get().GetItems(ID[0]).Where(x => x.Condition == GameScript.Get().GetPartMouseOver().GetCondition()).Any())
												{
													newInventoryItem = Inventory.Get().GetItems(ID[0]).FirstOrDefault(x => x.Condition == GameScript.Get().GetPartMouseOver().GetCondition());
												}
											}
									}
									else
									{
                                        carPart carPart = GameScript.Get().GetIOMouseOverCarLoader();
                                        string partName = carLoader.GetDefaultName(carPart.name);

                                        if (job.jobParts.Any(jobPart => jobPart.partList.Any(part => part.Contains(partName))))
                                            if (GameScript.Get().GetIOMouseOverCarLoader().condition > job.globalCondition)
											{
												if (Inventory.Get().GetItems(ID[0]).Where(x => x.Condition == GameScript.Get().GetIOMouseOverCarLoader().condition).Any())
												{
													newInventoryItem = Inventory.Get().GetItems(ID[0]).FirstOrDefault(x => x.Condition == GameScript.Get().GetIOMouseOverCarLoader().condition);
												}
											}
									}
								}
                            }
						}

						if (carLoader != null)
						{
							int carLoaderID = CarLoaderPlaces.Get().GetCarLoaderId(carLoader);

							if (OrderGenerator.Get().GetJobForCarLoader(carLoaderID) != null)
							{
                                Job job = OrderGenerator.Get().GetJobForCarLoader(carLoaderID);
                                string partName = newInventoryItem.ID.Contains('-') ? newInventoryItem.ID.Split('-')[1] : newInventoryItem.ID;

                                if (job.jobParts.Any(jobPart => jobPart.partList.Any(part => part.Contains(partName))))
                                    if (newInventoryItem.Condition < job.globalCondition)
										__instance.ShowPopup("MiniTweaksToolbox Mod:", $"Repair with minimal parts condition ({Helper.ConditionToString(job.globalCondition)})", PopupType.Normal);
                            }
                        }
                        
                        __instance.ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newInventoryItem.ID) + " (" + Helper.ConditionToString(newInventoryItem.Condition) + ")", PopupType.Normal);
                        
                        if (newInventoryItem.extraParameters.ContainsKey("PaintType"))
                        {
                            GameScript.Get().SetSelectedPartToMountPaintType((int)newInventoryItem.extraParameters.GetFromKey("PaintType"));
                        }
                        
                        if (ID[0].Equals("LicensePlate"))
                        {
                            GameScript.Get().SelectPartToMount(GameScript.Get().GetIOMouseOverCarLoader().name, newInventoryItem.Condition, newInventoryItem.GetItemColor(), newInventoryItem.IsExamined, newInventoryItem.GetItemQuality());
                        }
                        else
                        {
                            GameScript.Get().SelectPartToMount(newInventoryItem.ID, newInventoryItem.Condition, newInventoryItem.GetItemColor(), newInventoryItem.IsExamined, newInventoryItem.GetItemQuality());
                        }
                        
                        CarHelper.selectedItemToMount = newInventoryItem;

                        if (!Singleton<GameInventory>.Instance.GetItemProperty(newInventoryItem.ID).IsBody)
                            Inventory.Get().Delete(newInventoryItem);

                        type = "none";
					}

				}

				if (type.Equals("group") && windowName.Equals("NewChooseItemsMenu") && GameScript.Get().GetMachineOnMouseOverType() != IOSpecialType.EngineStand && Main.Settings.autoSelect)
				{
					List<NewGroupItem> list2 = new List<NewGroupItem>();

					if (ID[0].Contains("rim"))
					{
						/*int minRimSize = GameScript.Get().GetIOMouseOverCarLoader2().GetMinRimSize(true);
						float rearWheelMaxSize = GameScript.Get().GetIOMouseOverCarLoader2().GetRearWheelMaxSize();*/

                        object rimItem = CarHelper.CreateWheel("rim", GameScript.Get().GetIOMouseOverCarLoader2(), GameScript.Get().GetPartMouseOver().GetInstanceID());
                        NewInventoryItem newInventoryItem1 = (NewInventoryItem)rimItem.GetType().GetProperty("newInventoryItem").GetValue(rimItem, null);

                        float minRimSize = (float)newInventoryItem1.extraParameters.GetFromKey("Size");
                        float rearWheelMaxSize = (float)newInventoryItem1.extraParameters.GetFromKey("Size");

                        list2 = ModHelper.GetRimsWithSizeGreaterOrEqualThanAndLessThan(minRimSize, rearWheelMaxSize);

                        if (list2 == null || list2.Count() == 0)
                        {
                            list2 = GroupInventory.Get().GetRimsWithSizeGreaterOrEqualThanAndLessThan((int)minRimSize, rearWheelMaxSize);
                        }
                    }
					else if (ID[0].Contains("amortyzator"))
					{
						list2 = GroupInventory.Get().GetGroupInventory(ID[0]);
					}
					else
					{
						foreach (string id in ID)
						{
							if (GroupInventory.Get().GetGroupInventory(id) != null)
							{
								list2.AddRange(GroupInventory.Get().GetGroupInventory(id));
							}
						}
					}

					if (list2 == null || list2.Count() == 0)
					{
                        __instance.ShowInfoWindow(Localization.Instance.Get("GUI_NieMaPrzedmiotowDoZalozenia"));
						type = "none";
					}
					else
					{
						NewGroupItem newGroupItem = (from i in list2
													 orderby Helper.GetAvargeGroupCondition(i) descending
													 select i).ToList()[0];

						__instance.ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newGroupItem.GroupName) + " (" + Helper.ConditionToString(Helper.GetAvargeGroupCondition(newGroupItem)) + ")", PopupType.Normal);

						GameManager.Get().MountGroup(newGroupItem.UId);
						type = "none";
					}
				}

			}
			catch (Exception ex)
			{
				Main.mod.Logger.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("initCreateGroupMenu")]
	public static class UIManager_Patcher_initCreateGroupMenu_Prefix
	{
		[HarmonyPrefix]
		public static void UIManager_initCreateGroupMenu_Prefix(UIManager __instance, ref string windowName)
		{
			Main.mod.Logger.Log(windowName);

			if (windowName.Equals("MountGroup") && Main.Settings.autoSelect)
			{
				List<NewInventoryItem> newInventoryItems = new List<NewInventoryItem>();
				var originalParts = new List<KeyValuePair<string, float>>();

				if (Main.Settings.originalParts)
				{
					Transform parent = GameScript.Get().GetPartMouseOver().gameObject.transform;

					if (parent.childCount == 0)
					{
						parent = parent.parent;
					}

					originalParts.Add(new KeyValuePair<string, float>(parent.GetComponent<PartScript>().GetID(), parent.GetComponent<PartScript>().GetCondition()));

					string index = parent.name.Split('(')[1].Replace(")", "");
                    if (parent.GetComponent<PartScript>().GetID().Contains("tlok"))
					{
                        parent = parent.parent;

						for (int i = 0; i < parent.childCount; i++)
						{
							if (parent.GetChild(i).name.Contains("pierscienie") && parent.GetChild(i).name.Contains(index))
							{
								originalParts.Add(new KeyValuePair<string, float>(parent.GetChild(i).GetComponent<PartScript>().GetID(), parent.GetChild(i).GetComponent<PartScript>().GetCondition()));
							}
						}
                    }
					else
					{
                        for (int i = 0; i < parent.childCount; i++)
                        {
                            originalParts.Add(new KeyValuePair<string, float>(parent.GetChild(i).GetComponent<PartScript>().GetID(), parent.GetChild(i).GetComponent<PartScript>().GetCondition()));
                        }
                    }

				}

                CarLoader carLoader = GameScript.Get().GetIOMouseOverCarLoader2();

                if (carLoader == null && !GameScript.Get().EngineStandIsEmpty())
                    carLoader = GameScript.Get().GetGroupOnEngineStand().ItemList[0].extraParameters.GetHashTable()["Carloader"] as CarLoader;

                foreach (string item in GameScript.Get().GetGroupOfItems().ToArray())
				{
					List<NewInventoryItem> newInventoryItem = (from i in Inventory.Get().GetItems(item)
                                                               orderby Singleton<GameInventory>.Instance.GetItemProperty(i.ID).Price descending
                                                               orderby i.Condition descending
															   select i).ToList();

                    if (newInventoryItem.Count() > 0)
					{
						if (Main.Settings.originalParts)
							for (int i = 0; i < originalParts.Count(); i++)
							{
								if (originalParts[i].Key.Equals(item))
								{
									if (originalParts[i].Value > 0.30)
									{
										if (Inventory.Get().GetItems(item).Where(x => x.Condition == originalParts[i].Value).Any())
										{
											newInventoryItem[0] = Inventory.Get().GetItems(item).FirstOrDefault(x => x.Condition == originalParts[i].Value);
										}
									}

                                    if (carLoader != null)
                                    {
                                        int carLoaderID = CarLoaderPlaces.Get().GetCarLoaderId(carLoader);

                                        if (OrderGenerator.Get().GetJobForCarLoader(carLoaderID) != null)
                                        {
                                            Job job = OrderGenerator.Get().GetJobForCarLoader(carLoaderID);

                                            if (originalParts[i].Value > job.globalCondition)
                                            {
                                                if (Inventory.Get().GetItems(item).Where(x => x.Condition == originalParts[i].Value).Any())
                                                {
                                                    newInventoryItem[0] = Inventory.Get().GetItems(item).FirstOrDefault(x => x.Condition == originalParts[i].Value);
                                                }
                                            }
                                        }
                                    }

                                    originalParts.RemoveAt(i);
									break;
								}
							}

                        newInventoryItems.Add(newInventoryItem[0]);
						Inventory.Get().Delete(newInventoryItem[0]);
					}
                    else
                    {
                        if (newInventoryItems.Count() > 0)
                        {
							foreach (NewInventoryItem Newitem in newInventoryItems)
							{
								Inventory.Get().Add(Newitem);
							}
						}

						__instance.ShowInfoWindow(Localization.Instance.Get("GUI_NieMaPrzedmiotowDoZalozenia"));
						windowName = "None";

						break;
					}
				}

                if (!windowName.Equals("None"))
                {
					NewGroupItem newGroupItem = new NewGroupItem();
					newGroupItem.GroupName = newInventoryItems[0].ID;
					newGroupItem.IsNormalGroup = false;
					newGroupItem.ItemList = new List<NewInventoryItem>();
                    foreach (NewInventoryItem newInventoryItem in newInventoryItems)
                    {
                        if (carLoader != null)
                        {
                            int carLoaderID = CarLoaderPlaces.Get().GetCarLoaderId(carLoader);

                            if (OrderGenerator.Get().GetJobForCarLoader(carLoaderID) != null)
                            {
                                Job job = OrderGenerator.Get().GetJobForCarLoader(carLoaderID);

                                if (newInventoryItem.Condition < job.globalCondition)
                                    __instance.ShowPopup("MiniTweaksToolbox Mod:", $"Repair with minimal parts condition ({Helper.ConditionToString(job.globalCondition)})", PopupType.Normal);
                            }
                        }

                        __instance.ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newInventoryItem.ID) + " (" + Helper.ConditionToString(newInventoryItem.Condition) + ")", PopupType.Normal);
						newGroupItem.ItemList.Add(newInventoryItem);
					}

					GroupInventory.Get().Add(newGroupItem);
					GameManager.Get().MountGroup(newGroupItem.UId);

					windowName = "none";
				}
				
			}
			
		}
	}

	[HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("initCreateGroupMenu")]
	public static class UIManager_Patcher_initCreateGroupMenu_Postfix
	{
		[HarmonyPostfix]
		public static void UIManager_initCreateGroupMenu_Postfix(string windowName)
		{
			if (windowName.Equals("none") && Main.Settings.autoSelect)
			{
				GameMode.Get().SetCurrentMode(gameMode.PartMount);
			}
		}
	}

	[HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("ShowPopup")]
	public static class UIManager_Patcher_ShowPopup_Postfix
    {
		[HarmonyPostfix]
		public static void UIManager_ShowPopup_Postfix(UIManager __instance, ref PopupType popupType)
		{
			if (popupType != PopupType.Normal)
			{
				return;
			}

			Transform transform = __instance.transform.Find("Popup/PopupElements");

			if (transform != null)
			{
				if (transform.childCount < 2)
				{
					return;
				}

				foreach (Text text in transform.GetChild(1).GetComponentsInChildren<Text>())
				{
					Color green = new(0.5f, 1f, 0f, 1f);
					Color red = new(1f, 0f, 0f, 1f);

                    string text2 = text.text;
                    string color;

                    if (text2.Contains('§'))
					{
                        if (text2.Contains("ON"))
						{
                            color = ColorUtility.ToHtmlStringRGB(green);
                        }
						else if (text2.Contains("OFF"))
                        {
                            color = ColorUtility.ToHtmlStringRGB(red);
                        }
						else
						{
							return;
						}

                        text.supportRichText = true;
						text.text = string.Concat(new string[]
						{
							text2.Substring(0, text2.LastIndexOf('§')),
							"<color=#",
							color,
							">",
							text2.Substring(text2.LastIndexOf('§') + 1),
							"</color>"
						});
					}
					else if (text2.Contains("Quality"))
					{
						var helpColor = Helper.QualityToColor(int.Parse(text2.Substring(text2.LastIndexOf(' ') + 1)));
                        color = ColorUtility.ToHtmlStringRGB(helpColor);

                        text.supportRichText = true;
                        text.text = string.Concat(new string[]
                        {
							text2.Substring(0, text2.LastIndexOf(' ')),
							" <color=#",
							color,
							">",
							text2.Substring(text2.LastIndexOf(' ') + 1),
							"</color>"
                        });
                    }
				}
			}
		}
	}
}
