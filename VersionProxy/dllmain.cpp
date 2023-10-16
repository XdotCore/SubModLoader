#include <windows.h>
#include <format>
#include <filesystem>

using namespace std;

#pragma region Proxy

struct version_dll {
	HMODULE dll;
	FARPROC TrueGetFileVersionInfoA;
	FARPROC TrueGetFileVersionInfoByHandle;
	FARPROC TrueGetFileVersionInfoExA;
	FARPROC TrueGetFileVersionInfoExW;
	FARPROC TrueGetFileVersionInfoSizeA;
	FARPROC TrueGetFileVersionInfoSizeExA;
	FARPROC TrueGetFileVersionInfoSizeExW;
	FARPROC TrueGetFileVersionInfoSizeW;
	FARPROC TrueGetFileVersionInfoW;
	FARPROC TrueVerFindFileA;
	FARPROC TrueVerFindFileW;
	FARPROC TrueVerInstallFileA;
	FARPROC TrueVerInstallFileW;
	FARPROC TrueVerLanguageNameA;
	FARPROC TrueVerLanguageNameW;
	FARPROC TrueVerQueryValueA;
	FARPROC TrueVerQueryValueW;
} version;

extern "C" {
	FARPROC trueFuncPtr = 0;
	// defined in callTrueFuncPtr.asm
	extern "C" int callTrueFuncPtrASM();

	void FakeGetFileVersionInfoA() { trueFuncPtr = version.TrueGetFileVersionInfoA; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoByHandle() { trueFuncPtr = version.TrueGetFileVersionInfoByHandle; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoExA() { trueFuncPtr = version.TrueGetFileVersionInfoExA; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoExW() { trueFuncPtr = version.TrueGetFileVersionInfoExW; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoSizeA() { trueFuncPtr = version.TrueGetFileVersionInfoSizeA; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoSizeExA() { trueFuncPtr = version.TrueGetFileVersionInfoSizeExA; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoSizeExW() { trueFuncPtr = version.TrueGetFileVersionInfoSizeExW; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoSizeW() { trueFuncPtr = version.TrueGetFileVersionInfoSizeW; callTrueFuncPtrASM(); }
	void FakeGetFileVersionInfoW() { trueFuncPtr = version.TrueGetFileVersionInfoW; callTrueFuncPtrASM(); }
	void FakeVerFindFileA() { trueFuncPtr = version.TrueVerFindFileA; callTrueFuncPtrASM(); }
	void FakeVerFindFileW() { trueFuncPtr = version.TrueVerFindFileW; callTrueFuncPtrASM(); }
	void FakeVerInstallFileA() { trueFuncPtr = version.TrueVerInstallFileA; callTrueFuncPtrASM(); }
	void FakeVerInstallFileW() { trueFuncPtr = version.TrueVerInstallFileW; callTrueFuncPtrASM(); }
	void FakeVerLanguageNameA() { trueFuncPtr = version.TrueVerLanguageNameA; callTrueFuncPtrASM(); }
	void FakeVerLanguageNameW() { trueFuncPtr = version.TrueVerLanguageNameW; callTrueFuncPtrASM(); }
	void FakeVerQueryValueA() { trueFuncPtr = version.TrueVerQueryValueA; callTrueFuncPtrASM(); }
	void FakeVerQueryValueW() { trueFuncPtr = version.TrueVerQueryValueW; callTrueFuncPtrASM(); }
}

void setupProxy() {
	char path[MAX_PATH];
	GetSystemDirectory(path, sizeof(path));
	strcat_s(path, "\\version.dll");

	version.dll = LoadLibrary(path);
	if (version.dll == NULL) {
		MessageBox(NULL, "SubModLoader could not load version.dll to proxy and cannot continue.\nPress OK to exit...", "Error Loading Proxy", MB_OK);
		return;
	}

	version.TrueGetFileVersionInfoA = GetProcAddress(version.dll, "GetFileVersionInfoA");
	version.TrueGetFileVersionInfoByHandle = GetProcAddress(version.dll, "GetFileVersionInfoByHandle");
	version.TrueGetFileVersionInfoExA = GetProcAddress(version.dll, "GetFileVersionInfoExA");
	version.TrueGetFileVersionInfoExW = GetProcAddress(version.dll, "GetFileVersionInfoExW");
	version.TrueGetFileVersionInfoSizeA = GetProcAddress(version.dll, "GetFileVersionInfoSizeA");
	version.TrueGetFileVersionInfoSizeExA = GetProcAddress(version.dll, "GetFileVersionInfoSizeExA");
	version.TrueGetFileVersionInfoSizeExW = GetProcAddress(version.dll, "GetFileVersionInfoSizeExW");
	version.TrueGetFileVersionInfoSizeW = GetProcAddress(version.dll, "GetFileVersionInfoSizeW");
	version.TrueGetFileVersionInfoW = GetProcAddress(version.dll, "GetFileVersionInfoW");
	version.TrueVerFindFileA = GetProcAddress(version.dll, "VerFindFileA");
	version.TrueVerFindFileW = GetProcAddress(version.dll, "VerFindFileW");
	version.TrueVerInstallFileA = GetProcAddress(version.dll, "VerInstallFileA");
	version.TrueVerInstallFileW = GetProcAddress(version.dll, "VerInstallFileW");
	version.TrueVerLanguageNameA = GetProcAddress(version.dll, "VerLanguageNameA");
	version.TrueVerLanguageNameW = GetProcAddress(version.dll, "VerLanguageNameW");
	version.TrueVerQueryValueA = GetProcAddress(version.dll, "VerQueryValueA");
	version.TrueVerQueryValueW = GetProcAddress(version.dll, "VerQueryValueW");
}

void cleanupProxy() {
	if (version.dll != NULL)
		FreeLibrary(version.dll);
}

#pragma endregion

#pragma region SubModLoader

HMODULE subModLoaderDLL;

void startSubModLoader() {
	setupProxy();

	SetDllDirectory("SubModLoader/");
	subModLoaderDLL = LoadLibrary("SubModLoaderNative.dll");
	if (subModLoaderDLL == NULL) {
		int err = GetLastError();
		MessageBox(NULL, format("SubModLoader could not be loaded.\nerror code: {}, {}", err, system_category().message(err)).c_str(), "Error Loading SubModLoader",  MB_OK);
		return;
	}
}

void cleanupSubModLoader() {
	cleanupProxy();

	if (subModLoaderDLL != NULL)
		FreeLibrary(subModLoaderDLL);
}

#pragma endregion

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH: {
			startSubModLoader();
			break;
		}
		case DLL_PROCESS_DETACH: {
			cleanupSubModLoader();
			break;
		}
	}
	return TRUE;
}