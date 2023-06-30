using System.IO;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;
using System;
using System.Xml.Serialization;

namespace ModKit {
    public interface IUpdatableSettings {
        void AddMissingKeys(IUpdatableSettings from);
    }

    internal static class SettingsController {
        public static void SaveSettings<T>(this ModEntry modEntry, string fileName, T settings) {
            var userConfigFolder = modEntry.Path + "UserSettings";
            Directory.CreateDirectory(userConfigFolder);
            var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(userPath, FileMode.Create))
            {
                serializer.Serialize(stream, settings);
            }
        }
        public static void LoadSettings<T>(this ModEntry modEntry, string fileName, ref T settings) where T : new() {
            settings = new T { };
            var serializer = new XmlSerializer(typeof(T));
            var assembly = Assembly.GetExecutingAssembly();
            var userConfigFolder = modEntry.Path + "UserSettings";
            Directory.CreateDirectory(userConfigFolder);
            var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";
            try {
                foreach (var res in assembly.GetManifestResourceNames()) {
                    //Logger.Log("found resource: " + res);
                    if (res.Contains(fileName)) {
                        var stream = assembly.GetManifestResourceStream(res);
                        using StreamReader reader = new(stream);
                        var text = reader.ReadToEnd();
                        //Logger.Log($"read: {text}");
                        using var stringReader = new StringReader(text);
                        settings = (T)serializer.Deserialize(stringReader);

                        //Logger.Log($"read settings: {string.Join(Environment.NewLine, settings)}");
                    }
                }
            }
            catch (Exception e) {
                Mod.Error($"{fileName} resource is not present or is malformed. exception: {e}");
            }
            if (File.Exists(userPath)) {
                using var reader = File.OpenText(userPath);
                try {
                    var userSettings = (T)serializer.Deserialize(reader);
                    if (userSettings is IUpdatableSettings updatableSettings) {
                        updatableSettings.AddMissingKeys((IUpdatableSettings)settings);
                    }
                    settings = userSettings;
                }
                catch {
                    Mod.Error("Failed to load user settings. Settings will be rebuilt.");
                    try { File.Copy(userPath, userConfigFolder + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true); }
                    catch { Mod.Error("Failed to archive broken settings."); }
                }
            }
            using (var stream = new FileStream(userPath, FileMode.Create))
            {
                serializer.Serialize(stream, settings);
            }
        }
    }
}