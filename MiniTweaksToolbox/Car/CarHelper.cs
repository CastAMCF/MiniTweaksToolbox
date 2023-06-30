using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ModKit.UI;

namespace MiniTweaksToolbox
{
	internal static class CarHelper
	{
		public static object GetAllBuyableParts(CarLoader carLoader)
		{
			int partIndex = 0;
			List<string> jobParts = new List<string>();
			List<string> parts = new List<string>();
			List<int> tiresInstance = new List<int>();
			List<int> rimsInstance = new List<int>();
			PartScript[] componentsInChildren = carLoader.GetRoot().GetComponentsInChildren<PartScript>();
			Job jobForCarLoader = OrderGenerator.Get().GetJobForCarLoader(CarLoaderPlaces.Get().GetCarLoaderId(carLoader));

			if (jobForCarLoader == null)
			{
				foreach (PartScript part in componentsInChildren)
				{
					if (part.GetCondition() != 1f || part.IsUnmounted())
					{
						if (Singleton<GameInventory>.Instance.GetItemProperty(part.GetID()).SpecialGroup == 6)
						{
							tiresInstance.Add(part.GetInstanceID());
						}
						else if (Singleton<GameInventory>.Instance.GetItemProperty(part.GetID()).SpecialGroup == 7)
						{
							rimsInstance.Add(part.GetInstanceID());
						}

						parts.Add(part.GetID());
					}

				}

				foreach (carPart part in carLoader.GetCarParts())
				{
					if ((part.condition != 1f || part.unmounted) && !part.name.Equals("body") && !part.name.Equals("details"))
					{
						string text2 = carLoader.GetDefaultName(part.name);

						if (!text2.Equals(""))
						{
							if (Singleton<GameInventory>.Instance.GetItemProperty(carLoader.carToLoad + "-" + text2).Price != 0)
							{
								text2 = carLoader.carToLoad + "-" + text2;
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
								if ((jobPart._partfound[partIndex] || Main.Settings.uncheckedParts) && componentsInChildren.Any(x => x.GetGameObjectPathWithoutRoot() == partNo && (x.GetCondition() < jobForCarLoader.globalCondition || x.IsUnmounted())))
								{
									PartScript part = componentsInChildren.ToList().Find(x => x.GetGameObjectPathWithoutRoot() == partNo);

									if (Singleton<GameInventory>.Instance.GetItemProperty(part.GetID()).SpecialGroup == 6)
									{
										tiresInstance.Add(part.GetInstanceID());
									}
									else if (Singleton<GameInventory>.Instance.GetItemProperty(part.GetID()).SpecialGroup == 7)
									{
										rimsInstance.Add(part.GetInstanceID());
									}

									if (!parts.Any(x => x.Equals(part.GetID())))
									{
										jobParts.Add(part.GetID());
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
							if ((jobPart._partfound[partIndex] || Main.Settings.uncheckedParts) && carLoader.GetCarParts().Any(x => carLoader.GetDefaultName(x.name).Equals(partName) && (x.condition < jobForCarLoader.globalCondition || x.unmounted)))
							{
								carPart carPart = carLoader.GetCarPart(partName);

								if (!parts.Any(x => x.Equals(carLoader.carToLoad + "-" + carPart.name)))
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
										jobParts.Add(carLoader.carToLoad + "-" + carPart.name);
									}
								}
							}
							partIndex++;
						}
					}
					parts.AddRange(jobParts);
				}

			}

			return new { parts = parts, tiresInstance = tiresInstance, rimsInstance = rimsInstance };
		}


        public static List<string> GetAllBuyablePartsEngine()
        {
            List<string> parts = new List<string>();

            Component[] componentsInChildren = GameScript.Get().GetEngineStand().GetComponentsInChildren<PartScript>();
            foreach (PartScript partScript in componentsInChildren)
            {
				string partName = partScript.gameObject.name.Split('(')[0];

                if ((partScript.GetCondition() != 1f || partScript.IsUnmounted()) && 
					!partName.Contains("gearbox") && !partName.Contains("rozrusznik") && !partName.Contains("kolektor"))
                {
                    parts.Add(partName);
                }
            }

            return parts;
        }


