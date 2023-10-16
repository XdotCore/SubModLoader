using SubModLoader.Storage.Item;
using System.Globalization;
using System.Numerics;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Represents a <typeparamref name="T"/> to be saved in the settings
    /// </summary>
    public sealed class SettingsInteger<T> : SettingsItem<SettingsInteger<T>, T, SettingsIntegerWidget> where T : IBinaryInteger<T> {
        /// <summary>
        /// Saves the <typeparamref name="T"/> to string to be saved to file
        /// </summary>
        /// <returns>The <typeparamref name="T"/> as a string</returns>
        protected internal override string Save() => Value.ToString();
        /// <summary>
        /// Loads the <typeparamref name="T"/> from string from file
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> as a string</param>
        protected internal override void Load(string value) => Value = T.Parse(value, NumberFormatInfo.CurrentInfo);

        /// <summary>
        /// Draws the <typeparamref name="T"/> gui to the imgui settings screen
        /// </summary>
        protected internal override void ShowSettingsWidget() { }
    }

    /// <summary>
    /// Represents the type of widget this setting uses
    /// </summary>
    public enum SettingsIntegerWidget {

    }
}
