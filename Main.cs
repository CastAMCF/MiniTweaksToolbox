using System;
using System.IO;
using System.Reflection;
using Harmony12;
using UnityModManagerNet;

namespace QuickShopEnhanced
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
			modEntry.Logger.Log("Starting QuickShopEnhanced");
			Main.path = "Mods/QuickShopEnhanced/settings/settings.txt";

			if (!Directory.Exists("Mods/QuickShopEnhanced/settings/"))
				Directory.CreateDirectory("Mods/QuickShopEnhanced/settings/");

			if (File.Exists(Main.path))
			{
				string text = File.ReadAllText(Main.path);

				Main.tunnedParts = text.Split(',')[0].Split(':')[1].Equals("false") ? false : true;
				Main.groupParts = text.Split(',')[1].Split(':')[1].Equals("false") ? false : true;
				Main.customLPN = text.Split(',')[2].Split(':')[1].Equals("false") ? false : true;
			}
			else
			{
				File.WriteAllText(Main.path, "tunnedParts:false,groupParts:false,customLPN:false,");

				Main.tunnedParts = false;
				Main.groupParts = false;
				Main.customLPN = false;
			}

			modEntry.Logger.Log("Loaded");

			return true;
		}

		public static string path;

		public static bool tunnedParts;

		public static bool groupParts;

		public static bool customLPN;

		public static bool enabled;

		public static UnityModManager.ModEntry mod;
	}
}
