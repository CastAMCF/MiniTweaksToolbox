using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Shopping
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

            //Action[] elems = new Action[] { delegate() {
            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Buy Part", "Buy the part that is under the cursor",
                    buyHelp, delegate { UpdateBool("Buy Part"); });
                ModMenuHelper.NormalKeyBind("Buy All Parts", "Buy all parts from the car that is under the cursor" + "\n" +
                    "The car info/summary has to be open to the game update the discovered parts".bold().orange() + "\n" +
                    "When the ".bold().color((RGBA)3227578623U) + "Undiscovered Parts".bold().white() + " option is enable its gonna buy all parts of the order".bold().color((RGBA)3227578623U) + "\n" +
                    "When the ".bold().color((RGBA)3227578623U) + "Group Parts".bold().white() + " option is enable the parts that can be grouped will be already grouped and balanced".bold().color((RGBA)3227578623U),
                    buyAllHelp, delegate { UpdateBool("Buy All Parts"); });
            }
            //}};

            UI.Space(15f);
            GUILayout.Box("<b>Shed / Junkyard</b>", new GUILayoutOption[]
            {
                GUILayout.Height(21f)
            });
            UI.Space(15f);

            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Buy Car", "Buy the car that is under the cursor",
                    buyCarHelp, delegate { UpdateBool("Buy Car"); });

            }

            //UI.HStack("Shopping", 1, elems);
            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Buy Part":
                    buyHelp = !buyHelp;
                    break;

                case "Buy All Parts":
                    buyAllHelp = !buyAllHelp;
                    break;

                case "Buy Car":
                    buyCarHelp = !buyCarHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool buyHelp = false;

        private static bool buyAllHelp = false;

        private static bool buyCarHelp = false;
    }
}
