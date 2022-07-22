using System;
using System.IO;
using System.Reflection;
using Harmony12;
using UnityModManagerNet;

namespace MiniTweaksToolbox
{
	internal static class Main
	{
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Main.mod = modEntry;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.enabled = value;
			modEntry.Logger.Log("Starting MiniTweaksToolbox");
			Main.path = "Mods/MiniTweaksToolbox/settings/settings.txt";

			if (!Directory.Exists("Mods/MiniTweaksToolbox/settings/"))
				Directory.CreateDirectory("Mods/MiniTweaksToolbox/settings/");

			if (File.Exists(Main.path))
			{
				string text = File.ReadAllText(Main.path);

				Main.tunnedParts = text.Split(',')[0].Split(':')[1].Equals("False") ? false : true;
				Main.groupParts = text.Split(',')[1].Split(':')[1].Equals("False") ? false : true;
				Main.customLPN = text.Split(',')[2].Split(':')[1].Equals("False") ? false : true;
				Main.uncheckedParts = text.Split(',')[3].Split(':')[1].Equals("False") ? false : true;
				Main.invCheck = text.Split(',')[4].Split(':')[1].Equals("False") ? false : true;
				Main.paintParts = text.Split(',')[5].Split(':')[1].Equals("False") ? false : true;
			}
			else
			{
				File.WriteAllText(Main.path, "tunnedParts:False,groupParts:False,customLPN:False,uncheckedParts:False,invCheck:False,paintParts:False,");

				Main.tunnedParts = false;
				Main.groupParts = false;
				Main.customLPN = false;
				Main.uncheckedParts = false;
				Main.invCheck = false;
				Main.paintParts = false;
			}

			modEntry.Logger.Log("Loaded");

			return true;
		}

		internal static bool ToggleTunnedPartsSetting()
		{
			Main.tunnedParts = !Main.tunnedParts;
			Main.SaveSettings();
			return Main.tunnedParts;
		}

		internal static bool ToggleGroupPartsSetting()
		{
			Main.groupParts = !Main.groupParts;
			Main.SaveSettings();
			return Main.groupParts;
		}

		internal static bool ToggleCustomLicensePlatesSetting()
		{
			Main.customLPN = !Main.customLPN;
			Main.SaveSettings();
			return Main.customLPN;
		}

		internal static bool ToggleUncheckedPartsSetting()
		{
			Main.uncheckedParts = !Main.uncheckedParts;
			Main.SaveSettings();
			return Main.uncheckedParts;
		}

		internal static bool ToggleInvCheckSetting()
		{
			Main.invCheck = !Main.invCheck;
			Main.SaveSettings();
			return Main.invCheck;
		}

		internal static bool TogglePaintPartsSetting()
		{
			Main.paintParts = !Main.paintParts;
			Main.SaveSettings();
			return Main.paintParts;
		}

		internal static void SaveSettings()
		{
			File.WriteAllText(Main.path, $"tunnedParts:{Main.tunnedParts},groupParts:{Main.groupParts},customLPN:{Main.customLPN},uncheckedParts:{Main.uncheckedParts},invCheck:{Main.invCheck},paintParts:{Main.paintParts},");
		}

		public static string path;

		public static bool tunnedParts;

		public static bool groupParts;

		public static bool customLPN;

		public static bool uncheckedParts;

		public static bool invCheck;

		public static bool paintParts;

		public static bool dupeBool;

		public static string dupeText;

		public static bool enabled;

		public static UnityModManager.ModEntry mod;
	}
}
