using System;
using System.Collections;
using System.Collections.Generic;
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
				if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 6
					&& Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
					&& (!Main.dupeBool || !partID.Equals(Main.dupeText)))
				{
					return true;
				}
				else if (Singleton<GameInventory>.Instance.GetItemProperty(partID).SpecialGroup == 7
					&& Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f && Convert.ToInt32(x.extraParameters.GetHashTable()["ET"]) == tire.w_et && Convert.ToInt32(x.extraParameters.GetHashTable()["Profile"]) == (int)tire.w_tireSize && Convert.ToInt32(x.extraParameters.GetHashTable()["Width"]) == (int)tire.w_wheelWidth && Convert.ToInt32(x.extraParameters.GetHashTable()["Size"]) == (int)tire.w_rimSize)
					&& (!Main.dupeBool || !partID.Equals(Main.dupeText)))
				{
					return true;
				}
				else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList[0].ID.Equals(partID) && x.ItemList[0].Condition == 1f && x.ItemList[1].ID.Equals("sprezynnaPrzod_1") && x.ItemList[1].Condition == 1f && x.ItemList[2].ID.Equals("czapkaAmorPrzod_1") && x.ItemList[2].Condition == 1f) && (!Main.dupeBool || !partID.Equals(Main.dupeText)))
				{
					return true;
				}
				else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && (!Main.dupeBool || !partID.Equals(Main.dupeText)))
				{
					return true;
				}
				else if (Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f) && (!Main.dupeBool || !partID.Equals(Main.dupeText)))
				{
					return true;
				}
			}
			else if (GroupInventory.Get().GetGroupInventory().Any(x => x.ItemList.Any(y => partID.Equals(y.GetNormalID()) && y.Condition == 1f)) && (!Main.dupeBool || !partID.Equals(Main.dupeText)))
			{
				return true;
			}
			else if (Inventory.Get().GetItems("All").Any(x => partID.Equals(x.ID) && x.Condition == 1f) && (!Main.dupeBool || !partID.Equals(Main.dupeText)))
			{
				return true;
            }

			return false;
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


	}
}
