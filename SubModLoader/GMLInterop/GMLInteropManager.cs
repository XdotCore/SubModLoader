using SubmachineModLib;
using SubmachineModLib.Models;
using SubModLoader.GameData.Extensions;
using SubModLoader.GMLInterop.Enums;
using SubModLoader.Mods;
using SubModLoader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SubModLoader.GMLInterop {
    /// <summary>
    /// A class that takes care of GML to C# interoperability
    /// </summary>
    public static class GMLInteropManager {
        /// <summary>
        /// Interface for <see cref="RegisteredType{T}"/>
        /// </summary>
        public interface IRegisteredType {
            /// <summary>
            /// The given id of the registered type
            /// </summary>
            public GMLInteropTypeId Id { get; }
            /// <summary>
            /// The <see cref="System.Type"/> of the registered type
            /// </summary>
            public Type Type { get; }

            /// <summary>
            /// Writes this registered type to a <see cref="GMLInteropWriter"/>
            /// </summary>
            /// <param name="writer">The <see cref="GMLInteropWriter"/></param>
            /// <param name="value">An object of this registered type</param>
            public void Write(GMLInteropWriter writer, object value);
            /// <summary>
            /// Reads this registered type from a <see cref="GMLInteropReader"/>
            /// </summary>
            /// <param name="reader">The <see cref="GMLInteropReader"/></param>
            /// <returns>An object of this registered type</returns>
            public object Read(GMLInteropReader reader);

            /// <summary>
            /// The function, in gml, that writes this registered type from a buffer
            /// </summary>
            public GameMakerFunction GMLWrite { get; }
            /// <summary>
            /// The function, in gml, that read this registered type from a buffer
            /// </summary>
            public GameMakerFunction GMLRead { get; }
        }

        /// <summary>
        /// Holds all of the interop functionality for a registered type
        /// </summary>
        /// <typeparam name="T">The registered type</typeparam>
        public sealed class RegisteredType<T> : IRegisteredType {
            private readonly GMLInteropTypeId _id;
            private readonly Type _type;
            private readonly Action<GMLInteropWriter, T> _write;
            private readonly Func<GMLInteropReader, T> _read;
            private readonly GameMakerFunction _gmlWrite;
            private readonly GameMakerFunction _gmlRead;

            /// <inheritdoc/>
            public GMLInteropTypeId Id => _id;
            /// <inheritdoc/>
            public Type Type => _type;

            /// <inheritdoc/>
            public void Write(GMLInteropWriter writer, object value) => _write(writer, (T)value);
            /// <inheritdoc/>
            public object Read(GMLInteropReader reader) => _read(reader);

            /// <inheritdoc/>
            public GameMakerFunction GMLWrite => _gmlWrite;
            /// <inheritdoc/>
            public GameMakerFunction GMLRead => _gmlRead;

            internal RegisteredType(GameMakerData gameData, GMLInteropTypeId id, Action<GMLInteropWriter, T> write, Func<GMLInteropReader, T> read, string gmlWrite, string gmlRead) {
                _id = id;
                _type = typeof(T);
                if (_type == typeof(object))
                    _type = typeof(void);
                _write = write;
                _read = read;

                gmlWrite = $$"""
                var argBuffer = argument0
                var arg = argument1
                {{gmlWrite}}
                """;
                gmlRead = $$"""
                var resultBuffer = argument0
                {{gmlRead}}
                """;

                _gmlWrite = gameData.AddCodeAndFunction($"submodloader_gmlinterop_write_{Type.Name}", gmlWrite).function;
                _gmlRead = gameData.AddCodeAndFunction($"submodloader_gmlinterop_read_{Type.Name}", gmlRead).function;
            }
        }

        private static Dictionary<Type, IRegisteredType> RegisteredTypesByType { get; set; }
        private static Dictionary<GMLInteropTypeId, IRegisteredType> RegisteredTypesById { get; set; }

        private static GMLInteropTypeId NextTypeId = GMLInteropTypeId.RegisteredTypesStart;

        internal const string GML_gmlinterop_write = "submodloader_gmlinterop_write";
        internal const string GML_gmlinterop_read = "submodloader_gmlinterop_read";

        internal static void Initialize(GameMakerData gameData) {
            bool x64 = IntPtr.Size == 8;

            RegisteredTypesByType = new() {
                [typeof(void)] = new RegisteredType<object>(gameData, GMLInteropTypeId.Void, (_, _) => { }, _ => null, "", ""),
                [typeof(byte)] = new RegisteredType<byte>(gameData, GMLInteropTypeId.Byte,
                    (w, v) => w.WriteByte(v),
                    r => r.ReadByte(),
                    GMLInteropWriter.WriteByteFromGML("arg"),
                    $"return {GMLInteropReader.ReadByteFromGML()}"),
                [typeof(sbyte)] = new RegisteredType<sbyte>(gameData, GMLInteropTypeId.SByte,
                    (w, v) => w.WriteSByte(v),
                    r => r.ReadSByte(),
                    GMLInteropWriter.WriteSByteFromGML("arg"),
                    $"return {GMLInteropReader.ReadSByteFromGML()}"),
                [typeof(ushort)] = new RegisteredType<ushort>(gameData, GMLInteropTypeId.UShort,
                    (w, v) => w.WriteUShort(v),
                    r => r.ReadUShort(),
                    GMLInteropWriter.WriteUShortFromGML("arg"),
                    $"return {GMLInteropReader.ReadUShortFromGML()}"),
                [typeof(short)] = new RegisteredType<short>(gameData, GMLInteropTypeId.Short,
                    (w, v) => w.WriteShort(v),
                    r => r.ReadShort(),
                    GMLInteropWriter.WriteShortFromGML("arg"),
                    $"return {GMLInteropReader.ReadShortFromGML()}"),
                [typeof(uint)] = new RegisteredType<uint>(gameData, GMLInteropTypeId.UInt,
                    (w, v) => w.WriteUInt(v),
                    r => r.ReadUInt(),
                    GMLInteropWriter.WriteUIntFromGML("arg"),
                    $"return {GMLInteropReader.ReadUIntFromGML()}"),
                [typeof(int)] = new RegisteredType<int>(gameData, GMLInteropTypeId.Int,
                    (w, v) => w.WriteInt(v),
                    r => r.ReadInt(),
                    GMLInteropWriter.WriteIntFromGML("arg"),
                    $"return {GMLInteropReader.ReadIntFromGML()}"),
                [typeof(long)] = new RegisteredType<long>(gameData, GMLInteropTypeId.Long,
                    (w, v) => w.WriteLong(v),
                    r => r.ReadLong(),
                    GMLInteropWriter.WriteLongFromGML("arg"),
                    $"return {GMLInteropReader.ReadLongFromGML()}"),
                [typeof(Half)] = new RegisteredType<Half>(gameData, GMLInteropTypeId.Half,
                    (w, v) => w.WriteHalf(v),
                    r => r.ReadHalf(),
                    GMLInteropWriter.WriteHalfFromGML("arg"),
                    $"return {GMLInteropReader.ReadHalfFromGML()}"),
                [typeof(float)] = new RegisteredType<float>(gameData, GMLInteropTypeId.Float,
                    (w, v) => w.WriteFloat(v),
                    r => r.ReadFloat(),
                    GMLInteropWriter.WriteFloatFromGML("arg"),
                    $"return {GMLInteropReader.ReadFloatFromGML()}"),
                [typeof(double)] = new RegisteredType<double>(gameData, GMLInteropTypeId.Double,
                    (w, v) => w.WriteDouble(v),
                    r => r.ReadDouble(),
                    GMLInteropWriter.WriteDoubleFromGML("arg"),
                    $"return {GMLInteropReader.ReadDoubleFromGML()}"),
                [typeof(bool)] = new RegisteredType<bool>(gameData, GMLInteropTypeId.Bool,
                    (w, v) => w.WriteBool(v),
                    r => r.ReadBool(),
                    GMLInteropWriter.WriteBoolFromGML("arg"),
                    $"return {GMLInteropReader.ReadBoolFromGML()}"),
                [typeof(string)] = new RegisteredType<string>(gameData, GMLInteropTypeId.String,
                    (w, v) => w.WriteString(v),
                    r => r.ReadString(),
                    GMLInteropWriter.WriteStringFromGML("arg"),
                    $"return {GMLInteropReader.ReadStringFromGML()}"),
                [typeof(char)] = new RegisteredType<char>(gameData, GMLInteropTypeId.Char,
                    (w, v) => w.WriteString($"{v}"),
                    r => r.ReadString()[0],
                    GMLInteropWriter.WriteStringFromGML("arg"),
                    $"return {GMLInteropReader.ReadStringFromGML()}"),
                [typeof(IntPtr)] = new RegisteredType<IntPtr>(gameData, GMLInteropTypeId.IntPtr,
                    (w, v) => { if (x64) w.WriteLong(v.ToInt64()); else w.WriteInt(v.ToInt32()); },
                    r => x64 ? new IntPtr(r.ReadLong()) : new IntPtr(r.ReadInt()),
                    x64 ? GMLInteropWriter.WriteLongFromGML("arg") : GMLInteropWriter.WriteIntFromGML("arg"),
                    $"return {(x64 ? GMLInteropReader.ReadLongFromGML() : GMLInteropReader.ReadIntFromGML())}")
            };

            RegisteredTypesById = RegisteredTypesByType.ToDictionary(kvp => kvp.Value.Id, kvp => kvp.Value);

            // Add these now for Add_call_csharp as it requires them, but redefine them to not be empty in Finalize
            gameData.AddCodeAndFunction(GML_gmlinterop_write, "");
            gameData.AddCodeAndFunction(GML_gmlinterop_read, "");
        }

        internal static void Finalize(GameMakerData gameData) {
            string writeCases = "", readCases = "";

            foreach (IRegisteredType register in RegisteredTypesByType.Values) {
                writeCases += $$"""
                        case {{register.Id.ToValue()}}:
                            {{register.GMLWrite}}(argBuffer, arg)
                            break
                    
                    """;
                readCases += $$"""
                        case {{register.Id.ToValue()}}:
                            result = {{register.GMLRead}}(resultBuffer)
                            break
                    
                    """;
            }

            gameData.AddCodeAndFunction(GML_gmlinterop_write, $$"""
                var argBuffer = argument0
                var typeId = int64(argument1)
                var arg = argument2
                
                if ((typeId & {{GMLInteropTypeId.IsArray.ToValue()}}) != 0) {
                    var elementTypeId = typeId & ~{{GMLInteropTypeId.IsArray.ToValue()}}
                    var length = {{(gameData.IsVersionAtLeast(2, 3) ? "array_length" : "array_length_1d")}}(arg)
                    buffer_write(argBuffer, buffer_s32, length)
                    for (var i = 0; i < length; i++)
                        {{GML_gmlinterop_write}}(argBuffer, elementTypeId, arg[i])
                } else {
                    switch (typeId) {
                    {{writeCases}}
                    }
                }
                """);
            gameData.AddCodeAndFunction(GML_gmlinterop_read, $$"""
                var resultBuffer = argument0
                var typeId = int64(argument1)
                var result = undefined

                if ((typeId & {{GMLInteropTypeId.IsArray.ToValue()}}) != 0) {
                    var elementTypeId = typeId & ~{{GMLInteropTypeId.IsArray.ToValue()}}
                    var length = buffer_read(resultBuffer, buffer_s32)
                    result = array_create(length)
                    for (var i = 0; i < length; i++)
                        result[i] = {{GML_gmlinterop_read}}(resultBuffer, elementTypeId)
                } else {
                    switch (typeId) {
                    {{readCases}}
                    }
                }

                return result
                """);

            Add_call_csharp(gameData);
        }

        /// <summary>
        /// Gets registered type info
        /// </summary>
        /// <param name="id">The id of the registered type</param>
        /// <returns>The info</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IRegisteredType GetRegisteredType(GMLInteropTypeId id) {
            bool success = RegisteredTypesById.TryGetValue((GMLInteropTypeId)(ushort)id, out IRegisteredType result);
            if (!success)
                throw new ArgumentException($"GMLInteropTypeId {id} does not have a registered System.Type.", nameof(id));
            return result;
        }

        /// <param name="type">The <see cref="System.Type"/> of the registered type</param>
        /// <inheritdoc cref="GetRegisteredType(GMLInteropTypeId)"/>
        public static IRegisteredType GetRegisteredType(Type type) {
            bool success = RegisteredTypesByType.TryGetValue(type, out IRegisteredType result);
            if (!success)
                throw new ArgumentException($"System.Type {type} does not have a registered GMLInteropType.", nameof(type));
            return result;
        }

        /// <typeparam name="T">The type of the registered type</typeparam>
        /// <inheritdoc cref="GetRegisteredType(GMLInteropTypeId)"/>
        public static RegisteredType<T> GetRegisteredType<T>() => GetRegisteredType(typeof(T)) as RegisteredType<T>;

        /// <summary>
        /// Determines if the given type id has already been registered
        /// </summary>
        /// <param name="id">The type id</param>
        /// <returns>True if the type id has been registered, false otherwise</returns>
        public static bool IsTypeRegistered(GMLInteropTypeId id) => RegisteredTypesById.ContainsKey((GMLInteropTypeId)(ushort)id);

        /// <summary>
        /// Determines if the given type has already been registered
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type has been registered, false otherwise</returns>
        public static bool IsTypeRegistered(Type type) => RegisteredTypesByType.ContainsKey(type);

        /// <summary>
        /// Determines if the given type has already been registered
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>True if the type has been registered, false otherwise</returns>
        public static bool IsTypeRegistered<T>() => RegisteredTypesByType.ContainsKey(typeof(T));

        /// <summary>
        /// Registers the type with a read and write interop implimentation
        /// </summary>
        /// <typeparam name="T">The type to register</typeparam>
        /// <param name="gameData">The game data</param>
        /// <param name="write">The function to write the type using <see cref="GMLInteropWriter"/></param>
        /// <param name="read">The function to read the type using <see cref="GMLInteropReader"/></param>
        /// <param name="gmlWrite">The function, in gml, to write the type. Available variables are argBuffer, a buffer containing the argument data, and arg, the argument or array of arguments that represent the type to write to argBuffer.</param>
        /// <param name="gmlRead">The function, in gml, to read the type. Available variable is resultBuffer, a buffer containing the result data, from which to read the value or array of values that represent the type</param>
        /// <returns>The id given to the registered type</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static GMLInteropTypeId RegisterType<T>(GameMakerData gameData, Action<GMLInteropWriter, T> write, Func<GMLInteropReader, T> read, string gmlWrite, string gmlRead) {
            ArgumentNullException.ThrowIfNull(gameData, nameof(gameData));
            ArgumentNullException.ThrowIfNull(write, nameof(write));
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(gmlWrite, nameof(gmlWrite));
            ArgumentNullException.ThrowIfNull(gmlRead, nameof(gmlRead));

            if (Modding.HasAppliedMods)
                throw new InvalidOperationException("Can't register new types after mods have been applied.");

            Type type = typeof(T);
            if (type.IsArray)
                throw new InvalidOperationException($"Can't register array types, use GMLInteropWriter.WriteArray or GMLInteropReader.ReadArray. Type {type}.");
            if (IsTypeRegistered<T>())
                throw new InvalidOperationException($"Can't register type {type} twice. Use IsTypeRegistered and/or OverrideRegisteredType.");

            GMLInteropTypeId id = NextTypeId++;

            RegisteredType<T> register = new(gameData, id, write, read, gmlWrite, gmlRead);
            RegisteredTypesByType[type] = register;
            RegisteredTypesById[id] = register;
            return id;
        }

        /// <summary>
        /// Override an existing registered type with a new read and write interop implementation
        /// </summary>
        /// <typeparam name="T">The type to override</typeparam>
        /// <inheritdoc cref="RegisterType{T}"/>
        public static GMLInteropTypeId OverrideRegisteredType<T>(GameMakerData gameData, Action<GMLInteropWriter, T> write, Func<GMLInteropReader, T> read, string gmlWrite, string gmlRead) {
            ArgumentNullException.ThrowIfNull(gameData, nameof(gameData));
            ArgumentNullException.ThrowIfNull(write, nameof(write));
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(gmlWrite, nameof(gmlWrite));
            ArgumentNullException.ThrowIfNull(gmlRead, nameof(gmlRead));

            if (Modding.HasAppliedMods)
                throw new InvalidOperationException("Can't override types after mods have been applied.");

            Type type = typeof(T);
            if (type == typeof(object))
                throw new InvalidOperationException($"Can't override type System.Object.");
            if (type.IsArray)
                throw new InvalidOperationException($"Can't override array types, use GMLInteropWriter.WriteArray or GMLInteropReader.ReadArray. Type {type}.");
            if (!IsTypeRegistered<T>())
                throw new InvalidOperationException($"Can't override type {type} that hasn't been registered. Use IsTypeRegistered and/or RegisterType.");

            GMLInteropTypeId id = RegisteredTypesByType[type].Id;
            RegisteredTypesByType.Remove(type);
            RegisteredTypesById.Remove(id);
            return RegisterType(gameData, write, read, gmlWrite, gmlRead);
        }

        private const string GML_call_csharp = "submodloader_call_csharp";

        #region Call C#

        // TODO: allow ref and out params
        internal static void Add_call_csharp(GameMakerData gameData) {
            gameData.AddCodeAndFunction(GML_call_csharp, $$"""
                var metadataBuffer = buffer_create(256, buffer_grow, 1) // 256 is a good starting length with strings imo
                buffer_seek(metadataBuffer, buffer_seek_start, 4)

                var classType = argument[0];
                buffer_write(metadataBuffer, buffer_string, classType)
                var classMethod = argument[1];
                buffer_write(metadataBuffer, buffer_string, classMethod)

                var returnType = argument[2]
                buffer_write(metadataBuffer, buffer_u32, returnType)
                var argCount = argument[3]
                buffer_write(metadataBuffer, buffer_u32, argCount)

                buffer_seek(metadataBuffer, buffer_seek_start, 0)
                buffer_write(metadataBuffer, buffer_u32, buffer_get_size(metadataBuffer))
                
                var argBuffer = buffer_create(5 * argCount + 4, buffer_grow, 1) // assume each arg is 1 byte + 4 byte type, then add 4 byte size, and grow from there
                buffer_seek(argBuffer, buffer_seek_start, 4)

                for (var i = 4; i < argument_count; i += 2) {
                    {{GMLInteropWriter.WriteInteropTypeIdFromGML("argument[i]")}}
                    var prevSeek = buffer_tell(argBuffer)
                    {{GMLInteropWriter.WriteFromGML("argument[i]", "argument[i + 1]")}}
                    var newSeek = buffer_tell(argBuffer)
                    if (newSeek < prevSeek)
                        buffer_seek(argBuffer, buffer_seek_start, prevSeek)
                }
                
                buffer_seek(argBuffer, buffer_seek_start, 0)
                buffer_write(argBuffer, buffer_u32, buffer_get_size(argBuffer))

                var resultPtrBuffer = buffer_create(8, buffer_fixed, 1) // enough to fit a 64bit ptr, but still works for a 32bit ptr

                if (!variable_global_exists("{{GML_call_csharp}}_extern"))
                    global.{{GML_call_csharp}}_extern = external_define("SubModLoaderNative.dll", "CallCSharp", dll_cdecl, ty_real, 3, ty_string, ty_string, ty_string)
                external_call(global.{{GML_call_csharp}}_extern, buffer_get_address(metadataBuffer), buffer_get_address(argBuffer), buffer_get_address(resultPtrBuffer))

                var result = 0
                var hasResult = false
                if (returnType != {{GMLInteropTypeId.Void.ToValue()}}) {
                    if (!variable_global_exists("submodloader_delete_result_extern"))
                        global.submodloader_delete_result_extern = external_define("SubModLoaderNative.dll", "DeleteResult", dll_cdecl, ty_real, 1, ty_string)
                    if (!variable_global_exists("submodloader_get_result_size_extern"))
                        global.submodloader_get_result_size_extern = external_define("SubModLoaderNative.dll", "GetResultSize", dll_cdecl, ty_real, 1, ty_string)
                    if (!variable_global_exists("submodloader_copy_result_to_buffer_extern"))
                        global.submodloader_copy_result_to_buffer_extern = external_define("SubModLoaderNative.dll", "CopyResultToBuffer", dll_cdecl, ty_real, 2, ty_string, ty_string)

                    var resultSize = external_call(global.submodloader_get_result_size_extern, buffer_get_address(resultPtrBuffer))
                    var resultBuffer = buffer_create(resultSize, buffer_fixed, 1)
                    external_call(global.submodloader_copy_result_to_buffer_extern, buffer_get_address(resultBuffer), buffer_get_address(resultPtrBuffer))
                    buffer_seek(resultBuffer, buffer_seek_start, 4)

                    result = {{GMLInteropReader.ReadFromGML("returnType")}}
                    hasResult = true

                    buffer_delete(resultBuffer)
                    external_call(global.submodloader_delete_result_extern, buffer_get_address(resultPtrBuffer))
                }

                buffer_delete(metadataBuffer)
                buffer_delete(argBuffer)
                buffer_delete(resultPtrBuffer)

                if (hasResult)
                    return result
                """);
        }

        private unsafe delegate void CallCSharpDelegate(byte* metadata, byte* argData, byte** resultData);
        internal static unsafe void CallCSharp(byte* metadata, byte* argData, byte** resultData) {
            try {
                GMLInteropReader reader = new(metadata);

                string classTypeName = reader.ReadString();
                string classMethodName = reader.ReadString();
                GMLInteropTypeId returnType = reader.ReadInteropTypeId();
                uint argCount = reader.ReadUInt();

                reader.Reset(argData);
                object[] args = new object[argCount];
                Type[] types = new Type[argCount];
                for (int i = 0; i < argCount; i++) {
                    GMLInteropTypeId type = reader.ReadInteropTypeId();
                    args[i] = reader.Read(type);
                    types[i] = type.ToType();
                }

                MethodInfo method = Type.GetType(classTypeName).GetMethod(classMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, types) ?? throw new MissingMethodException(classTypeName, classMethodName);

                if (returnType == GMLInteropTypeId.Void)
                    method.Invoke(null, args);
                else {
                    object result = method.Invoke(null, args);
                    GMLInteropWriter writer = new();
                    writer.Write(returnType, result);
                    *resultData = writer.GetBytes();
                }
            } catch (Exception e) {
                Logger.WriteError(e);
            }
        }

        internal static string CallFromGML<T>(T function, params string[] gmlVarsOrExpressions) where T : Delegate {
            MethodInfo method = function.Method;
            if (!method.IsStatic)
                throw new ArgumentException("The function must be a static method since no instance information can be provided from gml.", nameof(function));

            ParameterInfo[] parameters = method.GetParameters();
            if (gmlVarsOrExpressions.Length < parameters.Length)
                throw new ArgumentException("Optional parameters are not currently supported, you must supply a gml variable or expression for each parameter. Create a wrapper method if you need optional parameters.", nameof(gmlVarsOrExpressions));

            string resultGML = $"{GML_call_csharp}(\"{method.DeclaringType.AssemblyQualifiedName}\", \"{method.Name}\", {method.ReturnType.ToGMLInteropTypeId().ToValue()}, {parameters.Length}";

            for (int i = 0; i < parameters.Length; i++)
                resultGML += $", {parameters[i].ParameterType.ToGMLInteropTypeId().ToValue()}, {gmlVarsOrExpressions[i]}";

            resultGML += ')';

            return resultGML;
        }

        #endregion
    }
}
