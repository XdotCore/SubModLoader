#include <windows.h>
#include "DataWinHook.h"
#include "ImGUIHooks.h"
#include "NetBootstrap.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
        case DLL_PROCESS_ATTACH: {
            if (Bootstrap::LoadSubModLoader()) {
                Bootstrap::AttachDataWinHook();
                Bootstrap::AttachImGuiHooks();
            }
            break;
        }
        case DLL_PROCESS_DETACH: {
            Bootstrap::DetachDataWinHook();
            Bootstrap::DetachImGuiHooks();
            break;
        }
        break;
    }
    return TRUE;
}
