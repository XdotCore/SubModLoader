using SubmachineModLib;
using SubmachineModLib.Models;
using SubModLoader.GameData.Extensions;
using SubModLoader.GMLInterop;
using SubModLoader.Mods.Attributes;
using SubModLoader.Storage;
using SubModLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

// TODO: make debugger work better

namespace SubModLoader.Mods {
    internal static class Modding {
        #region Data

        internal static bool HasAppliedMods { get; private set; } = false;

        private static Assembly[] LibraryAssemblies { get; set; }
        private static List<(ISubModInfoAttribute info, SubMod mod)> LoadedMods { get; } = new();

        private static GameMakerData GameData { get; set; }

        // Gets unmodded.win rather than data.win in order to get the original file through SubModLoaderNative/DataWinHook.cpp
        public static void LoadUnModdedData() {
            Logger.WriteLine("Loading data.win...");
            using FileStream dataWin = File.OpenRead("unmodded.win");
            GameMakerData result = GameMakerIO.Read(dataWin) ?? throw new IOException("Could not load data.win");
            GameData = result;
        }

        private static void WriteModdedData(GameMakerData data) {
            using FileStream moddedWin = File.OpenWrite("modded.win");
            GameMakerIO.Write(moddedWin, data);
        }

        internal static void LogAssemblyInformation() {
            GameMakerGeneralInfo info = GameData.GeneralInfo;

            Logger.DrawLine();
            Logger.WriteLine($"SubModLoader v{typeof(SubModLoader).Assembly.GetName().Version} x{(Environment.Is64BitProcess ? "64" : "86")}");
            Logger.WriteLine("Powered by GameMakerModTool");
            Logger.WriteLine($"OS: {OSUtils.GetOSName()}");
            Logger.WriteLine($"Game: {info.Name.Content}");
            Logger.WriteLine($"GameMaker: v{info.Major}.{info.Minor}.{info.Release}.{info.Build}");
            Logger.DrawLine();
            Logger.DrawSpacer();
        }

        private static void LogModInformation(ISubModInfoAttribute info) {
            Logger.DrawLine();
            Logger.WriteLine("Loaded mod:");
            Logger.WriteLine($"{info?.Name ?? "null"} v{info?.Version ?? "null"} by {info?.Author ?? "null"}");
            Logger.DrawLine();
        }

        #endregion

        #region Do mods

        internal static void ApplyMods() {
            Logger.GameName = GameData.GeneralInfo.DisplayName.Content;

            GMLInteropManager.Initialize(GameData);
            GMLInteropManager.Add_call_csharp(GameData);
            Add_gml_initialize(GameData);
            Color.AddTypeToInterop(GameData);
            Logger.Replace_show_debug_message(GameData);
            Logger.AddGMLFunctions(GameData);
            Logger.WriteLine("Added builtin mods...");
            Logger.DrawSpacer();

            LoadMods();
            ApplyMods(GameData);

            GMLInteropManager.Finalize(GameData);
            Logger.WriteLine("Finalized gml to c# interop...");
            Populate_gml_initialize(GameData);
            Logger.WriteLine("Populated gml initialize event...");

            WriteModdedData(GameData);
            Logger.WriteLine("Wrote modded.win...");
            Logger.WriteLine("Starting game...");
            Logger.DrawSpacer();

            HasAppliedMods = true;
            GameData = null; // No futher use
        }

        private static void LoadMods() {
            // TODO: figure out a better fix for this
            // idk why this happens, but for some reason loading new assemblies loads a new "DefaultContext" with assemblies loading a second time unless I do this
            LibraryAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyLoadContext.Default.Resolving += (alc, name) => {
                Assembly lib = LibraryAssemblies.FirstOrDefault(asm => asm.GetName().Name == name.Name);
                return lib;
            };

            if (!Directory.Exists("Mods/"))
                Directory.CreateDirectory("Mods/");

            foreach (string modFile in Directory.EnumerateFiles("Mods/", "*.dll")) {
                try {
                    Assembly modDll = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(modFile));

                    ISubModInfoAttribute info = modDll.GetCustomAttributes().FirstOrDefault(attr => attr is ISubModInfoAttribute) as ISubModInfoAttribute;
                    if (info is null)
                        return; // not a mod
                    LogModInformation(info);

                    SubModColorAttribute color = modDll.GetCustomAttribute<SubModColorAttribute>();

                    SubMod mod = (SubMod)Activator.CreateInstance(info.ModType);
                    mod.Logger = new(info.Name, color?.AuthorColor ?? Color.Empty);
                    mod.Settings = Settings.GetSettings(info.Name);
                    LoadedMods.Add((info, mod));

                    mod.OnLoad();
                    Logger.DrawSpacer();
                } catch (Exception e) {
                    Logger.WriteError($"Failed to load mod at \"{modFile}\" because: {e}");
                    Logger.DrawSpacer();
                }
            }
        }

        private static void ApplyMods(GameMakerData GameData) {
            foreach ((ISubModInfoAttribute info, SubMod mod) in LoadedMods) {
                try {
                    Logger.WriteLine($"Applying {info.Name}...");
                    mod.ApplyMod(GameData);
                    Logger.DrawSpacer();
                } catch (Exception e) {
                    Logger.WriteError($"Failed to apply mod \"{info.Name}\" because: {e}");
                    Logger.DrawSpacer();
                }
            }
        }

        #region Builtin Mods

        private const string Object_gml_initializer = "o_submodloader_gml_initializer";
        private static List<GameMakerFunction> OnGMLInitializeEvents { get; } = new();

        internal static void AddOnGMLInitialize(GameMakerFunction onInit) => OnGMLInitializeEvents.Add(onInit);

        // TODO: decide if I should keep this or not
        private static void Add_gml_initialize(GameMakerData gameData) {
            GameMakerGameObject gameObject = new() {
                Name = gameData.Strings.MakeString(Object_gml_initializer),
                Visible = false
            };
            gameData.GameObjects.Add(gameObject);

            GameMakerRoom.GameObject roomGameObject = new() { InstanceID = gameData.GeneralInfo.LastObj++, ObjectDefinition = gameObject };

            if (gameData.IsGameMaker2()) {
                GameMakerRoom.Layer layer = gameData.Rooms[0].AddLayer<GameMakerRoom.Layer.LayerInstancesData>(gameData, "submodloader_gml_initializer_layer");
                layer.InstancesData.Instances.Add(roomGameObject);
            }

            gameData.Rooms[0].GameObjects.Add(roomGameObject);
        }

        private static void Populate_gml_initialize(GameMakerData GameData) {
            string initializeEvent = "";
            if (OnGMLInitializeEvents.Count > 0) {
                initializeEvent = string.Join("()\n", OnGMLInitializeEvents);
                initializeEvent += "()";
            }
            GameData.AddCode($"gml_Object_{Object_gml_initializer}_Create_0", initializeEvent);
        }

        #endregion

        #endregion
    }
}
