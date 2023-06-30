using ModKit;
using System;
using System.Linq.Expressions;
using UnityEngine;

namespace MiniTweaksToolbox.ModOptions
{
    public static class ModMenuHelper
    {
        public static void NormalKeyBind(string title, string helpString, bool help, Action action)
        {
            using (UI.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                using (UI.VerticalScope(new GUILayoutOption[]
                {
                    120.width()
                }))
                {
                    using (UI.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        switch (title)
                        {
                            case "Use Welder":
                            case "Use Interior Detailing":

                                UI.Label("Use", new GUILayoutOption[] { 120.width() });
                                UI.KeyBindPicker(title, "", 50f, 0f);
                                break;

                            case "Buy Car":

                                UI.Label(title, new GUILayoutOption[] { 120.width() });
                                UI.KeyBindPicker("Buy Part", "", 50f, 0f);
                                break;

                            case "Sprint":

                                UI.Label(title, new GUILayoutOption[] { 120.width() });
                                UI.ModifierPicker(title, "", 50f, 0f);
                                break;

                            default:

                                UI.Label(title, new GUILayoutOption[] { 120.width() });
                                UI.KeyBindPicker(title, "", 50f, 0f);
                                break;
                        }
                    }
                }

                50.space();

                UI.ActionButton("Hint", action, Array.Empty<GUILayoutOption>());

                25.space();

                if (help)
                {
                    using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
                    {
                        UI.HelpLabel(helpString, Array.Empty<GUILayoutOption>());
                    }
                }

            }
        }

        public static void NormalToggles(string title, string helpString, bool help, ref bool setting, Action action)
        {
            using (UI.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                using (UI.VerticalScope(new GUILayoutOption[]
                {
                    120.width()
                }))
                {
                    using (UI.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        UI.Toggle(title, ref setting, new GUILayoutOption[]
                        {
                            120.width()
                        });
                        UI.KeyBindPicker(title, "", 50f, 0f);
                    }
                }

                50.space();

                UI.ActionButton("Hint", action, Array.Empty<GUILayoutOption>());

                25.space();

                if (help)
                {
                    using (UI.VerticalScope(Array.Empty<GUILayoutOption>()))
                    {
                        UI.HelpLabel(helpString, Array.Empty<GUILayoutOption>());
                    }
                }

            }
        }
    }
}
