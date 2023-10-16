using SubModLoader.Utils;
using System;

namespace SubModLoader.Mods.Attributes {
    internal interface ISubModInfoAttribute {
        Type ModType { get; }
        string Name { get; }
        string Version { get; }
        string Author { get; }
    }

    /// <summary>
    /// Defines data about your mod including the entrypoint
    /// </summary>
    /// <typeparam name="ModClass">The class type used for the mod entrypoint</typeparam>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SubModInfoAttribute<ModClass> : Attribute, ISubModInfoAttribute where ModClass : SubMod {
        /// <summary>
        /// The class type for the mod entrypoint
        /// </summary>
        public Type ModType { get; }
        /// <summary>
        /// The name of the mod
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The version of the mod
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// The author of the mod
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Standard ctor
        /// </summary>
        /// <param name="name">The name of the mod</param>
        /// <param name="version">The version of the mod</param>
        /// <param name="author">The author of the mod</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> must not be null</exception>
        public SubModInfoAttribute(string name, string version, string author) {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            if (name == "SubModLoader" || name == Logger.GameNamePlaceholder)
                throw new ArgumentException($"{name} is a reserved name, it cannot be used for mod names", nameof(name));

            ModType = typeof(ModClass);
            Name = name;
            Version = version ?? "0.0.0";
            Author = author ?? "UNKNOWN";
        }
    }
}
