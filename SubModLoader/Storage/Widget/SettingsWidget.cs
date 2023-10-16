namespace SubModLoader.Storage.Widget {
    /// <summary>
    /// The base class for all widgets to show in settings
    /// </summary>
    public abstract class SettingsWidget {
        /// <summary>
        /// The category that contains this widget
        /// </summary>
        public SettingsCategory Category { get; internal set; } = null;
        /// <summary>
        /// The name of the widget
        /// </summary>
        public string Name { get; internal init; }
        private string _displayName = null;
        /// <summary>
        /// The display name to show in the gui, defaults to <see cref="Name"/>
        /// </summary>
        public string DisplayName { get => _displayName ?? Name; set => _displayName = value; }
        /// <summary>
        /// The postfix for imgui widget names used for uniqueness
        /// </summary>
        protected string GUINamePostfix => $"##{Name}{Category?.GUINamePostfix}";
        /// <summary>
        /// The tooltip to be shown in the gui on hover
        /// </summary>
        public string Tooltip { get; set; }
        /// <summary>
        /// Whether or not to show the item in the gui
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Shows the settings widget, e.g. a checkbox or slider
        /// </summary>
        protected internal abstract void ShowSettingsWidget();
    }
}
