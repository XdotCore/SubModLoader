using ImGuiNET;
using SubmachineModLib;
using SubmachineModLib.Models;
using SubModLoader.GMLInterop;
using SubModLoader.Storage;
using SubModLoader.Utils;
using System;

namespace SubModLoader.Mods {
    /// <summary>
    /// The base class for all mods
    /// </summary>
    public abstract class SubMod {
        /// <summary>
        /// Used to log to the console and to the log file
        /// </summary>
        protected internal Logger Logger { get; internal set; }
        /// <summary>
        /// Used to save mod settings to the settings file
        /// </summary>
        protected internal Settings Settings { get; internal set; }

        /// <summary>
        /// Tells if the apply mods phase has passed and no further changes to the game can be made
        /// </summary>
        public static bool HasAppliedMods => Modding.HasAppliedMods;

        #region Virtual Funcs

        /// <summary>
        /// Called when the mod is first loaded
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called when the mod is expected to modify game data
        /// </summary>
        /// <param name="data">The game data for modification</param>
        public virtual void ApplyMod(GameMakerData data) { }

        /// <summary>
        /// Called when <see cref="ImGui"/> can be used, make your widgets here
        /// </summary>
        public virtual void Draw() { }

        // TODO: implement or remove
        /// <summary>
        /// Called when the mod is unloaded
        /// </summary>
        public virtual void OnUnload() { }

        #endregion

        /// <summary>
        /// Allows the use of C# function calls within gml
        /// </summary>
        /// <typeparam name="T">The function type</typeparam>
        /// <param name="function">The function</param>
        /// <param name="gmlVarsOrExpressions">The variables and expressions within gml to be evaluated for the function parameters</param>
        /// <remarks>
        /// Limitations:
        /// <list type="bullet">
        /// <item>Optional parameters are not currently supported, you must supply a gml variable or expression for each parameter. Create a wrapper method if you need optional parameters. This may change in the future.</item>
        /// <item>Param modifiers <see langword="out"/> and <see langword="ref"/> are not currently supported. This may change in the future.</item>
        /// <item>Generic methods are not currently supported. This may change in the future.</item>
        /// <item>The function must be a <see langword="static"/> method since no instance information can be provided from gml.</item>
        /// </list>
        /// </remarks>
        public static string CallFromGML<T>(T function, params string[] gmlVarsOrExpressions) where T : Delegate => GMLInteropManager.CallFromGML(function, gmlVarsOrExpressions);

        /// <summary>
        /// Adds the function to be called when the game starts in gml
        /// </summary>
        /// <param name="initFunc">The function to be called on intialize</param>
        public static void AddOnGMLInitialize(GameMakerFunction initFunc) => Modding.AddOnGMLInitialize(initFunc);
    }
}
