using SubModLoader.GMLInterop.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SubModLoader.GMLInterop {
    /// <summary>
    /// Writes to byte buffer which then can be returned as a new <see cref="byte"/>*
    /// </summary>
    public class GMLInteropWriter {
        private List<byte> Buffer { get; set; } = new();

        /// <summary>
        /// The default ctor
        /// </summary>
        public GMLInteropWriter() => Reset();

        /// <summary>
        /// Resets the buffer to 0 length
        /// </summary>
        public void Reset() {
            Buffer.Clear();
        }

        /// <summary>
        /// Writes a <see cref="GMLInteropTypeId"/> to the buffer as 1 byte
        /// </summary>
        /// <param name="type">The <see cref="GMLInteropTypeId"/></param>
        public void WriteInteropTypeId(GMLInteropTypeId type) => WriteUInt(type.ToValue());
        /// <summary>
        /// Writes a <see cref="GMLInteropTypeId"/> to the buffer as 1 byte from the gml side
        /// </summary>
        /// <param name="type">The <see cref="GMLInteropTypeId"/> as a gml variable/expression</param>
        /// <remarks>Only use in the write function given to <see cref="GMLInteropManager"/></remarks>
        public static string WriteInteropTypeIdFromGML(string type) => $"buffer_write(argBuffer, buffer_u32, {type})";

        /// <summary>
        /// Writes a <see cref="byte"/> to the buffer
        /// </summary>
        /// <param name="value">The <see cref="byte"/></param>
        public void WriteByte(byte value) => Buffer.Add(value);
        /// <summary>
        /// Writes a <see cref="byte"/> to the buffer from the gml side
        /// </summary>
        /// <param name="value">The <see cref="byte"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteByteFromGML(string value) => $"buffer_write(argBuffer, buffer_u8, {value})";

        /// <summary>
        /// Writes a <see cref="sbyte"/> to the buffer as 1 byte
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/></param>
        public void WriteSByte(sbyte value) => Buffer.Add((byte)value);
        /// <summary>
        /// Writes a <see cref="sbyte"/> to the buffer as 1 byte from the gml side
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteSByteFromGML(string value) => $"buffer_write(argBuffer, buffer_s8, {value})";

        /// <summary>
        /// Writes a <see cref="ushort"/> to the buffer as 2 bytes
        /// </summary>
        /// <param name="value">The <see cref="ushort"/></param>
        public void WriteUShort(ushort value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="ushort"/> to the buffer as 2 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteUShortFromGML(string value) => $"buffer_write(argBuffer, buffer_u16, {value})";

        /// <summary>
        /// Writes a <see cref="short"/> to the buffer as 2 bytes
        /// </summary>
        /// <param name="value">The <see cref="short"/></param>
        public void WriteShort(short value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="short"/> to the buffer as 2 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="short"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteShortFromGML(string value) => $"buffer_write(argBuffer, buffer_s16, {value})";

        /// <summary>
        /// Writes a <see cref="uint"/> to the buffer as 4 bytes
        /// </summary>
        /// <param name="value">The <see cref="uint"/></param>
        public void WriteUInt(uint value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="uint"/> to the buffer as 4 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="uint"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteUIntFromGML(string value) => $"buffer_write(argBuffer, buffer_u32, {value})";

        /// <summary>
        /// Writes a <see cref="int"/> to the buffer as 4 bytes
        /// </summary>
        /// <param name="value">The <see cref="int"/></param>
        public void WriteInt(int value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="int"/> to the buffer as 4 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="int"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteIntFromGML(string value) => $"buffer_write(argBuffer, buffer_s32, {value})";

        /// <summary>
        /// Writes a <see cref="long"/> to the buffer as 8 bytes
        /// </summary>
        /// <param name="value">The <see cref="long"/></param>
        public void WriteLong(long value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="long"/> to the buffer as 8 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="long"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteLongFromGML(string value) => $"buffer_write(argBuffer, buffer_u64, {value})";

        /// <summary>
        /// Writes a <see cref="Half"/> to the buffer as 2 bytes
        /// </summary>
        /// <param name="value">The <see cref="Half"/></param>
        public void WriteHalf(Half value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="Half"/> to the buffer as 2 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="Half"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteHalfFromGML(string value) => $"buffer_write(argBuffer, buffer_f16, {value})";

        /// <summary>
        /// Writes a <see cref="float"/> to the buffer as 4 bytes
        /// </summary>
        /// <param name="value">The <see cref="float"/></param>
        public void WriteFloat(float value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="float"/> to the buffer as 4 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="float"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteFloatFromGML(string value) => $"buffer_write(argBuffer, buffer_f32, {value})";

        /// <summary>
        /// Writes a <see cref="double"/> to the buffer as 8 bytes
        /// </summary>
        /// <param name="value">The <see cref="double"/></param>
        public void WriteDouble(double value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="double"/> to the buffer as 8 bytes from the gml side
        /// </summary>
        /// <param name="value">The <see cref="double"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteDoubleFromGML(string value) => $"buffer_write(argBuffer, buffer_f64, {value})";

        /// <summary>
        /// Writes a <see cref="bool"/> to the buffer as 1 byte
        /// </summary>
        /// <param name="value">The <see cref="bool"/></param>
        public void WriteBool(bool value) => Buffer.AddRange(BitConverter.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="bool"/> to the buffer as 1 byte from the gml side
        /// </summary>
        /// <param name="value">The <see cref="bool"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteBoolFromGML(string value) => $"buffer_write(argBuffer, buffer_bool, {value})";

        /// <summary>
        /// Writes a <see cref="string"/> to the buffer
        /// </summary>
        /// <param name="value">The <see cref="string"/></param>
        public void WriteString(string value) => Buffer.AddRange(Encoding.UTF8.GetBytes(value));
        /// <summary>
        /// Writes a <see cref="string"/> to the buffer from the gml side
        /// </summary>
        /// <param name="value">The <see cref="string"/> as a gml variable/expression</param>
        /// <inheritdoc cref="WriteInteropTypeIdFromGML"/>
        public static string WriteStringFromGML(string value) => $"buffer_write(argBuffer, buffer_string, {value})";

        /// <summary>
        /// Writes an object of the type corresponding to the given <see cref="GMLInteropTypeId"/> to the buffer
        /// </summary>
        /// <param name="id">The <see cref="GMLInteropTypeId"/> corresponding to the type of the object</param>
        /// <param name="value">The object</param>
        public void Write(GMLInteropTypeId id, object value) {
            if (id.IsArray())
                WriteArray(id.GetElementTypeId(), (Array)value);
            else {
                GMLInteropManager.IRegisteredType register = GMLInteropManager.GetRegisteredType(id);
                if (value.GetType() != register.Type)
                    throw new ArgumentException($"Given object of type {value.GetType()} does not match the type {register.Type} of the given id {id}.", nameof(value));
                register.Write(this, value);
            }
        }
        /// <summary>
        /// Writes an object of the type corresponding to the given <see cref="GMLInteropTypeId"/> to the buffer from the gml side
        /// </summary>
        /// <param name="id">The <see cref="GMLInteropTypeId"/> corresponding to the type of the object as a gml variable/expression</param>
        /// <param name="value">The object as a gml variable/expression</param>
        public static string WriteFromGML(string id, string value) => $"{GMLInteropManager.GML_gmlinterop_write}(argBuffer, {id}, {value})";

        /// <summary>
        /// Writes an object of the given <see cref="Type"/> to the buffer
        /// </summary>
        /// <param name="type">The type to write as a gml variable/expression</param>
        /// <param name="value">The object as a gml variable/expression</param>
        public void Write(Type type, object value) {
            if (type.IsArray)
                WriteArray(type.GetElementType(), (Array)value);
            else {
                GMLInteropManager.IRegisteredType register = GMLInteropManager.GetRegisteredType(type);
                if (value.GetType() != register.Type)
                    throw new ArgumentException($"Given object of type {type.GetType()} does not match the given type {type}.");
                register.Write(this, value);
            }
        }
        /// <summary>
        /// Writes an object of the given <see cref="Type"/> to the buffer from the gml side
        /// </summary>
        /// <param name="type">The type to write as a gml variable/expression</param>
        /// <param name="value">The object as a gml variable/expression</param>
        public static string WriteFromGML(Type type, string value) => WriteFromGML($"{GMLInteropManager.GetRegisteredType(type).Id.ToValue()}", value);

        /// <summary>
        /// Writes an object of type <typeparamref name="T"/> to the buffer
        /// </summary>
        /// <typeparam name="T">The type to write</typeparam>
        /// <param name="value">The object</param>
        public void Write<T>(T value) {
            Type type = typeof(T);
            if (type.IsArray)
                // TODO: see if this actually works as well
                WriteArray(type, (Array)(object)value);
            else
                GMLInteropManager.GetRegisteredType<T>().Write(this, value);
        }
        /// <summary>
        /// Writes an object of type <typeparamref name="T"/> to the buffer from the gml side
        /// </summary>
        /// <typeparam name="T">The type to write as a gml variable/expression</typeparam>
        /// <param name="value">The object as a gml variable/expression</param>
        public static string WriteFromGML<T>(string value) => WriteFromGML($"{GMLInteropManager.GetRegisteredType<T>().Id.ToValue()}", value);

        /// <summary>
        /// Writes an array with elements of the type corresponding to the given <see cref="GMLInteropTypeId"/> to the buffer
        /// </summary>
        /// <param name="id">The id of the type of the array elements</param>
        /// <param name="value">The array</param>
        /// <exception cref="ArgumentException"></exception>
        public void WriteArray(GMLInteropTypeId id, Array value) {
            GMLInteropManager.IRegisteredType register = GMLInteropManager.GetRegisteredType(id);
            if (value.GetType().GetElementType() != register.Type)
                throw new ArgumentException($"Given array of type {value.GetType()} does not match the type {register.Type} of the given id {id}.", nameof(value));
            WriteInt(value.Length);
            foreach (object item in value)
                register.Write(this, item);
        }
        /// <summary>
        /// Writes an array with elements of the type corresponding to the given <see cref="GMLInteropTypeId"/> to the buffer from the gml side
        /// </summary>
        /// <param name="id">The id of the type of the array elements as a gml variable/expression</param>
        /// <param name="value">The array as a gml variable/expression</param>
        public static string WriteArrayFromGML(string id, string value) => WriteFromGML($"int64({id}) | {GMLInteropTypeId.IsArray.ToValue()}", value);

        /// <summary>
        /// Writes an array with elements of the given <see cref="Type"/> to the buffer
        /// </summary>
        /// <param name="type">The type of the array elements</param>
        /// <param name="value">The array</param>
        /// <exception cref="ArgumentException"></exception>
        public void WriteArray(Type type, Array value) {
            GMLInteropManager.IRegisteredType register = GMLInteropManager.GetRegisteredType(type);
            if (value.GetType().GetElementType() != register.Type)
                throw new ArgumentException($"Given array of type {value.GetType()} does not match the given type {register.Type}.", nameof(value));
            WriteInt(value.Length);
            foreach (object item in value)
                register.Write(this, item);
        }
        /// <summary>
        /// Writes an array with elements of the given <see cref="Type"/> to the buffer from the gml side
        /// </summary>
        /// <param name="type">The type of the array elements as a gml variable/expression</param>
        /// <param name="value">The array as a gml variable/expression</param>
        public static string WriteArrayFromGML(Type type, string value) => WriteArrayFromGML($"{GMLInteropManager.GetRegisteredType(type).Id.ToValue()}", value);

        /// <summary>
        /// Writes an array with elements of type <typeparamref name="T"/> to the buffer
        /// </summary>
        /// <typeparam name="T">The type of the array elements</typeparam>
        /// <param name="value">The array</param>
        public void WriteArray<T>(T[] value) {
            GMLInteropManager.RegisteredType<T> register = GMLInteropManager.GetRegisteredType<T>();
            WriteInt(value.Length);
            foreach (T item in value)
                register.Write(this, item);
        }
        /// <summary>
        /// Writes an array with elements of type <typeparamref name="T"/> to the buffer from the gml side
        /// </summary>
        /// <typeparam name="T">The type of the array elements as a gml variable/expression</typeparam>
        /// <param name="value">The array as a gml variable/expression</param>
        public static string WriteArrayFromGML<T>(string value) => WriteArrayFromGML($"{GMLInteropManager.GetRegisteredType<T>().Id.ToValue()}", value);

        /// <summary>
        /// Saves the buffer to a new <see cref="byte"/>*
        /// </summary>
        /// <returns>A new <see cref="byte"/>*</returns>
        /// <remarks>Use <see cref="DeleteBytes(byte*)"/> when finished with the returned <see cref="byte"/>*</remarks>
        public unsafe byte* GetBytes() {
            byte[] buffer = Buffer.ToArray();
            IntPtr bytes = Marshal.AllocHGlobal(buffer.Length + 4);
            Marshal.WriteInt32(bytes, buffer.Length + 4);
            Marshal.Copy(buffer, 0, bytes + 4, buffer.Length);
            return (byte*)bytes.ToPointer();
        }

        private unsafe delegate void DeleteBytesDelegate(byte* bytes);
        /// <summary>
        /// Deletes the given <see cref="byte"/>*
        /// </summary>
        /// <param name="bytes">The <see cref="byte"/>* to delete</param>
        /// <remarks>To be safe, only use the result of <see cref="GetBytes"/> for <paramref name="bytes"/></remarks>
        public static unsafe void DeleteBytes(byte* bytes) {
            Marshal.FreeHGlobal(new IntPtr(bytes));
        }
    }
}
