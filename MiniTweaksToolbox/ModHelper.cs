using dnlib.DotNet.Emit;
using HighlightingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
using static ModKit.UI;
using static UnityEngine.ParticleSystem;

namespace MiniTweaksToolbox
{
	internal static class ModHelper
	{
        public static void PlayerMoves()
        {
            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Sprint").Key).ToString()):

                    playerState = "running";
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyUp(KeyBindings.GetBinding("Sprint").Key).ToString()):

                    playerState = "walk";
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(KeyBindings.GetBinding("Jump").Key).ToString()):

                    jump = true;
                    break;

                default:
                    break;
            }
        }

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
            string carName = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(GameScript.Get().GetIOMouseOverCarLoader2().carToLoad, GameScript.Get().GetIOMouseOverCarLoader2().ConfigVersion);
            UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", $"Getting all engines of " + carName, PopupType.Normal);
            
            GameMode.Get().SetCurrentMode(gameMode.UI);
            UIManager.Get().SetIODescription(string.Empty, UIHelper.None);

            UIManager.Get().StartCoroutine(CreateDownMenuFromItems(ID));
        }

        public static void ShedJunkXray()
        {
            toogleXrayCar = false;
            toogleXraySpecial = false;

            foreach (CarLoader carloader in UnityEngine.Object.FindObjectsOfType<CarLoader>())
            {
                if (carloader != null)
                {
                    string carName = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(carloader.carToLoad, carloader.ConfigVersion);
                    if (!carName.Equals("Untitled Car"))
                    {
                        foreach (InteractiveObject interactiveObject in carloader.GetRoot().transform.GetComponentsInChildren<InteractiveObject>())
                        {
                            if (interactiveObject != null)
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.Off();
                                }
                            }
                        }
                    }
                }
            }

            foreach (InteractiveObject interactiveObject in xrayScene.GetComponentsInChildren<InteractiveObject>())
            {
                if (interactiveObject.GetSpecialType() == IOSpecialType.Junk)
                {
                    if (toogleXray)
                    {
                        foreach (Junk junk in interactiveObject.GetComponentsInChildren<Junk>())
                        {
                            if (junk.gameObject.GetComponent<Junk>().ItemsInTrash.Count == 0)
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.Off();
                                }
                            }
                            else
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.ConstantOnImmediate(Color.cyan);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                        {
                            highlighter.Off();
                        }
                    }
                }
            }
        }

        public static void ShedJunkXrayCar()
        {
            foreach (CarLoader carloader in UnityEngine.Object.FindObjectsOfType<CarLoader>())
            {
                if (carloader != null)
                {
                    string carName = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(carloader.carToLoad, carloader.ConfigVersion);
                    if (!carName.Equals("Untitled Car"))
                    {
                        foreach (InteractiveObject interactiveObject in carloader.GetRoot().transform.Find("model(Clone)").transform.GetComponentsInChildren<InteractiveObject>())
                        {
                            if (interactiveObject != null)
                            {
                                if (toogleXrayCar)
                                {
                                    foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                    {
                                        highlighter.ConstantOnImmediate(Color.red);
                                    }

                                }
                                else
                                {
                                    foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                    {
                                        highlighter.Off();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            switch ("True")
            {
                case string s when s.Equals(toogleXray.ToString()):
                case string s1 when s1.Equals(toogleXraySpecial.ToString()):

                    foreach (InteractiveObject interactiveObject in xrayScene.GetComponentsInChildren<InteractiveObject>())
                    {
                        if (interactiveObject.GetSpecialType() == IOSpecialType.Junk)
                        {
                            foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                            {
                                highlighter.Off();
                            }
                        }
                    }
                    break;

                default:
                    break;
            }

            toogleXray = false;
            toogleXraySpecial = false;
        }

        public static void ShedJunkXraySpecial(string itemID)
        {
            toogleXray = false;
            toogleXrayCar = false;

            foreach (CarLoader carloader in UnityEngine.Object.FindObjectsOfType<CarLoader>())
            {
                if (carloader != null)
                {
                    string carName = Singleton<CarBundleLoader>.Instance.GetCarNameWithSuffix(carloader.carToLoad, carloader.ConfigVersion);
                    if (!carName.Equals("Untitled Car"))
                    {
                        foreach (InteractiveObject interactiveObject in carloader.GetRoot().transform.GetComponentsInChildren<InteractiveObject>())
                        {
                            if (interactiveObject != null)
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.Off();
                                }
                            }
                        }
                    }
                }
            }

            foreach (InteractiveObject interactiveObject in xrayScene.GetComponentsInChildren<InteractiveObject>())
            {
                if (interactiveObject.GetSpecialType() == IOSpecialType.Junk)
                {
                    if (toogleXraySpecial)
                    {
                        foreach (Junk junk in interactiveObject.GetComponentsInChildren<Junk>())
                        {
                            if (junk.gameObject.GetComponent<Junk>().ItemsInTrash.Any((NewInventoryItem x) => x.ID.Equals(itemID)))
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.ConstantOnImmediate(Color.green);
                                }
                            }
                            else
                            {
                                foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                                {
                                    highlighter.Off();
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Highlighter highlighter in interactiveObject.GetComponentsInChildren<Highlighter>())
                        {
                            highlighter.Off();
                        }
                    }
                }
            }
        }

        private static IEnumerator GetEnginePrice(string engineName, NewInventoryItem item)
        {
            int price = 0;
            var e_engine_h = Resources.Load(engineName) as GameObject;

            PartScript[] chlidrensPS = e_engine_h.gameObject.GetComponentsInChildren<PartScript>();
            foreach (PartScript partScript in chlidrensPS)
            {
                string partID = partScript.gameObject.name.Split('(')[0];
                NewInventoryItem newInventoryItem;

                if (!partID.Contains("gearbox") && !partID.Contains("rozrusznik") && !partID.Contains("kolektor"))
                {
                    if (Singleton<GameInventory>.Instance.GetItemProperty("t_" + partID).Price != 0 && Main.Settings.tunnedParts)
                    {
                        partID = "t_" + partID;
                    }

                    newInventoryItem = new NewInventoryItem(partID, 1f, Inventory.SetColor(Color.white), true);

                    if (Main.Settings.itemQuality)
                    {
                        newInventoryItem.extraParameters.Add("Quality", Main.Settings.quality);
                    }

                    price += (int)Mathf.Floor(Helper.GetPrice(newInventoryItem) * Singleton<UpgradeSystem>.Instance.GetUpgradeValue("shop_discount"));
                }
            }

            if (GlobalData.GetPlayerMoney() >= price)
            {
                item.extraParameters.Add("Quality", price);
            }
            else
            {
                item.extraParameters.Add("Quality", 0);
            }

            yield break;
        }

        private static IEnumerator CreateDownMenuFromItems(string[] itemName)
        {
            Singleton<InputManager>.Instance.ChangeInput(false, false, false);

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

                yield return GetEnginePrice(text, item);

                if (Convert.ToInt32(item.extraParameters.GetHashTable()["Quality"]) == 0)
                {
                    list.Remove(item);
                }
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

            list = (from i in list
                    orderby Convert.ToInt32(i.extraParameters.GetHashTable()["Quality"]) descending
                    select i).ToList();

            UIManager.Get().Show("NewChooseItemsMenu");

            foreach (NewInventoryItem newInventoryItem in list)
            {
                Action action = delegate ()
				{
					if (CarHelper.CanUnmountEngine(GameScript.Get().GetIOMouseOverCarLoader2()) && CarHelper.CanMountEngine(GameScript.Get().GetIOMouseOverCarLoader2()))
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

            Singleton<InputManager>.Instance.ChangeInput(true, true, true);
            yield break;
        }

        public static IEnumerator ClearInventory()
        {
            Singleton<InputManager>.Instance.ChangeInput(false, false, false);

            List<NewInventoryItem> inv = new List<NewInventoryItem>(Inventory.Get().GetItems("All"));

            for (int x = 0; x < inv.Count; x++)
            {
                NewInventoryItem item = inv[x];
                int specialGroup = Singleton<GameInventory>.Instance.GetItemProperty(item.ID).SpecialGroup;

                if (specialGroup != 14 && specialGroup != 13 && !item.ID.Equals("specialCase") && !item.ID.Equals("specialMap"))
                {
                    Inventory.Get().Delete(item);
                    UIManager.Get().DeleteItem(item.UId, "CanvasInventory/Inventory/Content/ScrollRect/Content");

                    if (x % 10 == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            List<NewGroupItem> invGroup = new List<NewGroupItem>(GroupInventory.Get().GetGroupInventory());

            for (int x = 0; x < invGroup.Count; x++)
            {
                NewGroupItem newGroupItem = invGroup[x];

                GroupInventory.Get().Delete(newGroupItem.UId);
                UIManager.Get().DeleteGroup(newGroupItem.UId, "CanvasInventory/Inventory/Content/ScrollRect/Content");

                if (x % 10 == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            Singleton<InputManager>.Instance.ChangeInput(true, true, true);
            yield break;
        }

        public static IEnumerator UseEngineCrane(CarLoader carLoader)
        {
            Debug.Log("Run Use Engine Crane");
            if (carLoader == null)
            {
                yield break;
            }

            GameObject engine = carLoader.GetEngine();
            InteractiveObject iO = engine.GetComponent<InteractiveObject>();
            if (iO == null)
            {
                yield break;
            }

            string part = iO.CanUnmountGroup();
            if (!string.IsNullOrEmpty(part))
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(string.Format(Localization.Instance.Get("GUI_EngineUnmountCrainWarning"), Localization.Instance.Get("!" + part)));
                yield break;
            }

            Debug.Log("W silniku jest :" + iO.GetMountedItemsAmount());
            if (iO.GetMountedItemsAmount() < 1)
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(Localization.Instance.Get("GUI_EngineEmptyCrainWarning"));
                yield break;
            }

            if (carLoader.GetOilLevel() > 0f)
            {
                GameMode.Get().SetCurrentMode(gameMode.UI);
                UIManager.Get().ShowInfoWindow(Localization.Instance.Get("GUI_EngineOilCrain"));
                yield break;
            }

            Singleton<InputManager>.Instance.ChangeInput(false, false, false);
            ScreenFader.Get().NormalFadeIn();

            while (ScreenFader.Get().IsRunning())
            {
                yield return new WaitForEndOfFrame();
            }

            if (engineCranePos && carLoader.GetCarPart("trunk") != null)
            {
                if (!carLoader.GetCarPart("trunk").switched)
                {
                    ToolsManager.Get().StartCoroutine(carLoader.SwitchCarPart(carLoader.GetCarPart("trunk"), true));
                }
                engineCranePos = false;
            }
            else
            {
                if (!carLoader.GetCarPart("hood").switched)
                {
                    ToolsManager.Get().StartCoroutine(carLoader.SwitchCarPart(carLoader.GetCarPart("hood"), true));
                }
            }

            ActionUnMountGroup(iO, carLoader);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            ScreenFader.Get().NormalFadeOut();
            while (ScreenFader.Get().IsRunning())
            {
                yield return new WaitForEndOfFrame();
            }

            GameMode.Get().SetCurrentMode(gameMode.Garage);
            GameScript.Get().SetMachineOnMouseOverType(IOSpecialType.None);
            Singleton<InputManager>.Instance.ChangeInput(true, true, false);

            yield break;
        }

        public static void ActionUnMountGroup(InteractiveObject iO, CarLoader carLoader)
        {
            NewGroupItem newGroupItem = new NewGroupItem();
            newGroupItem.GroupName = iO.gameObject.name;
            newGroupItem.ItemList = new List<NewInventoryItem>();
            newGroupItem.GroupSize = Helper.SetGroupSize(iO.gameObject.transform.localScale);

            Component[] componentsInChildren = iO.gameObject.GetComponentsInChildren<PartScript>();
            foreach (PartScript partScript in componentsInChildren)
            {
                if (partScript.IsUnmounted() || Singleton<GameInventory>.Instance.GetItemProperty(partScript.GetIDWithTuned()).SpecialGroup == 1)
                {
                    partScript.HidePreview();
                }
                else
                {
                    NewInventoryItem newInventoryItem = new NewInventoryItem(partScript.gameObject.name, partScript.GetCondition(), partScript.IsExamined());
                    newInventoryItem.extraParameters.Add("NormalID", partScript.GetIDWithTuned());
                    newInventoryItem.extraParameters.Add("Quality", partScript.GetQuality());
                    newGroupItem.ItemList.Add(newInventoryItem);

                    GameManager.Get().StartCoroutine(partScript.UnMountByGroup(true));
                }
            }

            newGroupItem.ItemList[0].extraParameters.Add("Carloader", carLoader);

            GroupInventory.Get().Add(newGroupItem);
        }

        public static void GetAllOwnedCars()
        {
            carNameIds = new List<string>();

            string profilePath = $"{Application.persistentDataPath}/profile{ProfileManager.Get().GetSelectedProfile()}";

            for (int i = 0; i < GlobalData.GetMaxParkingPlacesAmount(); i++)
            {
                string filePath = $"{profilePath}/#ParkingLvl1CarLoader{i}.dat";

                if (File.Exists(filePath))
                {
                    string carData = Helper.ReadAllText(filePath);

                    if (!string.IsNullOrEmpty(carData) && carData != "-")
                    {
                        NewCarData newCarData = new NewCarData();
                        newCarData = newCarData.Load(ProfileManager.Get().GetSelectedProfile(), $"/#ParkingLvl1CarLoader{i}.dat");

                        if (!string.IsNullOrEmpty(newCarData.carToLoad))
                        {
                            carNameIds.Add(newCarData.carToLoad);
                        }
                    }
                }
            }

            for (int i = 0; i < CarLoaderPlaces.Get().GetCarLoadersCount(); i++)
            {
                CarLoader carLoaderData = CarLoaderPlaces.Get().GetCarLoaderByIndex(i);

                if (!string.IsNullOrEmpty(carLoaderData.carToLoad))
                {
                    carNameIds.Add(carLoaderData.carToLoad);
                }
            }
        }

        public static bool dupeBool;
        public static string dupeText;
        public static string playerState = "walk";
        public static bool jump = false;
        public static Vector3 playerPosition;
        public static bool chooseEngine = false;
        public static bool engineCranePos = false;

        public static bool toogleXray = false;
        public static bool toogleXrayCar = false;
        public static bool toogleXraySpecial = false;
        public static Transform xrayScene;

        public static List<string> carNameIds = new List<string>();
        public static string carNameId = "";
        public static int dupeCar = 0;
    }
}
