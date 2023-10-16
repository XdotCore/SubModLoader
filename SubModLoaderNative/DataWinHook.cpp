#include <windows.h>
#include <string>
#include <filesystem>
#include "detours/detours.h"
#include "DataWinHook.h"
#include "NetBootstrap.h"

using namespace std;

namespace Bootstrap {
    HANDLE(__stdcall* TrueCreateFileW)(LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile) = CreateFileW;
    HANDLE __stdcall FakeCreateFileW(LPCWSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile) {
        static bool hasGenerated = false;
        static bool failedGenerating = false;

        wstring fileName = filesystem::path(lpFileName).filename();

        if (!failedGenerating && fileName == L"data.win") {
            if (!hasGenerated) {
                hasGenerated = true;

                if (!RunSubModLoader())
                    failedGenerating = true;
            }

            if (!failedGenerating)
                return TrueCreateFileW(L"modded.win", dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
        } else if (fileName == L"unmodded.win")
            return TrueCreateFileW(L"data.win", dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);

        return TrueCreateFileW(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
    }

    void AttachDataWinHook() {
        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourAttach(&(PVOID&)TrueCreateFileW, FakeCreateFileW);
        DetourTransactionCommit();
    }

    void DetachDataWinHook() {
        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourDetach(&(PVOID&)TrueCreateFileW, FakeCreateFileW);
        DetourTransactionCommit();
    }

}