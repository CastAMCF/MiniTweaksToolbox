using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace MiniTweaksToolbox
{
    public class Settings : ModSettings, IDrawable
    {
        internal int selectedTab;

        public void OnChange()
        {
            throw new NotImplementedException();
        }

        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public static void Save_Ops(ModEntry modEntry)
        {
            Save(Main.Settings, modEntry);
        }

        /*[Draw(DrawType.KeyBinding, Label = "Buy")]
        public KeyBinding Buy = new KeyBinding
        {
            keyCode = KeyCode.K
        };

        [Draw(DrawType.KeyBinding, Label = "Buy All")]
        public KeyBinding BuyAll = new KeyBinding
        {
            keyCode = KeyCode.K
        };

        [Draw(DrawType.Field, Label = "Player")]
        public DrawFieldsAttribute Player = new DrawFieldsAttribute(DrawFieldMask.OnlyDrawAttr);

        [Draw(DrawType.KeyBinding, Label = "Sprint")]
        public KeyBinding Sprint = new KeyBinding
        {
            
            keyCode = KeyCode.K
        };

        [Draw(DrawType.KeyBinding, Label = "Jump")]
        public KeyBinding Jump = new KeyBinding
        {
            keyCode = KeyCode.K
        };

        [Draw(DrawType.KeyBinding, Label = "Reset Postion")]
        public KeyBinding ResetPostion = new KeyBinding
        {
            keyCode = KeyCode.K
        };



        [Draw(DrawType.KeyBinding, Label = "Welder")]
        public KeyBinding Welder = new KeyBinding
        {
            keyCode = KeyCode.K
        };

        [Draw(DrawType.KeyBinding, Label = "Interior Detailing Toolkit")]
        public KeyBinding InteriorDetailingToolkit = new KeyBinding
        {
            keyCode = KeyCode.L
        };*/

        public bool tunnedParts;

		public bool groupParts;

		public bool customLPN;

		public bool uncheckedParts;

		public bool invCheck;

		public bool paintParts;

        public bool originalParts;

        public bool autoSelect;

        public bool noOilDrain;

        public bool itemQuality;
        public int quality = 0;

        public static bool ToggleTunnedPartsSetting()
		{
			Main.Settings.tunnedParts = !Main.Settings.tunnedParts;
            Save_Ops(Main.mod);
			return Main.Settings.tunnedParts;
		}

        public static bool ToggleGroupPartsSetting()
		{
			Main.Settings.groupParts = !Main.Settings.groupParts;
            Save_Ops(Main.mod);
			return Main.Settings.groupParts;
		}

        public static bool ToggleCustomLicensePlatesSetting()
		{
			Main.Settings.customLPN = !Main.Settings.customLPN;
            Save_Ops(Main.mod);
			return Main.Settings.customLPN;
		}

        public static bool ToggleUncheckedPartsSetting()
		{
			Main.Settings.uncheckedParts = !Main.Settings.uncheckedParts;
            Save_Ops(Main.mod);
			return Main.Settings.uncheckedParts;
		}

        public static bool ToggleInvCheckSetting()
		{
			Main.Settings.invCheck = !Main.Settings.invCheck;
            Save_Ops(Main.mod);
			return Main.Settings.invCheck;
		}

        public static bool TogglePaintPartsSetting()
		{
            Main.Settings.paintParts = !Main.Settings.paintParts;
            Save_Ops(Main.mod);
			return Main.Settings.paintParts;
		}

        public static bool ToggleOriginalPartsSetting()
        {
            Main.Settings.originalParts = !Main.Settings.originalParts;
            Save_Ops(Main.mod);
            return Main.Settings.originalParts;
        }

        public static bool ToggleAutoSelectSetting()
		{
            Main.Settings.autoSelect = !Main.Settings.autoSelect;
            Save_Ops(Main.mod);
			return Main.Settings.autoSelect;
		}

        public static bool ToggleNoOilDrainSetting()
        {
            Main.Settings.noOilDrain = !Main.Settings.noOilDrain;
            Save_Ops(Main.mod);
            return Main.Settings.noOilDrain;
        }

        public static bool ToggleQualitySetting()
        {
            Main.Settings.itemQuality = !Main.Settings.itemQuality;

            if (Main.Settings.quality == 0)
            {
                Main.Settings.quality = 1;
            }

            Save_Ops(Main.mod);
            return Main.Settings.itemQuality;
        }

        public static object ToggleItemQualitySetting()
        {
			if (Main.Settings.itemQuality && Main.Settings.quality == 5)
			{
                Main.Settings.quality = 0;
                Main.Settings.itemQuality = false;
            }
            else
            {
                Main.Settings.quality++;
				Main.Settings.itemQuality = true;
			}

            Save_Ops(Main.mod);
			return new { flag = Main.Settings.itemQuality, changer = Main.Settings.quality.ToString() };
		}
    }
}
