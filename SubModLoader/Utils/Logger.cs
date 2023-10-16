using ImGuiNET;
using SubmachineModLib;
using SubmachineModLib.Models;
using SubModLoader.GameData.Extensions;
using SubModLoader.GMLInterop;
using SubModLoader.Storage;
using SubModLoader.Storage.Widget;
using SubModLoader.Storage.Widget.Item;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using static SubModLoader.Utils.Color;

namespace SubModLoader.Utils {
    /// <summary>
    /// A logger implementation that writes to console and file, and can also use colors
    /// </summary>
    public class Logger {

        #region Instance implementation

        private string Caller { get; }
        private Color AuthorColor { get; }

        internal Logger(string caller, Color authorColor) {
            Caller = caller;
            AuthorColor = authorColor;
        }

        #region Write

        /// <summary>
        /// Logs the given <paramref name="message"/>
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="textColor">The text/foreground color for console output</param>
        /// <param name="backgroundColor">The background color for console output</param>
        /// <param name="modifiers">The modifiers for console output</param>
        public void Write(string message, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            Write(message, Caller, AuthorColor, false, textColor, backgroundColor, modifiers);
        /// <summary>
        /// Logs the given <paramref name="message"/> as a string using <see cref="object.ToString"/>
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public void Write(object message, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            Write(message, Caller, AuthorColor, false, textColor, backgroundColor, modifiers);
        /// <summary>
        /// Logs the given <paramref name="message"/> from the gml side
        /// </summary>
        /// <param name="message">The message to be logged as a gml variable/expression</param>
        /// <param name="textColor">The text/foreground color for console output as a gml variable/expression</param>
        /// <param name="backgroundColor">The background color for console output as a gml variable/expression</param>
        /// <param name="modifiers">The modifiers for console output as a gml variable/expression</param>
        /// <returns>The string for the gml code to call the function</returns>
        public string WriteFromGML(string message, string textColor = null, string backgroundColor = null, params string[] modifiers) =>
            $"{GML_logger_write}({message}, \"{Caller}\", {AuthorColor.ToGML()}, false, {textColor ?? Empty.ToGML()}, {backgroundColor ?? Empty.ToGML()}, {ModifiersToGMLArgs(modifiers)})";

        /// <summary>
        /// Forces the next write to be on a new line with the time and author information
        /// </summary>
        public void EndWrite() =>
            EndWrite(Caller);
        /// <summary>
        /// Forces the next write to be on a new line with the time and author information from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string EndWriteFromGML() =>
            $"{GML_logger_draw_spacer}(\"{Caller}\")";

        /// <summary>
        /// Logs the given <paramref name="message"/> followed by a newline
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteLine(string message = "", Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            WriteLine(message, Caller, AuthorColor, textColor, backgroundColor, modifiers);
        /// <summary>
        /// Logs the given <paramref name="message"/> as a string using <see cref="object.ToString"/> followed by a newline
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteLine(object message, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            WriteLine(message, Caller, AuthorColor, textColor, backgroundColor, modifiers);
        /// <summary>
        /// Logs the given <paramref name="message"/> followed by a newline from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string WriteLineFromGML(string message, string textColor = null, string backgroundColor = null, params string[] modifiers) =>
            $"{GML_logger_write_line}({message}, \"{Caller}\", {AuthorColor.ToGML()}, {textColor ?? Empty.ToGML()}, {backgroundColor ?? Empty.ToGML()}, {ModifiersToGMLArgs(modifiers)})";

        /// <summary>
        /// Draws a line of "-" followed by a newline
        /// </summary>
        /// <param name="length">How many "-" are in the line</param>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public void DrawLine(int length = 30, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            DrawLine(length, Caller, AuthorColor, textColor, backgroundColor, modifiers);
        /// <summary>
        /// Draws a line of "-" followed by a newline from the gml side
        /// </summary>
        /// <param name="length">How many "-" are in the line as a gml variable/expression</param>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string DrawLineFromGML(string length = "30", string textColor = null, string backgroundColor = null, params string[] modifiers) =>
            $"{GML_logger_draw_line}({length}, \"{Caller}\", {AuthorColor.ToGML()}, {textColor ?? Empty.ToGML()}, {backgroundColor ?? Empty.ToGML()}, {ModifiersToGMLArgs(modifiers)})";
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

        /// <summary>
        /// Draws a completely empty line to console
        /// </summary>
        public void DrawSpacer() =>
            DrawSpacer(Caller);
        /// <summary>
        /// Draws a completely empty line to console from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string DrawSpacerFromGML() =>
            $"{GML_logger_draw_spacer}(\"{Caller}\")";

        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleGreen"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteSuccess(string message) =>
            WriteSuccess(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> as a string using <see cref="object.ToString"/> with <see cref="ConsoleGreen"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteSuccess(object message) =>
            WriteSuccess(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleGreen"/> text from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string WriteSuccessFromGML(string message) =>
            $"{GML_logger_write_success}({message}, \"{Caller}\", \"{AuthorColor.ToGML()}\")";

        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleYellow"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteWarning(string message) =>
            WriteWarning(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> as a string using <see cref="object.ToString"/> with <see cref="ConsoleYellow"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteWarning(object message) =>
            WriteWarning(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleYellow"/> text from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string WriteWarningFromGML(string message) =>
            $"{GML_logger_write_warning}({message}, \"{Caller}\", \"{AuthorColor.ToGML()}\")";

        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleRed"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteError(string message) =>
            WriteError(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> as a string using <see cref="object.ToString"/> with <see cref="ConsoleRed"/> text
        /// </summary>
        /// <inheritdoc cref="Write(string, Color, Color, ConsoleModifier[])"/>
        public void WriteError(object message) =>
            WriteError(message, Caller, AuthorColor);
        /// <summary>
        /// Logs the given <paramref name="message"/> with <see cref="ConsoleRed"/> text from the gml side
        /// </summary>
        /// <inheritdoc cref="WriteFromGML(string, string, string, string[])"/>
        public string WriteErrorFromGML(string message) =>
            $"{GML_logger_write_error}({message}, \"{Caller}\", \"{AuthorColor.ToGML()}\")";

        #endregion

        #endregion

        #region Static implementation

        private static string PreviousCaller = null;

        private static StreamWriter File { get; } = new("SubModLoader/log.txt");

        internal const string GameNamePlaceholder = "$$GAMENAME$$";
        internal static string GameName { get; set; }

        private readonly struct MessageData {
            public string Message { get; init; }
            public Color TextColor { get; init; }
            public Color BackgroundColor { get; init; }
            public bool HasModifiers { get; init; }
            public bool IsBold { get; init; }
            public bool IsItalic { get; init; }
            public bool IsUnderline { get; init; }
            public bool IsNegative { get; init; }
            public bool IsStrike { get; init; }
        }
        private readonly struct LineData {
            public DateTime Then { get; init; }
            public string Caller { get; init; }
            public Color AuthorColor { get; init; }
            public bool IsSubModLoader { get; init; }
            public bool IsGame { get; init; }
            public bool IsSpacer { get; init; }
            public List<MessageData> Messages { get; init; }
        }
        private static List<LineData> LogData { get; } = new();
        private static bool LogChanged { get; set; } = true;

        private static readonly Color SubModLoaderColor = DarkSeaGreen;
        private static readonly Color GameColor = LightSeaGreen;
        private static readonly Color TimeAuthorColor = DarkGray;

        private const int consoleCharWidth = 120;
        private const int consoleCharHeight = 30;
        private static Vector2 ConsoleWindowSize { get; set; }

        private static bool HasDebugConsoleInitialized { get; set; } = false;

        private static SettingsCategory LoggerCategory { get; } = Settings.SubModLoaderSettings.GetCategory("Logger");
        internal static SettingsBool IsDebugConsoleOpen { get; } = SettingsBool.Get(LoggerCategory, "IsDebugConsoleOpen", false);
#if DEBUG
        private const bool showGameDebugMessagesDefault = true;
#else
        private const bool showGameDebugMessagesDefault = false;
#endif
        private static SettingsBool ShowGameDebugMessages { get; } = SettingsBool.Get(LoggerCategory, "ShowGameDebugMessages", showGameDebugMessagesDefault,
                                                                                      showInImGui: true, "Show Game Debug Messages", "Whether the game log should show in the debug console.");

        internal static void DebugConsoleInit() {
            // monospaced font, get size of all chars
            Vector2 dims = ImGui.CalcTextSize("0");
            ImGuiStylePtr style = ImGui.GetStyle();
            ConsoleWindowSize = new(dims.X * consoleCharWidth + style.ScrollbarSize, dims.Y * consoleCharHeight + ImGui.GetFrameHeightWithSpacing() + style.ItemSpacing.Y * 2);
        }

        internal static void ShowConsoleWindow() {
            if (!HasDebugConsoleInitialized) {
                DebugConsoleInit();
                HasDebugConsoleInitialized = true;
            }

            if (!IsDebugConsoleOpen.Value)
                return;

            ImGui.SetNextWindowSize(ConsoleWindowSize, ImGuiCond.FirstUseEver);
            bool isOpen = IsDebugConsoleOpen.Value;
            bool collapsed = !ImGui.Begin("Debug Console##SubModLoader", ref isOpen, ImGuiWindowFlags.HorizontalScrollbar);
            IsDebugConsoleOpen.Value = isOpen;
            if (collapsed) {
                ImGui.End();
                return;
            }

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 2));

            foreach (LineData line in LogData) {
                if (line.IsSpacer) {
                    ImGui.TextUnformatted("");
                    continue;
                }

                Color infoColor = line.IsSubModLoader ? SubModLoaderColor : line.IsGame ? GameColor : TimeAuthorColor;

                ImGui.PushStyleColor(ImGuiCol.Text, infoColor.ToImGuiVec4());
                ImGui.TextUnformatted($"{line.Then:HH:mm:ss.fff} [");

                bool useAuthorColor = !line.IsSubModLoader && !line.IsGame && line.AuthorColor != Empty;
                if (useAuthorColor)
                    ImGui.PushStyleColor(ImGuiCol.Text, line.AuthorColor.ToImGuiVec4());

                ImGui.SameLine();
                ImGui.TextUnformatted(line.Caller);

                if (useAuthorColor)
                    ImGui.PopStyleColor();

                ImGui.SameLine();
                ImGui.TextUnformatted("] ");
                ImGui.PopStyleColor();

                int i = 0;
                foreach (MessageData message in line.Messages) {
                    if (line.Caller == null)
                        Console.WriteLine(i++);

                    Color textColor = message.TextColor;
                    Color backgroundColor = message.BackgroundColor;

                    if (message.IsNegative) {
                        if (textColor == Empty)
                            textColor = ConsoleWhite;
                        if (backgroundColor == Empty)
                            backgroundColor = ConsoleBlack;
                        (textColor, backgroundColor) = (backgroundColor, textColor);
                    }

                    if (textColor == Empty)
                        textColor = ConsoleDefault;

                    uint textColorUint = textColor.ToImGuiUint();

                    if (backgroundColor != Empty || message.IsUnderline || message.IsStrike) {
                        Vector2 textSize = ImGui.CalcTextSize(message.Message);
                        Vector2 bottomLeft = ImGui.GetItemRectMax();

                        if (backgroundColor != Empty) {
                            Vector2 topRight = bottomLeft + new Vector2(textSize.X, -textSize.Y - ImGui.GetStyle().ItemSpacing.Y);
                            drawList.AddRectFilled(bottomLeft, topRight, backgroundColor.ToImGuiUint());
                        }

                        if (message.IsUnderline) {
                            Vector2 bottomRight = bottomLeft + new Vector2(textSize.X, 0) - Vector2.UnitY;
                            drawList.AddLine(bottomLeft - Vector2.UnitY, bottomRight, textColorUint);
                        }

                        if (message.IsStrike) {
                            int halfHeight = (int)(textSize.Y / 2);
                            Vector2 midLeft = bottomLeft + new Vector2(0, -halfHeight);
                            Vector2 midRight = bottomLeft + new Vector2(textSize.X, -halfHeight);
                            drawList.AddLine(midLeft, midRight, textColorUint);
                        }
                    }

                    // TODO: expose this font index stuff for public use
                    int fontIndex = 0;
                    if (message.IsBold) {
                        if (message.IsItalic)
                            fontIndex = 3;
                        else
                            fontIndex = 1;
                    } else if (message.IsItalic)
                        fontIndex = 2;

                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Text, textColorUint);
                    ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[fontIndex]);
                    ImGui.TextUnformatted(message.Message);
                    ImGui.PopStyleColor();
                    ImGui.PopFont();
                }
            }

            if (LogChanged && (ImGui.GetScrollY() + ImGui.GetStyle().ScrollbarSize) >= ImGui.GetScrollMaxY()) {
                ImGui.SetScrollHereY(1);
                LogChanged = false;
            }

            ImGui.PopStyleVar();

            ImGui.End();
        }

        #region Builtin mods

        internal static void AddGMLFunctions(GameMakerData gameData) {
            gameData.AddCodeAndFunction(GML_logger_write, $$"""
                var message = string(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                var alwaysShowCaller = false
                if (argument3 > 0.5) // official documented implementation for boolean
                    alwaysShowCaller = true
                var textColor = argument4
                var backgroundColor = argument5
                var modifiers = array_create(argument_count - 6)
                for (var i = 6; i < argument_count; i++)
                    modifiers[i] = argument[i]
                {{GMLInteropManager.CallFromGML<Action<string, string, Color, bool, Color, Color, ConsoleModifier[]>>(Write, "message", "caller", "authorColor", "alwaysShowCaller", "textColor", "backgroundColor", "modifiers")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_end_line, $$"""
                var caller = string(argument0)
                {{GMLInteropManager.CallFromGML<Action<string>>(EndWrite, "caller")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_write_line, $$"""
                var message = string(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                var textColor = argument3
                var backgroundColor = argument4
                var modifiers = array_create(argument_count - 5)
                for (var i = 5; i < argument_count; i++)
                    modifiers[i] = argument[i]
                {{GMLInteropManager.CallFromGML<Action<string, string, Color, Color, Color, ConsoleModifier[]>>(WriteLine, "message", "caller", "authorColor", "textColor", "backgroundColor", "modifiers")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_draw_line, $$"""
                var length = real(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                var textColor = argument3
                var backgroundColor = argument4
                var modifiers = array_create(argument_count - 5)
                for (var i = 5; i < argument_count; i++)
                    modifiers[i] = argument[i]
                {{GMLInteropManager.CallFromGML<Action<int, string, Color, Color, Color, ConsoleModifier[]>>(DrawLine, "length", "caller", "authorColor", "textColor", "backgroundColor", "modifiers")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_draw_spacer, $$"""
                var caller = string(argument0)
                {{GMLInteropManager.CallFromGML<Action<string>>(DrawSpacer, "caller")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_write_success, $$"""
                var message = string(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                {{GMLInteropManager.CallFromGML<Action<string, string, Color>>(WriteSuccess, "message", "caller", "authorColor")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_write_warning, $$"""
                var message = string(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                {{GMLInteropManager.CallFromGML<Action<string, string, Color>>(WriteWarning, "message", "caller", "authorColor")}}
                """);
            gameData.AddCodeAndFunction(GML_logger_write_error, $$"""
                var message = string(argument0)
                var caller = string(argument1)
                var authorColor = argument2
                {{GMLInteropManager.CallFromGML<Action<string, string, Color>>(WriteError, "message", "caller", "authorColor")}}
                """);
        }

        internal static void Replace_show_debug_message(GameMakerData gameData) {
            GameMakerFunction show_debug_message = gameData.Functions.ByName("show_debug_message");

            if (show_debug_message is not null) {
                string name = "submodloader_show_debug_message";
                string scriptString = GMLInteropManager.CallFromGML(GameWriteLine, "string(argument0)");
                (GameMakerCode code, GameMakerFunction function) = gameData.AddCodeAndFunction(name, scriptString);

                gameData.ReplaceAllCalls(show_debug_message, function, code);
            }
        }

        internal static string ModifiersToGMLArgs(ConsoleModifier[] modifiers) => string.Join(", ", modifiers.Select(m => (byte)m));
        internal static string ModifiersToGMLArgs(string[] modifiers) => string.Join(", ", modifiers);

        #endregion

        #region Write

        private static void WriteBoth(string thing) {
            Console.Write(thing);
            File.Write(thing);
        }

        private const string GML_logger_write = "submodloader_logger_write";
        internal static void Write(string message, string caller = null, Color authorColor = default, bool alwaysShowCaller = true, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) {
            modifiers = ReduceModifierArray(modifiers);

            if (alwaysShowCaller || PreviousCaller != caller) {
                WriteBoth(Environment.NewLine);

                bool isSubModLoader = caller is null;
                caller ??= "SubModLoader";
                bool isGame = !isSubModLoader && caller == GameNamePlaceholder;
                caller = isGame ? GameName : caller;
                Color infoColor = isSubModLoader ? SubModLoaderColor : isGame ? GameColor : TimeAuthorColor;

                Console.Write(GetColorVirtSeq(infoColor, Empty));
                DateTime now = DateTime.Now;
                WriteBoth($"{now:HH:mm:ss.fff} [");

                if (!isSubModLoader && !isGame)
                    Console.Write(GetColorVirtSeq(authorColor == Empty ? infoColor : authorColor, Empty));
                WriteBoth(caller);

                Console.Write(GetColorVirtSeq(infoColor, Empty));
                WriteBoth("] ");

                LogData.Add(new() {
                    Then = now,
                    Caller = caller,
                    AuthorColor = authorColor,
                    IsSubModLoader = isSubModLoader,
                    IsGame = isGame,
                    Messages = new()
                });
            }

            Console.Write(GetColorVirtSeq(textColor, backgroundColor, modifiers));
            WriteBoth(message);
            Console.Write(GetResetColorVirtSeq());
            File.Flush();

            LogData.Last().Messages.Add(new() {
                Message = message,
                TextColor = textColor,
                BackgroundColor = backgroundColor,
                HasModifiers = modifiers.Length > 0,
                IsBold = modifiers.Contains(ConsoleModifier.Bold),
                IsItalic = modifiers.Contains(ConsoleModifier.Italic),
                IsUnderline = modifiers.Contains(ConsoleModifier.Underline),
                IsNegative = modifiers.Contains(ConsoleModifier.Negative),
                IsStrike = modifiers.Contains(ConsoleModifier.Strike)
            });

            PreviousCaller = alwaysShowCaller ? null : caller;

            LogChanged = true;
        }
        internal static void Write(object message, string caller = null, Color authorColor = default, bool alwaysShowCaller = true, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            Write(message.ToString(), caller, authorColor, alwaysShowCaller, textColor, backgroundColor, modifiers);

        private const string GML_logger_end_line = "submodloader_logger_end_line";
        internal static void EndWrite(string caller = null) =>
            PreviousCaller = null;

        private const string GML_logger_write_line = "submodloader_logger_write_line";
        internal static void WriteLine(string message = "", string caller = null, Color authorColor = default, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            Write(message, caller, authorColor, alwaysShowCaller: true, textColor, backgroundColor, modifiers);
        internal static void WriteLine(object message, string caller = null, Color authorColor = default, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            WriteLine(message.ToString(), caller, authorColor, textColor, backgroundColor, modifiers);
        internal static void GameWriteLine(string message) {
            if (ShowGameDebugMessages.Value)
                WriteLine(message, GameNamePlaceholder);
        }

        private const string GML_logger_draw_line = "submodloader_logger_draw_line";
        internal static void DrawLine(int length = 30, string caller = null, Color authorColor = default, Color textColor = default, Color backgroundColor = default, params ConsoleModifier[] modifiers) =>
            WriteLine(new string('-', length), caller, authorColor, textColor, backgroundColor, modifiers);

        private const string GML_logger_draw_spacer = "submodloader_logger_draw_spacer";
        internal static void DrawSpacer(string caller = null) {
            WriteBoth(Environment.NewLine);
            File.Flush();

            LogData.Add(new LineData() { IsSpacer = true });

            PreviousCaller = null;

            LogChanged = true;
        }

        private const string GML_logger_write_success = "submodloader_logger_write_success";
        internal static void WriteSuccess(string message, string caller = null, Color authorColor = default) =>
            WriteLine(message, caller, authorColor, ConsoleGreen);
        internal static void WriteSuccess(object message, string caller = null, Color authorColor = default) =>
            WriteLine(message.ToString(), caller, authorColor, ConsoleGreen);

        private const string GML_logger_write_warning = "submodloader_logger_write_warning";
        internal static void WriteWarning(string message, string caller = null, Color authorColor = default) =>
            WriteLine(message, caller, authorColor, ConsoleYellow);
        internal static void WriteWarning(object message, string caller = null, Color authorColor = default) =>
            WriteLine(message.ToString(), caller, authorColor, ConsoleYellow);

        private const string GML_logger_write_error = "submodloader_logger_write_error";
        internal static void WriteError(string message, string caller = null, Color authorColor = default) =>
            WriteLine(message, caller, authorColor, ConsoleRed);
        internal static void WriteError(object message, string caller = null, Color authorColor = default) =>
            WriteLine(message.ToString(), caller, authorColor, ConsoleRed);

        #endregion

        #endregion

        #region Setting Color

        private const string STARTCOLOR = "\x1b[";
        private const string ENDCOLOR = "m";
        private const string COLORRESET = $"{STARTCOLOR}0{ENDCOLOR}";
        private const byte FOREGROUND = 38;
        private const byte BACKGROUND = 48;
        private const byte RGBCOLOR = 2;

        private static ConsoleModifier[] ReduceModifierArray(ConsoleModifier[] modifiers) {
            if (modifiers is null || modifiers.Length == 0)
                return Array.Empty<ConsoleModifier>();

            int bold = 0, italic = 0, underline = 0, negative = 0, strike = 0;

            foreach (ConsoleModifier modifier in modifiers) {
                switch (modifier) {
                    case ConsoleModifier.Bold: bold = 1; break;
                    case ConsoleModifier.Italic: italic = 1; break;
                    case ConsoleModifier.Underline: underline = 1; break;
                    case ConsoleModifier.Negative: negative = 1; break;
                    case ConsoleModifier.Strike: strike = 1; break;
                }
            }

            ConsoleModifier[] result = new ConsoleModifier[bold + italic + underline + negative + strike];
            int i = 0;
            if (bold != 0) result[i++] = ConsoleModifier.Bold;
            if (italic != 0) result[i++] = ConsoleModifier.Italic;
            if (underline != 0) result[i++] = ConsoleModifier.Underline;
            if (negative != 0) result[i++] = ConsoleModifier.Negative;
            if (strike != 0) result[i++] = ConsoleModifier.Strike;

            return result.ToArray();
        }

        private static string GetColorVirtSeq(Color foreground, Color background, params ConsoleModifier[] modifiers) {
            if (foreground.Type == ColorType.Empty && background.Type == ColorType.Empty && (modifiers == null || modifiers.Length == 0))
                return GetResetColorVirtSeq();

            bool bold = false, italic = false, underline = false, negative = false, strike = false;
            foreach (ConsoleModifier modifier in modifiers) {
                switch (modifier) {
                    case ConsoleModifier.Bold: bold = true; break;
                    case ConsoleModifier.Italic: italic = true; break;
                    case ConsoleModifier.Underline: underline = true; break;
                    case ConsoleModifier.Negative: negative = true; break;
                    case ConsoleModifier.Strike: strike = true; break;
                }
            }

            List<byte> codes = new();

            if (bold)
                codes.Add((byte)ConsoleModifier.Bold);
            if (italic)
                codes.Add((byte)ConsoleModifier.Italic);
            if (underline)
                codes.Add((byte)ConsoleModifier.Underline);
            if (negative)
                codes.Add((byte)ConsoleModifier.Negative);
            if (strike)
                codes.Add((byte)ConsoleModifier.Strike);

            if (foreground.Type == ColorType.RGBA) {
                codes.AddRange(new byte[] { FOREGROUND, RGBCOLOR, foreground.R, foreground.G, foreground.B });
            } else if (foreground.Type == ColorType.Console)
                codes.Add((byte)foreground.Foreground);

            if (background.Type == ColorType.RGBA) {
                codes.AddRange(new byte[] { BACKGROUND, RGBCOLOR, background.R, background.G, background.B });
            } else if (background.Type == ColorType.Console)
                codes.Add((byte)background.Background);

            if (codes.Count == 0)
                return "";

            string virtSeq = STARTCOLOR;
            for (int i = 0; i < codes.Count - 1; i++)
                virtSeq += $"{codes[i]};";
            virtSeq += $"{codes.Last()}{ENDCOLOR}";

            return virtSeq;
        }

        private static string GetResetColorVirtSeq() => COLORRESET;

        #endregion
    }
}
