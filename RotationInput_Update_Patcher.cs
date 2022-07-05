using System;
using System.Collections.Generic;
using System.IO;
using Harmony12;
using UnityEngine;
using UnityEngine.UI;

namespace QuickShopEnhanced
{
	[HarmonyPatch(typeof(GameScript))]
	[HarmonyPatch("Update")]
	public class RotationInput_Update_Patcher
	{
		[HarmonyPostfix]
		private static void Postfix()
		{
			if (GameMode.Get().GetCurrentMode() != gameMode.UI && GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.B))
			{
				try
				{
					string text1 = GameScript.Get().GetPartMouseOver().GetIDWithTuned();

					if (text1 != null && Singleton<GameInventory>.Instance.GetItemProperty(text1).Price != 0)
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price != 0 && GlobalData.GetPlayerMoney() > Singleton<GameInventory>.Instance.GetItemProperty("t_" + text1).Price && Main.tunnedParts)
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

							if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6 || Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7) && Main.groupParts)
							{

								string rim = GameScript.Get().GetIOMouseOverCarLoader2().GetWheelFLHandle().GetComponentsInChildren<PartScript>(true)[0].GetIDWithTuned();
								string tir = GameScript.Get().GetIOMouseOverCarLoader2().GetWheelFLHandle().GetComponentsInChildren<PartScript>(true)[1].GetIDWithTuned();

								text1 = rim;

								List<NewInventoryItem> items = new List<NewInventoryItem>();

								NewInventoryItem newInventoryItem2 = new NewInventoryItem(rim, 1f, true);
								newInventoryItem2.extraParameters.Add("Size", tire.w_rimSize);
								newInventoryItem2.extraParameters.Add("ET", tire.w_et);
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

								GroupInventory.Get().Add(newGroupItem);

								num1 = (int)Mathf.Floor(((float)Helper.GetTirePrice(text1, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) + (float)Helper.GetRimPrice(text1.ToString(), (int)tire.w_rimSize, tire.w_et)) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								Main.mod.Logger.Log(rim + " + " + tir);

								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), string.Concat(new object[] { Singleton<GameInventory>.Instance.GetItemLocalizeName(text1), " (", tire.w_wheelWidth, "/", tire.w_tireSize, "R", tire.w_rimSize, ") ", "ET", ":", tire.w_et }) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

								return;
							}
							else if ((Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 8 || text1.Equals("amortyzatorPrzod_1") || text1.Equals("sprezynnaPrzod_1") || text1.Equals("czapkaAmorPrzod_1")) && Main.groupParts)
							{
								List<NewInventoryItem> items = new List<NewInventoryItem>();

								NewInventoryItem newInventoryItem2 = new NewInventoryItem("amortyzatorPrzod_1", 1f, itemColor, true);
								newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);

								NewInventoryItem newInventoryItem3 = new NewInventoryItem("czapkaAmorPrzod_1", 1f, itemColor, true);
								newInventoryItem3.extraParameters.Add("PaintType", PaintType.Unpainted);

								NewInventoryItem newInventoryItem4 = new NewInventoryItem("sprezynnaPrzod_1", 1f, itemColor, true);
								newInventoryItem4.extraParameters.Add("PaintType", PaintType.Unpainted);

								items.Add(newInventoryItem2);
								items.Add(newInventoryItem3);
								items.Add(newInventoryItem4);

								NewGroupItem newGroupItem = new NewGroupItem();
								newGroupItem.GroupName = "amortyzatorPrzod_1";
								newGroupItem.ItemList = new List<NewInventoryItem>();
								newGroupItem.IsNormalGroup = false;
								foreach (NewInventoryItem item2 in items)
								{
									newGroupItem.ItemList.Add(item2);
								}

								GroupInventory.Get().Add(newGroupItem);

								num1 = (int)Mathf.Floor((Singleton<GameInventory>.Instance.GetItemProperty("amortyzatorPrzod_1").Price + Singleton<GameInventory>.Instance.GetItemProperty("czapkaAmorPrzod_1").Price + Singleton<GameInventory>.Instance.GetItemProperty("sprezynnaPrzod_1").Price) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

								Main.mod.Logger.Log("amortyzatorPrzod_1 + czapkaAmorPrzod_1 + sprezynnaPrzod_1");

								UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName("amortyzatorPrzod_1") + " (Group) (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);

								GlobalData.AddPlayerMoney(-num1);
								UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);

