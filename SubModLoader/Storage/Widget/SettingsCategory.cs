using ImGuiNET;
using SubModLoader.Storage.Item;
using SubModLoader.Storage.Widget.Item;
using SubModLoader.Storage.Widget.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubModLoader.Storage.Widget {
    /// <summary>
    /// Represents a tabbed category in the settings
    /// </summary>
    public class SettingsCategory : SettingsWidget {
        private Dictionary<string, int> CategoryIndices { get; } = new();
        private List<SettingsCategory> Categories { get; } = new();
        private List<int> CategoriesInOrder { get; } = new();

        private Dictionary<string, int> UIWidgetIndices { get; } = new();
        private List<SettingsUIWidget> UIWidgets { get; } = new();
        private Dictionary<string, int> ItemIndices { get; } = new();
        private List<SettingsItem> Items { get; } = new();
        private List<(IList list, int index)> WidgetsInOrder { get; } = new();

        internal SettingsCategory() { }

        /// <summary>
        /// Retrieves or creates a subcategory
        /// </summary>
        /// <param name="name">The name of the subcategory</param>
        /// <param name="showInImGui">Whether or not to show the subcategory in the gui</param>
        /// <param name="displayName">The display name to show in the gui</param>
        /// <param name="tooltip"></param>
        /// <returns>The retrieved subcategory</returns>
        public SettingsCategory GetCategory(string name, bool showInImGui = true, string displayName = null, string tooltip = null) {
            SettingsCategory CreateInstance() => new() {
                Category = this,
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
                    CopyLoadedToCategory(loaded, category);
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
                loaded = new() {
                    Category = this,
                    Name = name
                };
                Categories.Add(loaded);
                CategoryIndices[name] = Categories.Count - 1;
            }

            return loaded;
        }

        internal static void CopyLoadedToCategory(LoadedSettingsCategory loaded, SettingsCategory category) {
            foreach (KeyValuePair<string, int> kvp in loaded.CategoryIndices)
                category.CategoryIndices.Add(kvp.Key, kvp.Value);
            foreach (SettingsCategory subcategory in loaded.Categories)
                category.Categories.Add(subcategory);

            foreach (KeyValuePair<string, int> kvp in loaded.ItemIndices)
                category.ItemIndices.Add(kvp.Key, kvp.Value);
            foreach (SettingsItem item in loaded.Items)
                category.Items.Add(item);
        }

        /// <summary>
        /// Retrieves or creates a ui widget
        /// </summary>
        /// <param name="name">The name of the ui widget</param>
        /// <param name="displayName">The display name to show in the gui</param>
        /// <param name="tooltip">The tooltip to be shown in the gui on hover</param>
        public T GetUI<T>(string name, string displayName = null, string tooltip = null) where T : SettingsUIWidget, new() {
            T CreateInstance() => new() {
                Category = this,
                Name = name,
                DisplayName = displayName,
                Tooltip = tooltip
            };

            T uiWidget;

            if (UIWidgetIndices.TryGetValue(name, out int index))
                uiWidget = (T)UIWidgets[index];
            else {
                uiWidget = CreateInstance();
                UIWidgets.Add(uiWidget);
                index = UIWidgets.Count - 1;
                UIWidgetIndices[name] = index;
                WidgetsInOrder.Add((UIWidgets, index));
            }

            return uiWidget;
        }

        /// <summary>
        /// Retrieves or creates a <typeparamref name="T"/> item
        /// </summary>
        /// <inheritdoc cref="SettingsItem{TSelf, T, TWidget}"/>
        /// <param name="name">The name of the <typeparamref name="T"/> item without whitespace</param>
        /// <param name="defaultValue">The value of the <typeparamref name="T"/> item if it has not been set</param>
        /// <param name="showInImGui">Whether or not to show the <typeparamref name="T"/> item in the gui</param>
        /// <param name="displayName">The display name to show in the gui</param>
        /// <param name="tooltip">The tooltip to be shown in the gui on hover</param>
        /// <param name="widget">The choice of widget to be shown in the gui</param>
        /// <returns>The <typeparamref name="T"/> item</returns>
        public TSelf GetItem<TSelf, T, TWidget>(string name, T defaultValue, bool showInImGui = false, string displayName = null, string tooltip = null, TWidget widget = default) where TSelf : SettingsItem<TSelf, T, TWidget>, new() {
            TSelf CreateInstance() => new() {
                Category = this,
                Name = name,
                DefaultValue = defaultValue,
                Visible = showInImGui,
                DisplayName = displayName,
                Tooltip = tooltip,
                Widget = widget
            };

            TSelf item;

            if (ItemIndices.TryGetValue(name, out int index)) {
                SettingsItem baseItem = Items[index];

                if (baseItem is LoadedSettingsItem loaded) {
                    item = CreateInstance();
                    item.IsLoading = true;
                    item.Load(loaded.Value);
                    Items[index] = item;
                    WidgetsInOrder.Add((Items, index));
                } else
                    item = (TSelf)baseItem;
            } else {
                item = CreateInstance();
                Items.Add(item);
                index = Items.Count - 1;
                ItemIndices[name] = index;
                WidgetsInOrder.Add((Items, index));
                Settings.Save();
            }

            return item;
        }

        private void AddLoadedItem(string name, string itemString) {
            if (!ItemIndices.ContainsKey(name)) {
                LoadedSettingsItem category = new() {
                    Name = name,
                    DefaultValue = itemString
                };
                Items.Add(category);
                ItemIndices[name] = Items.Count - 1;
            }
        }

        internal string Save(string namePostfix = "") {
            string save = "";

            foreach (SettingsItem item in Items) {
                string itemName = item.Name;
                itemName = SettingsRegexHelper.GetEscaped(itemName);
                itemName += namePostfix;

                string itemSave = item.Save();
                itemSave = SettingsRegexHelper.GetEscaped(itemSave);

                save += $"{itemName}=\"{itemSave}\"\n";
            }

            string name = SettingsRegexHelper.GetEscaped(Name);
            foreach (SettingsCategory subcategory in Categories) {
                string subcategorySave = subcategory.Save($"{namePostfix}##{name}");
                save += subcategorySave;
            }

            return save;
        }

        internal void Load(string categorySave) {
            MatchCollection items = SettingsRegexHelper.GetAllItemsMatches(categorySave);

            foreach (Match item in items.Cast<Match>()) {
                string itemName = item.Groups[1].Value;
                string itemValue = item.Groups[2].Value;

                SettingsCategory category = this;
                string categoryName;
                while (((itemName, categoryName) = SettingsRegexHelper.GetTwoNames(itemName)).categoryName is not null) {
                    categoryName = SettingsRegexHelper.GetUnEscaped(categoryName);
                    category = category.GetLoadedCategory(categoryName);
                }

                itemName = SettingsRegexHelper.GetUnEscaped(itemName);
                itemValue = SettingsRegexHelper.GetUnEscaped(itemValue);

                category.AddLoadedItem(itemName, itemValue);
            }
        }

        /// <inheritdoc/>
        protected internal override void ShowSettingsWidget() {
            if (ImGui.BeginTabItem($"{DisplayName}{GUINamePostfix}")) {
                if (Tooltip is not null)
                    ImGui.SetItemTooltip(Tooltip);

                foreach ((IList list, int index) in WidgetsInOrder) {
                    SettingsWidget widget = (SettingsWidget)list[index];

                    if (widget.Visible) {
                        widget.ShowSettingsWidget();

                        if (widget.Tooltip is not null) {
                            ImGui.SameLine();
                            ImGui.TextDisabled("(?)");
                            ImGui.SetItemTooltip(widget.Tooltip);
                        }
                    }
                }

                if (Categories.Count > 0 && ImGui.BeginTabBar($"SubCategories{GUINamePostfix}")) {
                    foreach (int index in CategoriesInOrder) {
                        SettingsCategory subcategory = Categories[index];
                        if (subcategory.Visible)
                            subcategory.ShowSettingsWidget();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.EndTabItem();
            }
        }
    }
}
