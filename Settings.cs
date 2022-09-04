using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static HBAO_Core;

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

        public static bool noOilDrain;

        public static bool itemQuality;
        public static int quality = 0;

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

        internal static bool ToggleNoOilDrainSetting()
        {
            noOilDrain = !noOilDrain;
            Save();
            return noOilDrain;
        }

        internal static object ToggleItemQualitySetting()
        {
			if (itemQuality && quality == 5)
			{
				quality = 0;
                itemQuality = false;
            }
            else
            {
                quality++;
				itemQuality = true;
			}

			Save();
			return new { flag = itemQuality, changer = quality.ToString() };
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
                noOilDrain = text.Split(',')[7].Split(':')[1].Equals("True");
                itemQuality = text.Split(',')[8].Split(':')[1].Equals("True");
                quality = int.Parse(text.Split(',')[8].Split(':')[2]);

            }
			else
			{
				File.WriteAllText(filePath, "tunnedParts:False,groupParts:False,customLPN:False,uncheckedParts:False,invCheck:False,paintParts:False,autoSelect:False,noOilDrain:False,itemQuality:False:0,");

				tunnedParts = false;
				groupParts = false;
				customLPN = false;
				uncheckedParts = false;
				invCheck = false;
				paintParts = false;
				autoSelect = false;
                noOilDrain = false;
                itemQuality = false;
                quality = 0;

            }
		}

		internal static void Save()
		{
			File.WriteAllText(filePath, $"tunnedParts:{tunnedParts},groupParts:{groupParts},customLPN:{customLPN},uncheckedParts:{uncheckedParts},invCheck:{invCheck},paintParts:{paintParts},autoSelect:{autoSelect},noOilDrain:{noOilDrain},itemQuality:{itemQuality}:{quality},");
		}
	}
}
