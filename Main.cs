using System;
using System.IO;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace MiniTweaksToolbox
{
	internal static class Main
	{
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			mod = modEntry;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			enabled = value;
			modEntry.Logger.Log("Starting MiniTweaksToolbox");

			Settings.Load();

			modEntry.Logger.Log("Loaded");

			return value;
		}

		public static bool enabled;

		public static UnityModManager.ModEntry mod;
	}
}
