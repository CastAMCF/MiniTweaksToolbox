using System;

namespace ModKit {

    public static partial class Glyphs {
        public static string DefaultCheckOn = "✔";
        public static string DefaultCheckOff = "✖";
        public static string DefaultCheckEmpty = "▪";
        public static string DefaultDisclosureOn = "▼";
        public static string DefaultDisclosureOff = "▶";
        public static string DefaultDisclosureEmpty = "▪";
        public static string DefaultEdit = "✎";
        public static string CharCodeCheckOn = "[x]";
        public static string CharCodeCheckOff = "<b><color=green>[</color><color=red>o</color><color=green>]</color></b>";
        public static string CharCodeCheckEmpty = "<b> <color=yellow>-</color> </b>";
        public static string CharCodeDisclosureOn = "v";
        public static string CharCodeDisclosureOff = ">";
        public static string CharCodeDisclosureEmpty = "-";
        public static string CharCodeEdit = "edit";

        private static bool UseDefaultGlyphs = !IsLinux();
        private static bool IsLinux()
        {
            int platform = (int)Environment.OSVersion.Platform;
            return platform == 4 || platform == 6 || platform == 128;
        }

        public static string CheckOn
        {
            get { return UseDefaultGlyphs ? DefaultCheckOn : CharCodeCheckOn; }
        }
        public static string CheckOff
        {
            get { return UseDefaultGlyphs ? DefaultCheckOff : CharCodeCheckOff; }
        }
        public static string CheckEmpty
        {
            get { return UseDefaultGlyphs ? DefaultCheckEmpty : CharCodeCheckEmpty; }
        }
        public static string DisclosureOn
        {
            get { return UseDefaultGlyphs ? DefaultDisclosureOn : CharCodeDisclosureOn; }
        }
        public static string DisclosureOff
        {
            get { return UseDefaultGlyphs ? DefaultDisclosureOff : CharCodeDisclosureOff; }
        }
        public static string DisclosureEmpty
        {
            get { return UseDefaultGlyphs ? DefaultDisclosureEmpty : CharCodeDisclosureEmpty; }
        }
        public static string Edit
        {
            get { return UseDefaultGlyphs ? DefaultEdit : CharCodeEdit; }
        }
    }
}
