#include <windows.h>
#include <filesystem>
#include "detours/detours.h"
#include "../imgui/imgui.h"
#include "../imgui/misc/freetype/imgui_freetype.h"
#include "../imgui/backends/imgui_impl_dx11.h"
#include "../imgui/backends/imgui_impl_dx9.h"
#include "../imgui/backends/imgui_impl_win32.h"
#include <d3d11.h>
#include <d3d9.h>
#include "DXVtables.h"
#include "ImGUIHooks.h"

using namespace std;
using namespace SubModLoader::GUI;

extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

Overlay::DrawFunc Overlay::Draw = nullptr;
Overlay::GetIsImGuiShowingFunc Overlay::GetIsImGuiShowing = nullptr;

namespace Bootstrap {
    HWND window = nullptr;
    bool isImGuiSetUp = false;

#pragma region DX11 globals

    bool usingD3d11 = false;
    ID3D11Device* device11 = nullptr;
    ID3D11DeviceContext* context = nullptr;
    ID3D11RenderTargetView* renderTargetView = nullptr;

#pragma endregion
#pragma region DX9 globals

    bool usingD3d9 = false;
    IDirect3DDevice9* device9 = nullptr;
    D3DPRESENT_PARAMETERS params = {};

#pragma endregion


    WNDPROC TrueWndProc = nullptr;
    LRESULT FakeWndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
        if (!isImGuiSetUp)
            return TrueWndProc(hWnd, msg, wParam, lParam);

        if (!Overlay::GetIsImGuiShowing() && msg == WM_SETCURSOR) {
            // Keep game chosen cursor if imgui isn't showing
            bool trueResult = TrueWndProc(hWnd, msg, wParam, lParam);
            HCURSOR cursor = GetCursor();
            bool result = ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam);
            SetCursor(cursor);
            if (result)
                return true;
            else
                return trueResult;
        } else if (ImGui_ImplWin32_WndProcHandler(hWnd, msg, wParam, lParam))
            return true;

        static ImGuiIO& io = ImGui::GetIO();
        if (io.WantCaptureMouse)
            return DefWindowProc(hWnd, msg, wParam, lParam);

        return TrueWndProc(hWnd, msg, wParam, lParam);
    }

	HWND (__stdcall* TrueCreateWindowExW)(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam) = CreateWindowExW;
	HWND __stdcall FakeCreateWindowExW(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam) {
		HWND result = TrueCreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

        wchar_t className[256];
        GetClassName(result, className, 256);
        if (wcscmp(className, L"YYGameMakerYY") == 0) {
            window = result;

            TrueWndProc = (WNDPROC)SetWindowLongPtr(window, GWLP_WNDPROC, (LONG)FakeWndProc);
        }

		return result;
	}

    void PlatformIndependentImGuiSetup() {
        IMGUI_CHECKVERSION();
        ImGuiContext* ctx = ImGui::CreateContext();
        ImGuiIO& io = ImGui::GetIO();
        ImGui::StyleColorsDark();

        if (!filesystem::exists("SubModLoader/Settings"))
            filesystem::create_directory("SubModLoader/Settings");
        io.IniFilename = "SubModLoader/Settings/imgui.ini";

        // TODO: package the fonts with the app
        // regular [0]
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\consola.ttf", 15);
        static ImWchar ranges[] = { 0x1, 0x1FFFF, 0 };
        static ImFontConfig cfg;
        cfg.OversampleH = cfg.OversampleV = 1;
        cfg.MergeMode = true;
        cfg.FontBuilderFlags |= ImGuiFreeTypeBuilderFlags_LoadColor;
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguiemj.ttf", 15, &cfg, ranges);
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguisym.ttf", 15, &cfg, ranges);

        // bold [1]
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\consolab.ttf", 15);
        static ImWchar rangesBold[] = { 0x1, 0x1FFFF, 0 };
        static ImFontConfig cfgBold;
        cfgBold.OversampleH = cfgBold.OversampleV = 1;
        cfgBold.MergeMode = true;
        cfgBold.FontBuilderFlags |= ImGuiFreeTypeBuilderFlags_LoadColor | ImGuiFreeTypeBuilderFlags_Bold;
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguiemj.ttf", 15, &cfgBold, rangesBold);
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguisym.ttf", 15, &cfgBold, rangesBold);

        // italic [2]
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\consolai.ttf", 15);
        static ImWchar rangesItalic[] = { 0x1, 0x1FFFF, 0 };
        static ImFontConfig cfgItalic;
        cfgItalic.OversampleH = cfgItalic.OversampleV = 1;
        cfgItalic.MergeMode = true;
        cfgItalic.FontBuilderFlags |= ImGuiFreeTypeBuilderFlags_LoadColor | ImGuiFreeTypeBuilderFlags_Oblique;
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguiemj.ttf", 15, &cfgItalic, rangesItalic);
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguisym.ttf", 15, &cfgItalic, rangesItalic);

        // bold + italic [3]
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\consolaz.ttf", 15);
        static ImWchar rangesBoth[] = { 0x1, 0x1FFFF, 0 };
        static ImFontConfig cfgBoth;
        cfgBoth.OversampleH = cfgBoth.OversampleV = 1;
        cfgBoth.MergeMode = true;
        cfgBoth.FontBuilderFlags |= ImGuiFreeTypeBuilderFlags_LoadColor | ImGuiFreeTypeBuilderFlags_Bold | ImGuiFreeTypeBuilderFlags_Oblique;
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguiemj.ttf", 15, &cfgBoth, rangesBoth);
        io.Fonts->AddFontFromFileTTF("C:\\Windows\\Fonts\\seguisym.ttf", 15, &cfgBoth, rangesBoth);
    }

    void DrawImGui() {
        ImGui::NewFrame();

        Overlay::Draw();

        if (Overlay::GetIsImGuiShowing())
            ImGui::ShowDemoWindow();

        ImGui::EndFrame();
        ImGui::Render();
    }

