using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Player
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
            using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
            {
                ModMenuHelper.NormalKeyBind("Sprint", "Be careful with the wet floor",
                    sprintHelp, delegate { UpdateBool("Sprint"); });

                ModMenuHelper.NormalKeyBind("Jump", "The jump has no cooldown, you can jump in the air",
                    jumpHelp, delegate { UpdateBool("Jump"); });

                ModMenuHelper.NormalKeyBind("Reset Postion", "This works where the " + "Sprint".bold().white() + " and " + "Jump".bold().white() + " are applied",
                    resetPositionHelp, delegate { UpdateBool("Reset Postion"); });
            }
            //}};

            //UI.HStack("Player", 1, elems);
            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Sprint":
                    sprintHelp = !sprintHelp;
                    break;

                case "Jump":
                    jumpHelp = !jumpHelp;
                    break;

                case "Reset Postion":
                    resetPositionHelp = !resetPositionHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool sprintHelp = false;

        private static bool jumpHelp = false;

        private static bool resetPositionHelp = false;
    }
}
