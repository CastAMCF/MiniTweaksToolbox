using ModKit;
using System;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class Radio
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
                ModMenuHelper.NormalKeyBind("Start/Stop", "You really need a hint?",
                    radioHelp, delegate { UpdateBool("Start/Stop"); });

                ModMenuHelper.NormalKeyBind("Next Song", "Its in the name",
                    nextSongHelp, delegate { UpdateBool("Next Song"); });
            }
            //}};

            //UI.HStack("Radio", 1, elems);
            UI.Div(0f, 25f, 0f);
        }

        public static void UpdateBool(string ident)
        {
            switch (ident)
            {
                case "Start/Stop":
                    radioHelp = !radioHelp;
                    break;

                case "Next Song":
                    nextSongHelp = !nextSongHelp;
                    break;

                default:
                    break;
            }
        }

        private static bool radioHelp = false;

        private static bool nextSongHelp = false;
    }
}