								return;
							}
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 6)
							{
								newInventoryItem1.extraParameters.Add("Width", tire.w_wheelWidth);
								newInventoryItem1.extraParameters.Add("Profile", tire.w_tireSize);
								newInventoryItem1.extraParameters.Add("Size", tire.w_rimSize);
								num1 = (int)Mathf.Floor((float)Helper.GetTirePrice(text1, (int)tire.w_wheelWidth, (int)tire.w_tireSize, (int)tire.w_rimSize) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
							}
							else if (Singleton<GameInventory>.Instance.GetItemProperty(text1).SpecialGroup == 7)
							{
								newInventoryItem1.extraParameters.Add("Size", tire.w_rimSize);
								newInventoryItem1.extraParameters.Add("ET", tire.w_et);
								num1 = (int)Mathf.Floor((float)Helper.GetRimPrice(text1.ToString(), (int)tire.w_rimSize, tire.w_et) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
							}

						}
						else
						{
							newInventoryItem1 = new NewInventoryItem(text1, 1f, itemColor, true);
						}

						if (GlobalData.GetPlayerMoney() < num1)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
						}
						else
						{
							newInventoryItem1.extraParameters.Add("PaintType", PaintType.Unpainted);
							
							Main.mod.Logger.Log(text1);

							Inventory.Get().Add(newInventoryItem1);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text1) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);
							
							GlobalData.AddPlayerMoney(-num1);
							UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Part cost: " + Helper.MoneyToString(num1), PopupType.Buy);
						}
					}
				}
				catch (Exception)
				{
				}

				try
				{
					string text2 = GameScript.Get().GetIOMouseOverCarLoader2().GetDefaultName(GameScript.Get().GetIOMouseOverCarLoader().name);

					if (text2 != null && (Singleton<GameInventory>.Instance.GetItemProperty(text2).Price != 0 || Singleton<GameInventory>.Instance.GetItemProperty(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2).Price != 0))
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2).Price != 0)
						{
							text2 = GameScript.Get().GetIOMouseOverCarLoader2().carToLoad + "-" + text2;
						}

						NewInventoryItem newInventoryItem2;

						if (Singleton<GameInventory>.Instance.GetItemProperty(text2).IsBody)
						{
							newInventoryItem2 = new NewInventoryItem(text2, 1f, Inventory.SetColor(GlobalData.DEFAULT_ITEM_COLOR), true);
						}
						else
						{
							newInventoryItem2 = new NewInventoryItem(text2, 1f, true);
						}

						newInventoryItem2.extraParameters.Add("PaintType", PaintType.Unpainted);
						int num2 = (int)Mathf.Floor(Singleton<GameInventory>.Instance.GetItemProperty(text2).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
						
						if (GlobalData.GetPlayerMoney() < num2)
						{
							UIManager.Get().ShowInfoWindow("GUI_BrakKasy");
						}
						else
						{
							Main.mod.Logger.Log(text2);
							
							Inventory.Get().Add(newInventoryItem2);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(text2) + " (" + Helper.ConditionToString(1f) + ")", PopupType.Normal);
							
							GlobalData.AddPlayerMoney(-num2);
							UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Part cost: " + Helper.MoneyToString(num2), PopupType.Buy);
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

							Main.mod.Logger.Log(GameScript.Get().GetIOMouseOverCarLoader2().GetLicencePlateTextureName(GameScript.Get().GetIOMouseOverCarLoader().name));

							Inventory.Get().Add(newInventoryItem3);
							UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), "LicensePlate", PopupType.Normal);

							GlobalData.AddPlayerMoney(-num3);
							UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Part cost: " + Helper.MoneyToString(num3), PopupType.Buy);

							return;
						}
					}
				}
				catch (Exception)
				{
					return;
				}
			}

			if (GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.N))
			{
				string text = File.ReadAllText(Main.path);

				var groupParts = text.Split(',')[1].Split(':')[1];
				var customLPN = text.Split(',')[2].Split(':')[1];

				if (Main.tunnedParts)
				{
					File.WriteAllText(Main.path, $"tunnedParts:false,groupParts:{groupParts},customLPN:{customLPN},");

					Main.tunnedParts = false;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Tunned Parts: Off", PopupType.Normal);
				}
				else
				{
					File.WriteAllText(Main.path, $"tunnedParts:true,groupParts:{groupParts},customLPN:{customLPN},");

					Main.tunnedParts = true;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Tunned Parts: On", PopupType.Normal);
				}

			}

            if (GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.G))
            {
				string text = File.ReadAllText(Main.path);

				var tunnedParts = text.Split(',')[0].Split(':')[1];
				var customLPN = text.Split(',')[2].Split(':')[1];

				if (Main.groupParts)
				{
					File.WriteAllText(Main.path, $"tunnedParts:{tunnedParts},groupParts:false,customLPN:{customLPN},");

					Main.groupParts = false;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Group Parts: Off", PopupType.Normal);
				}
				else
				{
					File.WriteAllText(Main.path, $"tunnedParts:{tunnedParts},groupParts:true,customLPN:{customLPN},");

					Main.groupParts = true;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Group Parts: On", PopupType.Normal);
				}

			}

			if (GameScript.Get().CurrentSceneType == SceneType.Garage && Input.GetKeyUp(KeyCode.V))
			{
				string text = File.ReadAllText(Main.path);

				var tunnedParts = text.Split(',')[0].Split(':')[1];
				var groupParts = text.Split(',')[1].Split(':')[1];

				if (Main.customLPN)
				{
					File.WriteAllText(Main.path, $"tunnedParts:{tunnedParts},groupParts:{groupParts},customLPN:false,");

					Main.customLPN = false;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Custom License Plates: Off", PopupType.Normal);
				}
				else
				{
					File.WriteAllText(Main.path, $"tunnedParts:{tunnedParts},groupParts:{groupParts},customLPN:true,");

					Main.customLPN = true;
					UIManager.Get().ShowPopup("QuickShopEnhanced Mod:", "Custom License Plates: On", PopupType.Normal);
				}

			}
		}
	}
}
