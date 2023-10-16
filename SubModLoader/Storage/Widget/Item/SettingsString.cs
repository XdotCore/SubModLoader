﻿using SubModLoader.Storage.Item;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Represents a <see cref="string"/> to be saved in the settings
    /// </summary>
    public sealed class SettingsString : SettingsItem<SettingsString, string, SettingsStringWidget> {
        /// <summary>
        /// Saves the <see cref="string"/> to string to be saved to file
        /// </summary>
        /// <returns>The <see cref="string"/> as a string</returns>
        protected internal override string Save() => Value;
        /// <summary>
        /// Loads the <see cref="string"/> from string from file
        /// </summary>
        /// <param name="value">The <see cref="string"/> as a string</param>
        protected internal override void Load(string value) => Value = value;
        /// <summary>
        /// Draws the <see cref="string"/> gui to the imgui settings screen
        /// </summary>
        protected internal override void ShowSettingsWidget() { }
    }

    /// <summary>
    /// Represents the type of widget this setting uses
    /// </summary>
    public enum SettingsStringWidget {

    }
}
