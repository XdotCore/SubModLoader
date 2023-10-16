using SubModLoader.Storage.Item;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Represents a <see cref="char"/> to be saved in the settings
    /// </summary>
    public sealed class SettingsChar : SettingsItem<SettingsChar, char, SettingsCharWidget> {
        /// <summary>
        /// Saves the <see cref="char"/> to string to be saved to file
        /// </summary>
        /// <returns>The <see cref="char"/> as a string</returns>
        protected internal override string Save() => Value.ToString();
        /// <summary>
        /// Loads the <see cref="char"/> from string from file
        /// </summary>
        /// <param name="value">The <see cref="char"/> as a string</param>
        protected internal override void Load(string value) => Value = value[0];
        /// <summary>
        /// Draws the <see cref="char"/> gui to the imgui settings screen
        /// </summary>
        protected internal override void ShowSettingsWidget() { }
    }

    /// <summary>
    /// Represents the type of widget this setting uses
    /// </summary>
    public enum SettingsCharWidget {

    }
}
