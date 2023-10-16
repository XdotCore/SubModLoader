#pragma once

namespace SubModLoader::GUI::Overlay {
	typedef void(__stdcall* DrawFunc)();
	typedef bool(__stdcall* GetIsImGuiShowingFunc)();

	extern DrawFunc Draw;
	extern GetIsImGuiShowingFunc GetIsImGuiShowing;
}

namespace Bootstrap {
	void AttachImGuiHooks();
	void DetachImGuiHooks();
}