using ImGuiNET;
using SubModLoader.Storage.Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Represents a <typeparamref name="T"/> to be saved in the settings
    /// </summary>
    public sealed class SettingsEnum<T> : SettingsItem<SettingsEnum<T>, T, SettingsEnumWidget> where T : struct, Enum {
        private static T[] Values { get; } = Enum.GetValues<T>();
        private static int Min { get; } = Convert.ToInt32(Values.Min());
        private static int Max { get; } = Convert.ToInt32(Values.Max());

        /// <summary>
        /// Saves the <typeparamref name="T"/> to string to be saved to file
        /// </summary>
        /// <returns>The <typeparamref name="T"/> as a string</returns>
        protected internal override string Save() => Value.ToString();
        /// <summary>
        /// Loads the <typeparamref name="T"/> from string from file
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> as a string</param>
        protected internal override void Load(string value) => Value = Enum.Parse<T>(value);

        /// <summary>
        /// Draws the <typeparamref name="T"/> gui to the imgui settings screen
        /// </summary>
        protected internal override void ShowSettingsWidget() {
            switch (Widget) {
                case SettingsEnumWidget.Slider: {
                    int value = Convert.ToInt32(Value);
                    ImGui.SliderInt($"{DisplayName}{GUINamePostfix}", ref value, Min, Max, Value.ToString());
                    Value = (T)(object)value;
                }
                break;
                default: {
                    if (ImGui.BeginCombo($"{DisplayName}{GUINamePostfix}", Enum.GetName(Value))) {
                        foreach (T value in Enum.GetValues<T>()) {
                            bool isSelected = EqualityComparer<T>.Default.Equals(value, Value);
                            if (ImGui.Selectable($"{value}{GUINamePostfix}", isSelected))
                                Value = value;
                            if (isSelected)
                                ImGui.SetItemDefaultFocus();
                        }
                        ImGui.EndCombo();
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// Represents the type of widget this setting uses
    /// </summary>
    public enum SettingsEnumWidget {
        /// <summary>
        /// A combo box with a dropdown list
        /// </summary>
        Combo,
        /// <summary>
        /// A value slider
        /// </summary>
        Slider
    }
}
