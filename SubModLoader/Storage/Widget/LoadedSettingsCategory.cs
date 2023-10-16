namespace SubModLoader.Storage.Widget {
    /// <summary>
    /// Used for loading categories before they have been used, and will be replaced once <see cref="Settings.GetCategory"/> or <see cref="SettingsCategory.GetCategory"/> is first called
    /// </summary>
    internal class LoadedSettingsCategory : SettingsCategory {
        internal LoadedSettingsCategory() {
            Visible = false;
        }
    }
}
