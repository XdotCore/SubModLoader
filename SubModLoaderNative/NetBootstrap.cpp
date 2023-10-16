#include <windows.h>
#include <string>
#include <format>
#include "GMLToC#Interop.h"
#include "ImGUIHooks.h"
#include "NetBootstrap.h"

#include "nethost.h"
#include "hostfxr.h"
#include "coreclr_delegates.h"

using namespace std;
using namespace SubModLoader;

namespace Bootstrap {

    struct hostfxr_dll {
        HMODULE dll;
        hostfxr_initialize_for_runtime_config_fn init;
        hostfxr_get_runtime_delegate_fn getDelegate;
        hostfxr_close_fn close;
    } hostfxr;

    bool LoadHostfxr() {
        char_t hostfxrPath[MAX_PATH];
        size_t hostfxrPathSize = MAX_PATH;
        int err = get_hostfxr_path(hostfxrPath, &hostfxrPathSize, nullptr);
        if (err) {
            MessageBox(NULL, format(L"Could not find hostfxr library. error code: {:#x}", err).c_str(), L"Error", MB_OK);
            return false;
        }

        hostfxr.dll = LoadLibrary(hostfxrPath);
        if (hostfxr.dll == NULL) {
            err = GetLastError();
            MessageBoxA(NULL, format("Could not load hostfxr library. error code: {}, {}", err, system_category().message(err)).c_str(), "Error", MB_OK);
            return false;
        }

        hostfxr.init = (hostfxr_initialize_for_runtime_config_fn)GetProcAddress(hostfxr.dll, "hostfxr_initialize_for_runtime_config");
        hostfxr.getDelegate = (hostfxr_get_runtime_delegate_fn)GetProcAddress(hostfxr.dll, "hostfxr_get_runtime_delegate");
        hostfxr.close = (hostfxr_close_fn)GetProcAddress(hostfxr.dll, "hostfxr_close");

        return (hostfxr.init && hostfxr.getDelegate && hostfxr.close);
    }

    bool GetDotNetLoadAssembly(const char_t* configPath, load_assembly_and_get_function_pointer_fn* dotNetLoadAssembly) {
        void* result = nullptr;

        hostfxr_handle hostfxrHandle = nullptr;
        int err = hostfxr.init(configPath, nullptr, &hostfxrHandle);
        if (err || hostfxrHandle == nullptr) {
            MessageBox(NULL, format(L"Hostfxr init failed. error code: {:#x}", err).c_str(), L"Error", MB_OK);
            hostfxr.close(hostfxrHandle);
            return false;
        }
        
        err = hostfxr.getDelegate(hostfxrHandle, hdt_load_assembly_and_get_function_pointer, &result);
        if (err || result == nullptr) {
            MessageBox(NULL, format(L"Hostfxr get hdt_load_assembly_and_get_function_pointer failed. error code: {:#x}", err).c_str(), L"Error", MB_OK);
            hostfxr.close(hostfxrHandle);
            return false;
        }
        *dotNetLoadAssembly = (load_assembly_and_get_function_pointer_fn)result;

        hostfxr.close(hostfxrHandle);
        return true;
    }

    typedef bool(__stdcall* entryPointDelegate)();
    entryPointDelegate EntryPoint = nullptr;

    constexpr const char_t subModLoaderRuntimeConfig[] = L"SubModloader/SubModLoader.runtimeconfig.json";
    constexpr const char_t subModLoaderAssembly[] = L"SubModLoader/SubModLoader.dll";
    constexpr const char_t subModLoaderAssemblyName[] = L"SubModLoader";

    bool LoadDotNetFunc(load_assembly_and_get_function_pointer_fn dotNetLoadAssembly, const char_t* type, const char_t* method, void** delegate) {
        *delegate = nullptr;

        int err = dotNetLoadAssembly(subModLoaderAssembly, format(L"{}, {}", type, subModLoaderAssemblyName).c_str(), method, format(L"{}+{}Delegate, {}", type, method, subModLoaderAssemblyName).c_str(), nullptr, delegate);
        if (err || *delegate == nullptr) {
            MessageBox(NULL, format(L"Could not load {}.{} error code: {}", type, method, err).c_str(), L"Error", MB_OK);
            return false;
        }
        return true;
    }

    bool LoadSubModLoader() {
        if (!LoadHostfxr())
            return false;

        load_assembly_and_get_function_pointer_fn dotNetLoadAssembly = nullptr;
        if (!GetDotNetLoadAssembly(subModLoaderRuntimeConfig, &dotNetLoadAssembly)) {
            MessageBox(NULL, L"Get dotnet assembly loader failed", L"Error", MB_OK);
            return false;
        }

        if (!LoadDotNetFunc(dotNetLoadAssembly, L"SubModLoader.SubModLoader", L"EntryPoint", (void**)&EntryPoint))
            return false;

        if (!LoadDotNetFunc(dotNetLoadAssembly, L"SubModLoader.GMLInterop.GMLInteropManager", L"CallCSharp", (void**)&GMLInterop::GMLInteropManager::CallCSharp))
            return false;

        if (!LoadDotNetFunc(dotNetLoadAssembly, L"SubModLoader.GMLInterop.GMLInteropWriter", L"DeleteBytes", (void**)&GMLInterop::GMLInteropWriter::DeleteBytes))
            return false;

        if (!LoadDotNetFunc(dotNetLoadAssembly, L"SubModLoader.GUI.Overlay", L"Draw", (void**)&GUI::Overlay::Draw))
            return false;

        if (!LoadDotNetFunc(dotNetLoadAssembly, L"SubModLoader.GUI.Overlay", L"GetIsImGuiShowing", (void**)&GUI::Overlay::GetIsImGuiShowing))
            return false;

        return true;
    }

    void OpenConsole() {
        AllocConsole();

        FILE* fDummy;
        freopen_s(&fDummy, "CONIN$", "r", stdin);
        freopen_s(&fDummy, "CONOUT$", "w", stdout);
        freopen_s(&fDummy, "CONOUT$", "w", stderr);

        SetConsoleOutputCP(CP_UTF8);
    }

    void EnableVTMode() {
        HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
        if (!console || console == INVALID_HANDLE_VALUE)
            return;

        DWORD consoleMode = 0;
        if (!GetConsoleMode(console, &consoleMode))
            return;

        consoleMode |= ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        if (!SetConsoleMode(console, consoleMode))
            return;
    }

    bool RunSubModLoader() {
        OpenConsole();
        EnableVTMode();

        if (EntryPoint != nullptr)
            return EntryPoint();
        return false;
    }
}