using SubmachineModLib;
using SubmachineModLib.Decompiler;
using SubmachineModLib.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubModLoader.GameData.Extensions {
    /// <summary>
    /// Helpful extension funcs
    /// </summary>
    public static class GameMakerDataExt {
        /// <summary>
        /// Adds code as a function or replaces existing function
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="name">The name of the function</param>
        /// <param name="codeText">The code for the function</param>
        /// <returns>A 2-tuple of references to both the new code and the funtion</returns>
        /// <remarks>
        /// If in GMS >= 2.3, this wraps your code with "<c>function {name}() { }</c>" and is treated as a global script, while in lower versions it is treated as a script
        /// </remarks>
        public static (GameMakerCode code, GameMakerFunction function) AddCodeAndFunction(this GameMakerData gameData, string name, string codeText) {
            bool isv2_3 = gameData.IsVersionAtLeast(2, 3);

            if (isv2_3)
                codeText = $"function {name}() {{\n{codeText}\n}}";

            string codeName = $"{(isv2_3 ? "gml_GlobalScript_" : "gml_Script_")}{name}";

            gameData.AddCode(codeName, codeText);

            if (isv2_3)
                return (gameData.Code.ByName(codeName), gameData.Functions.ByName($"gml_Script_{name}"));
            else {
                GameMakerScript newScript = gameData.Scripts.ByName(name);
                GameMakerFunction func = new() { Name = newScript.Name, NameStringID = gameData.Strings.IndexOf(newScript.Name) };
                gameData.Functions.Add(func);
                return (newScript.Code, func);
            }
        }
        /// <summary>
        /// Adds code as a function
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The new code</param>
        /// <param name="function">The new function</param>
        /// <param name="name">The name of the function</param>
        /// <param name="codeText">The code for the function</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <remarks>
        /// If in GMS >= 2.3, this wraps your code with "<c>function {name}() { }</c>" and is treated as a global script, while in lower versions it is treated as a script
        /// </remarks>
        public static bool TryAddCodeAndFunction(this GameMakerData gameData, out GameMakerCode code, out GameMakerFunction function, string name, string codeText) {
            try {
                (code, function) = gameData.AddCodeAndFunction(name, codeText);
                return true;
            } catch {
                (code, function) = (null, null);
                return false;
            }
        }

        /// <summary>
        /// Replaces all calls to one function with another
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="function">The function to replace</param>
        /// <param name="replacement">The function to replace with</param>
        /// <param name="exceptions">Any code that you want to skip this change with</param>
        public static void ReplaceAllCalls(this GameMakerData gameData, GameMakerFunction function, GameMakerFunction replacement, params GameMakerCode[] exceptions) {
            ArgumentNullException.ThrowIfNull(function, nameof(function));
            ArgumentNullException.ThrowIfNull(replacement, nameof(replacement));

            foreach (GameMakerCode code in gameData.Code.Where(utc => utc.ParentEntry is null && !(exceptions?.Contains(utc) ?? false)))
                foreach (GameMakerInstruction instruction in code.Instructions.Where(uti => uti.Kind == GameMakerInstruction.Opcode.Call && uti.Function.Target == function))
                    instruction.Function.Target = replacement;
        }
        /// <summary>
        /// Replaces all calls to one function with another
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="function">The function to replace</param>
        /// <param name="replacement">The function to replace with</param>
        /// <param name="exceptions">Any code that you want to skip this change with</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryReplaceAllCalls(this GameMakerData gameData, GameMakerFunction function, GameMakerFunction replacement, params GameMakerCode[] exceptions) {
            try {
                gameData.ReplaceAllCalls(function, replacement, exceptions);
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Gets decompiled code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>The decompiled code</returns>
        public static string GetDecompiledText(this GameMakerData gameData, string codeName, GlobalDecompileContext context = null) => GetDecompiledText(gameData, gameData.Code.ByName(codeName), context);
        /// <summary>
        /// Gets the decompiled code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="text">The decompiled code</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryGetDecompiledText(this GameMakerData gameData, out string text, string codeName, GlobalDecompileContext context = null) {
            try {
                text = gameData.GetDecompiledText(codeName, context);
                return true;
            } catch {
                text = null;
                return false;
            }
        }
        /// <summary>
        /// Gets decompiled code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The code</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>The decompiled code</returns>
        internal static string GetDecompiledText(this GameMakerData gameData, GameMakerCode code, GlobalDecompileContext context = null) {
            ArgumentNullException.ThrowIfNull(code, nameof(code));
            if (code.ParentEntry is not null)
                throw new ArgumentException($"This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", decompile that instead.", nameof(code));

            GlobalDecompileContext decompCtx = context ?? new GlobalDecompileContext(gameData, false);
            return Decompiler.Decompile(code, decompCtx);
        }
        /// <summary>
        /// Gets decompiled code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="text">The decompiled code</param>
        /// <param name="code">The code</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>True if successful, false otherwise</returns>
        internal static bool TryGetDecompiledText(this GameMakerData gameData, out string text, GameMakerCode code, GlobalDecompileContext context = null) {
            try {
                text = gameData.GetDecompiledText(code, context);
                return true;
            } catch {
                text = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the decompiled assembly
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="codeName">The name of the code</param>
        /// <returns>The decompiled assembly</returns>
        public static string GetDisassemblyText(this GameMakerData gameData, string codeName) => gameData.GetDisassemblyText(gameData.Code.ByName(codeName));
        /// <summary>
        /// Gets the decompiled assembly
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="text">The decompiled assembly</param>
        /// <param name="codeName">The name of the code</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryGetDisassemblyText(this GameMakerData gameData, out string text, string codeName) {
            try {
                text = gameData.GetDisassemblyText(codeName);
                return true;
            } catch {
                text = null;
                return false;
            }
        }
        /// <summary>
        /// Gets the decompiled assembly
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The code</param>
        /// <returns>The decompiled assembly</returns>
        public static string GetDisassemblyText(this GameMakerData gameData, GameMakerCode code) {
            ArgumentNullException.ThrowIfNull(code, nameof(code));
            if (code.ParentEntry is not null)
                throw new ArgumentException($"This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", decompile that instead.", nameof(code));

            return code.Disassemble(gameData.Variables, gameData.CodeLocals.For(code));
        }
        /// <summary>
        /// Gets the decompiled assembly
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="text">The decompiled assembly</param>
        /// <param name="code">The code</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryGetDisassemblyText(this GameMakerData gameData, out string text, GameMakerCode code) {
            try {
                text = gameData.GetDisassemblyText(code);
                return true;
            } catch {
                text = null;
                return false;
            }
        }


        // wish this was implemented in GameMakerModLib
        /// <summary>
        /// Adds new code, or replaces existing code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="codeText">The code text</param>
        /// <param name="isGML">True for gml, false for gml assembly</param>
        /// <param name="doParse">True to register the code in elsewhere in the game data for scripts, globals, and object events</param>
        /// <returns>The new or replaced code</returns>
        public static GameMakerCode AddCode(this GameMakerData gameData, string codeName, string codeText, bool isGML = true, bool doParse = true) {
            GameMakerCode code = gameData.Code.ByName(codeName);
            if (code is null) {
                code = new() { Name = gameData.Strings.MakeString(codeName) };
                gameData.Code.Add(code);
            } else if (code.ParentEntry is not null)
                throw new ArgumentException($"This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", import with that instead.", nameof(codeName));

            if (gameData.GeneralInfo.BytecodeVersion > 14 && gameData.CodeLocals.ByName(codeName) == null) {
                GameMakerCodeLocals locals = new() {
                    Name = code.Name,
                    Locals = { new() {
                        Name = gameData.Strings.MakeString("arguments"),
                        Index = 0
                    } }
                };

                code.LocalsCount = 1;
                gameData.CodeLocals.Add(locals);
            }

            if (doParse) {
                if (codeName.StartsWith("gml_Script")) {
                    string scriptName = codeName[11..];
                    GameMakerScript script = gameData.Scripts.ByName(scriptName);
                    if (script is null) {
                        script = new() {
                            Name = gameData.Strings.MakeString(scriptName),
                            Code = code
                        };

                        gameData.Scripts.Add(script);
                    } else
                        script.Code = code;
                } else if (codeName.StartsWith("gml_GlobalScript")) {
                    GameMakerGlobalInit initEntry = gameData.GlobalInitScripts.FirstOrDefault(gi => gi.Code.Name.Content == codeName);
                    if (initEntry is null) {
                        initEntry = new() { Code = code };
                        gameData.GlobalInitScripts.Add(initEntry);
                    } else
                        initEntry.Code = code;
                } else if (codeName.StartsWith("gml_Object")) {
                    string afterPrefix = codeName[11..];
                    string[] words = afterPrefix.Split('_');
                    string methodNumberStr = words[^1];
                    string methodName = words[^2];
                    string objName = string.Join('_', words[..^2]);

                    bool wasNumber = int.TryParse(methodNumberStr, out int methodNumber);
                    if (wasNumber) {
                        if (methodName == "Collision" && (methodNumber >= gameData.GameObjects.Count || methodNumber < 0))
                            throw new Exception($"Object ID {methodNumber} was not found");
                    } else {
                        int collisionIndex = afterPrefix.LastIndexOf("_Collision_");
                        if (collisionIndex != -1) {
                            objName = afterPrefix[..collisionIndex];
                            methodNumberStr = afterPrefix[(collisionIndex + 1)..];
                            methodName = "Collision";

                            if (gameData.IsVersionAtLeast(2, 3)) {
                                bool foundObject = false;
                                for (int i = 0; i < gameData.GameObjects.Count && !foundObject; i++) {
                                    if (gameData.GameObjects[i].Name.Content == methodNumberStr) {
                                        methodNumber = i;
                                        foundObject = true;
                                    }
                                }

                                if (!foundObject)
                                    throw new Exception("Could not get collision other object");
                            } else
                                throw new Exception($"Could not get collision other object");
                        } else
                            throw new Exception("Could not get event method number");
                    }

                    GameMakerGameObject obj = gameData.GameObjects.ByName(objName) ?? throw new NullReferenceException("Could not find object for event");

                    int eventIdx = (int)Enum.Parse<EventType>(methodName);
                    bool isDuplicate = false;
                    try {
                        isDuplicate = obj.Events?[eventIdx]?.Any(evnt => evnt?.Actions?.Any(action => action?.CodeId?.Name?.Content == codeName) ?? false) ?? false;
                    } catch { /*I sure do hope that no errors will be cast with all this null checking*/ }

                    if (!isDuplicate) {
                        GameMakerPointerList<GameMakerGameObject.Event> eventList = obj.Events[eventIdx];

                        GameMakerGameObject.Event evnt = new() {
                            EventSubtype = (uint)methodNumber,
                            Actions = { new() {
                                ActionName = code.Name,
                                CodeId = code
                            } }
                        };
                        eventList.Add(evnt);
                    }
                }
            }

            if (isGML)
                code.ReplaceGML(codeText, gameData);
            else
                code.Replace(Assembler.Assemble(codeText, gameData));

            return code;
        }
        /// <summary>
        /// Adds new code, or replaces existing code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The new or replaced code</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="codeText">The code text</param>
        /// <param name="isGML">True for gml, false for gml assembly</param>
        /// <param name="doParse">True to register the code in elsewhere in the game data for scripts, globals, and object events</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryAddCode(this GameMakerData gameData, out GameMakerCode code, string codeName, string codeText, bool isGML = true, bool doParse = true) {
            try {
                code = gameData.AddCode(codeName, codeText, isGML, doParse);
                return true;
            } catch {
                code = null;
                return false;
            }
        }

        /// <summary>
        /// Adds new code, or replaces existing code, from a file
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="file">The file to get the code from</param>
        /// <param name="isGML">True for gml, false for gml assembly</param>
        /// <param name="doParse">True to register the code in elsewhere in the game data for scripts, globals, and object events</param>
        /// <returns>The new or replaced code</returns>
        public static GameMakerCode AddCodeFromFile(this GameMakerData gameData, string file, bool isGML = true, bool doParse = true) {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string fileExtLower = Path.GetExtension(file).ToLower();
            string expectedExt = isGML ? ".gml" : ".asm";
            string fileNameLower = fileName.ToLower();

            if (fileExtLower != expectedExt)
                throw new ArgumentException($"File extension is not correct, got {fileExtLower} when expecting {expectedExt}", nameof(file));
            if (gameData.IsGameMaker2() && (fileNameLower.EndsWith("cleanup_0") || fileNameLower.EndsWith("precreate_0")))
                throw new ArgumentException("Cleanup and Precreate events do not exist before GMS 2", nameof(file));

            string gmlCode = File.ReadAllText(file);
            return gameData.AddCode(fileName, gmlCode, isGML, doParse);
        }
        /// <summary>
        /// Adds new code, or replaces existing code, from a file
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The new or replaced code</param>
        /// <param name="file">The file to get the code from</param>
        /// <param name="isGML">True for gml, false for gml assembly</param>
        /// <param name="doParse">True to register the code in elsewhere in the game data for scripts, globals, and object events</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryAddCodeFromFile(this GameMakerData gameData, out GameMakerCode code, string file, bool isGML = true, bool doParse = true) {
            try {
                code = gameData.AddCodeFromFile(file, isGML, doParse);
                return true;
            } catch {
                code = null;
                return false;
            }
        }

        /// <summary>
        /// Replaces a portion of the gml code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The code</param>
        /// <param name="keyword">The text to replace</param>
        /// <param name="replacement">The text to replace with</param>
        /// <param name="caseSensitive">Whether the text to replace is case sensitive</param>
        /// <param name="isRegex">Whether the replacement process uses regular expressions</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        public static void ReplaceTextInGML(this GameMakerData gameData, GameMakerCode code, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null) {
            string text = gameData.GetDecompiledText(code, context);

            keyword = keyword.ReplaceLineEndings("\n");
            replacement = replacement.ReplaceLineEndings("\n");
            string newText;
            if (isRegex)
                newText = Regex.Replace(text, keyword, replacement, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            else if (caseSensitive)
                newText = text.Replace(keyword, replacement);
            else
                newText = Regex.Replace(text, Regex.Escape(keyword), replacement.Replace("$", "$$"), RegexOptions.IgnoreCase);

            code.ReplaceGML(newText, gameData);
        }
        /// <summary>
        /// Replaces a portion of the gml code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="code">The code</param>
        /// <param name="keyword">The text to replace</param>
        /// <param name="replacement">The text to replace with</param>
        /// <param name="caseSensitive">Whether the text to replace is case sensitive</param>
        /// <param name="isRegex">Whether the replacement process uses regular expressions</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryReplaceTextInGML(this GameMakerData gameData, GameMakerCode code, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null) {
            try {
                gameData.ReplaceTextInGML(code, keyword, replacement, caseSensitive, isRegex, context);
                return true;
            } catch {
                return false;
            }
        }
        /// <summary>
        /// Replaces a portion of the gml code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="keyword">The text to replace</param>
        /// <param name="replacement">The text to replace with</param>
        /// <param name="caseSensitive">Whether the text to replace is case sensitive</param>
        /// <param name="isRegex">Whether the replacement process uses regular expressions</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        public static void ReplaceTextInGML(this GameMakerData gameData, string codeName, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null) =>
            gameData.ReplaceTextInGML(gameData.Code.ByName(codeName), keyword, replacement, caseSensitive, isRegex, context);
        /// <summary>
        /// Replaces a portion of the gml code
        /// </summary>
        /// <param name="gameData">The game data</param>
        /// <param name="codeName">The name of the code</param>
        /// <param name="keyword">The text to replace</param>
        /// <param name="replacement">The text to replace with</param>
        /// <param name="caseSensitive">Whether the text to replace is case sensitive</param>
        /// <param name="isRegex">Whether the replacement process uses regular expressions</param>
        /// <param name="context">The <see cref="GlobalDecompileContext"/></param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryReplaceTextInGML(this GameMakerData gameData, string codeName, string keyword, string replacement, bool caseSensitive = false, bool isRegex = false, GlobalDecompileContext context = null) {
            try {
                gameData.ReplaceTextInGML(codeName, keyword, replacement, caseSensitive, isRegex, context);
                return true;
            } catch {
                return false;
            }
        }
    }
}