        public static object EngineParts(CarLoader carLoader)
		{
			GameObject engine = carLoader.GetEngine();
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
					NewInventoryItem newInventoryItem2 = new NewInventoryItem(partID + $"({items.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, Inventory.SetColor(Color.white), true);
					newInventoryItem2.extraParameters.Add("NormalID", partID);

					if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && Main.Settings.tunnedParts)
					{
						newInventoryItem2 = new NewInventoryItem(partID + $"({items.Where(x => partID.Equals(x.ID.Split('(')[0])).Count()})", 1f, Inventory.SetColor(Color.white), true);
						partID = "t_" + partID;
						newInventoryItem2.extraParameters.Add("NormalID", partID);
					}

					if (Main.Settings.itemQuality)
					{
						newInventoryItem2.extraParameters.Add("Quality", Main.Settings.quality);
					}

					items.Add(newInventoryItem2);
					total += (int)Mathf.Floor(Helper.GetPrice(newInventoryItem2) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
				}

			}

			newGroupItem.ItemList.AddRange(items);

			return new { newGroupItem = newGroupItem, total = total, items = items };
		}

		public static object AmortyzatorGroup(List<string> amortyzatorParts)
		{
			List<NewInventoryItem> items = new List<NewInventoryItem>();
			int total = 0;

            foreach (string part in amortyzatorParts)
            {
                NewInventoryItem newInventoryItem = new NewInventoryItem(part, 1f, Inventory.SetColor(Color.white), true);
                newInventoryItem.extraParameters.Add("PaintType", PaintType.Unpainted);
                if (Main.Settings.itemQuality)
                {
                    newInventoryItem.extraParameters.Add("Quality", Main.Settings.quality);
                }

                total += (int)Mathf.Floor(Helper.GetPrice(newInventoryItem) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

                items.Add(newInventoryItem);
            }

			NewGroupItem newGroupItem = new NewGroupItem();
			newGroupItem.GroupName = amortyzatorParts[0];
			newGroupItem.ItemList = items;
			newGroupItem.IsNormalGroup = false;

			return new { newGroupItem = newGroupItem, total = total };
		}

		public static object CreateWheel(string type, CarLoader carLoader, int instance)
		{
			Tire tire = carLoader.GetTires()[0];
			List<string> susps = new List<string>() { "FLSusp/", "FRSusp/", "RLSusp/", "RRSusp/" };

			for (int i = 0; i < 4; i++)
			{
				if (carLoader.GetRoot().transform.Find(susps[i] + carLoader.GetTires()[i].w_rim).GetComponent<PartScript>().GetInstanceID() == instance || carLoader.GetRoot().transform.Find(susps[i] + carLoader.GetTires()[i].w_rim).transform.Find("tire").GetComponent<PartScript>().GetInstanceID() == instance)
				{
					tire = carLoader.GetTires()[i];
					break;
				}
			}

			NewInventoryItem newInventoryItem;

			if (type.Equals("rim"))
			{
				newInventoryItem = new NewInventoryItem(tire.w_rim, 1f, true);
				newInventoryItem.extraParameters.Add("ET", tire.w_et);
			}
			else
			{
				newInventoryItem = new NewInventoryItem(tire.w_tire, 1f, true);
			}

			newInventoryItem.extraParameters.Add("Width", tire.w_wheelWidth);
			newInventoryItem.extraParameters.Add("Profile", tire.w_tireSize);
			newInventoryItem.extraParameters.Add("Size", tire.w_rimSize);
			newInventoryItem.extraParameters.Add("PaintType", PaintType.Unpainted);

			if (Main.Settings.itemQuality)
			{
				newInventoryItem.extraParameters.Add("Quality", Main.Settings.quality);
			}

			return new { rimName = tire.w_rim, tireName = tire.w_tire, newInventoryItem = newInventoryItem, tire = tire };
		}

		public static object CreateWheelGroup(CarLoader carLoader, int instance)
		{
			List<NewInventoryItem> items = new List<NewInventoryItem>();

			object rimItem = CreateWheel("rim", carLoader, instance);
			NewInventoryItem newInventoryItem2 = (NewInventoryItem)rimItem.GetType().GetProperty("newInventoryItem").GetValue(rimItem, null);
			newInventoryItem2.extraParameters.Add("IsBalanced", true);
			if (Main.Settings.itemQuality)
			{
				newInventoryItem2.extraParameters.Add("Quality", Main.Settings.quality);
			}