#pragma region DX11 Hooks

    void CreateRenderTarget(IDXGISwapChain* swapChain) {
        ID3D11Texture2D* backBuffer = nullptr;
        swapChain->GetBuffer(0, IID_PPV_ARGS(&backBuffer));
        device11->CreateRenderTargetView(backBuffer, NULL, &renderTargetView);
        backBuffer->Release();
    }

    void CleanupRenderTarget() {
        if (renderTargetView) {
            renderTargetView->Release();
            renderTargetView = nullptr;
        }
    }

    typedef HRESULT(__stdcall* IDXGISwapChain_PresentFunc) (IDXGISwapChain* This, UINT SyncInterval, UINT Flags);
    IDXGISwapChain_PresentFunc TrueIDXGISwapChain_Present = nullptr;
    HRESULT __stdcall FakeIDXGISwapChain_Present(IDXGISwapChain* This, UINT SyncInterval, UINT Flags) {
        if (!isImGuiSetUp) {
            CreateRenderTarget(This);
            isImGuiSetUp = true;
        }

        ImGui_ImplDX11_NewFrame();
        ImGui_ImplWin32_NewFrame();

        DrawImGui();

        context->OMSetRenderTargets(1, &renderTargetView, NULL);
        ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());

        return TrueIDXGISwapChain_Present(This, SyncInterval, Flags);
    }

    // render target view needs to be gone when the game tries to resize the window, as is said in the ms docs
    typedef HRESULT(__stdcall* IDXGISwapChain_ResizeBuffersFunc)(IDXGISwapChain* This, UINT BufferCount, UINT Width, UINT Height, DXGI_FORMAT NewFormat, UINT SwapChainFlags);
    IDXGISwapChain_ResizeBuffersFunc TrueIDXGISwapChain_ResizeBuffers;
    HRESULT __stdcall FakeIDXGISwapChain_ResizeBuffers(IDXGISwapChain* This, UINT BufferCount, UINT Width, UINT Height, DXGI_FORMAT NewFormat, UINT SwapChainFlags) {
        CleanupRenderTarget();
        HRESULT result = TrueIDXGISwapChain_ResizeBuffers(This, BufferCount, Width, Height, NewFormat, SwapChainFlags);
        CreateRenderTarget(This);
        return result;
    }

    PFN_D3D11_CREATE_DEVICE TrueD3D11CreateDevice = nullptr;
    HRESULT __stdcall FakeD3D11CreateDevice(IDXGIAdapter* pAdapter, D3D_DRIVER_TYPE DriverType, HMODULE Software, UINT Flags, CONST D3D_FEATURE_LEVEL* pFeatureLevels, UINT FeatureLevels, UINT SDKVersion, ID3D11Device** ppDevice, D3D_FEATURE_LEVEL* pFeatureLevel, ID3D11DeviceContext** ppImmediateContext) {
        HRESULT result = TrueD3D11CreateDevice(pAdapter, DriverType, Software, Flags, pFeatureLevels, FeatureLevels, SDKVersion, ppDevice, pFeatureLevel, ppImmediateContext);

        IDXGIFactory1* dummyFactory = nullptr;
        if (FAILED(CreateDXGIFactory1(__uuidof(IDXGIFactory1), (void**)&dummyFactory))) {
            MessageBox(NULL, L"Failed to get dx11 factory1.", L"Failed", MB_OK);
            return result;
        }

        DXGI_SWAP_CHAIN_DESC dummyDesc;
        ZeroMemory(&dummyDesc, sizeof(dummyDesc));
        // This is the minimum needed to be set for it to work
        dummyDesc.BufferCount = 1;
        dummyDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        dummyDesc.OutputWindow = window;
        dummyDesc.SampleDesc.Count = 1;
        dummyDesc.Windowed = true;

        IDXGISwapChain* dummySwapChain = nullptr;

        if (FAILED(dummyFactory->CreateSwapChain(*ppDevice, &dummyDesc, &dummySwapChain))) {
            MessageBox(NULL, L"Failed to get dx11 swapchain.", L"Failed", MB_OK);
            return result;
        }

        TrueIDXGISwapChain_Present = (IDXGISwapChain_PresentFunc)vtblPresent(dummySwapChain);
        TrueIDXGISwapChain_ResizeBuffers = (IDXGISwapChain_ResizeBuffersFunc)vtblResizeBuffers(dummySwapChain);

        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourAttach(&(PVOID&)TrueIDXGISwapChain_Present, FakeIDXGISwapChain_Present);
        DetourAttach(&(PVOID&)TrueIDXGISwapChain_ResizeBuffers, FakeIDXGISwapChain_ResizeBuffers);
        DetourTransactionCommit();

        dummySwapChain->Release();

        PlatformIndependentImGuiSetup();
        ImGui_ImplWin32_Init(window);
        ImGui_ImplDX11_Init(*ppDevice, *ppImmediateContext);

        device11 = *ppDevice;
        context = *ppImmediateContext;

        return result;
    }

