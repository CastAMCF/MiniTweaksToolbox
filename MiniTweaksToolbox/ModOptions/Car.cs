using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Car
    {
        public static void OnLoad()
        {

        }

        public static void ResetGUI()
        {
        }

        public static void OnGUI()
        {
            GUILayout.Box("<b>Garage</b>", new GUILayoutOption[]
            {
                GUILayout.Height(21f)
            });
            UI.Space(15f);

            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Swap Engine", "Show a custom menu with all swap engines that can be choosen from the car that is under the cursor" + "\n" +
                    "The quality represents the price of all parts to build the engine".bold().color((RGBA)3227578623U) + "\n" +
                    "If ".bold().orange() + "Tunned Parts".bold().white() + " and/or ".bold().orange() + "Quality".bold().white() + " option is/are enable its gonna impact on the price ".bold().orange(),
                swapEngineHelp, delegate { UpdateBool("Swap Engine"); });

                ModMenuHelper.NormalKeyBind("Hood", "Open/close hood from the car that is under the cursor",
                hoodHelp, delegate { UpdateBool("Hood"); });

                ModMenuHelper.NormalKeyBind("Front Left Door", "Open/close front left door from the car that is under the cursor",
                frontLeftDoorHelp, delegate { UpdateBool("Front Left Door"); });

                ModMenuHelper.NormalKeyBind("Front Right Door", "Open/close front right door from the car that is under the cursor",
                frontRightDoorHelp, delegate { UpdateBool("Front Right Door"); });

                ModMenuHelper.NormalKeyBind("Rear Left Door", "Open/close rear left door the car that is under the cursor",
                rearLeftDoorHelp, delegate { UpdateBool("Rear Left Door"); });

                ModMenuHelper.NormalKeyBind("Rear Right Door", "Open/close rear right door from the car that is under the cursor",
                rearRightDoorHelp, delegate { UpdateBool("Rear Right Door"); });

                ModMenuHelper.NormalKeyBind("Trunk", "Open/close trunk from the car that is under the cursor",
                trunkHelp, delegate { UpdateBool("Trunk"); });

                ModMenuHelper.NormalKeyBind("All Car Parts", "Open/close all car parts from the car that is under the cursor",
                allCarPartsHelp, delegate { UpdateBool("All Car Parts"); });
            }

            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Swap Engine":
                    swapEngineHelp = !swapEngineHelp;
                    break;

                case "Hood":
                    hoodHelp = !hoodHelp;
                    break;

                case "Front Left Door":
                    frontLeftDoorHelp = !frontLeftDoorHelp;
                    break;

                case "Front Right Door":
                    frontRightDoorHelp = !frontRightDoorHelp;
                    break;

                case "Rear Left Door":
                    rearLeftDoorHelp = !rearLeftDoorHelp;
                    break;

                case "Rear Right Door":
                    rearRightDoorHelp = !rearRightDoorHelp;
                    break;

                case "Trunk":
                    trunkHelp = !trunkHelp;
                    break;

                case "All Car Parts":
                    allCarPartsHelp = !allCarPartsHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool swapEngineHelp = false;

        private static bool hoodHelp = false;

        private static bool frontLeftDoorHelp = false;

        private static bool frontRightDoorHelp = false;

        private static bool rearLeftDoorHelp = false;

        private static bool rearRightDoorHelp = false;

        private static bool trunkHelp = false;

        private static bool allCarPartsHelp = false;
    }
}