			object tireItem = CreateWheel("tire", carLoader, instance);
			NewInventoryItem newInventoryItem3 = (NewInventoryItem)tireItem.GetType().GetProperty("newInventoryItem").GetValue(tireItem, null);
			if (Main.Settings.itemQuality)
			{
				newInventoryItem3.extraParameters.Add("Quality", Main.Settings.quality);
			}

			items.Add(newInventoryItem2);
			items.Add(newInventoryItem3);

			NewGroupItem newGroupItem = new NewGroupItem();
			newGroupItem.GroupName = (string)rimItem.GetType().GetProperty("rimName").GetValue(rimItem, null);
			newGroupItem.ItemList = new List<NewInventoryItem>();
			newGroupItem.IsNormalGroup = false;

			newGroupItem.ItemList.AddRange(items);

			string rimName = (string)rimItem.GetType().GetProperty("rimName").GetValue(rimItem, null);
			string tireName = (string)tireItem.GetType().GetProperty("tireName").GetValue(tireItem, null);
			Tire tire = (Tire)rimItem.GetType().GetProperty("tire").GetValue(rimItem, null);

			return new { rimName = rimName, tireName = tireName, rimItem = newInventoryItem2, tireItem = newInventoryItem3, newGroupItem = newGroupItem, tire = tire };
		}


		public static IEnumerator AddManyParts(CarLoader carLoader, List<string> partsListSorted)
		{
			int count = 0;
			List<string> groupDupe = new List<string>(groupInventory.SelectMany(x => x.ItemList).Select(x => x.GetNormalID()).ToList());

			for (int i = 0; i < partsListSorted.Count(); i++)
			{
				Tire tire = carLoader.GetTires()[0];
				carPart parT = null;
				string partID = partsListSorted[i];
				int partPice = 0;
                //Main.mod.Logger.Log(partID);

                if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && GlobalData.GetPlayerMoney() >= ((Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) + total) && Main.Settings.tunnedParts)
                    partID = "t_" + partID;

                NewInventoryItem newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);

                switch (partID)
                {
                    case "license_plate_front":
                    case "license_plate_rear":

                        newInventoryItem = new NewInventoryItem("LicensePlate", 1f, Inventory.SetColor(Color.white), true);
                        newInventoryItem.extraParameters.Add("LPName", carLoader.GetLicencePlateTextureName(partID));

                        partPice = (int)Mathf.Floor(100f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));

                        if (Main.Settings.customLPN)
                        {
                            newInventoryItem.extraParameters.Add("CustomLPN", carLoader.GetCarPart(partID).handle.GetComponentInChildren<Text>().text);
                            partPice = (int)Mathf.Floor(1000f * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
                        }

                        break;
                    case string s when s.StartsWith("car_"):

                        parT = carLoader.GetCarParts().FirstOrDefault(x => x.name.Equals("body"));

                        if (Main.Settings.paintParts)
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

                        break;
                    default:
                        break;
                }

                switch (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup)
                {
                    case 6:

                        object tireItem = CreateWheel("tire", carLoader, tiresInstance.First());
                        newInventoryItem = (NewInventoryItem)tireItem.GetType().GetProperty("newInventoryItem").GetValue(tireItem, null);

                        tire = (Tire)tireItem.GetType().GetProperty("tire").GetValue(tireItem, null);

                        tiresInstance.RemoveAt(0);

                        break;
                    case 7:

                        object rimItem = CreateWheel("rim", carLoader, rimsInstance.First());
                        newInventoryItem = (NewInventoryItem)rimItem.GetType().GetProperty("newInventoryItem").GetValue(rimItem, null);

                        tire = (Tire)rimItem.GetType().GetProperty("tire").GetValue(rimItem, null);

                        rimsInstance.RemoveAt(0);

                        break;
                    default:
						break;
                }

				if (Main.Settings.itemQuality)
				{
					newInventoryItem.extraParameters.Add("Quality", Main.Settings.quality);
				}

