using System;
using System.Reflection;
using Harmony12;
using MiniTweaksToolbox.ModOptions;
using ModKit;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace MiniTweaksToolbox
{
	internal static class Main
	{
		private static bool Load(ModEntry modEntry)
		{
            Mod.OnLoad(modEntry);
            Settings = ModSettings.Load<Settings>(modEntry);

            modEntry.OnGUI = new Action<ModEntry>(OnGUI);
            modEntry.OnSaveGUI = new Action<ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new Func<ModEntry, bool, bool>(OnToggle);

            ModKit.UI.KeyBindings.OnLoad(modEntry);

            mod = modEntry;

            return true;
		}

        private static void OnGUI(ModEntry modEntry)
        {
            GUILayout.Box("<b>Your current CMS2018 Version is " + Application.version + " (" + GameSettings.m_buildVersion + ")</b>", new GUILayoutOption[0]);

            ModKit.UI.Space(2f);
            ModKit.UI.Label("Note ".magenta().bold() + "If you get some frame drops close this mod option tab".orange().bold(), new GUILayoutOption[0]);
            ModKit.UI.Space(2f);

            try
            {
                ModKit.UI.TabBar(ref Settings.selectedTab, delegate
                {
                    ModKit.UI.Space(15f);
                }, new NamedAction[]
                    {
                        new NamedAction("Menu", new Action(ModMenu.OnGUI), null),
                        new NamedAction("Player", new Action(Player.OnGUI), null),
                        new NamedAction("Shopping", new Action(Shopping.OnGUI), null),
                        new NamedAction("Car", new Action(Car.OnGUI), null),
                        new NamedAction("Tools", new Action(Tools.OnGUI), null),
                        new NamedAction("Paint", new Action(Paint.OnGUI), null),
                        new NamedAction("Radio", new Action(Radio.OnGUI), null),
                        new NamedAction("Other", new Action(Other.OnGUI), null),
                    });
            }
            catch (Exception ex)
            {
                modEntry.Logger.Log(string.Format("{0}", ex));
            }
            
        }

        private static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        private static bool OnToggle(ModEntry modEntry, bool value)
		{
            if (value == enabled)
            {
                return true;
            }

            enabled = value;
            
            if (enabled)
			{
                modEntry.Logger.Log("Starting MiniTweaksToolbox");

                harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

                modEntry.Logger.Log("Loaded");
            }
			else
			{
                modEntry.Logger.Log("Stopping MiniTweaksToolbox");

                harmonyInstance.UnpatchAll(modEntry.Info.Id);

                modEntry.Logger.Log("Stopped Successfully");
            }
                

			return true;
		}

        public static Settings Settings;

        public static HarmonyInstance harmonyInstance;

        public static bool enabled;

		public static ModEntry mod;
    }
}
