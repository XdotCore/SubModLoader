using ImGuiNET;
using SubModLoader.Storage.Item;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Represents a <see cref="bool"/> to be saved in the settings
    /// </summary>
    public sealed class SettingsBool : SettingsItem<SettingsBool, bool, SettingsBoolWidget> {
        /// <summary>
        /// Saves the <see cref="bool"/> to string to be saved to file
        /// </summary>
        /// <returns>The <see cref="bool"/> as a string</returns>
        protected internal override string Save() => Value.ToString();
        /// <summary>
        /// Loads the <see cref="bool"/> from string from file
        /// </summary>
        /// <param name="value">The <see cref="bool"/> as a string</param>
        protected internal override void Load(string value) => Value = bool.Parse(value);
        /// <summary>
        /// Draws the <see cref="bool"/> gui to the imgui settings screen
        /// </summary>
        protected internal override void ShowSettingsWidget() {
            switch (Widget) {
                case SettingsBoolWidget.RadioButtons: {
                    if (ImGui.BeginTable($"Table{GUINamePostfix}", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV)) {
                        ImGui.TableNextColumn();

                        if (ImGui.RadioButton($"True{GUINamePostfix}", Value))
                            Value = true;
                        ImGui.SameLine();
                        if (ImGui.RadioButton($"False{GUINamePostfix}", !Value))
                            Value = false;

                        ImGui.TableNextColumn();

                        ImGui.TextUnformatted(DisplayName);

                        ImGui.EndTable();
                    }
                }
                break;
                default: {
                    bool value = Value;
                    ImGui.Checkbox($"{DisplayName}{GUINamePostfix}", ref value);
                    Value = value;
                }
                break;
            }
        }
    }

    /// <summary>
    /// Represents the type of widget this setting uses
    /// </summary>
    public enum SettingsBoolWidget {
        /// <summary>
        /// A check box
        /// </summary>
        CheckBox,
        /// <summary>
        /// Two radio buttons true and false
        /// </summary>
        RadioButtons
    }
}
