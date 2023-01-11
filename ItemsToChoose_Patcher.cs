using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(ItemsToChoose))]
	[HarmonyPatch("UpdateForItem")]
	public static class ItemsToChoose_Patcher_UpdateForItem
    {
		[HarmonyPostfix]
		public static void ItemsToChoose_UpdateForItem(ItemsToChoose __instance, int index, ref GameObject ___preview, ref GameObject ___sender, ref List<NewInventoryItem> ___ListOfItems)
		{
            ___sender = __instance.transform.Find("SmallItems/Scroll View/Viewport/Content").GetChild(index).gameObject;
            NewHash hash = ___sender.GetComponent<ButtonAction>().Get();
            NewInventoryItem item = ___ListOfItems[index];
            string windowType = hash.GetFromKey("WindowType") as string;

            if (GameScript.Get().GetIOMouseOverCarLoader2() == null)
            {
                ModHelper.chooseEngine = false;
            }
            else
            {
                string[] stringsList = ___ListOfItems.ConvertAll(x => x.GetNormalID()).ToArray();
                if (stringsList == GameScript.Get().GetIOMouseOverCarLoader2().GetEngineSwapOptions())
                    ModHelper.chooseEngine = false;
            }

            if (windowType.Equals("NewChooseEngineMenu") && ModHelper.chooseEngine)
            {
                Action action = delegate ()
                {
                    if (ModHelper.CanUnmountEngine(GameScript.Get().GetIOMouseOverCarLoader2()) && ModHelper.CanMountEngine(GameScript.Get().GetIOMouseOverCarLoader2()))
                    {
                        Main.mod.Logger.Log(item.ID);
                        GameScript.Get().GetIOMouseOverCarLoader2().SetEngineSwap(item.ID);

                        NewGroupItem newGroupItem = new NewGroupItem();
                        newGroupItem.GroupName = item.ID;
                        newGroupItem.ItemList = new List<NewInventoryItem>();
                        newGroupItem.IsNormalGroup = false;

                        GameScript.Get().GetIOMouseOverCarLoader2().StartCoroutine(GameScript.Get().GetIOMouseOverCarLoader2().SwapEngine(newGroupItem));
                    }

                    ModHelper.chooseEngine = false;
                    UIManager.Get().Hide("NewChooseItemsMenu");
                };

                NewHash hash1 = new NewHash(new object[]
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

                ___preview.GetComponent<ButtonAction>().Set(hash1);
            }

        }
    }

}
