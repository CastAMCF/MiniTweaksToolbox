using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(GameScript))]
	[HarmonyPatch("Update")]
	public class GameScript_Patcher_Update
	{
		[HarmonyPostfix]
		private static void Postfix()
		{
			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage)
			{
				if (GameScript.Get().GetIOMouseOverCarLoader2() != null)
				{
                    if (Input.GetKeyUp(KeyCode.L))
                    {
                        if (GlobalData.GetPlayerMoney() < GlobalData.Cost_UseInteriorDetailingToolkit)
                        {
                            UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
                            return;
                        }

                        ModHelper.UseTool(IOSpecialType.InteriorDetailingToolkit, GameScript.Get().GetIOMouseOverCarLoader2());
                    }
                    else if (Input.GetKeyUp(KeyCode.K))
                    {
                        if (GlobalData.GetPlayerMoney() < GlobalData.Cost_UseWelder)
                        {
                            UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
                            return;
                        }

                        ModHelper.UseTool(IOSpecialType.Welder, GameScript.Get().GetIOMouseOverCarLoader2());
                    }
                    else if (Input.GetKeyUp(KeyCode.P))
                    {
                        CarHelper.Paint(GameScript.Get().GetIOMouseOverCarLoader2());
                    }
                }
				else if (Singleton<UpgradeSystem>.Instance.GetUpgradeValue("garage_upgrade") >= 2f && Input.GetKeyUp(KeyCode.Y))
				{
					GameScript.Get().IncraseEngineStandAngle(-90f);
				}
				else if (Singleton<UpgradeSystem>.Instance.GetUpgradeValue("garage_upgrade") >= 2f && Input.GetKeyUp(KeyCode.U))
				{
					GameScript.Get().IncraseEngineStandAngle(90f);
				}

			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI)
			{
				if (Input.GetKeyUp(KeyCode.V))
				{
					Jukebox.Get().TurnOffOrOn();
				}
				else if (Input.GetKeyUp(KeyCode.N))
				{
					Jukebox.Get().NextSong();
				}

            }

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.J))
			{
				CarHelper.total = 0;
				CarLoader carLoader = GameScript.Get().GetIOMouseOverCarLoader2();
				object allParts = CarHelper.GetAllBuyableParts(carLoader);
				List<string> parts = (List<string>)allParts.GetType().GetProperty("parts").GetValue(allParts, null);
				CarHelper.tiresInstance = (List<int>)allParts.GetType().GetProperty("tiresInstance").GetValue(allParts, null);
				CarHelper.rimsInstance = (List<int>)allParts.GetType().GetProperty("rimsInstance").GetValue(allParts, null);
				CarHelper.inventory = new List<NewInventoryItem>(Inventory.Get().GetItems("All"));
				CarHelper.groupInventory = new List<NewGroupItem>(GroupInventory.Get().GetGroupInventory());

				CarHelper.dupeCount = 0;


				string engineName = carLoader.GetEngine().GetComponent<InteractiveObject>().GetID().Split('(')[0];
				object engineParts = CarHelper.EngineParts(carLoader);
				List<NewInventoryItem> itemsEn = new List<NewInventoryItem>((List<NewInventoryItem>)engineParts.GetType().GetProperty("items").GetValue(engineParts, null));

				if (itemsEn.Except(itemsEn.Where(x => x.GetNormalID().Equals("bagnet_1") || x.GetNormalID().Equals("korekOleju_1") || x.GetNormalID().Equals("korek_spustowy_1"))).All(x => parts.Any(y => x.ID.Split('(')[0].Equals(y))) && Settings.groupParts)
				{
					int partEnPice = (int)engineParts.GetType().GetProperty("total").GetValue(engineParts, null);

					if (CarHelper.groupInventory.Any(x => x.GroupName.Equals(engineName) && x.ItemList.All(y => y.Condition == 1f && itemsEn.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == itemsEn.Count()) && Settings.invCheck)
					{
						CarHelper.dupeCount++;
						CarHelper.groupInventory.Remove(CarHelper.groupInventory.FirstOrDefault(x => x.GroupName.Equals(engineName) && x.ItemList.All(y => y.Condition == 1f && itemsEn.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == itemsEn.Count()));
					}
					else
					{
						if (GlobalData.GetPlayerMoney() >= CarHelper.total + partEnPice)
						{
							GroupInventory.Get().Add((NewGroupItem)engineParts.GetType().GetProperty("newGroupItem").GetValue(engineParts, null));

							Main.mod.Logger.Log(engineName);

							parts = parts.Where(x => !itemsEn.Select(p => p.ID.Split('(')[0]).Contains(x)).ToList();

							CarHelper.total += partEnPice;
						}
					}
				}


				string amortyzator = carLoader.GetRoot().GetComponentsInChildren<PartScript>().ToList().FirstOrDefault(part => Singleton<GameInventory>.Instance.GetItemProperty(part.GetIDWithTuned()).SpecialGroup == 8).GetIDWithTuned();

				if (parts.Any(x => x.Equals(amortyzator) || x.Equals("sprezynnaPrzod_1") || x.Equals("czapkaAmorPrzod_1") || x.Equals("amortyzator_double_rear") || x.Equals("sprezynaTyl_1") || x.Equals("czapkaSprezynyTyl_1")) && Settings.groupParts)
				{
					var amortyzatorsList = parts.Where(x => x.Equals(amortyzator) || x.Equals("sprezynnaPrzod_1") || x.Equals("czapkaAmorPrzod_1") || x.Equals("amortyzator_double_rear") || x.Equals("sprezynaTyl_1") || x.Equals("czapkaSprezynyTyl_1")).ToList();
					int amortyzatorsListLength = amortyzatorsList.Count();

					for (int i = 0; i < amortyzatorsListLength; i++)
					{
						if ((amortyzatorsList.Contains(amortyzator) && amortyzatorsList.Contains("sprezynnaPrzod_1") && amortyzatorsList.Contains("czapkaAmorPrzod_1")) || (amortyzatorsList.Contains("amortyzator_double_rear") && amortyzatorsList.Contains("sprezynaTyl_1") && amortyzatorsList.Contains("czapkaSprezynyTyl_1")))
						{
							object amortyzatorGroup = CarHelper.AmortyzatorGroup(amortyzator, "sprezynnaPrzod_1", "czapkaAmorPrzod_1");

							amortyzatorsList.Remove(amortyzator);
							amortyzatorsList.Remove("sprezynnaPrzod_1");
							amortyzatorsList.Remove("czapkaAmorPrzod_1");

							if (CarHelper.groupInventory.Any(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && Settings.invCheck)
							{
								CarHelper.dupeCount++;
								CarHelper.groupInventory.Remove(CarHelper.groupInventory.FirstOrDefault(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f));
							}
							else
							{
								int partPice = (int)amortyzatorGroup.GetType().GetProperty("total").GetValue(amortyzatorGroup, null);

								if (GlobalData.GetPlayerMoney() >= CarHelper.total + partPice)
								{
									parts.Remove(amortyzator);
									parts.Remove("sprezynnaPrzod_1");
									parts.Remove("czapkaAmorPrzod_1");

									GroupInventory.Get().Add((NewGroupItem)amortyzatorGroup.GetType().GetProperty("newGroupItem").GetValue(amortyzatorGroup, null));

									Main.mod.Logger.Log(amortyzator + " + sprezynnaPrzod_1 + czapkaAmorPrzod_1");

									CarHelper.total += partPice;
								}
							}

						}

                        if (amortyzatorsList.Contains("amortyzator_double_rear") && amortyzatorsList.Contains("sprezynaTyl_1") && amortyzatorsList.Contains("czapkaSprezynyTyl_1"))
                        {
                            object amortyzatorGroup = CarHelper.AmortyzatorGroup("amortyzator_double_rear", "sprezynaTyl_1", "czapkaSprezynyTyl_1");

                            amortyzatorsList.Remove("amortyzator_double_rear");
                            amortyzatorsList.Remove("sprezynaTyl_1");
                            amortyzatorsList.Remove("czapkaSprezynyTyl_1");

                            if (CarHelper.groupInventory.Any(x => x.ItemList[0].ID.Equals("amortyzator_double_rear") && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynaTyl_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaSprezynyTyl_1") && x.ItemList[2].Condition == 1f) && Settings.invCheck)
                            {
                                CarHelper.dupeCount++;
                                CarHelper.groupInventory.Remove(CarHelper.groupInventory.FirstOrDefault(x => x.ItemList[0].ID.Equals("amortyzator_double_rear") && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynaTyl_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaSprezynyTyl_1") && x.ItemList[2].Condition == 1f));
                            }
                            else
                            {
                                int partPice = (int)amortyzatorGroup.GetType().GetProperty("total").GetValue(amortyzatorGroup, null);

                                if (GlobalData.GetPlayerMoney() >= CarHelper.total + partPice)
                                {
                                    parts.Remove("amortyzator_double_rear");
                                    parts.Remove("sprezynaTyl_1");
                                    parts.Remove("czapkaSprezynyTyl_1");

                                    GroupInventory.Get().Add((NewGroupItem)amortyzatorGroup.GetType().GetProperty("newGroupItem").GetValue(amortyzatorGroup, null));

                                    Main.mod.Logger.Log("amortyzator_double_rear + sprezynaTyl_1 + czapkaSprezynyTyl_1");

                                    CarHelper.total += partPice;
                                }
                            }

                        }
                    }
				}


				if (parts.Any(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 7) && Settings.groupParts)
				{

					for (int i = 0; i < 10; i++)
					{
						if (CarHelper.tiresInstance.Any() && CarHelper.rimsInstance.Any())
						{
							object wheelGroup = CarHelper.CreateWheelGroup(carLoader, CarHelper.rimsInstance[i]);
							NewGroupItem newGroupItem = (NewGroupItem)wheelGroup.GetType().GetProperty("newGroupItem").GetValue(wheelGroup, null);

							Tire tire = (Tire)wheelGroup.GetType().GetProperty("tire").GetValue(wheelGroup, null);

							string rimName = (string)wheelGroup.GetType().GetProperty("rimName").GetValue(wheelGroup, null);
							string tirName = (string)wheelGroup.GetType().GetProperty("tireName").GetValue(wheelGroup, null);
							NewInventoryItem rim = (NewInventoryItem)wheelGroup.GetType().GetProperty("rimItem").GetValue(wheelGroup, null);
							NewInventoryItem tir = (NewInventoryItem)wheelGroup.GetType().GetProperty("tireItem").GetValue(wheelGroup, null);

							CarHelper.tiresInstance.RemoveAt(0);
							CarHelper.rimsInstance.RemoveAt(0);
							i--;

							if (CarHelper.groupInventory.Any(x => x.ItemList[0].ID.Equals(rimName) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize &&
									x.ItemList[1].ID.Equals(tirName) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Settings.invCheck)
							{
								CarHelper.dupeCount++;
								CarHelper.groupInventory.Remove(CarHelper.groupInventory.FirstOrDefault(x => x.ItemList[0].ID.Equals(rimName) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize &&
											x.ItemList[1].ID.Equals(tirName) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
							}
							else
							{
								int partPice = (int)Mathf.Floor((Helper.GetPrice(tir) + Helper.GetPrice(rim)) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (GlobalData.GetPlayerMoney() >= CarHelper.total + partPice)
								{
									parts.Remove(tirName);
									parts.Remove(rimName);

									GroupInventory.Get().Add(newGroupItem);

									Main.mod.Logger.Log(rimName + " + " + tirName);

									CarHelper.total += partPice;
								}
							}

						}
						else
						{
							break;
						}
					}
				}


				var partsListSorted = (from part in parts
									   orderby Singleton<GameInventory>.Instance.GetItemProperty(part).Price descending
									   select part).ToList();

				Inventory.Get().StartCoroutine(CarHelper.AddManyParts(carLoader, partsListSorted));

				return;
			}



			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.B))
			{
				try
				{
					string text1 = GameScript.Get().GetPartMouseOver().GetIDWithTuned();

					if (text1 != null && Singleton<GameInventory>.Instance.GetItemProperty(text1).Price != 0)
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price != 0 && GlobalData.GetPlayerMoney() >= (Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) && Settings.tunnedParts)
						{
							text1 = "t_" + text1;
						}

						float[] itemColor = (!Singleton<GameInventory>.Instance.GetItemProperty(text1).IsBody) ? Inventory.SetColor(Color.white) : Inventory.SetColor(GlobalData.DEFAULT_ITEM_COLOR);
						int num1 = (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(text1).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
						NewInventoryItem newInventoryItem1;

						if (GameScript.Get().GetIOMouseOverCarLoader2() != null)
						{
							Tire tire = GameScript.Get().GetIOMouseOverCarLoader2().GetTires()[0];
							newInventoryItem1 = new NewInventoryItem(text1, 1f, true);

							if (Singleton<GameInventory>.Instance.GetItemProperty(text1).ShopGroup.Equals("Engine") && Settings.groupParts)
							{
								string engineName = GameScript.Get().GetIOMouseOverCarLoader2().GetEngine().GetComponent<InteractiveObject>().GetID().Split('(')[0];
								object engineParts = CarHelper.EngineParts(GameScript.Get().GetIOMouseOverCarLoader2());
								int total = (int)engineParts.GetType().GetProperty("total").GetValue(engineParts, null);
								NewGroupItem newGroupItem = (NewGroupItem)engineParts.GetType().GetProperty("newGroupItem").GetValue(engineParts, null);
								List<NewInventoryItem> items = (List<NewInventoryItem>)engineParts.GetType().GetProperty("items").GetValue(engineParts, null);

								if (GlobalData.GetPlayerMoney() >= total)
								{

									if (GroupInventory.Get().GetGroupInventory().Any(x => x.GroupName.Equals(engineName) && x.ItemList.All(y => y.Condition == 1f && items.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == items.Count()) && Settings.invCheck && (!ModHelper.dupeBool || !text1.Equals(ModHelper.dupeText)))
									{
										ModHelper.dupeBool = true;
										ModHelper.dupeText = engineName;
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
										return;
									}

									Main.mod.Logger.Log(engineName);
									ModHelper.dupeBool = false;
									ModHelper.dupeText = engineName;

									GroupInventory.Get().Add(newGroupItem);
									UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(engineName) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

									GlobalData.AddPlayerMoney(-total);
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(total), PopupType.Buy);

									return;
								}
							}
							else if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7) && Settings.groupParts)
							{

								object wheelGroup = CarHelper.CreateWheelGroup(GameScript.Get().GetIOMouseOverCarLoader2(), GameScript.Get().GetPartMouseOver().GetInstanceID());
								NewGroupItem newGroupItem = (NewGroupItem)wheelGroup.GetType().GetProperty("newGroupItem").GetValue(wheelGroup, null);

								tire = (Tire)wheelGroup.GetType().GetProperty("tire").GetValue(wheelGroup, null);

								string rimName = (string)wheelGroup.GetType().GetProperty("rimName").GetValue(wheelGroup, null);
								NewInventoryItem rim = (NewInventoryItem)wheelGroup.GetType().GetProperty("rimItem").GetValue(wheelGroup, null);
								NewInventoryItem tir = (NewInventoryItem)wheelGroup.GetType().GetProperty("tireItem").GetValue(wheelGroup, null);

								num1 = (int)Mathf.Floor((Helper.GetPrice(tir) + Helper.GetPrice(rim)) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (GlobalData.GetPlayerMoney() >= num1)
								{

									if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(rim) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize &&
									x.ItemList[1].ID.Equals(tir) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Settings.invCheck && (!ModHelper.dupeBool || !text1.Equals(ModHelper.dupeText)))
									{
										ModHelper.dupeBool = true;
										ModHelper.dupeText = text1;
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
										return;
									}

									Main.mod.Logger.Log(rim + " + " + tir);
									ModHelper.dupeBool = false;
									ModHelper.dupeText = text1;

									GroupInventory.Get().Add(newGroupItem);
									UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), string.Concat(new object[] { Singleton<GameInventory>.Instance.GetItemLocalizeName(rimName), " (", tire.w_wheelWidth, "/", tire.w_tireSize, "R", tire.w_rimSize, ") ", "ET", ":", tire.w_et }) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

									GlobalData.AddPlayerMoney(-num1);
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

									return;
								}
							}
							else if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 8 || text1.Equals("sprezynnaPrzod_1") || text1.Equals("czapkaAmorPrzod_1")) && Settings.groupParts)
							{
								string amortyzator = GameScript.Get().GetIOMouseOverCarLoader2().GetRoot().GetComponentsInChildren<PartScript>().ToList().FirstOrDefault(part => Singleton<GameInventory>.Instance.GetItemProperty(part.GetIDWithTuned()).SpecialGroup == 8).GetIDWithTuned();

                                if (text1.Equals("amortyzator_double_rear"))
                                {
                                    object amortyzatorGroup = CarHelper.AmortyzatorGroup("amortyzator_double_rear", "sprezynaTyl_1", "czapkaSprezynyTyl_1");
                                    num1 = (int)amortyzatorGroup.GetType().GetProperty("total").GetValue(amortyzatorGroup, null);

                                    if (GlobalData.GetPlayerMoney() >= num1)
                                    {
                                        if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals("amortyzator_double_rear") && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynaTyl_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaSprezynyTyl_1") && x.ItemList[2].Condition == 1f) && Settings.invCheck && (!ModHelper.dupeBool || !text1.Equals(ModHelper.dupeText)))
                                        {
                                            ModHelper.dupeBool = true;
                                            ModHelper.dupeText = text1;
                                            UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
                                            return;
                                        }

                                        Main.mod.Logger.Log("amortyzator_double_rear + sprezynaTyl_1 + czapkaSprezynyTyl_1");
                                        ModHelper.dupeBool = false;
                                        ModHelper.dupeText = text1;

                                        GroupInventory.Get().Add((NewGroupItem)amortyzatorGroup.GetType().GetProperty("newGroupItem").GetValue(amortyzatorGroup, null));
                                        UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName("amortyzator_double_rear") + " (Group) (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

                                        GlobalData.AddPlayerMoney(-num1);
                                        UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

                                        return;
                                    }
                                }
                                else if (text1.Equals(amortyzator) || text1.Equals("sprezynnaPrzod_1") || text1.Equals("czapkaAmorPrzod_1"))
								{
									object amortyzatorGroup = CarHelper.AmortyzatorGroup(amortyzator, "sprezynnaPrzod_1", "czapkaAmorPrzod_1");
									num1 = (int)amortyzatorGroup.GetType().GetProperty("total").GetValue(amortyzatorGroup, null);

									if (GlobalData.GetPlayerMoney() >= num1)
									{
										if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && Settings.invCheck && (!ModHelper.dupeBool || !text1.Equals(ModHelper.dupeText)))
										{
											ModHelper.dupeBool = true;
											ModHelper.dupeText = text1;
											UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
											return;
										}

										Main.mod.Logger.Log(amortyzator + " + sprezynnaPrzod_1 + czapkaAmorPrzod_1");
										ModHelper.dupeBool = false;
										ModHelper.dupeText = text1;

										GroupInventory.Get().Add((NewGroupItem)amortyzatorGroup.GetType().GetProperty("newGroupItem").GetValue(amortyzatorGroup, null));
										UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(amortyzator) + " (Group) (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

										GlobalData.AddPlayerMoney(-num1);
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

										return;
									}
								}
                            }
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6)
							{
								object tireItem = CarHelper.CreateWheel("tire", GameScript.Get().GetIOMouseOverCarLoader2(), GameScript.Get().GetPartMouseOver().GetInstanceID());
								newInventoryItem1 = (NewInventoryItem)tireItem.GetType().GetProperty("newInventoryItem").GetValue(tireItem, null);

								tire = (Tire)tireItem.GetType().GetProperty("tire").GetValue(tireItem, null);
							}
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7)
							{
								object rimItem = CarHelper.CreateWheel("rim", GameScript.Get().GetIOMouseOverCarLoader2(), GameScript.Get().GetPartMouseOver().GetInstanceID());
								newInventoryItem1 = (NewInventoryItem)rimItem.GetType().GetProperty("newInventoryItem").GetValue(rimItem, null);

								tire = (Tire)rimItem.GetType().GetProperty("tire").GetValue(rimItem, null);
							}

							if (ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2(), tire, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

						}
						else
						{
							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}
						}

						if (text1.Equals("wentylatorChlodnicy_1"))
						{
							int check = 0;
							bool flag = ModHelper.dupeText.Equals("wentylatorChlodnicy_1+wentylatorChlodnicy_1_fan_1");
							text1 = "wentylatorChlodnicy_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "wentylatorChlodnicy_1_fan_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 2)
								ModHelper.dupeText = "wentylatorChlodnicy_1+wentylatorChlodnicy_1_fan_1";

							return;
						}
						else if (text1.Equals("wentylatorChlodnicy_2"))
						{
							int check = 0;
							bool flag = ModHelper.dupeText.Equals("wentylatorChlodnicy_2+wentylatorChlodnicy_2_fan_1+wentylatorChlodnicy_2_fan_2");
							text1 = "wentylatorChlodnicy_2";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "wentylatorChlodnicy_2_fan_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "wentylatorChlodnicy_2_fan_2";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 3 || check == 2)
								ModHelper.dupeText = "wentylatorChlodnicy_2+wentylatorChlodnicy_2_fan_1+wentylatorChlodnicy_2_fan_2";

							return;
						}
						else if (text1.Equals("amortyzatorPrzod_1") || text1.Equals("amortyzator_double") || text1.Equals("amortyzator_double_rear"))
						{
							string amortyzator = text1;
							int check = 0;
							bool flag = ModHelper.dupeText.Equals(amortyzator + "+sprezynnaPrzod_1+czapkaAmorPrzod_1");

							if (amortyzator.Equals("amortyzator_double_rear"))
							{
                                flag = ModHelper.dupeText.Equals(amortyzator + "+sprezynaTyl_1+czapkaSprezynyTyl_1");
							}
							else
							{
                                flag = ModHelper.dupeText.Equals(amortyzator + "+sprezynnaPrzod_1+czapkaAmorPrzod_1");
                            }

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


                            if (amortyzator.Equals("amortyzator_double_rear"))
                            {
                                text1 = "sprezynaTyl_1";
                            }
                            else
                            {
                                text1 = "sprezynnaPrzod_1";
                            }

                            newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


                            if (amortyzator.Equals("amortyzator_double_rear"))
                            {
                                text1 = "czapkaSprezynyTyl_1";
                            }
                            else
                            {
                                text1 = "czapkaAmorPrzod_1";
                            }

                            newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 3 || check == 2)
								if (amortyzator.Equals("amortyzator_double_rear"))
								{
                                    ModHelper.dupeText = amortyzator + "+sprezynaTyl_1+czapkaSprezynyTyl_1";
                                }
								else
								{
                                    ModHelper.dupeText = amortyzator + "+sprezynnaPrzod_1+czapkaAmorPrzod_1";
                                }

							return;
						}
						else if (text1.Equals("tlok_1") || text1.Equals("t_tlok_1"))
						{
							int check = 0;
							string part = text1;
							bool flag = ModHelper.dupeText.Equals(part + "+tlok_1_pierscienie_1");

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "tlok_1_pierscienie_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 2)
								ModHelper.dupeText = part + "+tlok_1_pierscienie_1";

							return;
						}
						else if (text1.Equals("v8_hemi_tlok") || text1.Equals("t_v8_hemi_tlok"))
						{
							int check = 0;
							string part = text1;
							bool flag = ModHelper.dupeText.Equals(part + "+v8_hemi_tlok_pierscienie_1");

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "v8_hemi_tlok_pierscienie_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 2)
								ModHelper.dupeText = part + "+v8_hemi_tlok_pierscienie_1";

							return;
						}
						else if (text1.Equals("w12_tlok") || text1.Equals("t_w12_tlok"))
						{
							int check = 0;
							string part = text1;
							bool flag = ModHelper.dupeText.Equals(part + "+w12_tlok_pierscienie_1");

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}


							text1 = "w12_tlok_pierscienie_1";

							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							if (Settings.itemQuality)
							{
								newInventoryItem1.extraParameters.Add("Quality", Settings.quality);
							}

							num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (GlobalData.GetPlayerMoney() < num1)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							}
							else if (!flag && ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								check++;
							}
							else
							{
								Main.mod.Logger.Log(text1);
								ModHelper.dupeBool = false;
								ModHelper.dupeText = text1;

								Inventory.Get().Add(newInventoryItem1);
								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
							}

							if (check == 2)
								ModHelper.dupeText = part + "+w12_tlok_pierscienie_1";

							return;
						}

						num1 = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem1) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

						if (GlobalData.GetPlayerMoney() < num1)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							return;
						}
						else
						{
							if (ModHelper.IsInInventory(GameScript.Get().GetIOMouseOverCarLoader2() ? GameScript.Get().GetIOMouseOverCarLoader2() : null, null, text1) && Settings.invCheck)
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);

							Main.mod.Logger.Log(text1);
							ModHelper.dupeBool = false;
							ModHelper.dupeText = text1;

							Inventory.Get().Add(newInventoryItem1);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

							GlobalData.AddPlayerMoney(-num1);
							UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
						}
					}
				}
				catch (Exception)
				{
				}

				try
				{
					carPart part = GameScript.Get().GetIOMouseOverCarLoader();
					string text2 = GameScript.Get().GetIOMouseOverCarLoader2().GetDefaultName(part.name);
					int num2 = 0;

					if (text2 != null && (Singleton<GameInventory>.Instance.GetItemProperty(text2).Price != 0 || Singleton<GameInventory>.Instance.GetItemProperty(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2).Price != 0))
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2).Price != 0)
						{
							text2 = GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2;
						}

						NewInventoryItem newInventoryItem2;

						if (Singleton<GameInventory>.Instance.GetItemProperty(text2).IsBody)
						{

							if (Settings.paintParts)
							{
								newInventoryItem2 = new NewInventoryItem(text2, 1f, Inventory.SetColor(GameScript.Get().GetIOMouseOverCarLoader().color), true);
								newInventoryItem2.extraParameters.Add("PaintType", part.paintType);
								newInventoryItem2.extraParameters.Add("Livery", part.livery);
								newInventoryItem2.extraParameters.Add("LiveryStrength", part.liveryStrength);
								num2 += 100;
							}
							else
							{
								newInventoryItem2 = new NewInventoryItem(text2, 1f, Inventory.SetColor(GlobalData.DEFAULT_ITEM_COLOR), true);
								newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);
							}
						}
						else
						{
							newInventoryItem2 = new NewInventoryItem(text2, 1f, true);
							newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);
						}

						if (Settings.itemQuality)
						{
							newInventoryItem2.extraParameters.Add("Quality", Settings.quality);
						}

						num2 += (int)Mathf.Floor(Helper.GetPrice(newInventoryItem2) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

						if (GlobalData.GetPlayerMoney() < num2)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							return;
						}
						else
						{
							if (Inventory.Get().GetItems("All").Any(x => text2.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(part.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(part.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(part.liveryStrength)) && Settings.paintParts && Settings.invCheck && (!ModHelper.dupeBool || !text2.Equals(ModHelper.dupeText)))
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text2;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}
							else if (Inventory.Get().GetItems("All").Any(x => text2.Equals(x.ID) && x.Condition == 1f) && Settings.invCheck && (!ModHelper.dupeBool || !text2.Equals(ModHelper.dupeText)))
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = text2;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							Main.mod.Logger.Log(text2);
							ModHelper.dupeBool = false;
							ModHelper.dupeText = text2;

							Inventory.Get().Add(newInventoryItem2);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text2) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

							GlobalData.AddPlayerMoney(-num2);
							UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num2), PopupType.Buy);
						}
						return;
					}
				}
				catch (Exception)
				{
				}

				try
				{
					if (GameScript.Get().GetIOMouseOverCarLoader() != null)
					{
						if (Singleton<GameInventory>.Instance.IsLicensePlate(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(GameScript.Get().GetIOMouseOverCarLoader().name).Split('[')[0]))
						{
							if (GlobalData.GetPlayerMoney() < (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) && Settings.customLPN)
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
								return;
							}
							else if (GlobalData.GetPlayerMoney() < (int)Mathf.Floor(100f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")))
							{
								UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
								return;
							}

							NewInventoryItem newInventoryItem3 = new NewInventoryItem("LicensePlate", 1f, Inventory.SetColor(Color.white), true);
							newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);
							newInventoryItem3.extraParameters.Add("LPName", GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(GameScript.Get().GetIOMouseOverCarLoader().name));

							int num3 = (int)Mathf.Floor(100f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

							if (Settings.customLPN)
							{
								newInventoryItem3.extraParameters.Add("CustomLPN", GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart(GameScript.Get().GetIOMouseOverCarLoader().name).handle.GetComponentInChildren<Text>().text);
								num3 = (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
							}

							if (Inventory.Get().GetItems("All").Any(x => x.ID.Equals("LicensePlate") && x.Condition == 1f) && Settings.invCheck && (!ModHelper.dupeBool || !GameScript.Get().GetIOMouseOverCarLoader().name.Equals(ModHelper.dupeText)))
							{
								ModHelper.dupeBool = true;
								ModHelper.dupeText = GameScript.Get().GetIOMouseOverCarLoader().name;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							Main.mod.Logger.Log(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(GameScript.Get().GetIOMouseOverCarLoader().name));
							ModHelper.dupeBool = false;
							ModHelper.dupeText = GameScript.Get().GetIOMouseOverCarLoader().name;

							Inventory.Get().Add(newInventoryItem3);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetBodyLocalizedName(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + GameScript.Get().GetIOMouseOverCarLoader().name), PopupType.Normal);

							GlobalData.AddPlayerMoney(-num3);
							UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num3), PopupType.Buy);

							return;
						}
					}
				}
				catch (Exception)
				{
					return;
				}
			}
			else if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Salon || GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard) && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.B))
			{
				string carName = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad, GameScript.Get().GetIOMouseOverCarLoader2().ConfigVersion);
				string carExtension = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad, GameScript.Get().GetIOMouseOverCarLoader2().ConfigVersion).Replace(carName, "");

				int carPrice = GameScript.Get().GetIOMouseOverCarLoader2().GetCarPrice();
				int buyPrice = (int)Mathf.Floor(carPrice * GameScript.Get().GetIOMouseOverCarLoader2().GetNegotationPriceMod());

				Action action = delegate ()
				{
					UIManager.Get().SetCarLoaderOnCarLocationAskWindow(GameScript.Get().GetIOMouseOverCarLoader2());
					UIManager.Get().Show("CarLocationAskWindow");
					GlobalData.AddPlayerMoney(-buyPrice);
					GlobalData.Save();
					ProfileManager.Get().SetSaveDateForCurrentProfile(string.Format("{0:yyyy-MM-dd H:mm:ss}", DateTime.Now));
					UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_SaveGame"), "M_SAVECOMPLETE", PopupType.Normal);
				};
				NewHash hash = new NewHash(new object[]
				{
						"WindowType",
						"CarBuy",
						"Type",
						"RunAction",
						"Action",
						action,
						"CarName",
						carName + carExtension,
						"Price",
						buyPrice
				});


				if (GlobalData.GetPlayerMoney() >= buyPrice)
				{
					if (CarPlaceManager.GetAmountOfFreePlacesInGarage() > 0 || !CarPlaceManager.ParkingIsFull())
					{
						UIManager.Get().ShowAskWindow(hash);
						GameMode.Get().SetCurrentMode(gameMode.UI);
					}
					else
					{
						UIManager.Get().ShowInfoWindow(string.Format(Localization.Instance.Get("GUI_CarsLimitInGarage"), GameSettings.MaxCarsAmountInGarage));
					}
				}
				else
				{
					UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
				}

			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Parking || GameScript.Get().CurrentSceneType == SceneType.Salon || GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage))
			{
				if (GameScript.Get().GetIOMouseOverCarLoader2() != null)
				{
					if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("hood").handle != null && (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)))
					{
						GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("hood");
					}
					else if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_front_left").handle != null && (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.Alpha4)))
					{
						if (!GameScript.Get().GetIOMouseOverCarLoader2().IsCarOnLifter())
						{
							GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_front_left");
						}
						else
						{
							SoundManager.Get().PlaySFX("Error");
						}
					}
					else if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_front_right").handle != null && (Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.Alpha6)))
					{
						if (!GameScript.Get().GetIOMouseOverCarLoader2().IsCarOnLifter())
						{
							GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_front_right");
						}
						else
						{
							SoundManager.Get().PlaySFX("Error");
						}
					}
					else if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_rear_left").handle != null && (Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)))
					{
						GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_rear_left");
					}
					else if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_rear_right").handle != null && (Input.GetKeyUp(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.Alpha3)))
					{
						GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_rear_right");
					}
					else if (GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("trunk").handle != null && (Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)))
					{
						GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("trunk");
					}
					else if (Input.GetKeyUp(KeyCode.Keypad5) || Input.GetKeyUp(KeyCode.Alpha5))
					{
						CarHelper.OpenCloseCar(GameScript.Get().GetIOMouseOverCarLoader2());
					}
				}

				if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
				{
					ModHelper.playerState = "running";
				}
				else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
				{
					ModHelper.playerState = "walk";
				}
				else if (Input.GetKeyDown(KeyCode.Space))
				{
					ModHelper.jump = true;
				}

				if (CarHelper.selectedItemToMount != null && GameMode.Get().GetPreviousMode() == gameMode.PartMount && GameScript.Get().GetPartMouseOver().mountWasCanceled)
				{
					Inventory.Get().Add(CarHelper.selectedItemToMount);
					CarHelper.selectedItemToMount = null;
				}

                if (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard)
                {
                    if (GameScript.Get().GetIOMouseOverCarLoader2() != null)
                    {
                        int carPrice = GameScript.Get().GetIOMouseOverCarLoader2().GetCarPrice();
                        int buyPrice = (int)Mathf.Floor(carPrice * GameScript.Get().GetIOMouseOverCarLoader2().GetNegotationPriceMod());

                        if (Singleton<UpgradeSystem>.Instance.GetUpgradeValueBool("expected_price"))
                        {
                            UIManager.Get().transform.Find("CarValueContainer/Value").GetComponent<Text>().text = Helper.MoneyToString(carPrice);
                        }
                        else
                        {
                            UIManager.Get().transform.Find("CarValueContainer/Value").GetComponent<Text>().text = Helper.MoneyToString(buyPrice);
                        }
                        UIManager.Get().transform.Find("CarValueContainer").gameObject.SetActive(true);
                    }
                    else
                    {
                        UIManager.Get().transform.Find("CarValueContainer").gameObject.SetActive(false);
                    }
                }
            }


		}
	}

}
