using SubModLoader.Storage.Item;

namespace SubModLoader.Storage.Widget.Item {
    /// <summary>
    /// Used for loading items before they have been used, and will be replaced once <see cref="SettingsItem{TSelf, T, TWidget}.Get"/> is first called
    /// </summary>
    internal class LoadedSettingsItem : SettingsItem<SettingsString, string, SettingsStringWidget> {
        public LoadedSettingsItem() {
            Visible = false;
        }

        /// <inheritdoc cref="LoadedSettingsItem"/>
        protected internal override string Save() => Value;
        /// <inheritdoc cref="LoadedSettingsItem"/>
        protected internal override void Load(string value) => Value = value;
        /// <inheritdoc cref="LoadedSettingsItem"/>
        protected internal override void ShowSettingsWidget() { }
    }
}
