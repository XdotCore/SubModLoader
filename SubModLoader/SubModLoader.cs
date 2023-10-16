using SubModLoader.Mods;
using SubModLoader.Storage;
using SubModLoader.Utils;
using System;

namespace SubModLoader {
    internal static class SubModLoader {
        private delegate bool EntryPointDelegate();
        internal static bool EntryPoint() => Initialize();

        private static bool Initialize() {
            try {
                Settings.Load();

                Modding.LoadUnModdedData();
                Modding.LogAssemblyInformation();
                Modding.ApplyMods();
            } catch (Exception e) {
                Logger.WriteError(e);
                return false;
            }

            return true;
        }
    }
}
