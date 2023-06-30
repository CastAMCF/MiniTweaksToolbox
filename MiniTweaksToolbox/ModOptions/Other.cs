using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Other
    {
        public static void OnLoad()
        {

        }

        public static void ResetGUI()
        {
        }

        public static void OnGUI()
        {
            GUILayout.Box("<b>Shed / Junkyard</b>", new GUILayoutOption[]
            {
                GUILayout.Height(21f)
            });
            UI.Space(15f);

            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Xray", "Highlights in " + "cyan".bold().cyan() + " all shelfs/junk cars that have at least one part in them",
                xrayHelp, delegate { UpdateBool("Xray"); });

                ModMenuHelper.NormalKeyBind("Special Xray", "Highlights in " + "green".bold().green() + " the shelf/car that have the crate/map in it" + "\n" +
                    "Every time".bold().orange() + " a shed/junkyard is generated spawn ".bold().color((RGBA)3227578623U) + "one".bold().red() + ": ".bold().color((RGBA)3227578623U) + "Crate".bold().yellow() + " (Shed) ".bold().color((RGBA)3227578623U) + "|".bold().white() + " ".bold().color((RGBA)3227578623U) + "Map".bold().yellow() + " (Junkyard)".bold().color((RGBA)3227578623U),
                specialXrayHelp, delegate { UpdateBool("Special Xray"); });

                ModMenuHelper.NormalKeyBind("Car Xray", "Highlights in " + "red".bold().red() + " all cars that are buyable",
                xrayCarHelp, delegate { UpdateBool("Car Xray"); });
            }

            GUILayout.Box("<b>Garage</b>", new GUILayoutOption[]
            {
                GUILayout.Height(21f)
            });
            UI.Space(15f);

            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Delete All Parts", "Deletes ".red() + "everything".bold().red() + " that is in the inventory".bold().color((RGBA)3227578623U),
                    sellAllHelp, delegate { UpdateBool("Delete All Parts"); });
            }

            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Xray":
                    xrayHelp = !xrayHelp;
                    break;

                case "Special Xray":
                    specialXrayHelp = !specialXrayHelp;
                    break;

                case "Car Xray":
                    xrayCarHelp = !xrayCarHelp;
                    break;

                case "Delete All Parts":
                    sellAllHelp = !sellAllHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool xrayHelp = false;

        private static bool specialXrayHelp = false;

        private static bool xrayCarHelp = false;

        private static bool sellAllHelp = false;
    }
}
