using SubModLoader.Storage.Widget;

namespace SubModLoader.Storage.Item {
    /// <summary>
    /// The non-generic base class for <see cref="SettingsItem{TSelf, T, TWidget}"/>
    /// </summary>
    public abstract class SettingsItem : SettingsWidget {
        /// <summary>
        /// The value as an object for the non-generic base class
        /// </summary>
        public abstract object ValueObject { get; set; }

        /// <summary>
        /// Saves the item to string to be saved to file
        /// </summary>
        /// <returns>The item as a string</returns>
        protected internal abstract string Save();
        /// <summary>
        /// Loads the item from string from file
        /// </summary>
        /// <param name="value">The item as a string</param>
        protected internal abstract void Load(string value);
    }

    // TODO: add changed events
    /// <summary>
    /// Represents a value to be saved in the settings
    /// </summary>
    /// <typeparam name="TSelf">The type of item class</typeparam>
    /// <typeparam name="T">The type of the item value</typeparam>
    /// <typeparam name="TWidget">The widget choices for the item</typeparam>
    public abstract partial class SettingsItem<TSelf, T, TWidget> : SettingsItem where TSelf : SettingsItem<TSelf, T, TWidget>, new() {
        internal bool IsLoading;
        private T _value;
        /// <summary>
        /// The value of the <typeparamref name="T"/>
        /// </summary>
        public T Value {
            get => _value;
            set {
                if (!value.Equals(_value)) {
                    _value = value;
                    if (!IsLoading)
                        Settings.Save();
                    else
                        IsLoading = false;
                }
            }
        }
        private T _defaultValue;
        /// <summary>
        /// The initial value of the <typeparamref name="T"/>
        /// </summary>
        public T DefaultValue {
            get => _defaultValue;
            init => _value = _defaultValue = value;
        }
        /// <summary>
        /// The choice of widget to be shown in the gui
        /// </summary>
        public TWidget Widget { get; set; }
        /// <inheritdoc/>
        public override object ValueObject { get => Value; set => Value = (T)value; }

        /// <summary>
        /// For when the value of the <typeparamref name="T"/> has changed
        /// </summary>
        /// <param name="newValue">The new <typeparamref name="T"/></param>
        public delegate void ValueChangedDelegate(T newValue);
        /// <summary>
        /// Called when the value 
        /// </summary>
        public event ValueChangedDelegate OnValueChanged;

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        /// <summary>
        /// Retrieves or creates a <typeparamref name="T"/> item
        /// </summary>
        /// <param name="category">The category to retrieve from</param>
        /// <inheritdoc cref="SettingsCategory.GetItem"/>
        /// <returns>The <typeparamref name="T"/> item</returns>
        public static TSelf Get(SettingsCategory category, string name, T defaultValue, bool showInImGui = false, string displayName = null, string tooltip = null, TWidget widgetType = default) =>
            category.GetItem<TSelf, T, TWidget>(name, defaultValue, showInImGui, displayName, tooltip, widgetType);
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    }
}
