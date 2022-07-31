using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("prepareChooseItemsMenu")]
	public static class UIManager_Patcher_prepareChooseItemsMenu
	{
		[HarmonyPrefix]
		public static void UIManager_prepareChooseItemsMenu(UIManager __instance, ref string windowName, ref string type, ref string[] ID)
		{
			Main.mod.Logger.Log(windowName);
			Main.mod.Logger.Log(type);

			if (type.Equals("item"))
			{
				if (windowName.Equals("NewChooseItemsMenuPaintshopPart") || windowName.Equals("NewChooseEngineMenu"))
				{
					return;
				}
				else if (Inventory.Get().MakeInventoryMount(ID[0]) == 0)
				{
					GameScript.Get().SelectPartToMount("999", 0f, Color.black, true, 0);
					__instance.ShowInfoWindow(Localization.Instance.Get("GUI_NieMaPrzedmiotowDoZalozenia"));

					type = "none";
				}
				else if (Settings.autoSelect)
				{
					NewInventoryItem newInventoryItem = (from i in Inventory.Get().GetItems(ID[0])
														 orderby i.Condition descending
														 select i).ToList()[0];

					UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newInventoryItem.ID) + " (" + Helper.ConditionToString(newInventoryItem.Condition) + ")", PopupType.Normal);

					if (newInventoryItem.extraParameters.ContainsKey("PaintType"))
					{
						GameScript.Get().SetSelectedPartToMountPaintType((int)newInventoryItem.extraParameters.GetFromKey("PaintType"));
					}

					GameScript.Get().SelectPartToMount(newInventoryItem.ID, newInventoryItem.Condition, newInventoryItem.GetItemColor(), newInventoryItem.IsExamined, newInventoryItem.GetItemQuality());

					Inventory.Get().Delete(newInventoryItem);

					type = "none";
				}
			}

			if (type.Equals("group") && windowName.Equals("NewChooseItemsMenu") && Settings.autoSelect)
			{
				List<NewGroupItem> list2 = new List<NewGroupItem>();

				if (ID[0].Contains("rim"))
				{
					int minRimSize = GameScript.Get().GetIOMouseOverCarLoader2().GetMinRimSize(true);
					float rearWheelMaxSize = GameScript.Get().GetIOMouseOverCarLoader2().GetRearWheelMaxSize();
					list2 = GroupInventory.Get().GetRimsWithSizeGreaterOrEqualThanAndLessThan(minRimSize, rearWheelMaxSize);
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

					UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newGroupItem.GroupName) + " (" + Helper.ConditionToString(Helper.GetAvargeGroupCondition(newGroupItem)) + ")", PopupType.Normal);

					GameManager.Get().MountGroup(newGroupItem.UId);
					type = "none";
				}
			}
		}
	}

	[HarmonyPatch(typeof(UIManager))]
	[HarmonyPatch("initCreateGroupMenu")]
	public static class UIManager_Patcher_initCreateGroupMenu
	{
		[HarmonyPrefix]
		public static void UIManager_initCreateGroupMenu(UIManager __instance, ref string windowName)
		{
			Main.mod.Logger.Log(windowName);

			if (windowName.Equals("MountGroup") && Settings.autoSelect)
			{
				List<NewInventoryItem> newInventoryItems = new List<NewInventoryItem>();

				foreach (string item in GameScript.Get().GetGroupOfItems().ToArray())
				{
					List<NewInventoryItem> newInventoryItem = (from i in Inventory.Get().GetItems(item)
																orderby i.Condition descending
																select i).ToList();

					if (newInventoryItem.Count() > 0)
					{
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
						windowName = "none";

						break;
					}
				}

                if (!windowName.Equals("none"))
                {
					NewGroupItem newGroupItem = new NewGroupItem();
					newGroupItem.GroupName = newInventoryItems[0].ID;
					newGroupItem.IsNormalGroup = false;
					newGroupItem.ItemList = new List<NewInventoryItem>();
                    foreach (NewInventoryItem newInventoryItem in newInventoryItems)
                    {
						UIManager.Get().ShowPopup(Localization.Instance.Get("PopUp_NewItem"), Singleton<GameInventory>.Instance.GetItemLocalizeName(newInventoryItem.ID) + " (" + Helper.ConditionToString(newInventoryItem.Condition) + ")", PopupType.Normal);
						newGroupItem.ItemList.Add(newInventoryItem);
					}

					GroupInventory.Get().Add(newGroupItem);
					GameManager.Get().MountGroup(newGroupItem.UId);

					windowName = "none";
				}
				
			}
			
		}
	}
}
