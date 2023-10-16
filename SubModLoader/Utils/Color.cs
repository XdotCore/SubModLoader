using SubmachineModLib;
using SubModLoader.GameData.Extensions;
using SubModLoader.GMLInterop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SubModLoader.Utils {
    /// <summary>
    /// Represents a color
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Color {
        private readonly ColorType _type = ColorType.Empty;

        private readonly byte _r = 0;
        private readonly byte _g = 0;
        private readonly byte _b = 0;
        private readonly byte _a = 0;

        private readonly uint _rgba = 0;
        private readonly uint _rgb = 0;

        private readonly ConsoleForegroundColor _foreground = ConsoleForegroundColor.None;
        private readonly ConsoleBackgroundColor _background = ConsoleBackgroundColor.None;

        /// <summary>
        /// Gets the <see cref="ColorType"/> of the color
        /// </summary>
        public readonly ColorType Type => _type;

        /// <summary>
        /// The red component of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly byte R => _r;
        /// <summary>
        /// The green component of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly byte G => _g;
        /// <summary>
        /// The blue component of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly byte B => _b;
        /// <summary>
        /// The alpha component of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly byte A => _a;
        /// <summary>
        /// The the combined red, green, blue, and alpha components of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly uint RGBA => _rgba;
        /// <summary>
        /// The the combined red, green, and blue components of the color if <see cref="Type"/> is <see cref="ColorType.RGBA"/>
        /// </summary>
        public readonly uint RGB => _rgb;

        /// <summary>
        /// The color if used as a forground console color if <see cref="Type"/> is <see cref="ColorType.Console"/>
        /// </summary>
        public readonly ConsoleForegroundColor Foreground => _foreground;
        /// <summary>
        /// The color if used as a background console color if <see cref="Type"/> is <see cref="ColorType.Console"/>
        /// </summary>
        public readonly ConsoleBackgroundColor Background => _background;

        /// <summary>
        /// Creates a new <see cref="ColorType.RGBA"/> color with the given red, green, blue, and alpha components
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        public Color(byte r, byte g, byte b, byte a) {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
            _rgba = (uint)(r << 24 | g << 16 | b << 8 | a);
            _rgb = (uint)(r << 16 | g << 8 | b);
            _type = ColorType.RGBA;
        }

        /// <summary>
        /// Creates a new <see cref="ColorType.RGBA"/> color with the given red, green, and blue components
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <remarks>The alpha component will be set as 255</remarks>
        public Color(byte r, byte g, byte b) : this(r, g, b, 255) { }

        /// <summary>
        /// Creates a new <see cref="ColorType.RGBA"/> color with the given rgba <see cref="int"/>
        /// </summary>
        /// <param name="rgba">An rgba <see cref="int"/></param>
        public Color(uint rgba) {
            _r = (byte)(rgba >> 24);
            _g = (byte)(rgba >> 16);
            _b = (byte)(rgba >> 8);
            _a = (byte)rgba;
            _rgba = rgba;
            _rgb = rgba >> 8;
            _type = ColorType.RGBA;
        }

        private Color(ConsoleForegroundColor foreground, ConsoleBackgroundColor background, /* For ImGui console */uint rgba) : this(rgba) {
            _foreground = foreground;
            _background = background;
            _type = ColorType.Console;
        }

        /// <summary>
        /// This constructor is not recommended. Use <see cref="Empty"/> instead.
        /// </summary>
        [Obsolete("This constructor is not recommended. Use Color.Empty or default instead.")]
        public Color() { }

        /// <summary>
        /// Gets the corresponding console <see cref="Color"/> by the <see cref="ConsoleForegroundColor"/>
        /// </summary>
        /// <param name="color">The color to get</param>
        /// <returns>The corresponding color</returns>
        public static Color GetByConsoleForeground(ConsoleForegroundColor color) => color switch {
            ConsoleForegroundColor.Black => ConsoleBlack,
            ConsoleForegroundColor.Red => ConsoleRed,
            ConsoleForegroundColor.Green => ConsoleGreen,
            ConsoleForegroundColor.Yellow => ConsoleYellow,
            ConsoleForegroundColor.Blue => ConsoleBlue,
            ConsoleForegroundColor.Magenta => ConsoleMagenta,
            ConsoleForegroundColor.Cyan => ConsoleCyan,
            ConsoleForegroundColor.White => ConsoleWhite,
            ConsoleForegroundColor.BoldBlack => ConsoleBoldBlack,
            ConsoleForegroundColor.BoldRed => ConsoleBoldRed,
            ConsoleForegroundColor.BoldGreen => ConsoleBoldGreen,
            ConsoleForegroundColor.BoldYellow => ConsoleBoldYellow,
            ConsoleForegroundColor.BoldBlue => ConsoleBoldBlue,
            ConsoleForegroundColor.BoldMagenta => ConsoleBoldMagenta,
            ConsoleForegroundColor.BoldCyan => ConsoleBoldCyan,
            ConsoleForegroundColor.BoldWhite => ConsoleBoldWhite,
            _ => Empty,
        };

        /// <summary>
        /// Gets the corresponding console <see cref="Color"/> by the <see cref="ConsoleBackgroundColor"/>
        /// </summary>
        /// <param name="color">The color to get</param>
        /// <returns>The corresponding color</returns>
        public static Color GetByConsoleBackground(ConsoleBackgroundColor color) => color switch {
            ConsoleBackgroundColor.Black => ConsoleBlack,
            ConsoleBackgroundColor.Red => ConsoleRed,
            ConsoleBackgroundColor.Green => ConsoleGreen,
            ConsoleBackgroundColor.Yellow => ConsoleYellow,
            ConsoleBackgroundColor.Blue => ConsoleBlue,
            ConsoleBackgroundColor.Magenta => ConsoleMagenta,
            ConsoleBackgroundColor.Cyan => ConsoleCyan,
            ConsoleBackgroundColor.White => ConsoleWhite,
            ConsoleBackgroundColor.BoldBlack => ConsoleBoldBlack,
            ConsoleBackgroundColor.BoldRed => ConsoleBoldRed,
            ConsoleBackgroundColor.BoldGreen => ConsoleBoldGreen,
            ConsoleBackgroundColor.BoldYellow => ConsoleBoldYellow,
            ConsoleBackgroundColor.BoldBlue => ConsoleBoldBlue,
            ConsoleBackgroundColor.BoldMagenta => ConsoleBoldMagenta,
            ConsoleBackgroundColor.BoldCyan => ConsoleBoldCyan,
            ConsoleBackgroundColor.BoldWhite => ConsoleBoldWhite,
            _ => Empty,
        };

        private static bool AreStructsAvailable { get; set; }
        /// <summary>
        /// Converts the color object into a gml string to be set to a variable or function argument
        /// </summary>
        /// <returns>The converted string</returns>
        public string ToGML() => AreStructsAvailable switch {
            true => Type switch {
                ColorType.RGBA => $"new Color({(byte)ColorType.RGBA}, {RGBA})",
                ColorType.Console => $"new Color({(byte)ColorType.Console}, {(byte)Foreground})",
                _ => $"new Color({(byte)Type})"
            },
            false => Type switch {
                ColorType.RGBA => $"submodloader_color_constructor({(byte)ColorType.RGBA}, {RGBA})",
                ColorType.Console => $"submodloader_color_constructor({(byte)ColorType.Console}, {(byte)Foreground})",
                _ => $"submodloader_color_constructor({(byte)Type})"
            }
        };
        internal static void AddTypeToInterop(GameMakerData gameData) {
            AreStructsAvailable = gameData.IsVersionAtLeast(2, 3);

            static void writeCSharp(GMLInteropWriter w, Color v) {
                w.WriteByte((byte)v.Type);
                switch (v.Type) {
                    case ColorType.RGBA:
                        w.Write(v.RGBA);
                        break;
                    case ColorType.Console:
                        w.WriteByte((byte)v.Foreground);
                        break;
                }
            }
            static Color readCSharp(GMLInteropReader r) {
                ColorType type = (ColorType)r.ReadByte();
                switch (type) {
                    case ColorType.RGBA:
                        return new(r.ReadUInt());
                    case ColorType.Console:
                        byte color = r.ReadByte();
                        if (IsConsoleForeground(color))
                            return GetByConsoleForeground((ConsoleForegroundColor)color);
                        else if (IsConsoleBackground(color))
                            return GetByConsoleBackground((ConsoleBackgroundColor)color);
                        return Empty;
                    default:
                        return Empty;
                }
            }

            if (AreStructsAvailable) {
                gameData.AddCode("gml_GlobalScript_submodloader_color_constructor", $$"""
                    function Color() constructor {
                        type = argument0
                        rgba = 0
                        rgb = 0
                        r = 0
                        g = 0
                        b = 0
                        a = 0
                        foreground = 0
                        background = 0

                        switch (type) {
                            case {{(byte)ColorType.RGBA}}:
                                switch (argument_count) {
                                    case 2:
                                        rgba = int64(argument1)
                                        rgb = rgba >> 8
                                        r = (rgba >> 24) & 0xFF
                                        g = (rgba >> 16) & 0xFF
                                        b = (rgba >> 8) & 0xFF
                                        a = rgba & 0xFF
                                        break
                                    case 4:
                                        r = int64(argument1) & 0xFF
                                        g = int64(argument2) & 0xFF
                                        b = int64(argument3) & 0xFF
                                        a = int64(255)
                                        rgba = (r << 24) | (g << 16) | (b << 8) | a
                                        rgb = rgba >> 8
                                        break
                                    case 5:
                                        r = int64(argument1) & 0xFF
                                        g = int64(argument2) & 0xFF
                                        b = int64(argument3) & 0xFF
                                        a = int64(argument4) & 0xFF
                                        rgba = (r << 24) | (g << 16) | (b << 8) | a
                                        rgb = rgba >> 8
                                        break
                                }
                                break
                            case {{(byte)ColorType.Console}}:
                                var color = argument1
                                if ({{GMLInteropManager.CallFromGML(IsConsoleForeground, "color")}}) {
                                    foreground = color
                                    background = color + 10 // little trick I noticed
                                } else if ({{GMLInteropManager.CallFromGML(IsConsoleBackground, "color")}}) {
                                    foreground = color - 10
                                    background = color
                                }
                                break
                        }
                    }
                    """);
                GMLInteropManager.RegisterType(gameData, writeCSharp, readCSharp, $$"""
                    var color = arg
                    buffer_write(argBuffer, buffer_u8, color.type)
                    switch (color.type) {
                        case {{(byte)ColorType.RGBA}}:
                            buffer_write(argBuffer, buffer_u32, color.rgba)
                            break
                        case {{(byte)ColorType.Console}}:
                            buffer_write(argBuffer, buffer_u8, color.foreground)
                            break
                    }
                    """, $$"""
                    var type = buffer_read(resultBuffer, buffer_u8)
                    switch(type) {
                        case {{(byte)ColorType.RGBA}}:
                            var rgba = buffer_read(resultBuffer, buffer_u32)
                            return new Color(type, rgba)
                        case {{(byte)ColorType.Console}}:
                            var foreground = buffer_read(resultBuffer, buffer_u8)
                            return new Color(type, foreground)
                        default:
                            return new Color(type)
                    }
                    """);
            } else {
                gameData.AddCodeAndFunction("submodloader_color_constructor", $$"""
                    var color = ds_map_create()
                    ds_map_set(color, "type", argument0)
                    ds_map_set(color, "rgba", 0)
                    ds_map_set(color, "rgb", 0)
                    ds_map_set(color, "r", 0)
                    ds_map_set(color, "g", 0)
                    ds_map_set(color, "b", 0)
                    ds_map_set(color, "a", 0)
                    ds_map_set(color, "foreground", 0)
                    ds_map_set(color, "background", 0)

                    switch (ds_map_find_value(color, "type")) {
                        case {{(byte)ColorType.RGBA}}:
                            switch (argument_count) {
                                case 2:
                                    ds_map_set(color, "rgba", int64(argument1))
                                    ds_map_set(color, "rgb", ds_map_find_value(color, "rgba") >> 8)
                                    ds_map_set(color, "r", (ds_map_find_value(color, "rgba") >> 24) & 0xFF)
                                    ds_map_set(color, "g", (ds_map_find_value(color, "rgba") >> 16) & 0xFF)
                                    ds_map_set(color, "b", (ds_map_find_value(color, "rgba") >> 8) & 0xFF)
                                    ds_map_set(color, "a", ds_map_find_value(color, "rgba") & 0xFF)
                                    break
                                case 4:
                                    ds_map_set(color, "r", int64(argument1) & 0xFF)
                                    ds_map_set(color, "g", int64(argument2) & 0xFF)
                                    ds_map_set(color, "b", int64(argument3) & 0xFF)
                                    ds_map_set(color, "a", int64(255))
                                    ds_map_set(color, "rgba", (ds_map_find_value(color, "r") << 24) | (ds_map_find_value(color, "g") << 16) | (ds_map_find_value(color, "b") << 8) | ds_map_find_value(color, "a"))
                                    ds_map_set(color, "rgb", ds_map_find_value(color, "rgba") >> 8)
                                    break
                                case 5:
                                    ds_map_set(color, "r", int64(argument1) & 0xFF)
                                    ds_map_set(color, "g", int64(argument2) & 0xFF)
                                    ds_map_set(color, "b", int64(argument3) & 0xFF)
                                    ds_map_set(color, "a", int64(argument4) & 0xFF)
                                    ds_map_set(color, "rgba", (ds_map_find_value(color, "r") << 24) | (ds_map_find_value(color, "g") << 16) | (ds_map_find_value(color, "b") << 8) | ds_map_find_value(color, "a"))
                                    ds_map_set(color, "rgb", ds_map_find_value(color, "rgba") >> 8)
                                    break
                            }
                            break
                        case {{(byte)ColorType.Console}}:
                            var color = argument1
                            if ({{GMLInteropManager.CallFromGML(IsConsoleForeground, "color")}}) {
                                ds_map_set(color, "foreground", color)
                                ds_map_set(color, "background", color + 10)
                            } else if ({{GMLInteropManager.CallFromGML(IsConsoleBackground, "color")}}) {
                                ds_map_set(color, "foreground", color - 10)
                                ds_map_set(color, "background", color)
                            }
                            break
                    }
                    return color
                    """);
                GMLInteropManager.RegisterType(gameData, writeCSharp, readCSharp, $$"""
                    var isEmpty = false
                    switch (typeof(arg)) {
                        case "number":
                            if (ds_exists(arg, ds_type_map)) {
                                var type = ds_map_find_value(arg, "type")
                                switch (type) {
                                    case {{(byte)ColorType.RGBA}}:
                                        buffer_write(argBuffer, buffer_u8, {{(byte)ColorType.RGBA}})

                                        var rgba = ds_map_find_value(arg, "rgba")
                                        if (!is_undefined(rgba)) {
                                            buffer_write(argBuffer, buffer_u32, rgba)
                                            break
                                        }

                                        var rgb = ds_map_find_value(arg, "rgb")
                                        if (!is_undefined(rgb)) {
                                            buffer_write(argBuffer, buffer_u32, int64(rgb) << 8 | 255)
                                            break
                                        }

                                        var r = int64(ds_map_find_value(arg, "r")) & 0xFF
                                        if (is_undefined(r))
                                            r = int64(0)
                                        var g = int64(ds_map_find_value(arg, "g")) & 0xFF
                                        if (is_undefined(g))
                                            g = int64(0)
                                        var b = int64(ds_map_find_value(arg, "b")) & 0xFF
                                        if (is_undefined(b))
                                            b = int64(0)
                                        var a = int64(ds_map_find_value(arg, "a")) & 0xFF
                                        if (is_undefined(a))
                                            a = int64(255)

                                        buffer_write(argBuffer, buffer_u32, r << 24 | g << 16 | b << 8 | a)
                                        break
                                    case {{(byte)ColorType.Console}}:
                                        var foreground = ds_map_find_value(arg, "foreground")
                                        if (!is_undefined(rgba)) {
                                            buffer_write(argBuffer, buffer_u8, {{(byte)ColorType.Console}})
                                            buffer_write(argBuffer, buffer_u8, foreground)
                                            break
                                        }

                                        var background = ds_map_find_value(arg, "background")
                                        if (!is_undefined(background)) {
                                            buffer_write(argBuffer, buffer_u8, {{(byte)ColorType.Console}})
                                            buffer_write(argBuffer, buffer_u8, background)
                                            break
                                        }

                                        isEmpty = true
                                        break
                                    default:
                                        isEmpty = true
                                        break
                                }   
                            } else
                                isEmpty = true
                            break
                        default:
                            isEmpty = true
                            break
                    }

                    if (isEmpty)
                        buffer_write(argBuffer, buffer_u8, {{(byte)ColorType.Empty}})
                    """, $$"""
                    var color = ds_map_create()
                    var type = buffer_read(resultBuffer, buffer_u8)
                    ds_map_set(color, "type", type)
                    switch (type) {
                        case {{(byte)ColorType.RGBA}}:
                            var rgba = int64(buffer_read(resultBuffer, buffer_u32))
                            ds_map_set(color, "rgba", rgba)
                            ds_map_set(color, "rgb", rgba >> 8)
                            ds_map_set(color, "r", (rgba >> 24) & 0xFF)
                            ds_map_set(color, "g", (rgba >> 16) & 0xFF)
                            ds_map_set(color, "b", (rgba >> 8) & 0xFF)
                            ds_map_set(color, "a", rgba & 0xFF)
                            break
                        case {{(byte)ColorType.Console}}:
                            ds_map_set(color, "foreground", buffer_read(resultBuffer, buffer_u8))
                            break
                    }
                    return color
                    """);
            }
        }

        /// <summary>
        /// Converts the color to string
        /// </summary>
        /// <returns>
        /// The return is determined in the following order
        /// <list type="number">
        /// <item>Returns <see cref="Foreground"/>,<see cref="Background"/> if <see cref="Type"/> is <see cref="ColorType.Console"/></item>
        /// <item>Returns #<see cref="RGBA"/> as hex if <see cref="Type"/> is <see cref="ColorType.RGBA"/></item>
        /// <item>Returns an empty string otherwise (the color is considered <see cref="ColorType.Empty"/>)</item>
        /// </list>
        /// </returns>
        public override string ToString() {
            return Type switch {
                ColorType.Console => $"{Foreground},{Background}",
                ColorType.RGBA => $"#{RGBA:X}",
                ColorType.Empty => "Empty",
                _ => ""
            };
        }

        /// <summary>
        /// Converts to the imgui uint format
        /// </summary>
        /// <returns>The color as a <see langword="uint"/></returns>
        public uint ToImGuiUint() => (uint)((_a << 24) | (_b << 16) | (_g << 8) | _r);

        /// <summary>
        /// Converts to ImGui's <see cref="Vector4"/> format with each component converted to 0.0-1.0
        /// </summary>
        /// <returns>The new <see cref="Vector4"/></returns>
        public Vector4 ToImGuiVec4() => new(R / 255f, G / 255f, B / 255f, A / 255f);

        /// <summary>
        /// Determines if given object is an equivalent color
        /// </summary>
        /// <param name="obj">The given object to compare to</param>
        /// <returns>
        /// <list type="number">
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.Console"/>:<para>Returns <see langword="true"/> if both <see cref="Foreground"/> and <see cref="Background"/> are the same</para></item>
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.RGBA"/>:<para>Returns <see langword="true"/> if both <see cref="RGBA"/> are the same</para></item>
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.Empty"/>:<para>Returns <see langword="true"/></para></item>
        /// <item>Returns <see langword="false"/> otherwise</item>
        /// </list>
        /// </returns>
        public override bool Equals([NotNullWhen(true)] object obj) {
            if (obj is Color other && _type == other._type) {
                return _type switch {
                    ColorType.Console => _foreground == other._foreground && _background == other._background,
                    ColorType.RGBA => _rgba == other._rgba,
                    ColorType.Empty => true,
                    _ => false
                };
            }
            return false;
        }

        /// <summary>
        /// Determines if both colors are equivalent
        /// </summary>
        /// <param name="left">The left side color</param>
        /// <param name="right">The right side color</param>
        /// <returns><inheritdoc cref="Equals(object)"/></returns>
        public static bool operator ==(Color left, Color right) => left.Equals(right);

        /// <summary>
        /// Determines if both colors are not equivalent
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        /// <list type="number">
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.Console"/>:<para>Returns <see langword="false"/> if both <see cref="Foreground"/> and <see cref="Background"/> are the same</para></item>
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.RGBA"/>:<para>Returns <see langword="false"/> if both <see cref="RGBA"/> are the same</para></item>
        /// <item>If both <see cref="Type"/> are <see cref="ColorType.Empty"/>:<para>Returns <see langword="false"/></para></item>
        /// <item>Returns <see langword="true"/> otherwise</item>
        /// </list>
        /// </returns>
        public static bool operator !=(Color left, Color right) => !left.Equals(right);

        /// <summary>
        /// Returns a hash code for the value of this Color
        /// </summary>
        /// <returns>An int hash code</returns>
        public override int GetHashCode() => HashCode.Combine(_rgba, _foreground, _background, _type);

        /// <summary>
        /// A color with no information, useful for default parameter values
        /// </summary>
        public static Color Empty { get; } = default;

        #region Console Colors

        /// <summary>
        /// Default console color for foreground
        /// </summary>
        public static Color ConsoleDefault { get; } = new(ConsoleForegroundColor.Default, ConsoleBackgroundColor.Default, 0xCCCCCCFF);

        /// <summary>
        /// Black console color
        /// </summary>
        public static Color ConsoleBlack { get; } = new(ConsoleForegroundColor.Black, ConsoleBackgroundColor.Black, 0x0C0C0CFF);

        /// <summary>
        /// Red console color
        /// </summary>
        public static Color ConsoleRed { get; } = new(ConsoleForegroundColor.Red, ConsoleBackgroundColor.Red, 0xc50F1FFF);

        /// <summary>
        /// Green console color
        /// </summary>
        public static Color ConsoleGreen { get; } = new(ConsoleForegroundColor.Green, ConsoleBackgroundColor.Green, 0x13A10EFF);

        /// <summary>
        /// Yellow console color
        /// </summary>
        public static Color ConsoleYellow { get; } = new(ConsoleForegroundColor.Yellow, ConsoleBackgroundColor.Yellow, 0xC19C00FF);

        /// <summary>
        /// Blue console color
        /// </summary>
        public static Color ConsoleBlue { get; } = new(ConsoleForegroundColor.Blue, ConsoleBackgroundColor.Blue, 0x0037DAFF);

        /// <summary>
        /// Magenta console color
        /// </summary>
        public static Color ConsoleMagenta { get; } = new(ConsoleForegroundColor.Magenta, ConsoleBackgroundColor.Magenta, 0x881798FF);

        /// <summary>
        /// Cyan console color
        /// </summary>
        public static Color ConsoleCyan { get; } = new(ConsoleForegroundColor.Cyan, ConsoleBackgroundColor.Cyan, 0x3A96DDFF);

        /// <summary>
        /// White console color
        /// </summary>
        public static Color ConsoleWhite { get; } = new(ConsoleForegroundColor.White, ConsoleBackgroundColor.White, 0xCCCCCCFF);

        /// <summary>
        /// Bold / bright black console color
        /// </summary>
        public static Color ConsoleBoldBlack { get; } = new(ConsoleForegroundColor.BoldBlack, ConsoleBackgroundColor.BoldBlack, 0x767676FF);

        /// <summary>
        /// Bold / bright red console color
        /// </summary>
        public static Color ConsoleBoldRed { get; } = new(ConsoleForegroundColor.BoldRed, ConsoleBackgroundColor.BoldRed, 0xE74856FF);

        /// <summary>
        /// Bold / bright green console color
        /// </summary>
        public static Color ConsoleBoldGreen { get; } = new(ConsoleForegroundColor.BoldGreen, ConsoleBackgroundColor.BoldGreen, 0x16C60CFF);

        /// <summary>
        /// Bold / bright yellow console color
        /// </summary>
        public static Color ConsoleBoldYellow { get; } = new(ConsoleForegroundColor.BoldYellow, ConsoleBackgroundColor.BoldYellow, 0xF9F1A5FF);

        /// <summary>
        /// Bold / bright blue console color
        /// </summary>
        public static Color ConsoleBoldBlue { get; } = new(ConsoleForegroundColor.BoldBlue, ConsoleBackgroundColor.BoldBlue, 0x3B78FFFF);

        /// <summary>
        /// Bold / bright magenta console color
        /// </summary>
        public static Color ConsoleBoldMagenta { get; } = new(ConsoleForegroundColor.BoldMagenta, ConsoleBackgroundColor.BoldMagenta, 0xB4009EFF);

        /// <summary>
        /// Bold / bright cyan console color
        /// </summary>
        public static Color ConsoleBoldCyan { get; } = new(ConsoleForegroundColor.BoldCyan, ConsoleBackgroundColor.BoldCyan, 0x61D6D6FF);

        /// <summary>
        /// Bold / bright white console color
        /// </summary>
        public static Color ConsoleBoldWhite { get; } = new(ConsoleForegroundColor.BoldWhite, ConsoleBackgroundColor.BoldWhite, 0xF2F2F2FF);

        #endregion

        #region Pink Colors

        /// <summary>
        /// Pink color <c>#FFC0CB</c>
        /// </summary>
        public static Color Pink { get; } = new(0xFFC0CBFF);

        /// <summary>
        /// LightPink color <c>#FFB6C1</c>
        /// </summary>
        public static Color LightPink { get; } = new(0xFFB6C1FF);

        /// <summary>
        /// HotPink color <c>#FF69B4</c>
        /// </summary>
        public static Color HotPink { get; } = new(0xFF69B4FF);

        /// <summary>
        /// DeepPink color <c>#FF1493</c>
        /// </summary>
        public static Color DeepPink { get; } = new(0xFF1493FF);

        /// <summary>
        /// PaleVioletRed color <c>#DB7093</c>
        /// </summary>
        public static Color PaleVioletRed { get; } = new(0xDB7093FF);

        /// <summary>
        /// MediumVioletRed color <c>#C71585</c>
        /// </summary>
        public static Color MediumVioletRed { get; } = new(0xC71585FF);

        #endregion

        #region Purple Colors

        /// <summary>
        /// Lavender color <c>#E6E6FA</c>
        /// </summary>
        public static Color Lavender { get; } = new(0xE6E6FAFF);

        /// <summary>
        /// Thistle color <c>#D8BFD8</c>
        /// </summary>
        public static Color Thistle { get; } = new(0xD8BFD8FF);

        /// <summary>
        /// Plum color <c>#DDA0DD</c>
        /// </summary>
        public static Color Plum { get; } = new(0xDDA0DDFF);

        /// <summary>
        /// Orchid color <c>#DA70D6</c>
        /// </summary>
        public static Color Orchid { get; } = new(0xDA70D6FF);

        /// <summary>
        /// Violet color <c>#EE82EE</c>
        /// </summary>
        public static Color Violet { get; } = new(0xEE82EEFF);

        /// <summary>
        /// Fuchsia color <c>#FF00FF</c>
        /// </summary>
        public static Color Fuchsia { get; } = new(0xFF00FFFF);

        /// <summary>
        /// Magenta color <c>#FF00FF</c>
        /// </summary>
        public static Color Magenta { get; } = new(0xFF00FFFF);

        /// <summary>
        /// MediumOrchid color <c>#BA55D3</c>
        /// </summary>
        public static Color MediumOrchid { get; } = new(0xBA55D3FF);

        /// <summary>
        /// DarkOrchid color <c>#9932CC</c>
        /// </summary>
        public static Color DarkOrchid { get; } = new(0x9932CCFF);

        /// <summary>
        /// DarkViolet color <c>#9400D3</c>
        /// </summary>
        public static Color DarkViolet { get; } = new(0x9400D3FF);

        /// <summary>
        /// BlueViolet color <c>#8A2BE2</c>
        /// </summary>
        public static Color BlueViolet { get; } = new(0x8A2BE2FF);

        /// <summary>
        /// DarkMagenta color <c>#8B008B</c>
        /// </summary>
        public static Color DarkMagenta { get; } = new(0x8B008BFF);

        /// <summary>
        /// Purple color <c>#800080</c>
        /// </summary>
        public static Color Purple { get; } = new(0x800080FF);

        /// <summary>
        /// MediumPurple color <c>#9370DB</c>
        /// </summary>
        public static Color MediumPurple { get; } = new(0x9370DBFF);

        /// <summary>
        /// MediumSlateBlue color <c>#7B68EE</c>
        /// </summary>
        public static Color MediumSlateBlue { get; } = new(0x7B68EEFF);

        /// <summary>
        /// SlateBlue color <c>#6A5ACD</c>
        /// </summary>
        public static Color SlateBlue { get; } = new(0x6A5ACDFF);

        /// <summary>
        /// DarkSlateBlue color <c>#483D8B</c>
        /// </summary>
        public static Color DarkSlateBlue { get; } = new(0x483D8BFF);

        /// <summary>
        /// RebeccaPurple color <c>#663399</c>
        /// </summary>
        public static Color RebeccaPurple { get; } = new(0x663399FF);

        /// <summary>
        /// Indigo  color <c>#4B0082</c>
        /// </summary>
        public static Color Indigo { get; } = new(0x4B0082FF);

        #endregion

        #region Red Colors

        /// <summary>
        /// LightSalmon color <c>#FFA07A</c>
        /// </summary>
        public static Color LightSalmon { get; } = new(0xFFA07AFF);

        /// <summary>
        /// Salmon color <c>#FA8072</c>
        /// </summary>
        public static Color Salmon { get; } = new(0xFA8072FF);

        /// <summary>
        /// DarkSalmon color <c>#E9967A</c>
        /// </summary>
        public static Color DarkSalmon { get; } = new(0xE9967AFF);

        /// <summary>
        /// LightCoral color <c>#F08080</c>
        /// </summary>
        public static Color LightCoral { get; } = new(0xF08080FF);

        /// <summary>
        /// IndianRed  color <c>#CD5C5C</c>
        /// </summary>
        public static Color IndianRed { get; } = new(0xCD5C5CFF);

        /// <summary>
        /// Crimson color <c>#DC143C</c>
        /// </summary>
        public static Color Crimson { get; } = new(0xDC143CFF);

        /// <summary>
        /// Red color <c>#FF0000</c>
        /// </summary>
        public static Color Red { get; } = new(0xFF0000FF);

        /// <summary>
        /// FireBrick color <c>#B22222</c>
        /// </summary>
        public static Color FireBrick { get; } = new(0xB22222FF);

        /// <summary>
        /// DarkRed color <c>#8B0000</c>
        /// </summary>
        public static Color DarkRed { get; } = new(0x8B0000FF);

        #endregion

        #region Orange Colors

        /// <summary>
        /// Orange color <c>#FFA500</c>
        /// </summary>
        public static Color Orange { get; } = new(0xFFA500FF);

        /// <summary>
        /// DarkOrange color <c>#FF8C00</c>
        /// </summary>
        public static Color DarkOrange { get; } = new(0xFF8C00FF);

        /// <summary>
        /// Coral color <c>#FF7F50</c>
        /// </summary>
        public static Color Coral { get; } = new(0xFF7F50FF);

        /// <summary>
        /// Tomato color <c>#FF6347</c>
        /// </summary>
        public static Color Tomato { get; } = new(0xFF6347FF);

        /// <summary>
        /// OrangeRed color <c>#FF4500</c>
        /// </summary>
        public static Color OrangeRed { get; } = new(0xFF4500FF);

        #endregion

        #region Yellow Colors

        /// <summary>
        /// Gold color <c>#FFD700</c>
        /// </summary>
        public static Color Gold { get; } = new(0xFFD700FF);

        /// <summary>
        /// Yellow color <c>#FFFF00</c>
        /// </summary>
        public static Color Yellow { get; } = new(0xFFFF00FF);

        /// <summary>
        /// LightYellow color <c>#FFFFE0</c>
        /// </summary>
        public static Color LightYellow { get; } = new(0xFFFFE0FF);

        /// <summary>
        /// LemonChiffon color <c>#FFFACD</c>
        /// </summary>
        public static Color LemonChiffon { get; } = new(0xFFFACDFF);

        /// <summary>
        /// LightGoldenRodYellow color <c>#FAFAD2</c>
        /// </summary>
        public static Color LightGoldenRodYellow { get; } = new(0xFAFAD2FF);

        /// <summary>
        /// PapayaWhip color <c>#FFEFD5</c>
        /// </summary>
        public static Color PapayaWhip { get; } = new(0xFFEFD5FF);

        /// <summary>
        /// Moccasin color <c>#FFE4B5</c>
        /// </summary>
        public static Color Moccasin { get; } = new(0xFFE4B5FF);

        /// <summary>
        /// PeachPuff color <c>#FFDAB9</c>
        /// </summary>
        public static Color PeachPuff { get; } = new(0xFFDAB9FF);

        /// <summary>
        /// PaleGoldenRod color <c>#EEE8AA</c>
        /// </summary>
        public static Color PaleGoldenRod { get; } = new(0xEEE8AAFF);

        /// <summary>
        /// Khaki color <c>#F0E68C</c>
        /// </summary>
        public static Color Khaki { get; } = new(0xF0E68CFF);

        /// <summary>
        /// DarkKhaki color <c>#BDB76B</c>
        /// </summary>
        public static Color DarkKhaki { get; } = new(0xBDB76BFF);

        #endregion

        #region Green Colors

        /// <summary>
        /// GreenYellow color <c>#ADFF2F</c>
        /// </summary>
        public static Color GreenYellow { get; } = new(0xADFF2FFF);

        /// <summary>
        /// Chartreuse color <c>#7FFF00</c>
        /// </summary>
        public static Color Chartreuse { get; } = new(0x7FFF00FF);

        /// <summary>
        /// LawnGreen color <c>#7CFC00</c>
        /// </summary>
        public static Color LawnGreen { get; } = new(0x7CFC00FF);

        /// <summary>
        /// Lime color <c>#00FF00</c>
        /// </summary>
        public static Color Lime { get; } = new(0x00FF00FF);

        /// <summary>
        /// LimeGreen color <c>#32CD32</c>
        /// </summary>
        public static Color LimeGreen { get; } = new(0x32CD32FF);

        /// <summary>
        /// PaleGreen color <c>#98FB98</c>
        /// </summary>
        public static Color PaleGreen { get; } = new(0x98FB98FF);

        /// <summary>
        /// LightGreen color <c>#90EE90</c>
        /// </summary>
        public static Color LightGreen { get; } = new(0x90EE90FF);

        /// <summary>
        /// MediumSpringGreen color <c>#00FA9A</c>
        /// </summary>
        public static Color MediumSpringGreen { get; } = new(0x00FA9AFF);

        /// <summary>
        /// SpringGreen color <c>#00FF7F</c>
        /// </summary>
        public static Color SpringGreen { get; } = new(0x00FF7FFF);

        /// <summary>
        /// MediumSeaGreen color <c>#3CB371</c>
        /// </summary>
        public static Color MediumSeaGreen { get; } = new(0x3CB371FF);

        /// <summary>
        /// SeaGreen color <c>#2E8B57</c>
        /// </summary>
        public static Color SeaGreen { get; } = new(0x2E8B57FF);

        /// <summary>
        /// ForestGreen color <c>#228B22</c>
        /// </summary>
        public static Color ForestGreen { get; } = new(0x228B22FF);

        /// <summary>
        /// Green color <c>#008000</c>
        /// </summary>
        public static Color Green { get; } = new(0x008000FF);

        /// <summary>
        /// DarkGreen color <c>#006400</c>
        /// </summary>
        public static Color DarkGreen { get; } = new(0x006400FF);

        /// <summary>
        /// YellowGreen color <c>#9ACD32</c>
        /// </summary>
        public static Color YellowGreen { get; } = new(0x9ACD32FF);

        /// <summary>
        /// OliveDrab color <c>#6B8E23</c>
        /// </summary>
        public static Color OliveDrab { get; } = new(0x6B8E23FF);

        /// <summary>
        /// DarkOliveGreen color <c>#556B2F</c>
        /// </summary>
        public static Color DarkOliveGreen { get; } = new(0x556B2FFF);

        /// <summary>
        /// MediumAquaMarine color <c>#66CDAA</c>
        /// </summary>
        public static Color MediumAquaMarine { get; } = new(0x66CDAAFF);

        /// <summary>
        /// DarkSeaGreen color <c>#8FBC8F</c>
        /// </summary>
        public static Color DarkSeaGreen { get; } = new(0x8FBC8FFF);

        /// <summary>
        /// LightSeaGreen color <c>#20B2AA</c>
        /// </summary>
        public static Color LightSeaGreen { get; } = new(0x20B2AAFF);

        /// <summary>
        /// DarkCyan color <c>#008B8B</c>
        /// </summary>
        public static Color DarkCyan { get; } = new(0x008B8BFF);

        /// <summary>
        /// Teal color <c>#008080</c>
        /// </summary>
        public static Color Teal { get; } = new(0x008080FF);

        #endregion

        #region Cyan Colors

        /// <summary>
        /// Aqua color <c>#00FFFF</c>
        /// </summary>
        public static Color Aqua { get; } = new(0x00FFFFFF);

        /// <summary>
        /// Cyan color <c>#00FFFF</c>
        /// </summary>
        public static Color Cyan { get; } = new(0x00FFFFFF);

        /// <summary>
        /// LightCyan color <c>#E0FFFF</c>
        /// </summary>
        public static Color LightCyan { get; } = new(0xE0FFFFFF);

        /// <summary>
        /// PaleTurquoise color <c>#AFEEEE</c>
        /// </summary>
        public static Color PaleTurquoise { get; } = new(0xAFEEEEFF);

        /// <summary>
        /// Aquamarine color <c>#7FFFD4</c>
        /// </summary>
        public static Color Aquamarine { get; } = new(0x7FFFD4FF);

        /// <summary>
        /// Turquoise color <c>#40E0D0</c>
        /// </summary>
        public static Color Turquoise { get; } = new(0x40E0D0FF);

        /// <summary>
        /// MediumTurquoise color <c>#48D1CC</c>
        /// </summary>
        public static Color MediumTurquoise { get; } = new(0x48D1CCFF);

        /// <summary>
        /// DarkTurquoise color <c>#00CED1</c>
        /// </summary>
        public static Color DarkTurquoise { get; } = new(0x00CED1FF);

        #endregion

        #region Blue Colors

        /// <summary>
        /// CadetBlue color <c>#5F9EA0</c>
        /// </summary>
        public static Color CadetBlue { get; } = new(0x5F9EA0FF);

        /// <summary>
        /// SteelBlue color <c>#4682B4</c>
        /// </summary>
        public static Color SteelBlue { get; } = new(0x4682B4FF);

        /// <summary>
        /// LightSteelBlue color <c>#B0C4DE</c>
        /// </summary>
        public static Color LightSteelBlue { get; } = new(0xB0C4DEFF);

        /// <summary>
        /// LightBlue color <c>#ADD8E6</c>
        /// </summary>
        public static Color LightBlue { get; } = new(0xADD8E6FF);

        /// <summary>
        /// PowderBlue color <c>#B0E0E6</c>
        /// </summary>
        public static Color PowderBlue { get; } = new(0xB0E0E6FF);

        /// <summary>
        /// LightSkyBlue color <c>#87CEFA</c>
        /// </summary>
        public static Color LightSkyBlue { get; } = new(0x87CEFAFF);

        /// <summary>
        /// SkyBlue color <c>#87CEEB</c>
        /// </summary>
        public static Color SkyBlue { get; } = new(0x87CEEBFF);

        /// <summary>
        /// CornflowerBlue color <c>#6495ED</c>
        /// </summary>
        public static Color CornflowerBlue { get; } = new(0x6495EDFF);

        /// <summary>
        /// DeepSkyBlue color <c>#00BFFF</c>
        /// </summary>
        public static Color DeepSkyBlue { get; } = new(0x00BFFFFF);

        /// <summary>
        /// DodgerBlue color <c>#1E90FF</c>
        /// </summary>
        public static Color DodgerBlue { get; } = new(0x1E90FFFF);

        /// <summary>
        /// RoyalBlue color <c>#4169E1</c>
        /// </summary>
        public static Color RoyalBlue { get; } = new(0x4169E1FF);

        /// <summary>
        /// Blue color <c>#0000FF</c>
        /// </summary>
        public static Color Blue { get; } = new(0x0000FFFF);

        /// <summary>
        /// MediumBlue color <c>#0000CD</c>
        /// </summary>
        public static Color MediumBlue { get; } = new(0x0000CDFF);

        /// <summary>
        /// DarkBlue color <c>#00008B</c>
        /// </summary>
        public static Color DarkBlue { get; } = new(0x00008BFF);

        /// <summary>
        /// Navy color <c>#000080</c>
        /// </summary>
        public static Color Navy { get; } = new(0x000080FF);

        /// <summary>
        /// MidnightBlue color <c>#191970</c>
        /// </summary>
        public static Color MidnightBlue { get; } = new(0x191970FF);

        #endregion

        #region Brown Colors

        /// <summary>
        /// Cornsilk color <c>#FFF8DC</c>
        /// </summary>
        public static Color Cornsilk { get; } = new(0xFFF8DCFF);

        /// <summary>
        /// BlanchedAlmond color <c>#FFEBCD</c>
        /// </summary>
        public static Color BlanchedAlmond { get; } = new(0xFFEBCDFF);

        /// <summary>
        /// Bisque color <c>#FFE4C4</c>
        /// </summary>
        public static Color Bisque { get; } = new(0xFFE4C4FF);

        /// <summary>
        /// NavajoWhite color <c>#FFDEAD</c>
        /// </summary>
        public static Color NavajoWhite { get; } = new(0xFFDEADFF);

        /// <summary>
        /// Wheat color <c>#F5DEB3</c>
        /// </summary>
        public static Color Wheat { get; } = new(0xF5DEB3FF);

        /// <summary>
        /// BurlyWood color <c>#DEB887</c>
        /// </summary>
        public static Color BurlyWood { get; } = new(0xDEB887FF);

        /// <summary>
        /// Tan color <c>#D2B48C</c>
        /// </summary>
        public static Color Tan { get; } = new(0xD2B48CFF);

        /// <summary>
        /// RosyBrown color <c>#BC8F8F</c>
        /// </summary>
        public static Color RosyBrown { get; } = new(0xBC8F8FFF);

        /// <summary>
        /// SandyBrown color <c>#F4A460</c>
        /// </summary>
        public static Color SandyBrown { get; } = new(0xF4A460FF);

        /// <summary>
        /// GoldenRod color <c>#DAA520</c>
        /// </summary>
        public static Color GoldenRod { get; } = new(0xDAA520FF);

        /// <summary>
        /// DarkGoldenRod color <c>#B8860B</c>
        /// </summary>
        public static Color DarkGoldenRod { get; } = new(0xB8860BFF);

        /// <summary>
        /// Peru color <c>#CD853F</c>
        /// </summary>
        public static Color Peru { get; } = new(0xCD853FFF);

        /// <summary>
        /// Chocolate color <c>#D2691E</c>
        /// </summary>
        public static Color Chocolate { get; } = new(0xD2691EFF);

        /// <summary>
        /// Olive color <c>#808000</c>
        /// </summary>
        public static Color Olive { get; } = new(0x808000FF);

        /// <summary>
        /// SaddleBrown color <c>#8B4513</c>
        /// </summary>
        public static Color SaddleBrown { get; } = new(0x8B4513FF);

        /// <summary>
        /// Sienna color <c>#A0522D</c>
        /// </summary>
        public static Color Sienna { get; } = new(0xA0522DFF);

        /// <summary>
        /// Brown color <c>#A52A2A</c>
        /// </summary>
        public static Color Brown { get; } = new(0xA52A2AFF);

        /// <summary>
        /// Maroon color <c>#800000</c>
        /// </summary>
        public static Color Maroon { get; } = new(0x800000FF);

        #endregion

        #region White Colors

        /// <summary>
        /// White color <c>#FFFFFF</c>
        /// </summary>
        public static Color White { get; } = new(0xFFFFFFFF);

        /// <summary>
        /// Snow color <c>#FFFAFA</c>
        /// </summary>
        public static Color Snow { get; } = new(0xFFFAFAFF);

        /// <summary>
        /// HoneyDew color <c>#F0FFF0</c>
        /// </summary>
        public static Color HoneyDew { get; } = new(0xF0FFF0FF);

        /// <summary>
        /// MintCream color <c>#F5FFFA</c>
        /// </summary>
        public static Color MintCream { get; } = new(0xF5FFFAFF);

        /// <summary>
        /// Azure color <c>#F0FFFF</c>
        /// </summary>
        public static Color Azure { get; } = new(0xF0FFFFFF);

        /// <summary>
        /// AliceBlue color <c>#F0F8FF</c>
        /// </summary>
        public static Color AliceBlue { get; } = new(0xF0F8FFFF);

        /// <summary>
        /// GhostWhite color <c>#F8F8FF</c>
        /// </summary>
        public static Color GhostWhite { get; } = new(0xF8F8FFFF);

        /// <summary>
        /// WhiteSmoke color <c>#F5F5F5</c>
        /// </summary>
        public static Color WhiteSmoke { get; } = new(0xF5F5F5FF);

        /// <summary>
        /// SeaShell color <c>#FFF5EE</c>
        /// </summary>
        public static Color SeaShell { get; } = new(0xFFF5EEFF);

        /// <summary>
        /// Beige color <c>#F5F5DC</c>
        /// </summary>
        public static Color Beige { get; } = new(0xF5F5DCFF);

        /// <summary>
        /// OldLace color <c>#FDF5E6</c>
        /// </summary>
        public static Color OldLace { get; } = new(0xFDF5E6FF);

        /// <summary>
        /// FloralWhite color <c>#FFFAF0</c>
        /// </summary>
        public static Color FloralWhite { get; } = new(0xFFFAF0FF);

        /// <summary>
        /// Ivory color <c>#FFFFF0</c>
        /// </summary>
        public static Color Ivory { get; } = new(0xFFFFF0FF);

        /// <summary>
        /// AntiqueWhite color <c>#FAEBD7</c>
        /// </summary>
        public static Color AntiqueWhite { get; } = new(0xFAEBD7FF);

        /// <summary>
        /// Linen color <c>#FAF0E6</c>
        /// </summary>
        public static Color Linen { get; } = new(0xFAF0E6FF);

        /// <summary>
        /// LavenderBlush color <c>#FFF0F5</c>
        /// </summary>
        public static Color LavenderBlush { get; } = new(0xFFF0F5FF);

        /// <summary>
        /// MistyRose color <c>#FFE4E1</c>
        /// </summary>
        public static Color MistyRose { get; } = new(0xFFE4E1FF);

        #endregion

        #region Grey Colors

        /// <summary>
        /// Gainsboro color <c>#DCDCDC</c>
        /// </summary>
        public static Color Gainsboro { get; } = new(0xDCDCDCFF);

        /// <summary>
        /// LightGray color <c>#D3D3D3</c>
        /// </summary>
        public static Color LightGray { get; } = new(0xD3D3D3FF);

        /// <summary>
        /// Silver color <c>#C0C0C0</c>
        /// </summary>
        public static Color Silver { get; } = new(0xC0C0C0FF);

        /// <summary>
        /// DarkGray color <c>#A9A9A9</c>
        /// </summary>
        public static Color DarkGray { get; } = new(0xA9A9A9FF);

        /// <summary>
        /// DimGray color <c>#696969</c>
        /// </summary>
        public static Color DimGray { get; } = new(0x696969FF);

        /// <summary>
        /// Gray color <c>#808080</c>
        /// </summary>
        public static Color Gray { get; } = new(0x808080FF);

        /// <summary>
        /// LightSlateGray color <c>#778899</c>
        /// </summary>
        public static Color LightSlateGray { get; } = new(0x778899FF);

        /// <summary>
        /// SlateGray color <c>#708090</c>
        /// </summary>
        public static Color SlateGray { get; } = new(0x708090FF);

        /// <summary>
        /// DarkSlateGray color <c>#2F4F4F</c>
        /// </summary>
        public static Color DarkSlateGray { get; } = new(0x2F4F4FFF);

        /// <summary>
        /// Black color <c>#000000</c>
        /// </summary>
        public static Color Black { get; } = new(0, 0, 0);

        #endregion

        /// <summary>
        /// The types of colors
        /// </summary>
        public enum ColorType : byte {
            /// <summary>
            /// Represents a blank color with no information
            /// </summary>
            Empty,
            /// <summary>
            /// Holds the foreground and background codes of a console color
            /// </summary>
            Console,
            /// <summary>
            /// Represents an RGBA color
            /// </summary>
            RGBA
        }

        /// <summary>
        /// Modifiers for Logger text output
        /// </summary>
        public enum ConsoleModifier : byte {
            /// <summary>
            /// Does nothing
            /// </summary>
            None = 0,
            /// <summary>
            /// Adds brightness/intensity to foreground color (windows console) or adds boldness (imgui)
            /// </summary>
            Bold = 1,
            /// <summary>
            /// Adds italics
            /// </summary>
            Italic = 3,
            /// <summary>
            /// Adds underline
            /// </summary>
            Underline = 4,
            /// <summary>
            /// Swaps foreground and background colors
            /// </summary>
            Negative = 7,
            /// <summary>
            /// Adds strikethrough
            /// </summary>
            Strike = 9
        }

        private static bool IsConsoleForeground(byte color) => Enum.IsDefined((ConsoleForegroundColor)color);
        /// <summary>
        /// All the predefined virtual console foreground colors for text
        /// </summary>
        public enum ConsoleForegroundColor : byte {
            /// <summary>
            /// Used if <see cref="Type"/> is not <see cref="ColorType.Console"/>
            /// </summary>
            None = 0,
            /// <summary>
            /// Applies foreground default color
            /// </summary>
            Default = 39,
            /// <summary>
            /// Applies non - bold / bright black to foreground
            /// </summary>
            Black = 30,
            /// <summary>
            /// Applies non - bold / bright red to foreground
            /// </summary>
            Red = 31,
            /// <summary>
            /// Applies non - bold / bright green to foreground
            /// </summary>
            Green = 32,
            /// <summary>
            /// Applies non - bold / bright yellow to foreground
            /// </summary>
            Yellow = 33,
            /// <summary>
            /// Applies non - bold / bright blue to foreground
            /// </summary>
            Blue = 34,
            /// <summary>
            /// Applies non - bold / bright magenta to foreground
            /// </summary>
            Magenta = 35,
            /// <summary>
            /// Applies non - bold / bright cyan to foreground
            /// </summary>
            Cyan = 36,
            /// <summary>
            /// Applies non - bold / bright white to foreground
            /// </summary>
            White = 37,
            /// <summary>
            /// Applies bold / bright black to foreground
            /// </summary>
            BoldBlack = 90,
            /// <summary>
            /// Applies bold / bright red to foreground
            /// </summary>
            BoldRed = 91,
            /// <summary>
            /// Applies bold / bright green to foreground
            /// </summary>
            BoldGreen = 92,
            /// <summary>
            /// Applies bold / bright yellow to foreground
            /// </summary>
            BoldYellow = 93,
            /// <summary>
            /// Applies bold / bright blue to foreground
            /// </summary>
            BoldBlue = 94,
            /// <summary>
            /// Applies bold / bright magenta to foreground
            /// </summary>
            BoldMagenta = 95,
            /// <summary>
            /// Applies bold / bright cyan to foreground
            /// </summary>
            BoldCyan = 96,
            /// <summary>
            /// Applies bold / bright white to foreground
            /// </summary>
            BoldWhite = 97
        }

        private static bool IsConsoleBackground(byte color) => Enum.IsDefined((ConsoleBackgroundColor)color);
        /// <summary>
        /// All the predefined virtual console background colors for text
        /// </summary>
        public enum ConsoleBackgroundColor : byte {
            /// <summary>
            /// Used if <see cref="Type"/> is not <see cref="ColorType.Console"/>
            /// </summary>
            None = 0,
            /// <summary>
            /// Applies background default color
            /// </summary>
            Default = 49,
            /// <summary>
            /// Applies non - bold / bright black to background
            /// </summary>
            Black = 40,
            /// <summary>
            /// Applies non - bold / bright red to background
            /// </summary>
            Red = 41,
            /// <summary>
            /// Applies non - bold / bright green to background
            /// </summary>
            Green = 42,
            /// <summary>
            /// Applies non - bold / bright yellow to background
            /// </summary>
            Yellow = 43,
            /// <summary>
            /// Applies non - bold / bright blue to background
            /// </summary>
            Blue = 44,
            /// <summary>
            /// Applies non - bold / bright magenta to background
            /// </summary>
            Magenta = 45,
            /// <summary>
            /// Applies non - bold / bright cyan to background
            /// </summary>
            Cyan = 46,
            /// <summary>
            /// Applies non - bold / bright white to background
            /// </summary>
            White = 47,
            /// <summary>
            /// Applies bold / bright black to background
            /// </summary>
            BoldBlack = 100,
            /// <summary>
            /// Applies bold / bright red to background
            /// </summary>
            BoldRed = 101,
            /// <summary>
            /// Applies bold / bright green to background
            /// </summary>
            BoldGreen = 102,
            /// <summary>
            /// Applies bold / bright yellow to background
            /// </summary>
            BoldYellow = 103,
            /// <summary>
            /// Applies bold / bright blue to background
            /// </summary>
            BoldBlue = 104,
            /// <summary>
            /// Applies bold / bright magenta to background
            /// </summary>
            BoldMagenta = 105,
            /// <summary>
            /// Applies bold / bright cyan to background
            /// </summary>
            BoldCyan = 106,
            /// <summary>
            /// Applies bold / bright white to background
            /// </summary>
            BoldWhite = 107
        }
    }
}
