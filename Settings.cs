using System.IO;

namespace MiniTweaksToolbox
{
	internal class Settings
	{
		public static string filePath = "Mods/MiniTweaksToolbox/settings/settings.txt";

		public static string path = "Mods/MiniTweaksToolbox/settings/";

		public static bool tunnedParts;

		public static bool groupParts;

		public static bool customLPN;

		public static bool uncheckedParts;

		public static bool invCheck;

		public static bool paintParts;

		public static bool autoSelect;

		internal static bool ToggleTunnedPartsSetting()
		{
			tunnedParts = !tunnedParts;
			Save();
			return tunnedParts;
		}

		internal static bool ToggleGroupPartsSetting()
		{
			groupParts = !groupParts;
			Save();
			return groupParts;
		}

		internal static bool ToggleCustomLicensePlatesSetting()
		{
			customLPN = !customLPN;
			Save();
			return customLPN;
		}

		internal static bool ToggleUncheckedPartsSetting()
		{
			uncheckedParts = !uncheckedParts;
			Save();
			return uncheckedParts;
		}

		internal static bool ToggleInvCheckSetting()
		{
			invCheck = !invCheck;
			Save();
			return invCheck;
		}

		internal static bool TogglePaintPartsSetting()
		{
			paintParts = !paintParts;
			Save();
			return paintParts;
		}

		internal static bool ToggleAutoSelectSetting()
		{
			autoSelect = !autoSelect;
			Save();
			return autoSelect;
		}

		internal static void Load()
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			if (File.Exists(filePath))
			{
				string text = File.ReadAllText(filePath);

				tunnedParts = text.Split(',')[0].Split(':')[1].Equals("True");
				groupParts = text.Split(',')[1].Split(':')[1].Equals("True");
				customLPN = text.Split(',')[2].Split(':')[1].Equals("True");
				uncheckedParts = text.Split(',')[3].Split(':')[1].Equals("True");
				invCheck = text.Split(',')[4].Split(':')[1].Equals("True");
				paintParts = text.Split(',')[5].Split(':')[1].Equals("True");
				autoSelect = text.Split(',')[6].Split(':')[1].Equals("True");
			}
			else
			{
				File.WriteAllText(filePath, "tunnedParts:False,groupParts:False,customLPN:False,uncheckedParts:False,invCheck:False,paintParts:False,autoSelect:False,");

				tunnedParts = false;
				groupParts = false;
				customLPN = false;
				uncheckedParts = false;
				invCheck = false;
				paintParts = false;
				autoSelect = false;
			}
		}

		internal static void Save()
		{
			File.WriteAllText(filePath, $"tunnedParts:{tunnedParts},groupParts:{groupParts},customLPN:{customLPN},uncheckedParts:{uncheckedParts},invCheck:{invCheck},paintParts:{paintParts},autoSelect:{autoSelect},");
		}
	}
}