				partPice += (int)Mathf.Floor(Helper.GetPrice(newInventoryItem) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
				

				if (!partID.StartsWith("car_"))
					newInventoryItem.extraParameters.Add("PaintType", PaintType.Unpainted);

				if (GlobalData.GetPlayerMoney() >= total + partPice)
				{
					if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 6
						&& inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
						&& Main.Settings.invCheck)
					{
						dupeCount++;
						inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
					}
					else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 7
						&& inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
						&& Main.Settings.invCheck)
					{
						dupeCount++;
						inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize));
					}
					else if ((partID.Equals("license_plate_front") || partID.Equals("license_plate_rear")) && inventory.Any(x => x.ID.Equals("LicensePlate") && x.Condition == 1f && Convert.ToString(x.extraParameters.GetHashTable()["LPName"]).Equals(carLoader.GetLicencePlateTextureName(partID))) && Main.Settings.invCheck)
					{
						dupeCount++;
						inventory.Remove(inventory.FirstOrDefault(x => x.ID.Equals("LicensePlate") && Convert.ToString(x.extraParameters.GetHashTable()["LPName"]).Equals(carLoader.GetLicencePlateTextureName(partID))));
					}
					else if (parT != null)
					{
						if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(parT.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(parT.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(parT.liveryStrength)) && Main.Settings.paintParts && Main.Settings.invCheck)
						{
							dupeCount++;
							inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f && x.extraParameters.GetHashTable()["PaintType"].Equals(parT.paintType) && x.extraParameters.GetHashTable()["Livery"].Equals(parT.livery) && x.extraParameters.GetHashTable()["LiveryStrength"].Equals(parT.liveryStrength)));
						}
						else if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f && x.GetItemColor() == GlobalData.DEFAULT_ITEM_COLOR) && Main.Settings.invCheck && !Main.Settings.paintParts)
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
					else if (groupInventory.Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && groupDupe.Contains(partID) && Main.Settings.invCheck)
					{
						dupeCount++;
						groupDupe.Remove(groupInventory.FirstOrDefault(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)).ItemList.FirstOrDefault(x => partID.Equals(x.GetNormalID()) && x.Condition == 1f).GetNormalID());
					}
					else if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f) && Main.Settings.invCheck)
					{
						dupeCount++;
						inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f));
					}
					else if (partID.Equals("#Dummy"))
					{
						count++;
					}
					else
					{
						Main.mod.Logger.Log(partID);
						Inventory.Get().Add(newInventoryItem);
						total += partPice;

						count++;
					}

				}

				yield return new WaitForEndOfFrame();
			}

			if (dupeCount != 0 && dupeCount == partsListSorted.Count() && Main.Settings.invCheck)
			{
				UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"All parts were already in the inventory", PopupType.Normal);
			}
			else if (total == 0)
			{
				UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "No parts were buyed", PopupType.Normal);
			}
			else
			{
				if (dupeCount != 0 && Main.Settings.invCheck)
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

			yield break;
		}


		public static IEnumerator AddManyPartsEngine(List<string> partsListSorted)
		{
			int count = 0;

			for (int i = 0; i < partsListSorted.Count(); i++)
			{
				string partID = partsListSorted[i];
				//Main.mod.Logger.Log(partID);

				NewInventoryItem newInventoryItem;

				if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && GlobalData.GetPlayerMoney() >= ((Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount")) + total) && Main.Settings.tunnedParts)
					partID = "t_" + partID;

				newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);


				if (Main.Settings.itemQuality)
				{
					newInventoryItem.extraParameters.Add("Quality", Main.Settings.quality);
				}

                int partPice = (int)Mathf.Floor(Helper.GetPrice(newInventoryItem) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));


				if (GlobalData.GetPlayerMoney() >= total + partPice)
				{
					if (inventory.Any(x => partID.Equals(x.ID) && x.Condition == 1f) && Main.Settings.invCheck)
					{
						dupeCount++;
						inventory.Remove(inventory.FirstOrDefault(x => partID.Equals(x.ID) && x.Condition == 1f));
					}
					else if (partID.Equals("#Dummy"))
					{
						count++;
					}
					else
					{
						Main.mod.Logger.Log(partID);
						Inventory.Get().Add(newInventoryItem);
						total += partPice;

						count++;
					}

				}

				yield return new WaitForEndOfFrame();
			}

			if (dupeCount != 0 && dupeCount == partsListSorted.Count() && Main.Settings.invCheck)
			{
				UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"All parts were already in the inventory", PopupType.Normal);
			}
			else if (total == 0)
			{
				UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "No parts were buyed", PopupType.Normal);
			}
			else
			{
				if (dupeCount != 0 && Main.Settings.invCheck)
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

			yield break;
		}

        public static bool CanUnmountEngine(CarLoader carLoader)
        {
            GameObject engine = carLoader.GetEngine();
            InteractiveObject iO = engine.GetComponent<InteractiveObject>();

            if (iO == null)
            {
                return false;
            }

            string part = iO.CanUnmountGroup();
            if (!string.IsNullOrEmpty(part))
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(string.Format(Localization.Instance.Get("GUI_EngineUnmountCrainWarning"), Localization.Instance.Get("!" + part)));
                return false;
            }

            if (carLoader.GetOilLevel() > 0f)
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(Localization.Instance.Get("GUI_EngineOilCrain"));
                return false;
            }

            return true;
        }

        public static bool CanMountEngine(CarLoader carLoader)
        {
            GameObject engine = carLoader.GetEngine();
            InteractiveObject iO = engine.GetComponent<InteractiveObject>();

            if (iO == null)
            {
                return false;
            }

            string text = iO.CanMountGroup();
            if (!string.IsNullOrEmpty(text))
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(string.Format(Localization.Instance.Get("GUI_EngineMountCrainWarning"), Localization.Instance.Get(text)));
                return false;
            }

            return true;
        }


        public static void Paint(CarLoader carLoader)
		{
			carColorCheck = "color";
			GameScript.Get().GetCarPaintLogic().SetCarLoader(carLoader);
			GameScript.Get().GetCarPaintLogic().PrepareCarPaint();

			GameMode.Get().SetCurrentMode(gameMode.UI);
			UIManager.Get().Show("CarPaintMenu");
			Cursor3D.Get().SetCursorIsEnable(true);
		}


		public static void OpenCloseCar(CarLoader carLoader)
		{
			List<carPart> list;

			if (carLoader.IsCarOnLifter())
			{
				list = new List<carPart>() { carLoader.GetCarPart("hood"), carLoader.GetCarPart("door_rear_left"), carLoader.GetCarPart("door_rear_right"), carLoader.GetCarPart("trunk") };
			}
			else
			{
				list = new List<carPart>() { carLoader.GetCarPart("hood"), carLoader.GetCarPart("door_front_left"), carLoader.GetCarPart("door_front_right"), carLoader.GetCarPart("door_rear_left"), carLoader.GetCarPart("door_rear_right"), carLoader.GetCarPart("trunk") };
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
					if (item.handle != null && item.switched != mostSwitch)
					{
						carLoader.SwitchCarPart(item.name);
					}
				}
			}
			else
			{
				foreach (var item in list)
				{
					if (item.handle != null)
					{
						carLoader.SwitchCarPart(item.name);
					}
				}
			}
		}

        public static void OpenCloseCarPart(CarLoader carLoader)
        {
            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Hood").Key).ToString()):
                    if (carLoader.GetCarPart("hood").handle != null)
                    {
                        carLoader.SwitchCarPart("hood");
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Front Left Door").Key).ToString()):
                    if (carLoader.GetCarPart("door_front_left").handle != null)
                    {
                        if (!carLoader.IsCarOnLifter())
                        {
                            carLoader.SwitchCarPart("door_front_left");
                        }
                        else
                        {
                            SoundManager.Get().PlaySFX("Error");
                        }
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Front Right Door").Key).ToString()):
                    if (carLoader.GetCarPart("door_front_right").handle != null)
                    {
                        if (!carLoader.IsCarOnLifter())
                        {
                            carLoader.SwitchCarPart("door_front_right");
                        }
                        else
                        {
                            SoundManager.Get().PlaySFX("Error");
                        }
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Rear Left Door").Key).ToString()):
                    if (carLoader.GetCarPart("door_rear_left").handle != null)
                    {
                        carLoader.SwitchCarPart("door_rear_left");
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Rear Right Door").Key).ToString()):
                    if (carLoader.GetCarPart("door_rear_right").handle != null)
                    {
                        carLoader.SwitchCarPart("door_rear_right");
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Trunk").Key).ToString()):
                    if (carLoader.GetCarPart("trunk").handle != null)
                    {
                        carLoader.SwitchCarPart("trunk");
                    }
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("All Car Parts").Key).ToString()):

                    OpenCloseCar(carLoader);
                    break;

                default:
                    break;
            }
        }

        public static List<NewInventoryItem> inventory;
		public static List<NewGroupItem> groupInventory;
		public static List<int> rimsInstance;
		public static List<int> tiresInstance;
		public static int dupeCount;
		public static int total;
        public static string carColorCheck = "";
		public static NewInventoryItem selectedItemToMount;
        public static NewInventoryItem specialEngine;
    }
}
