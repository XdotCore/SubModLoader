using ImGuiNET;
using SubModLoader.Storage.Widget;
using SubModLoader.Storage.Widget.Item;
using SubModLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SubModLoader.Storage {
    /// <summary>
    /// A save settings implementation
    /// </summary>
    public class Settings {
        #region Instance Implementation

        private string Caller { get; }
        private Dictionary<string, int> CategoryIndices { get; } = new();
        private List<SettingsCategory> Categories { get; } = new();
        private List<int> CategoriesInOrder { get; } = new();

        private Settings(string caller) {
            Caller = caller;
        }

        /// <summary>
        /// Retrieves or creates a category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <param name="showInImGui">Whether or not to show the category in the gui</param>
        /// <param name="displayName">The display name to show in the gui</param>
        /// <param name="tooltip"></param>
        /// <returns>The retrieved category</returns>
        public SettingsCategory GetCategory(string name, bool showInImGui = true, string displayName = null, string tooltip = null) {
            SettingsCategory CreateInstance() => new() {
                Name = name,
                DisplayName = displayName,
                Tooltip = tooltip,
                Visible = showInImGui
            };

            SettingsCategory category;

            if (CategoryIndices.TryGetValue(name, out int index)) {
                category = Categories[index];

                if (category is LoadedSettingsCategory loaded) {
                    category = CreateInstance();
                    SettingsCategory.CopyLoadedToCategory(loaded, category);
                    Categories[index] = category;
                    CategoriesInOrder.Add(index);
                }
            } else {
                category = CreateInstance();
                Categories.Add(category);
                index = Categories.Count - 1;
                CategoryIndices[name] = index;
                CategoriesInOrder.Add(index);
            }

            return category;
        }

        internal LoadedSettingsCategory GetLoadedCategory(string name) {
            LoadedSettingsCategory loaded;

            if (CategoryIndices.TryGetValue(name, out int index))
                loaded = (LoadedSettingsCategory)Categories[index];
            else {
                loaded = new() { Name = name };
                Categories.Add(loaded);
                CategoryIndices[name] = Categories.Count - 1;
            }

            return loaded;
        }

        #endregion

        #region Static Implementation

        private const string LocationDirectory = "SubModLoader/Settings";
        private const string Location = $"{LocationDirectory}/settings.ini";
        private const string SubModLoaderSettingsName = "SubModLoader";

        private static List<Settings> AllSettings { get; } = new();
        internal static Settings SubModLoaderSettings { get; private set; }

        internal static SettingsBool IsSettingsOpen { get; private set; }

        internal static Settings GetSettings(string caller) {
            int index = AllSettings.FindIndex(settings => settings.Caller == caller);
            if (index < 0) {
                Settings settings = new(caller);
                if (caller == SubModLoaderSettingsName)
                    SubModLoaderSettings = settings;

                AllSettings.Add(settings);
                index = AllSettings.Count - 1;
            }
            return AllSettings[index];
        }

        /// <summary>
        /// Saves all settings to file
        /// </summary>
        public static void Save() {
            string save = "";

            foreach (Settings settings in AllSettings) {
                string settingsName;
                if (settings.Caller is not null)
                    settingsName = SettingsRegexHelper.GetEscaped(settings.Caller);
                else
                    settingsName = SubModLoaderSettingsName;
                foreach (SettingsCategory category in settings.Categories) {
                    string categoryName = SettingsRegexHelper.GetEscaped(category.Name);
                    save += $"[{settingsName}##{categoryName}]\n";
                    save += $"{category.Save()}\n";
                }
            }

            File.WriteAllText(Location, save);
        }

        internal static void Load() {
            if (!Directory.Exists(LocationDirectory))
                Directory.CreateDirectory(LocationDirectory);

            if (File.Exists(Location)) {
                try {
                    string save = File.ReadAllText(Location);

                    MatchCollection allSettingsMatches = SettingsRegexHelper.GetAllSettingsMatches(save);

                    foreach (Match settingsMatch in allSettingsMatches.Cast<Match>()) {
                        string names = settingsMatch.Groups[1].Value;
                        string itemsString = settingsMatch.Groups[2].Value;

                        string settingsName, categoryName;
                        (settingsName, categoryName) = SettingsRegexHelper.GetTwoNames(names);
                        if (categoryName is null)
                            throw new NullReferenceException("Category name is null");
                        settingsName = SettingsRegexHelper.GetUnEscaped(settingsName);
                        categoryName = SettingsRegexHelper.GetUnEscaped(categoryName);

                        Settings settings = GetSettings(settingsName);
                        if (!settings.CategoryIndices.ContainsKey(categoryName)) {
                            SettingsCategory category = settings.GetLoadedCategory(categoryName);
                            category.Load(itemsString);
                        }
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                    Console.WriteLine("Exception while loading settings occurred, deleting settings and starting fresh...");
                    File.Delete(Location);
                    Load();
                }
            }

            SettingsCategory settingsSettingsCategory = GetSettings(SubModLoaderSettingsName).GetCategory("Settings", false);
            SubModLoaderSettings.GetCategory("Overlay");
            SubModLoaderSettings.GetCategory("Logger");

            IsSettingsOpen = SettingsBool.Get(settingsSettingsCategory, "IsSettingsOpen", false);
        }

        private static Settings SelectedSettings { get; set; } = null;
        internal static void ShowSettingsWindow() {
            SelectedSettings ??= SubModLoaderSettings;

            if (!IsSettingsOpen.Value)
                return;

            ImGui.SetNextWindowSize(new Vector2(800, 500), ImGuiCond.FirstUseEver);
            bool isOpen = IsSettingsOpen.Value;
            bool collapsed = !ImGui.Begin("Settings##SubModLoader", ref isOpen);
            IsSettingsOpen.Value = isOpen;
            if (collapsed) {
                ImGui.End();
                return;
            }

            if (ImGui.BeginTable("NamesAndSettings", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.Resizable)) {
                ImGui.TableSetupColumn("NamesColumn", ImGuiTableColumnFlags.WidthStretch, 0.25f);
                ImGui.TableSetupColumn("SettingsColumn", ImGuiTableColumnFlags.WidthStretch, 0.75f);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.BeginChild("Names", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

                foreach (Settings settings in AllSettings) {
                    if (!settings.Categories.Any(c => c.Visible))
                        continue;

                    if (ImGui.Selectable(settings.Caller ?? "SubModLoader", settings == SelectedSettings))
                        SelectedSettings = settings;
                }

                ImGui.EndChild();
                ImGui.TableSetColumnIndex(1);
                ImGui.BeginChild("Settings", Vector2.Zero, false, ImGuiWindowFlags.HorizontalScrollbar);

                if (SelectedSettings.Categories.Count > 0 && ImGui.BeginTabBar("SettingsCategories")) {
                    foreach (int index in SelectedSettings.CategoriesInOrder) {
                        SettingsCategory category = SelectedSettings.Categories[index];
                        if (category.Visible)
                            category.ShowSettingsWidget();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.EndChild();
                ImGui.EndTable();
            }

            ImGui.End();
        }

        #endregion
    }
}
