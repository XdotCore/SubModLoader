using SubModLoader.Utils;
using System;

namespace SubModLoader.Mods.Attributes {
    /// <summary>
    /// Defines the colors used in the console for the class
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SubModColorAttribute : Attribute {
        /// <summary>
        /// The color for when the author is printed
        /// </summary>
        public Color AuthorColor { get; }

        /// <summary>
        /// Standard ctor
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        public SubModColorAttribute(byte r, byte g, byte b) {
            AuthorColor = new(r, g, b);
        }

        /// <summary>
        /// Standard ctor
        /// </summary>
        /// <param name="color">The console color</param>
        public SubModColorAttribute(Color.ConsoleForegroundColor color) {
            AuthorColor = Color.GetByConsoleForeground(color);
        }
    }
}