#pragma endregion

#pragma region DX9 Hooks

    void ResetDevice() {
        ImGui_ImplDX9_InvalidateDeviceObjects();
        HRESULT hr = device9->Reset(&params);
        ImGui_ImplDX9_CreateDeviceObjects();
    }

    typedef HRESULT (__stdcall* IDirect3DDevice9_EndSceneFunc)(IDirect3DDevice9* This);
    IDirect3DDevice9_EndSceneFunc TrueIDirect3DDevice9_EndScene;
    HRESULT __stdcall FakeIDirect3DDevice9_EndSceneFunc(IDirect3DDevice9* This) {
        ImGui_ImplDX9_NewFrame();
        ImGui_ImplWin32_NewFrame();

        DrawImGui();

        ImGui_ImplDX9_RenderDrawData(ImGui::GetDrawData());

        return TrueIDirect3DDevice9_EndScene(This);
    }

    typedef HRESULT(__stdcall* IDirect3D9_CreateDeviceFunc)(IDirect3D9* This, UINT Adapter, D3DDEVTYPE DeviceType, HWND hFocusWindow, DWORD BehaviorFlags, D3DPRESENT_PARAMETERS* pPresentationParameters, IDirect3DDevice9** ppReturnedDeviceInterface);
    IDirect3D9_CreateDeviceFunc TrueIDirect3D9_CreateDevice;
    HRESULT __stdcall FakeIDirect3D9_CreateDevice(IDirect3D9* This, UINT Adapter, D3DDEVTYPE DeviceType, HWND hFocusWindow, DWORD BehaviorFlags, D3DPRESENT_PARAMETERS* pPresentationParameters, IDirect3DDevice9** ppReturnedDeviceInterface) {
        HRESULT result = TrueIDirect3D9_CreateDevice(This, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, ppReturnedDeviceInterface);

        TrueIDirect3DDevice9_EndScene = (IDirect3DDevice9_EndSceneFunc)vtblEndScene(*ppReturnedDeviceInterface);

        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourAttach(&(PVOID&)TrueIDirect3DDevice9_EndScene, FakeIDirect3DDevice9_EndSceneFunc);
        DetourTransactionCommit();

        PlatformIndependentImGuiSetup();
        ImGui_ImplWin32_Init(window);
        ImGui_ImplDX9_Init(*ppReturnedDeviceInterface);

        params = *pPresentationParameters;
        device9 = *ppReturnedDeviceInterface;

        isImGuiSetUp = true;

        return result;
    }

    typedef HRESULT (__stdcall* Direct3DCreate9ExFunc)(UINT SDKVersion, IDirect3D9Ex** d3d9Ex);
    Direct3DCreate9ExFunc TrueDirect3DCreate9Ex;
    HRESULT __stdcall FakeDirect3DCreate9FuncEx(UINT SDKVersion, IDirect3D9Ex** d3d9Ex) {
        HRESULT result = TrueDirect3DCreate9Ex(SDKVersion, d3d9Ex);

        TrueIDirect3D9_CreateDevice = (IDirect3D9_CreateDeviceFunc)vtblCreateDevice(*d3d9Ex);

        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourAttach(&(PVOID&)TrueIDirect3D9_CreateDevice, FakeIDirect3D9_CreateDevice);
        DetourTransactionCommit();

        return result;
    }

