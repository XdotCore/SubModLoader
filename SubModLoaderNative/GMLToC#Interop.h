#pragma once

namespace SubModLoader::GMLInterop {
	namespace GMLInteropManager {
		typedef void(__stdcall* CallCSharpFunc)(void* metadata, void* argData, void** resultData);

		extern CallCSharpFunc CallCSharp;
	}

	namespace GMLInteropWriter {
		typedef void(__stdcall* DeleteBytesFunc)(void* bytes);

		extern DeleteBytesFunc DeleteBytes;
	}
}