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
	public class RotationInput_Update_Patcher
	{
		[HarmonyPostfix]
		private static void Postfix()
		{
			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.L))
			{
				if (GlobalData.GetPlayerMoney() < GlobalData.Cost_UseInteriorDetailingToolkit)
				{
					UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
					return;
				}
				
				Action action = delegate ()
				{
					GlobalData.AddPlayerMoney(-GlobalData.Cost_UseInteriorDetailingToolkit);
					GameScript.Get().GetIOMouseOverCarLoader2().UseInteriorDetailingToolkit();
					AchievementSystem.Get().IncrementStat(14, 1);
					GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
				};
				NewHash hash = new NewHash(new object[]
				{
						"WindowType",
						"UseTool",
						"Type",
						"RunAction",
						"CarName",
						GameScript.Get().GetIOMouseOverCarLoader2().GetName(),
						"Action",
						action,
						"Price",
						GlobalData.Cost_UseInteriorDetailingToolkit
				});
				UIManager.Get().ShowAskWindow(hash);
				GameMode.Get().SetCurrentMode(gameMode.UI);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.K))
			{
				if (GlobalData.GetPlayerMoney() < GlobalData.Cost_UseWelder)
				{
					UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
					return;
				}

				Action action = delegate ()
				{
					GlobalData.AddPlayerMoney(-GlobalData.Cost_UseWelder);
					GameScript.Get().GetIOMouseOverCarLoader2().UseWelder();
					GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
				};
				NewHash hash = new NewHash(new object[]
				{
						"WindowType",
						"UseTool",
						"Type",
						"RunAction",
						"CarName",
						GameScript.Get().GetIOMouseOverCarLoader2().GetName(),
						"Action",
						action,
						"Price",
						GlobalData.Cost_UseWelder
				});
				UIManager.Get().ShowAskWindow(hash);
				GameMode.Get().SetCurrentMode(gameMode.UI);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.O))
			{
				if (GlobalData.GetPlayerMoney() < 1000)
				{
					UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
					return;
				}

				int paintType = 0;
				Color color = GameScript.Get().GetIOMouseOverCarLoader2().GetFactoryColor();
				color.a = 1f;

				Action action = delegate ()
				{
					ScreenFader.Get().NormalFadeIn();

					GameScript.Get().GetIOMouseOverCarLoader2().SetCarColor(null, color);
					GameScript.Get().GetIOMouseOverCarLoader2().SetCarPaintType(null, (PaintType)paintType);

					SoundManager.Get().PlaySFX("PaintShopFX");

					ScreenFader.Get().NormalFadeOut();

					GlobalData.AddPlayerMoney(-1000);
					UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "The car was painted with the factory color", PopupType.Normal);
					GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
				};
				NewHash hash = new NewHash(new object[]
				{
						"WindowType",
						"PaintCar",
						"Type",
						"RunAction",
						"CarName",
						GameScript.Get().GetIOMouseOverCarLoader2().GetName(),
						"Action",
						action,
						"Price",
						1000
				});

				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("UI/NewAskWindow", typeof(GameObject)), GameObject.Find("Ask").gameObject.transform, false) as GameObject;
				UIManager.Get().SetBlocker(gameObject.transform.parent);

				string[] guiFullTxt = Localization.Instance.Get("GUI_PotwierdzenieNaprawy").Split('\n');
				string[] guiFrLiTxt = guiFullTxt[0].Trim().Split(' ');
				
				guiFrLiTxt[guiFrLiTxt.Length - 1] = Localization.Instance.Get("GUI_Paint_PaintPart").Split(' ')[0].ToLower();
				guiFullTxt[0] = string.Join(" ", guiFrLiTxt);

				gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = string.Format(string.Join("\n", guiFullTxt), (hash.GetFromKey("CarName") as string).ToUpper(), Helper.MoneyToString(Convert.ToSingle(hash.GetFromKey("Price"))));
				gameObject.GetComponent<AskWindowBehaviour>().hashtable = hash;

				SoundManager.Get().PlaySFX("AskWindow");

				GameMode.Get().SetCurrentMode(gameMode.UI);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.P))
			{
				if (GlobalData.GetPlayerMoney() < 1000)
				{
					UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
					return;
				}

				List<carPart> carParts = GameScript.Get().GetIOMouseOverCarLoader2().GetCarParts();

				int paintType = carParts.FirstOrDefault(x => x.name.Equals("body")).paintType;
				string livery = carParts.FirstOrDefault(x => x.name.Equals("body")).livery;
				Color color = GameScript.Get().GetIOMouseOverCarLoader2().GetCarColor();
				color.a = 1f;

				if (paintType == 1)
					paintType = 0;

				Action action = delegate ()
				{
					ScreenFader.Get().NormalFadeIn();

					GameScript.Get().GetIOMouseOverCarLoader2().SetCarColor(null, color);
					GameScript.Get().GetIOMouseOverCarLoader2().SetCarPaintType(null, (PaintType)paintType);

					SoundManager.Get().PlaySFX("PaintShopFX");

					ScreenFader.Get().NormalFadeOut();

					GlobalData.AddPlayerMoney(-1000);
					UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "The car was painted with the current color", PopupType.Normal);
					GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
				};
				NewHash hash = new NewHash(new object[]
				{
						"WindowType",
						"PaintCar",
						"Type",
						"RunAction",
						"CarName",
						GameScript.Get().GetIOMouseOverCarLoader2().GetName(),
						"Action",
						action,
						"Price",
						1000
				});

				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("UI/NewAskWindow", typeof(GameObject)), GameObject.Find("Ask").gameObject.transform, false) as GameObject;
				UIManager.Get().SetBlocker(gameObject.transform.parent);

				string[] guiFullTxt = Localization.Instance.Get("GUI_PotwierdzenieNaprawy").Split('\n');
				string[] guiFrLiTxt = guiFullTxt[0].Trim().Split(' ');

				guiFrLiTxt[guiFrLiTxt.Length - 1] = Localization.Instance.Get("GUI_Paint_PaintPart").Split(' ')[0].ToLower();
				guiFullTxt[0] = string.Join(" ", guiFrLiTxt);

				gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = string.Format(string.Join("\n", guiFullTxt), (hash.GetFromKey("CarName") as string).ToUpper(), Helper.MoneyToString(Convert.ToSingle(hash.GetFromKey("Price"))));
				gameObject.GetComponent<AskWindowBehaviour>().hashtable = hash;

				SoundManager.Get().PlaySFX("AskWindow");

				GameMode.Get().SetCurrentMode(gameMode.UI);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() == null && Singleton<UpgradeSystem>.Instance.GetUpgradeValue("garage_upgrade") >= 2f && Input.GetKeyUp(KeyCode.Y))
			{
				GameScript.Get().IncraseEngineStandAngle(-90f);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() == null && Singleton<UpgradeSystem>.Instance.GetUpgradeValue("garage_upgrade") >= 2f && Input.GetKeyUp(KeyCode.U))
			{
				GameScript.Get().IncraseEngineStandAngle(90f);
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && Input.GetKeyUp(KeyCode.V))
			{
				Jukebox.Get().TurnOffOrOn();
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && Input.GetKeyUp(KeyCode.N))
			{
				Jukebox.Get().NextSong();
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.J))
			{
				int total = 0;
				int partIndex = 0;
				List<string> jobParts = new List<string>();
				List<string> parts = new List<string>();
				List<NewInventoryItem> inventory = new List<NewInventoryItem>(Inventory.Get().GetItems("All"));
				List<NewGroupItem> groupInventory = new List<NewGroupItem>(GroupInventory.Get().GetGroupInventory());

				CarLoader iomouseOverCarLoader = GameScript.Get().GetIOMouseOverCarLoader2();
				PartScript[] componentsInChildren = iomouseOverCarLoader.GetRoot().GetComponentsInChildren<PartScript>();
				Job jobForCarLoader = OrderGenerator.Get().GetJobForCarLoader(CarLoaderPlaces.Get().GetCarLoaderId(GameScript.Get().GetIOMouseOverCarLoader2()));

				if (jobForCarLoader == null)
				{
					foreach (PartScript part in componentsInChildren)
                    {
                        if (part.GetCondition() != 1f || part.IsUnmounted())
                        {
							parts.Add(part.GetID());
						}

					}

					foreach (carPart part in iomouseOverCarLoader.GetCarParts())
					{
						if ((part.condition != 1f || part.unmounted) && !part.name.Equals("body") && !part.name.Equals("details"))
						{
							string text2 = GameScript.Get().GetIOMouseOverCarLoader2().GetDefaultName(part.name);

							if (!text2.Equals(""))
							{
								if (Singleton<GameInventory>.Instance.GetItemProperty(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2).Price != 0)
								{
									text2 = GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2;
								}

								parts.Add(text2);
							}
							
						}

					}
				}
				else
				{

					foreach (JobPart jobPart in jobForCarLoader.jobParts)
					{
						partIndex = 0;
						jobParts = new List<string>();

						if (jobPart.type != "Body")
						{
							using (List<string>.Enumerator enumerator = jobPart.partList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									string partNo = enumerator.Current;
									if ((jobPart._partfound[partIndex] || Main.uncheckedParts) && componentsInChildren.Any(x => x.GetGameObjectPathWithoutRoot() == partNo && (x.GetCondition() != 1f || x.IsUnmounted())))
									{
										string partID = componentsInChildren.ToList().Find(x => x.GetGameObjectPathWithoutRoot() == partNo).GetID();

										if (!parts.Any(x => x.Equals(partID)))
										{
											jobParts.Add(partID);
										}
									}
									partIndex++;
								}
							}
						}

						if (jobPart.type == "Body" && jobPart.subtype == "General")
						{
							foreach (string partName in jobPart.partList)
							{
								if ((jobPart._partfound[partIndex] || Main.uncheckedParts) && iomouseOverCarLoader.GetCarParts().Any(x => GameScript.Get().GetIOMouseOverCarLoader2().GetDefaultName(x.name).Equals(partName) && (x.condition != 1f || x.unmounted)))
								{
									carPart carPart = iomouseOverCarLoader.GetCarPart(partName);

									if (!parts.Any(x => x.Equals(iomouseOverCarLoader.carToLoad + "-" + carPart.name)))
									{
										if (carPart.name.Equals("license_plate_front"))
										{
											jobParts.Add("license_plate_front");
										}
										else if (carPart.name.Equals("license_plate_rear"))
										{
											jobParts.Add("license_plate_rear");
										}
										else
										{
											jobParts.Add(iomouseOverCarLoader.carToLoad + "-" + carPart.name);
										}
									}
								}
								partIndex++;
							}
						}
						parts.AddRange(jobParts);
					}

				}

				int dupeCount = 0;


				GameObject engine = GameScript.Get().GetIOMouseOverCarLoader2().GetEngine();
				InteractiveObject iO = engine.GetComponent<InteractiveObject>();

				List<NewInventoryItem> itemsEnCk = new List<NewInventoryItem>();
				List<NewInventoryItem> itemsEn = new List<NewInventoryItem>();

				int partEnPice = 0;

				foreach (PartScript part in iO.GetComponentsInChildren<PartScript>())
				{
					string partID = part.GetID();

					if (!(partID.Contains("gearbox") || partID.Contains("rozrusznik")))
					{
						NewInventoryItem newInventoryItem2 = new NewInventoryItem(partID + $"({itemsEnCk.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, Inventory.SetColor(Color.white), true);
						newInventoryItem2.extraParameters.Add("NormalID", partID);

						itemsEnCk.Add(newInventoryItem2);

						if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && Main.tunnedParts)
						{
							newInventoryItem2 = new NewInventoryItem(partID + $"({itemsEn.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, Inventory.SetColor(Color.white), true);
							partID = "t_" + partID;
							newInventoryItem2.extraParameters.Add("NormalID", partID);
						}

						itemsEn.Add(newInventoryItem2);

						partEnPice += (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
					}

				}

				if (itemsEnCk.Except(itemsEnCk.Where(x => x.GetNormalID().Equals("bagnet_1") || x.GetNormalID().Equals("korekOleju_1") || x.GetNormalID().Equals("korek_spustowy_1"))).All(x => parts.Any(y => x.GetNormalID().Equals(y))) && Main.groupParts)
				{
					NewGroupItem newGroupItem = new NewGroupItem();
					newGroupItem.GroupName = iO.GetID().Split('(')[0];
					newGroupItem.ItemList = new List<NewInventoryItem>();
					newGroupItem.IsNormalGroup = false;

					newGroupItem.ItemList.AddRange(itemsEn);

					if (groupInventory.Any(x => x.GroupName.Equals(iO.GetID().Split('(')[0]) && x.ItemList.All(y => y.Condition == 1f && itemsEn.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == itemsEn.Count()) && Main.invCheck)
					{
						dupeCount++;
						groupInventory.Remove(groupInventory.FirstOrDefault(x => x.GroupName.Equals(iO.GetID().Split('(')[0]) && x.ItemList.All(y => y.Condition == 1f && itemsEn.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == itemsEn.Count()));
					}
					else
					{
						if (GlobalData.GetPlayerMoney() >= total + partEnPice)
						{
							parts = parts.Where(x => !itemsEnCk.Select(p => p.GetNormalID()).Contains(x)).ToList();

							GroupInventory.Get().Add(newGroupItem);

							Main.mod.Logger.Log(iO.GetID().Split('(')[0]);

							total += partEnPice;
						}
					}
				}


				string amortyzator = GameScript.Get().GetIOMouseOverCarLoader2().GetRoot().GetComponentsInChildren<PartScript>().ToList().FirstOrDefault(part => Singleton<GameInventory>.Instance.GetItemProperty(part.GetIDWithTuned()).SpecialGroup == 8).GetIDWithTuned();

				if (parts.Any(x => x.Equals(amortyzator) || x.Equals("sprezynnaPrzod_1") || x.Equals("czapkaAmorPrzod_1")) && Main.groupParts)
				{
					var amortyzatorsList = parts.Where(x => x.Equals(amortyzator) || x.Equals("sprezynnaPrzod_1") || x.Equals("czapkaAmorPrzod_1")).ToList();
					int amortyzatorsListLength = amortyzatorsList.Count();

					for (int i = 0; i < amortyzatorsListLength; i++)
					{
						if (amortyzatorsList.Contains(amortyzator) && amortyzatorsList.Contains("sprezynnaPrzod_1") && amortyzatorsList.Contains("czapkaAmorPrzod_1"))
						{

							List<NewInventoryItem> items = new List<NewInventoryItem>();

							NewInventoryItem newInventoryItem2 = new NewInventoryItem(amortyzator, 1f, Inventory.SetColor(Color.white), true);
							newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);

							NewInventoryItem newInventoryItem3 = new NewInventoryItem("sprezynnaPrzod_1", 1f, Inventory.SetColor(Color.white), true);
							newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);

							NewInventoryItem newInventoryItem4 = new NewInventoryItem("czapkaAmorPrzod_1", 1f, Inventory.SetColor(Color.white), true);
							newInventoryItem4.extraParameters.Add("PaintType", PaintType.Unpainted);

							items.Add(newInventoryItem2);
							items.Add(newInventoryItem3);
							items.Add(newInventoryItem4);

							NewGroupItem newGroupItem = new NewGroupItem();
							newGroupItem.GroupName = amortyzator;
							newGroupItem.ItemList = new List<NewInventoryItem>();
							newGroupItem.IsNormalGroup = false;
							foreach (NewInventoryItem item2 in items)
							{
								newGroupItem.ItemList.Add(item2);
							}

							amortyzatorsList.Remove(amortyzator);
							amortyzatorsList.Remove("sprezynnaPrzod_1");
							amortyzatorsList.Remove("czapkaAmorPrzod_1");

							if (groupInventory.Any(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && Main.invCheck)
							{
								dupeCount++;
								groupInventory.Remove(groupInventory.FirstOrDefault(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f));
							}
                            else
                            {
								int partPice = (int)Mathf.Floor((Singleton<GameInventory>.Instance.GetItemProperty(amortyzator).Price + Singleton<GameInventory>.Instance.GetItemProperty("sprezynnaPrzod_1").Price + Singleton<GameInventory>.Instance.GetItemProperty("czapkaAmorPrzod_1").Price) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (GlobalData.GetPlayerMoney() >= total + partPice)
								{
									parts.Remove(amortyzator);
									parts.Remove("sprezynnaPrzod_1");
									parts.Remove("czapkaAmorPrzod_1");

									GroupInventory.Get().Add(newGroupItem);

									Main.mod.Logger.Log(amortyzator + " + sprezynnaPrzod_1 + czapkaAmorPrzod_1");

									total += partPice;
								}
							}

						}
					}
				}


				if (parts.Any(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 7) && Main.groupParts)
				{
					var wheelsList = parts.Where(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 7).ToList();
					int wheelsListLength = wheelsList.Count();

					for (int i = 0; i < wheelsListLength; i++)
					{
						if (wheelsList.Any(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 6) && wheelsList.Any(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 7))
						{
							Tire tire = GameScript.Get().GetIOMouseOverCarLoader2().GetTires()[0];
							string tir = wheelsList.FirstOrDefault(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 6);
							string rim = wheelsList.FirstOrDefault(x => Singleton<GameInventory>.Instance.GetItemProperty(x).SpecialGroup == 7);

							List<NewInventoryItem> items = new List<NewInventoryItem>();

							NewInventoryItem newInventoryItem2 = new NewInventoryItem(rim, 1f, true);
							newInventoryItem2.extraParameters.Add("ET", tire.w_et);
							newInventoryItem2.extraParameters.Add("Width", tire.w_wheelWidth);
							newInventoryItem2.extraParameters.Add("Profile", tire.w_tireSize);
							newInventoryItem2.extraParameters.Add("Size", tire.w_rimSize);
							newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);
							newInventoryItem2.extraParameters.Add("IsBalanced", true);

							NewInventoryItem newInventoryItem3 = new NewInventoryItem(tir, 1f, true);
							newInventoryItem3.extraParameters.Add("Width", tire.w_wheelWidth);
							newInventoryItem3.extraParameters.Add("Profile", tire.w_tireSize);
							newInventoryItem3.extraParameters.Add("Size", tire.w_rimSize);
							newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);

							items.Add(newInventoryItem2);
							items.Add(newInventoryItem3);

							NewGroupItem newGroupItem = new NewGroupItem();
							newGroupItem.GroupName = rim;
							newGroupItem.ItemList = new List<NewInventoryItem>();
							newGroupItem.IsNormalGroup = false;
							foreach (NewInventoryItem item2 in items)
							{
								newGroupItem.ItemList.Add(item2);
							}

							wheelsList.Remove(tir);
							wheelsList.Remove(rim);

							if (groupInventory.Any(x => x.ItemList[0].ID.Equals(rim) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize &&
									x.ItemList[1].ID.Equals(tir) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Main.invCheck)
							{
								dupeCount++;
								groupInventory.Remove(groupInventory.FirstOrDefault(x => x.ItemList[0].ID.Equals(rim) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize &&
											x.ItemList[1].ID.Equals(tir) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
							}
							else
							{
								int partPice = (int)Mathf.Floor((Helper.GetTirePrice(tir, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) + Helper.GetRimPrice(rim.ToString(), (int)tire.w_rimSize, tire.w_et)) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (GlobalData.GetPlayerMoney() >= total + partPice)
								{
									parts.Remove(tir);
									parts.Remove(rim);

									GroupInventory.Get().Add(newGroupItem);

									Main.mod.Logger.Log(rim + " + " + tir);

									total += partPice;
								}
							}

						}
					}
				}


				List<string> groupDupe = new List<string>(groupInventory.SelectMany(x => x.ItemList).Select(x => x.GetNormalID()).ToList());

				var partsListSorted = from part in parts
									  orderby Singleton<GameInventory>.Instance.GetItemProperty(part).Price descending
									  select part;

				int count = 0;

				foreach (string part in partsListSorted)
				{
					Tire tire = GameScript.Get().GetIOMouseOverCarLoader2().GetTires()[0];
					carPart parT = null;
					string partID = part;
					int partPice = 0;
					//Main.mod.Logger.Log(partID);

					NewInventoryItem newInventoryItem;

					if (partID.StartsWith("car_"))
                    {
						parT = GameScript.Get().GetIOMouseOverCarLoader2().GetCarParts().FirstOrDefault(x => x.name.Equals("body"));

						if (Main.paintParts)
						{
							newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(parT.color), true);
							newInventoryItem.extraParameters.Add("PaintType", parT.paintType);
							newInventoryItem.extraParameters.Add("Livery", parT.livery);
							newInventoryItem.extraParameters.Add("LiveryStrength", parT.liveryStrength);
							partPice += 100;
						}
						else
						{
							newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(GlobalData.DEFAULT_ITEM_COLOR), true);
							newInventoryItem.extraParameters.Add("PaintType", PaintType.Unpainted);
						}

						partPice += (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
					}
					else if (partID.Equals("license_plate_front") || partID.Equals("license_plate_rear"))
					{
						newInventoryItem = new NewInventoryItem("LicensePlate", 1f, Inventory.SetColor(Color.white), true);
						newInventoryItem.extraParameters.Add("LPName", GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(partID));

						partPice = (int)Mathf.Floor(100f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

						if (Main.customLPN)
						{
							newInventoryItem.extraParameters.Add("CustomLPN", GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart(partID).handle.GetComponentInChildren<Text>().text);
							partPice = (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
						}
					}
					else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 6)
                    {
						newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);
						newInventoryItem.extraParameters.Add("Width", tire.w_wheelWidth);
						newInventoryItem.extraParameters.Add("Profile", tire.w_tireSize);
						newInventoryItem.extraParameters.Add("Size", tire.w_rimSize);

						partPice = (int)Mathf.Floor(Helper.GetTirePrice(partID, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
					}
					else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 7)
					{
						newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);
						newInventoryItem.extraParameters.Add("ET", tire.w_et);
						newInventoryItem.extraParameters.Add("Width", tire.w_wheelWidth);
						newInventoryItem.extraParameters.Add("Profile", tire.w_tireSize);
						newInventoryItem.extraParameters.Add("Size", tire.w_rimSize);

						partPice = (int)Mathf.Floor(Helper.GetRimPrice(partID, (int)tire.w_rimSize, tire.w_et) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
					}
					else
                    {
						if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && GlobalData.GetPlayerMoney() >= ((Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) + total) && Main.tunnedParts)
						{
							partID = "t_" + partID;
						}

						newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);

						partPice = (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
					}

					if (!partID.StartsWith("car_"))
						newInventoryItem.extraParameters.Add("PaintType", PaintType.Unpainted);

					if (GlobalData.GetPlayerMoney() >= total + partPice)
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 6
							&& inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
							&& Main.invCheck)
						{
							dupeCount++;
							inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
						}
						else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 7
							&& inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
							&& Main.invCheck)
						{
							dupeCount++;
							inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
						}
						else if ((partID.Equals("license_plate_front") || partID.Equals("license_plate_rear")) && inventory.Any(x => x.ID.Equals("LicensePlate") && x.Condition == 1f && Convert.ToString(x.extraParameters.GetHashTable()["LPName"]).Equals(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(partID))) && Main.invCheck)
						{
							dupeCount++;
							inventory.Remove(inventory.FirstOrDefault(x => x.ID.Equals("LicensePlate") && Convert.ToString(x.extraParameters.GetHashTable()["LPName"]).Equals(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(partID))));
						}
						else if (parT != null) {
							if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(parT.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(parT.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(parT.liveryStrength)) && Main.paintParts && Main.invCheck)
							{
								dupeCount++;
								inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(parT.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(parT.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(parT.liveryStrength)));
							}
							else if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && x.GetItemColor() == GlobalData.DEFAULT_ITEM_COLOR) && Main.invCheck && !Main.paintParts)
							{
								dupeCount++;
								inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && x.GetItemColor() == GlobalData.DEFAULT_ITEM_COLOR));
                            }
                            else
                            {
								Main.mod.Logger.Log(partID);
								Inventory.Get().Add(newInventoryItem);
								total += partPice;

								count++;
							}
						}
						else if (groupInventory.Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && groupDupe.Contains(partID) && Main.invCheck)
						{
							dupeCount++;
							groupDupe.Remove(groupInventory.FirstOrDefault(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)).ItemList.FirstOrDefault(x => partID.Equals(x.GetNormalID()) && x.Condition == 1f).GetNormalID());
						}
						else if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f) && Main.invCheck)
						{
							dupeCount++;
							inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f));
						}
						else if(!partID.Equals("#Dummy"))
						{
							Main.mod.Logger.Log(partID);
							Inventory.Get().Add(newInventoryItem);
							total += partPice;

							count++;
						}

					}

				}

				if (dupeCount != 0 && dupeCount == partsListSorted.Count() && Main.invCheck)
				{
					UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"All parts were already in the inventory", PopupType.Normal);
				}
				else if (total == 0)
                {
					UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "No parts were buyed", PopupType.Normal);
                }
                else
                {
					if (dupeCount != 0 && Main.invCheck)
					{
						UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"{dupeCount} {(dupeCount == 1 ? "part" : "parts")} were already in the inventory", PopupType.Normal);
					}
					else if (count == partsListSorted.Count())
                    {
						UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "All discovered parts were buyed", PopupType.Normal);
                    }
                    else
                    {
						UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"{partsListSorted.Count() - count} {(partsListSorted.Count() - count == 1 ? "part" : "parts")} weren't buyed", PopupType.Normal);
					}

					GlobalData.AddPlayerMoney(-total);
					UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "All parts cost: " + Helper.MoneyToString(total), PopupType.Buy);
				}

				return;
			}

			

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.B))
			{
				try
				{
					string text1 = GameScript.Get().GetPartMouseOver().GetIDWithTuned();

					if (text1 != null && Singleton<GameInventory>.Instance.GetItemProperty(text1).Price != 0)
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price != 0 && GlobalData.GetPlayerMoney() >= (Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) && Main.tunnedParts)
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

							if (Singleton<GameInventory>.Instance.GetItemProperty(text1).ShopGroup.Equals("Engine") && Main.groupParts)
							{
								GameObject engine = GameScript.Get().GetIOMouseOverCarLoader2().GetEngine();
								InteractiveObject iO = engine.GetComponent<InteractiveObject>();

								List<NewInventoryItem> items = new List<NewInventoryItem>();

								NewGroupItem newGroupItem = new NewGroupItem();
								newGroupItem.GroupName = iO.GetID().Split('(')[0];
								newGroupItem.ItemList = new List<NewInventoryItem>();
								newGroupItem.IsNormalGroup = false;
								
								int total = 0;

								foreach (PartScript part in iO.GetComponentsInChildren<PartScript>())
								{
									string partID = part.GetID();

									if (!(partID.Contains("gearbox") || partID.Contains("rozrusznik")))
									{
										NewInventoryItem newInventoryItem2 = new NewInventoryItem(partID + $"({items.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, itemColor, true);
										newInventoryItem2.extraParameters.Add("NormalID", partID);

										if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && Main.tunnedParts)
										{
											newInventoryItem2 = new NewInventoryItem(partID + $"({items.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, itemColor, true);
											partID = "t_" + partID;
											newInventoryItem2.extraParameters.Add("NormalID", partID);
										}

										items.Add(newInventoryItem2);
										total += (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
									}
									
								}

								newGroupItem.ItemList.AddRange(items);

								if (GlobalData.GetPlayerMoney() >= total)
								{

									if (GroupInventory.Get().GetGroupInventory().Any(x => x.GroupName.Equals(iO.GetID().Split('(')[0]) && x.ItemList.All(y => y.Condition == 1f && items.FirstOrDefault(z => z.GetNormalID().Equals(y.GetNormalID())) != null) && x.ItemList.Count() == items.Count()) && Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
									{
										Main.dupeBool = true;
										Main.dupeText = iO.GetID().Split('(')[0];
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
										return;
									}

									Main.mod.Logger.Log(iO.GetID().Split('(')[0]);
									Main.dupeBool = false;
									Main.dupeText = iO.GetID().Split('(')[0];

									GroupInventory.Get().Add(newGroupItem);
									UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(iO.GetID().Split('(')[0]) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

									GlobalData.AddPlayerMoney(-total);
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(total), PopupType.Buy);

									return;
								}
							}
							else if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7) && Main.groupParts)
							{

								string rim = GameScript.Get().GetIOMouseOverCarLoader2().GetWheelFLHandle().GetComponentsInChildren<PartScript>(true)[0].GetIDWithTuned();
								string tir = GameScript.Get().GetIOMouseOverCarLoader2().GetWheelFLHandle().GetComponentsInChildren<PartScript>(true)[1].GetIDWithTuned();

								List<NewInventoryItem> items = new List<NewInventoryItem>();

								NewInventoryItem newInventoryItem2 = new NewInventoryItem(rim, 1f, true);
								newInventoryItem2.extraParameters.Add("ET", tire.w_et);
								newInventoryItem2.extraParameters.Add("Width", tire.w_wheelWidth);
								newInventoryItem2.extraParameters.Add("Profile", tire.w_tireSize);
								newInventoryItem2.extraParameters.Add("Size", tire.w_rimSize);
								newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);
								newInventoryItem2.extraParameters.Add("IsBalanced", true);

								NewInventoryItem newInventoryItem3 = new NewInventoryItem(tir, 1f, true);
								newInventoryItem3.extraParameters.Add("Width", tire.w_wheelWidth);
								newInventoryItem3.extraParameters.Add("Profile", tire.w_tireSize);
								newInventoryItem3.extraParameters.Add("Size", tire.w_rimSize);
								newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);

								items.Add(newInventoryItem2);
								items.Add(newInventoryItem3);

								NewGroupItem newGroupItem = new NewGroupItem();
								newGroupItem.GroupName = rim;
								newGroupItem.ItemList = new List<NewInventoryItem>();
								newGroupItem.IsNormalGroup = false;
								
								newGroupItem.ItemList.AddRange(items);

								num1 = (int)Mathf.Floor((Helper.GetTirePrice(tir, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) + Helper.GetRimPrice(rim.ToString(), (int)tire.w_rimSize, tire.w_et)) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (GlobalData.GetPlayerMoney() >= num1)
								{

									if(GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(rim) && x.ItemList[0].Condition == 1f && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[0].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize && 
									x.ItemList[1].ID.Equals(tir) && x.ItemList[1].Condition == 1f && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.ItemList[1].extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
                                    {
										Main.dupeBool = true;
										Main.dupeText = text1;
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
										return;
									}

									Main.mod.Logger.Log(rim + " + " + tir);
									Main.dupeBool = false;
									Main.dupeText = text1;

									GroupInventory.Get().Add(newGroupItem);
									UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), string.Concat(new object[] { Singleton<GameInventory>.Instance.GetItemLocalizeName(rim), " (", tire.w_wheelWidth, "/", tire.w_tireSize, "R", tire.w_rimSize, ") ", "ET", ":", tire.w_et }) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

									GlobalData.AddPlayerMoney(-num1);
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

									return;
								}
							}
							else if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 8 || text1.Equals("sprezynnaPrzod_1") || text1.Equals("czapkaAmorPrzod_1")) && Main.groupParts)
							{
								string amortyzator = GameScript.Get().GetIOMouseOverCarLoader2().GetRoot().GetComponentsInChildren<PartScript>().ToList().FirstOrDefault(part => Singleton<GameInventory>.Instance.GetItemProperty(part.GetIDWithTuned()).SpecialGroup == 8).GetIDWithTuned();

								if (text1.Equals(amortyzator) || text1.Equals("sprezynnaPrzod_1") || text1.Equals("czapkaAmorPrzod_1"))
								{
									List<NewInventoryItem> items = new List<NewInventoryItem>();

									NewInventoryItem newInventoryItem2 = new NewInventoryItem(amortyzator, 1f, itemColor, true);
									newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);

									NewInventoryItem newInventoryItem3 = new NewInventoryItem("sprezynnaPrzod_1", 1f, itemColor, true);
									newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);

									NewInventoryItem newInventoryItem4 = new NewInventoryItem("czapkaAmorPrzod_1", 1f, itemColor, true);
									newInventoryItem4.extraParameters.Add("PaintType", PaintType.Unpainted);

									items.Add(newInventoryItem2);
									items.Add(newInventoryItem3);
									items.Add(newInventoryItem4);

									NewGroupItem newGroupItem = new NewGroupItem();
									newGroupItem.GroupName = amortyzator;
									newGroupItem.ItemList = new List<NewInventoryItem>();
									newGroupItem.IsNormalGroup = false;
									
									newGroupItem.ItemList.AddRange(items);

									num1 = (int)Mathf.Floor((Singleton<GameInventory>.Instance.GetItemProperty(amortyzator).Price + Singleton<GameInventory>.Instance.GetItemProperty("sprezynnaPrzod_1").Price + Singleton<GameInventory>.Instance.GetItemProperty("czapkaAmorPrzod_1").Price) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

									if (GlobalData.GetPlayerMoney() >= num1)
									{
										if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(amortyzator) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
										{
											Main.dupeBool = true;
											Main.dupeText = text1;
											UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
											return;
										}

										Main.mod.Logger.Log(amortyzator + " + sprezynnaPrzod_1 + czapkaAmorPrzod_1");
										Main.dupeBool = false;
										Main.dupeText = text1;

										GroupInventory.Get().Add(newGroupItem);
										UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(amortyzator) + " (Group) (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

										GlobalData.AddPlayerMoney(-num1);
										UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

										return;
									}
								}
							}
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6)
							{
								newInventoryItem1.extraParameters.Add("Width", tire.w_wheelWidth);
								newInventoryItem1.extraParameters.Add("Profile", tire.w_tireSize);
								newInventoryItem1.extraParameters.Add("Size", tire.w_rimSize);
								num1 = (int)Mathf.Floor(Helper.GetTirePrice(text1, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
							}
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7)
							{
								newInventoryItem1.extraParameters.Add("ET", tire.w_et);
								newInventoryItem1.extraParameters.Add("Width", tire.w_wheelWidth);
								newInventoryItem1.extraParameters.Add("Profile", tire.w_tireSize);
								newInventoryItem1.extraParameters.Add("Size", tire.w_rimSize);
								num1 = (int)Mathf.Floor(Helper.GetRimPrice(text1.ToString(), (int)tire.w_rimSize, tire.w_et) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								if (Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
							}

						}
						else
						{
							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
						}

						if (GlobalData.GetPlayerMoney() < num1)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							return;
						}
						else
						{
							if (GameScript.Get().GetIOMouseOverCarLoader2() != null && Main.invCheck)
							{
								Tire tire = GameScript.Get().GetIOMouseOverCarLoader2().GetTires()[0];

								if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6 
									&& Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
								else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7
									&& Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
									&& (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
								else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => text1.Equals(y.GetNormalID()) && y.Condition == 1f)) && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
								else if (Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f) && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
								{
									Main.dupeBool = true;
									Main.dupeText = text1;
									UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
									return;
								}
							}
							else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => text1.Equals(y.GetNormalID()) && y.Condition == 1f)) && Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
							{
								Main.dupeBool = true;
								Main.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}
							else if (Inventory.Get().GetItems("All").Any(x => text1.Equals(x.ID) && x.Condition == 1f) && Main.invCheck && (!Main.dupeBool || !text1.Equals(Main.dupeText)))
							{
								Main.dupeBool = true;
								Main.dupeText = text1;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);
							
							Main.mod.Logger.Log(text1);
							Main.dupeBool = false;
							Main.dupeText = text1;

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
							
							if (Main.paintParts)
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

						num2 += (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(text2).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
						
						if (GlobalData.GetPlayerMoney() < num2)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
							return;
						}
						else
						{
							if (Inventory.Get().GetItems("All").Any(x => text2.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(part.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(part.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(part.liveryStrength)) && Main.paintParts && Main.invCheck && (!Main.dupeBool || !text2.Equals(Main.dupeText)))
							{
								Main.dupeBool = true;
								Main.dupeText = text2;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}
							else if (Inventory.Get().GetItems("All").Any(x => text2.Equals(x.ID) && x.Condition == 1f) && Main.invCheck && (!Main.dupeBool || !text2.Equals(Main.dupeText)))
							{
								Main.dupeBool = true;
								Main.dupeText = text2;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							Main.mod.Logger.Log(text2);
							Main.dupeBool = false;
							Main.dupeText = text2;

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
							if (GlobalData.GetPlayerMoney() < (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) && Main.customLPN)
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

							if (Main.customLPN)
							{
								newInventoryItem3.extraParameters.Add("CustomLPN", GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart(GameScript.Get().GetIOMouseOverCarLoader().name).handle.GetComponentInChildren<Text>().text);
								num3 = (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
							}

							if (Inventory.Get().GetItems("All").Any(x => x.ID.Equals("LicensePlate") && x.Condition == 1f) && Main.invCheck && (!Main.dupeBool || !GameScript.Get().GetIOMouseOverCarLoader().name.Equals(Main.dupeText)))
							{
								Main.dupeBool = true;
								Main.dupeText = GameScript.Get().GetIOMouseOverCarLoader().name;
								UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "You already have this part, if you still wanna buy it press 'B' again", PopupType.Normal);
								return;
							}

							Main.mod.Logger.Log(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(GameScript.Get().GetIOMouseOverCarLoader().name));
							Main.dupeBool = false;
							Main.dupeText = GameScript.Get().GetIOMouseOverCarLoader().name;

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
			else if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard) && GameScript.Get().GetIOMouseOverCarLoader2() != null && Input.GetKeyUp(KeyCode.B))
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

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("hood").handle != null && (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)))
			{
				GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("hood");
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_front_left").handle != null && (Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.Alpha4)))
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

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_front_right").handle != null && (Input.GetKeyUp(KeyCode.Keypad6) || Input.GetKeyUp(KeyCode.Alpha6)))
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

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_rear_left").handle != null && (Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)))
			{
				GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_rear_left");
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("door_rear_right").handle != null && (Input.GetKeyUp(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.Alpha3)))
			{
				GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("door_rear_right");
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && GameScript.Get().GetIOMouseOverCarLoader2().GetCarPart("trunk").handle != null && (Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)))
			{
				GameScript.Get().GetIOMouseOverCarLoader2().SwitchCarPart("trunk");
			}

			if (GameMode.Get().GetCurrentMode() != gameMode.UI && (GameScript.Get().CurrentSceneType == SceneType.Shed || GameScript.Get().CurrentSceneType == SceneType.Junkyard || GameScript.Get().CurrentSceneType == SceneType.Garage) && GameScript.Get().GetIOMouseOverCarLoader2() != null && (Input.GetKeyUp(KeyCode.Keypad5) || Input.GetKeyUp(KeyCode.Alpha5)))
			{
				var carloader = GameScript.Get().GetIOMouseOverCarLoader2();
				List<carPart> list;

				if (carloader.IsCarOnLifter())
                {
					list = new List<carPart>() { carloader.GetCarPart("hood"), carloader.GetCarPart("door_rear_left"), carloader.GetCarPart("door_rear_right"), carloader.GetCarPart("trunk") };
                }
                else
                {
					list = new List<carPart>() { carloader.GetCarPart("hood"), carloader.GetCarPart("door_front_left"), carloader.GetCarPart("door_front_right"), carloader.GetCarPart("door_rear_left"), carloader.GetCarPart("door_rear_right"), carloader.GetCarPart("trunk") };
				}

				for (int i = 0; i < list.Count(); i++)
                {
					if (list[i].handle != null)
					{
						if (LeanTween.isTweening(list[i].handle))
						{
							return;
						}
                    }
                    else
                    {
						list.RemoveAt(i);
						i--;
                    }
				}

				Main.mod.Logger.Log("Open or close car");

				if (list.Select(x => x.switched).Distinct().Skip(1).Any())
                {
					bool mostSwitch = list.GroupBy(i => i.switched).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
					foreach (var item in list)
					{
						Main.mod.Logger.Log("" + item.switched);
						Main.mod.Logger.Log(item.name);
					}
					foreach (var item in list)
					{
						if (item.handle != null && item.switched != mostSwitch)
						{
							carloader.SwitchCarPart(item.name);
						}
					}
				}
                else
                {
					foreach (var item in list)
					{
						if (item.handle != null)
						{
							carloader.SwitchCarPart(item.name);
						}
					}
				}
			}

		}
	}
}
