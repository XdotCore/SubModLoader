#define CINTERFACE
#include <dxgi.h>
#include <d3d9.h>
#include "DXVtables.h"

namespace Bootstrap {
	void* vtblPresent(IDXGISwapChain* swapChain) {
		return swapChain->lpVtbl->Present;
	}

	void* vtblResizeBuffers(IDXGISwapChain* swapChain) {
		return swapChain->lpVtbl->ResizeBuffers;
	}

	void* vtblCreateDevice(IDirect3D9Ex* d3d9Ex) {
		return d3d9Ex->lpVtbl->CreateDevice;
	}

	void* vtblEndScene(IDirect3DDevice9* device) {
		return device->lpVtbl->EndScene;
	}
}
