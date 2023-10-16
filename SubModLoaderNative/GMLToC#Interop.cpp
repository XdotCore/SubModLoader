#include "Exports.h"
#include "GMLToC#Interop.h"
#include <cstdint>
#include <cstring>

using namespace std;
using namespace SubModLoader::GMLInterop;

GMLInteropManager::CallCSharpFunc GMLInteropManager::CallCSharp = nullptr;
GMLInteropWriter::DeleteBytesFunc GMLInteropWriter::DeleteBytes = nullptr;

GAMEMAKEREXPORT void CallCSharp(void* metadata, void* argData, void** resultData) {
	if (GMLInteropManager::CallCSharp == nullptr)
		return;
	GMLInteropManager::CallCSharp(metadata, argData, resultData);
}

GAMEMAKEREXPORT void DeleteResult(void** resultData) {
	if (GMLInteropWriter::DeleteBytes == nullptr)
		return;
	GMLInteropWriter::DeleteBytes(*resultData);
}

inline uint32_t GetSize(void* buffer) {
	return ((uint32_t*)buffer)[0];
}

GAMEMAKEREXPORT double GetResultSize(void** resultData) {
	return GetSize(*resultData);
}

GAMEMAKEREXPORT void CopyResultToBuffer(void* buffer, void** resultData) {
	uint32_t size = GetSize(*resultData);
	memcpy_s(buffer, size, *resultData, size);
}