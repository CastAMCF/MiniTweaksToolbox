using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Paint
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
            using (UI.VerticalScope(new GUILayoutOption[0]))
            {
                ModMenuHelper.NormalKeyBind("Menu", "Open the " + "Paint Menu".cyan() + " to the car that is under the cursor",
                paintHelp, UpdateBool);
            }
            //}};

            //UI.HStack("Paint", 1, elems);
            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool()
        {
            paintHelp = !paintHelp;
        }

        private static bool paintHelp = false;
    }
}
