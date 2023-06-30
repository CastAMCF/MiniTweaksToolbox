using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class ModMenu
    {
        public static void OnLoad()
        {
        }

        public static void ResetGUI()
        {
        }

        public static void OnGUI()
        {
            //Action[] elems = new Action[] { delegate() {
            using (UI.VerticalScope(new GUILayoutOption[0]))
            {
                ModMenuHelper.NormalToggles("Tunned Parts", "When buying any part that can be tunned it will buy it, if not it will buy the standard one" + "\n" +
                    "If you don't have enough money to afford the tunned part, it will automatically buy the standard one".bold().color((RGBA)3227578623U), tunnedPartsHelp,
                    ref Main.Settings.tunnedParts, delegate { UpdateBool("Tunned Parts"); });


                ModMenuHelper.NormalToggles("Group Parts", "When buying tires and wheels they it will be on a group already balanced" + "\n" +
                    "The suspension and engine group are buyable too".bold().color((RGBA)3227578623U), groupPartsHelp,
                    ref Main.Settings.groupParts, delegate { UpdateBool("Group Parts"); });


                ModMenuHelper.NormalToggles("Custom License Plates", "When buying license plates it will check for the name/number of that license plate" + "\n" +
                    "The custom license plates cost ".bold().red() + "1000$".bold().yellow() + ", the standard ones cost ".bold().red() + "100$".bold().yellow(),customLPNHelp,
                    ref Main.Settings.customLPN, delegate { UpdateBool("Custom License Plates"); });
                
                
                ModMenuHelper.NormalToggles("Undiscovered Parts", "When buying all the parts at once its gonna buy all parts of the order" + "\n" +
                    "Discovered and undiscovered".bold().orange(), uncheckedPartsHelp,
                    ref Main.Settings.uncheckedParts, delegate { UpdateBool("Undiscovered Parts"); });
                
                
                ModMenuHelper.NormalToggles("Inventory Check", "When buying a part that you already have its gonna warning you that you already have it" + "\n" +
                    "You can still buy it if you press the buy key again".bold().color((RGBA)3227578623U), invCheckHelp,
                    ref Main.Settings.invCheck, delegate { UpdateBool("Inventory Check"); });
                
                
                ModMenuHelper.NormalToggles("Painted Parts", "When buying any body part it will be already painted with the current color and livery" + "\n" +
                    "It will be ".bold().red() + "100$".bold().yellow() + " more pricy".bold().red(), paintPartsHelp,
                    ref Main.Settings.paintParts, delegate { UpdateBool("Painted Parts"); });
                
                
                ModMenuHelper.NormalToggles("Auto Select", "When you select a part to mount its gonna choose the best one in the inventory", autoSelectHelp,
                    ref Main.Settings.autoSelect, delegate { UpdateBool("Auto Select"); });

                if (Main.Settings.autoSelect)
                {
                    ModMenuHelper.NormalToggles("Original Parts", "When selecting a part to mount its gonna choose a part which has the same condition has the previous one" + "\n" +
                        "If there isn't any will choose the best one".bold().color((RGBA)3227578623U), originalPartsHelp,
                        ref Main.Settings.originalParts, delegate { UpdateBool("Original Parts"); });
                }
            
                ModMenuHelper.NormalToggles("No Oil Drain", "Once the max amount of oil has been hit, the gallon will forcibly stop" + "\n" +
                    "In some cases will charge you for ".bold().red() + "20$".bold().yellow(), noOilDrainHelp,
                    ref Main.Settings.noOilDrain, delegate { UpdateBool("No Oil Drain"); });
                
                
                ModMenuHelper.NormalToggles("Quality", "When buying a part its gonna be at the quality number selected" + "\n" +
                    "It will be much more pricy depending on the quality".bold().red(), itemQualityHelp,
                    ref Main.Settings.itemQuality, delegate { UpdateBool("Quality"); });

                if (Main.Settings.itemQuality)
                {
                    UI.Slider(ref Main.Settings.quality, 1, 5, 1, "", new GUILayoutOption[]
                    {
                        120.width()
                    });
                }
            }
            //}};

            //UI.HStack("Menu", 1, elems);
            UI.Div(0f, 25f, 0f);
        }

        public static void CheckBindings()
        {
            switch ("True")
            {
                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Tunned Parts").Key).ToString()):
                    Settings.ToggleTunnedPartsSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Tunned Parts §" + (Main.Settings.tunnedParts ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Group Parts").Key).ToString()):
                    Settings.ToggleGroupPartsSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Group Parts §" + (Main.Settings.groupParts ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Custom License Plates").Key).ToString()):
                    Settings.ToggleCustomLicensePlatesSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Custom License Plates §" + (Main.Settings.customLPN ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Undiscovered Parts").Key).ToString()):
                    Settings.ToggleUncheckedPartsSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Undiscovered Parts §" + (Main.Settings.uncheckedParts ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Inventory Check").Key).ToString()):
                    Settings.ToggleInvCheckSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Inventory Check §" + (Main.Settings.invCheck ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Painted Parts").Key).ToString()):
                    Settings.TogglePaintPartsSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Painted Parts §" + (Main.Settings.paintParts ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Auto Select").Key).ToString()):
                    Settings.ToggleAutoSelectSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Auto Select §" + (Main.Settings.autoSelect ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Original Parts").Key).ToString()):
                    Settings.ToggleOriginalPartsSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Original Parts §" + (Main.Settings.originalParts ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("No Oil Drain").Key).ToString()):
                    Settings.ToggleNoOilDrainSetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "No Oil Drain §" + (Main.Settings.noOilDrain ? "ON" : "OFF"), PopupType.Normal);
                    break;

                case string keyCode when keyCode.Equals(Input.GetKeyDown(UI.KeyBindings.GetBinding("Quality").Key).ToString()):
                    Settings.ToggleQualitySetting();
                    UIManager.Get().ShowPopup("MiniTweaksToolbox Mod:", "Quality " + (Main.Settings.itemQuality ? $"{Main.Settings.quality}" : "§OFF"), PopupType.Normal);
                    break;

                default:
                    break;
            }
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Tunned Parts":
                    tunnedPartsHelp = !tunnedPartsHelp;
                    break;

                case "Group Parts":
                    groupPartsHelp = !groupPartsHelp;
                    break;

                case "Custom License Plates":
                    customLPNHelp = !customLPNHelp;
                    break;

                case "Undiscovered Parts":
                    uncheckedPartsHelp = !uncheckedPartsHelp;
                    break;

                case "Inventory Check":
                    invCheckHelp = !invCheckHelp;
                    break;

                case "Painted Parts":
                    paintPartsHelp = !paintPartsHelp;
                    break;

                case "Auto Select":
                    autoSelectHelp = !autoSelectHelp;
                    break;

                case "Original Parts":
                    originalPartsHelp = !originalPartsHelp;
                    break;

                case "No Oil Drain":
                    noOilDrainHelp = !noOilDrainHelp;
                    break;

                case "Quality":
                    itemQualityHelp = !itemQualityHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool tunnedPartsHelp = false;

        private static bool groupPartsHelp = false;

        private static bool customLPNHelp = false;

        private static bool uncheckedPartsHelp = false;

        private static bool invCheckHelp = false;

        private static bool paintPartsHelp = false;

        private static bool autoSelectHelp = false;

        private static bool originalPartsHelp = false;

        private static bool noOilDrainHelp = false;

        private static bool itemQualityHelp = false;
    }
}
