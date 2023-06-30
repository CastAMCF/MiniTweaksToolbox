using ModKit;
using Steamworks;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Tools
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

            Action[] welder = new Action[] { delegate() {
                using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    ModMenuHelper.NormalKeyBind("Use Welder", "Use the " + "Welder".cyan() + " the car that is under the cursor",
                    welderHelp, delegate { UpdateBool("Welder"); });
                }
            }};

            UI.HStack("Welder", 1, welder);
            UI.Div(0f, 25f, 0f);

            Action[] interior = new Action[] { delegate() {
                using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    ModMenuHelper.NormalKeyBind("Use Interior Detailing", "Use the " + "Interior Detailing Toolkit".cyan() + " the car that is under the cursor",
                    interiorHelp, delegate { UpdateBool("Interior Detailing"); });
                }
            }};

            UI.HStack("Interior Detailing Toolkit", 1, interior);
            UI.Div(0f, 25f, 0f);

            Action[] stand = new Action[] { delegate() {
                using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    ModMenuHelper.NormalKeyBind("Rotate Left", "Rotate " + "Engine Stand".cyan() + " to the left",
                    standLeftHelp, delegate { UpdateBool("Rotate Left"); });

                    ModMenuHelper.NormalKeyBind("Rotate Right", "Rotate " + "Engine Stand".cyan() + " to the right",
                    standRightHelp, delegate { UpdateBool("Rotate Right"); });
                }
            }};

            UI.HStack("Engine Stand", 1, stand);
            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Welder":
                    welderHelp = !welderHelp;
                    break;

                case "Interior Detailing":
                    interiorHelp = !interiorHelp;
                    break;

                case "Rotate Left":
                    standLeftHelp = !standLeftHelp;
                    break;

                case "Rotate Right":
                    standRightHelp = !standRightHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool welderHelp = false;

        private static bool interiorHelp = false;

        private static bool standLeftHelp = false;

        private static bool standRightHelp = false;
    }
}
