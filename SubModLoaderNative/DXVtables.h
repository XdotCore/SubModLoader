#pragma once

namespace Bootstrap {
	void* vtblPresent(IDXGISwapChain* swapChain);
	void* vtblResizeBuffers(IDXGISwapChain* swapChain);
	void* vtblCreateDevice(IDirect3D9Ex* d3d9);
	void* vtblEndScene(IDirect3DDevice9* device);
}
