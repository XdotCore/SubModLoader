using ImGuiNET;
using SubModLoader.Storage;
using SubModLoader.Storage.Widget;
using SubModLoader.Storage.Widget.Item;
using SubModLoader.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SubModLoader.GUI {
    internal static class Overlay {
        private static SettingsCategory OverlayCategory { get; } = Settings.SubModLoaderSettings.GetCategory("Overlay");

        // TODO: expand this to more keys
        public enum SpecialKeys {
            Escape = ImGuiKey.Escape,
            LeftControl = ImGuiKey.LeftCtrl,
            RightControl = ImGuiKey.RightCtrl,
            LeftAlt = ImGuiKey.LeftAlt,
            RightAlt = ImGuiKey.RightAlt,
            Menu = ImGuiKey.Menu,
            F1 = ImGuiKey.F1,
            F2 = ImGuiKey.F2,
            F3 = ImGuiKey.F3,
            F4 = ImGuiKey.F4,
            F5 = ImGuiKey.F5,
            F6 = ImGuiKey.F6,
            F7 = ImGuiKey.F7,
            F8 = ImGuiKey.F8,
            F9 = ImGuiKey.F9,
            F10 = ImGuiKey.F10,
            F11 = ImGuiKey.F11,
            F12 = ImGuiKey.F12,
        }
        private static SettingsEnum<SpecialKeys> ShowKey { get; } = SettingsEnum<SpecialKeys>.Get(OverlayCategory, "ShowOverlayKey", SpecialKeys.F3,
                                                                                                  showInImGui: true, "Show Overlay Key", "The key that opens and closes the SubModLoader GUI.", SettingsEnumWidget.Combo);
        private static SettingsBool IsOverlayShowing { get; } = SettingsBool.Get(OverlayCategory, "IsOverlayShowing", true);

        private delegate void DrawDelegate();
        internal static void Draw() {
            try {
                if (ImGui.IsKeyPressed((ImGuiKey)ShowKey.Value, false))
                    IsOverlayShowing.Value = !IsOverlayShowing.Value;

                if (!IsOverlayShowing.Value)
                    return;

                ImGui.BeginMainMenuBar();

                if (ImGui.BeginMenu("SubModLoader")) {
                    if (ImGui.MenuItem("Settings", null, Settings.IsSettingsOpen.Value))
                        Settings.IsSettingsOpen.Value = !Settings.IsSettingsOpen.Value;
                    if (ImGui.MenuItem("Debug Console", null, Logger.IsDebugConsoleOpen.Value))
                        Logger.IsDebugConsoleOpen.Value = !Logger.IsDebugConsoleOpen.Value;
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();

                Logger.ShowConsoleWindow();

                Settings.ShowSettingsWindow();
            } catch (Exception e) {
                Logger.WriteError(e);
            }
        }

        private delegate bool GetIsImGuiShowingDelegate();
        internal static bool GetIsImGuiShowing() => IsOverlayShowing.Value;
    }
}
