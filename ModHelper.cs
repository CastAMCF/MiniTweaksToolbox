using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace MiniTweaksToolbox
{
	internal static class ModHelper
	{
		
		public static bool IsInInventory(CarLoader carLoader, Tire tire, string partID)
		{
			if (carLoader != null)
			{
				if (tire != null)
				{
					if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 6
					&& Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
					&& (!dupeBool || !partID.Equals(dupeText)))
					{
						return true;
					}
					else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 7
						&& Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
						&& (!dupeBool || !partID.Equals(dupeText)))
					{
						return true;
					}
				}
				else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(partID) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && (!dupeBool || !partID.Equals(dupeText)))
				{
					return true;
				}
				else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && (!dupeBool || !partID.Equals(dupeText)))
				{
					return true;
				}
				else if (Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f) && (!dupeBool || !partID.Equals(dupeText)))
				{
					return true;
				}
			}
			else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && (!dupeBool || !partID.Equals(dupeText)))
			{
				return true;
			}
			else if (Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f) && (!dupeBool || !partID.Equals(dupeText)))
			{
				return true;
            }

			return false;
		}


        public static List<NewGroupItem> GetRimsWithSizeGreaterOrEqualThanAndLessThan(float minRimSize, float maxSize)
        {
            List<NewGroupItem> list = new List<NewGroupItem>();
            return (from groupItem in GroupInventory.Get().GetGroupInventory()
                    where groupItem.GroupName.Contains("rim") && groupItem.ItemList != null && groupItem.ItemList.Count > 1 && (bool)groupItem.ItemList[0].extraParameters.GetFromKey("IsBalanced")
                    && (float)groupItem.ItemList[0].extraParameters.GetFromKey("Size") >= minRimSize && (float)groupItem.ItemList[0].extraParameters.GetFromKey("Size") <= maxSize
                    select groupItem).ToList<NewGroupItem>();
        }


        public static void UseTool(IOSpecialType tool, CarLoader carLoader)
		{
			Action action;
			int price;

			switch (tool)
			{
				case IOSpecialType.Welder:
					price = GlobalData.Cost_UseWelder;

					action = delegate ()
					{
						GlobalData.AddPlayerMoney(-price);
						carLoader.UseWelder();
						GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
					};

					break;
				case IOSpecialType.InteriorDetailingToolkit:
					price = GlobalData.Cost_UseInteriorDetailingToolkit;

					action = delegate ()
					{
						GlobalData.AddPlayerMoney(-price);
						carLoader.UseInteriorDetailingToolkit();
						AchievementSystem.Get().IncrementStat(14, 1);
						GameMode.Get().SetCurrentMode(GameMode.Get().GetPreviousMode());
					};

					break;
				default:
					return;
			}

			NewHash hash = new NewHash(new object[]
			{
				"WindowType",
				"UseTool",
				"Type",
				"RunAction",
				"CarName",
				carLoader.GetName(),
				"Action",
				action,
				"Price",
				price
			});

			UIManager.Get().ShowAskWindow(hash);
			GameMode.Get().SetCurrentMode(gameMode.UI);
		}


		public static IEnumerator SellPerCondition(float con)
		{
			List<NewGroupItem> junks = (from groupInventoryItem in GroupInventory.Get().GetGroupInventory()
										where Helper.RoundCondition((float)Helper.GetAvargeGroupCondition(groupInventoryItem)) <= con
										select groupInventoryItem).ToList();

			if (junks.Count <= 0)
			{
				if (GameSettings.ConsoleMode)
				{
					UIManager.Get().RebuildInventory(true);
					UIManager.Get().EnableGridInCurrentContext(true);
				}
				yield break;
			}

			int price = 0;

			for (int x = 0; x < junks.Count; x++)
			{
				NewGroupItem newGroupItem = junks[x];
				int itemPrice = Helper.GetPrice(newGroupItem);
				itemPrice = (int)(itemPrice * 0.5f);
				price += itemPrice;

				GroupInventory.Get().Delete(newGroupItem.UId);

				if (Warehouse.Get() && UIManager.Get().IsActive("Warehouse"))
				{
					Warehouse.Get().Delete(newGroupItem);
				}
				UIManager.Get().DeleteGroup(newGroupItem.UId, "CanvasInventory/Inventory/Content/ScrollRect/Content");
				UIManager.Get().DeleteGroup(newGroupItem.UId, "CanvasWarehouse/Warehouse/ContentWarehouse/ScrollRect/Content");
				UIManager.Get().DeleteGroup(newGroupItem.UId, "CanvasWarehouse/Warehouse/ContentInventory/ScrollRect/Content");

				if (x % 10 == 0)
				{
					yield return new WaitForEndOfFrame();
				}
			}

			GlobalData.AddPlayerMoney(price);
		}


        public static void ShowSwapEngines(string[] ID)
        {
            GameMode.Get().SetCurrentMode(gameMode.UI);
            UIManager.Get().SetIODescription(string.Empty, UIHelper.None);

            UIManager.Get().StartCoroutine(CreateDownMenuFromItems(ID));
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

        private static IEnumerator CreateDownMenuFromItems(string[] itemName)
        {
            UIManager.Get().transform.Find("NewChooseItemsMenu/ItemPreview/NewInventoryItem").gameObject.SetActive(false);
            UIManager.Get().transform.Find("NewChooseItemsMenu/ItemPreview/NewInventoryGroupItem").gameObject.SetActive(false);

            string pathToInsertItems = "ItemsToChoose/SmallItems/Scroll View/Viewport/Content";
            yield return UIManager.Get().StartCoroutine(UIManager.Get().cleanElement(pathToInsertItems));
            UIManager.Get().SmallItemsSliderStepSize = 0f;
            UIManager.Get().SmallItemsPrevIndex = -1;
            UIManager.Get().SmallItemsLeftBorderIndex = 0;
            UIManager.Get().SmallItemsRightBorderIndex = 5;
            UIManager.Get().SmallItemsAmount = 0;
            UIManager.Get().SmallItemsSliderValue = 0f;
			int index = 0;

            chooseEngine = true;

            List<NewInventoryItem> list = new List<NewInventoryItem>();

            List<string> items = itemName.ToList();
			foreach (string text in items)
			{
				NewInventoryItem item = new NewInventoryItem(text, true);
				list.Add(item);
			}
            UIManager.Get().CurrentSelectedBigItem = 0;

            if (list == null || list.Count == 0)
            {
                UIManager.Get().Hide("CreateGroupMenu");
                UIManager.Get().Hide("NewChooseItemsMenu");
                UIManager.Get().Hide("ItemsToChoose");
                GameScript.Get().PlayRepairSound(false);

                if (GameScript.Get())
                {
                    GameScript.Get().CanOpenPieMenu = true;
                }

                chooseEngine = false;
                UIManager.Get().ShowInfoWindow(Localization.Instance.Get("GUI_NieMaPrzedmiotowDoZalozenia"));
                yield break;
            }

            UIManager.Get().Show("NewChooseItemsMenu");

            foreach (NewInventoryItem newInventoryItem in list)
            {
                Action action = delegate ()
				{
					if (CanUnmountEngine(GameScript.Get().GetIOMouseOverCarLoader2()) && CanMountEngine(GameScript.Get().GetIOMouseOverCarLoader2()))
					{
                        Main.mod.Logger.Log(newInventoryItem.ID);
                        GameScript.Get().GetIOMouseOverCarLoader2().SetEngineSwap(newInventoryItem.ID);

                        NewGroupItem newGroupItem = new NewGroupItem();
                        newGroupItem.GroupName = newInventoryItem.ID;
                        newGroupItem.ItemList = new List<NewInventoryItem>();
                        newGroupItem.IsNormalGroup = false;

                        GameScript.Get().GetIOMouseOverCarLoader2().StartCoroutine(GameScript.Get().GetIOMouseOverCarLoader2().SwapEngine(newGroupItem));
                    }

                    chooseEngine = false;
                    UIManager.Get().Hide("NewChooseItemsMenu");
                };

				NewHash hash = new NewHash(new object[]
				{
                        "WindowType",
                        "NewChooseEngineMenu",
                        "Type",
                        "RunAction",
                        "Action",
                        action,
                        "ItemType",
						"Single",
						"Path",
						"NewChooseItemsMenu/ItemPreview",
						"Index",
						index
				});
				GameObject go = UIManager.Get().CreateInventoryItem(newInventoryItem, pathToInsertItems, hash, SortType.ByCondition, string.Empty, "All");

				if (go != null)
				{
					UIManager.Get().SmallItemsAmount++;
                    index++;
                }
            }
            UIManager.Get().transform.Find("ItemsToChoose").GetComponent<ItemsToChoose>().SetListOfItems(list);

            if (UIManager.Get().SmallItemsAmount > 6)
            {
                Vector2 vector = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().pivot;
                vector.x = 0f;
                UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().pivot = vector;
                UIManager.Get().SmallItemsSliderValue = -346.6f;
                vector = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().anchoredPosition;
                vector.x = -346.6f;
                UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().anchoredPosition = vector;
            }
            else
            {
                Vector2 vector2 = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().pivot;
                vector2.x = 0.5f;
                UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().pivot = vector2;
                UIManager.Get().SmallItemsSliderValue = 0f;
                vector2 = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().anchoredPosition;
                vector2.x = 0f;
                UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetComponent<RectTransform>().anchoredPosition = vector2;
            }

            Transform t = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetChild(0);
            if (t != null && UIManager.Get().SmallItemsAmount > 6)
            {
                float num2 = t.GetComponent<RectTransform>().localScale.x * t.GetComponent<RectTransform>().sizeDelta.x;
                Transform child = UIManager.Get().transform.Find("ItemsToChoose/SmallItems/Scroll View/Viewport/Content").GetChild(1);
                var spaceBetweenItems = child.GetComponent<RectTransform>().localPosition.x - (t.GetComponent<RectTransform>().localPosition.x + num2);
                UIManager.Get().SmallItemsSliderStepSize = num2 * 0.9f + spaceBetweenItems * 0.9f;
            }
            yield break;
        }

        public static bool dupeBool;
        public static string dupeText;
        public static string playerState = "walk";
        public static bool jump = false;
        public static Vector3 playerPosition;
        public static bool chooseEngine = false;
    }
}
