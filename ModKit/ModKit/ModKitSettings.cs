﻿namespace ModKit {
    public partial class Mod {
        public static ModKitSettings ModKitSettings;
    }
    public class ModKitSettings {
        public static void Save() => Mod.modEntry.SaveSettings("ModKitSettings.xml", Mod.ModKitSettings);
        public static void Load() => Mod.modEntry.LoadSettings("ModKitSettings.xml", ref Mod.ModKitSettings);

        public int browserSearchLimit = 20;
        public int browserDetailSearchLimit = 10;
        public bool toggleKeyBindingsOutputToTranscript = true;

        // Localization
        public string uiCultureCode = "en";
    }
}