#pragma endregion

	void AttachImGuiHooks() {
        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourAttach(&(PVOID&)TrueCreateWindowExW, FakeCreateWindowExW);

		HMODULE d3d11 = GetModuleHandle(L"d3d11.dll");
		usingD3d11 = d3d11;
		if (usingD3d11) {
			TrueD3D11CreateDevice = (PFN_D3D11_CREATE_DEVICE)GetProcAddress(d3d11, "D3D11CreateDevice");
			DetourAttach(&(PVOID&)TrueD3D11CreateDevice, FakeD3D11CreateDevice);
        } else {
            // TODO: find out how to know if d3d9.dll is a dependency, since we load before d3d9
            HMODULE d3d9 = LoadLibrary(L"d3d9.dll");
            usingD3d9 = d3d9;
            if (usingD3d9) {
                TrueDirect3DCreate9Ex = (Direct3DCreate9ExFunc)GetProcAddress(d3d9, "Direct3DCreate9Ex");
                DetourAttach(&(PVOID&)TrueDirect3DCreate9Ex, FakeDirect3DCreate9FuncEx);
            }
        }

        DetourTransactionCommit();
	}

	void DetachImGuiHooks() {
        DetourTransactionBegin();
        DetourUpdateThread(GetCurrentThread());
        DetourDetach(&(PVOID&)TrueCreateWindowExW, FakeCreateWindowExW);

        if (usingD3d11) {
            DetourDetach(&(PVOID&)TrueD3D11CreateDevice, FakeD3D11CreateDevice);
            DetourDetach(&(PVOID&)TrueIDXGISwapChain_Present, FakeIDXGISwapChain_Present);
            DetourDetach(&(PVOID&)TrueIDXGISwapChain_ResizeBuffers, FakeIDXGISwapChain_ResizeBuffers);

            ImGui_ImplDX11_Shutdown();
            ImGui_ImplWin32_Shutdown();
        } else if (usingD3d9) {
            DetourDetach(&(PVOID&)TrueDirect3DCreate9Ex, FakeDirect3DCreate9FuncEx);
            DetourDetach(&(PVOID&)TrueIDirect3D9_CreateDevice, FakeIDirect3D9_CreateDevice);
            DetourDetach(&(PVOID&)TrueIDirect3DDevice9_EndScene, FakeIDirect3DDevice9_EndSceneFunc);

            ImGui_ImplDX9_Shutdown();
            ImGui_ImplWin32_Shutdown();
        }

        DetourTransactionCommit();
        ImGui::DestroyContext();
	}
}