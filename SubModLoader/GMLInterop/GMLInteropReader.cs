using SubModLoader.GMLInterop.Enums;
using System;
using System.Text;

namespace SubModLoader.GMLInterop {
    /// <summary>
    /// Reads from a <see cref="byte"/>* buffer
    /// </summary>
    public class GMLInteropReader {
        private unsafe byte* Buffer { get; set; }
        private int Offset { get; set; }
        private uint Length { get; set; }

        /// <summary>
        /// The default ctor
        /// </summary>
        /// <param name="buffer">The buffer to read from</param>
        public unsafe GMLInteropReader(byte* buffer) => Reset(buffer);

        /// <summary>
        /// Resets the buffer and starts from the beginning of a new buffer
        /// </summary>
        /// <param name="buffer">The new buffer to read from</param>
        public unsafe void Reset(byte* buffer) {
            Buffer = buffer;
            Offset = 0;
            Length = ReadT<uint>(noError: true);
        }

        private static void ThrowOutOfRange() => throw new IndexOutOfRangeException("Tried to read past end of buffer");

        private unsafe T ReadT<T>(bool noError = false) where T : unmanaged {
            if (!noError && Offset + sizeof(T) > Length)
                ThrowOutOfRange();
            T result = ((T*)&Buffer[Offset])[0];
            Offset += sizeof(T);
            return result;
        }

        private unsafe byte Read1() {
            if (Offset + 1 > Length)
                ThrowOutOfRange();
            return Buffer[Offset++];
        }

        /// <summary>
        /// Reads a <see cref="GMLInteropTypeId"/> from the buffer as 1 byte
        /// </summary>
        /// <returns>The <see cref="GMLInteropTypeId"/></returns>
        public GMLInteropTypeId ReadInteropTypeId() => (GMLInteropTypeId)ReadUInt();
        /// <summary>
        /// Reads a <see cref="GMLInteropTypeId"/> from the buffer as 1 byte from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="GMLInteropTypeId"/> from gml</returns>
        /// <remarks>Only use in the read function given to <see cref="GMLInteropManager"/></remarks>
        public static string ReadInteropTypeIdFromGML() => "buffer_read(resultBuffer, buffer_u32)";

        /// <summary>
        /// Reads a <see cref="byte"/> from the buffer as 1 byte
        /// </summary>
        /// <returns>The <see cref="byte"/></returns>
        public byte ReadByte() => Read1();
        /// <summary>
        /// Reads a <see cref="byte"/> from the buffer as 1 byte from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="byte"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadByteFromGML() => "buffer_read(resultBuffer, buffer_u8)";

        /// <summary>
        /// Reads a <see cref="sbyte"/> from the buffer as 1 byte
        /// </summary>
        /// <returns>The <see cref="sbyte"/></returns>
        public sbyte ReadSByte() => (sbyte)Read1();
        /// <summary>
        /// Reads a <see cref="sbyte"/> from the buffer as 1 byte from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="sbyte"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadSByteFromGML() => "buffer_read(resultBuffer, buffer_s8)";

        /// <summary>
        /// Reads a <see cref="ushort"/> from the buffer as 2 bytes
        /// </summary>
        /// <returns>The <see cref="ushort"/></returns>
        public ushort ReadUShort() => ReadT<ushort>();
        /// <summary>
        /// Reads a <see cref="ushort"/> from the buffer as 2 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="ushort"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadUShortFromGML() => "buffer_read(resultBuffer, buffer_u16)";

        /// <summary>
        /// Reads a <see cref="short"/> from the buffer as 2 bytes
        /// </summary>
        /// <returns>The <see cref="short"/></returns>
        public short ReadShort() => ReadT<short>();
        /// <summary>
        /// Reads a <see cref="short"/> from the buffer as 2 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="short"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadShortFromGML() => "buffer_read(resultBuffer, buffer_s16)";

        /// <summary>
        /// Reads a <see cref="uint"/> from the buffer as 4 bytes
        /// </summary>
        /// <returns>The <see cref="uint"/></returns>
        public uint ReadUInt() => ReadT<uint>();
        /// <summary>
        /// Reads a <see cref="uint"/> from the buffer as 4 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="uint"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadUIntFromGML() => "buffer_read(resultBuffer, buffer_u32)";

        /// <summary>
        /// Reads a <see cref="int"/> from the buffer as 4 bytes
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int ReadInt() => ReadT<int>();
        /// <summary>
        /// Reads a <see cref="int"/> from the buffer as 4 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="int"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadIntFromGML() => "buffer_read(resultBuffer, buffer_s32)";

        /// <summary>
        /// Reads a <see cref="long"/> from the buffer as 8 bytes
        /// </summary>
        /// <returns>The <see cref="long"/></returns>
        public long ReadLong() => ReadT<long>();
        /// <summary>
        /// Reads a <see cref="long"/> from the buffer as 8 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="long"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadLongFromGML() => "buffer_read(resultBuffer, buffer_u64)";

        /// <summary>
        /// Reads a <see cref="Half"/> from the buffer as 2 bytes
        /// </summary>
        /// <returns>The <see cref="Half"/></returns>
        public Half ReadHalf() => ReadT<Half>();
        /// <summary>
        /// Reads a <see cref="Half"/> from the buffer as 2 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="Half"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadHalfFromGML() => "buffer_read(resultBuffer, buffer_f16)";

        /// <summary>
        /// Reads a <see cref="float"/> from the buffer as 4 bytes
        /// </summary>
        /// <returns>The <see cref="float"/></returns>
        public float ReadFloat() => ReadT<float>();
        /// <summary>
        /// Reads a <see cref="float"/> from the buffer as 4 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="float"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadFloatFromGML() => "buffer_read(resultBuffer, buffer_f32)";

        /// <summary>
        /// Reads a <see cref="double"/> from the buffer as 8 bytes
        /// </summary>
        /// <returns>The <see cref="double"/></returns>
        public double ReadDouble() => ReadT<double>();
        /// <summary>
        /// Reads a <see cref="double"/> from the buffer as 8 bytes from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="double"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadDoubleFromGML() => "buffer_read(resultBuffer, buffer_f64)";

        /// <summary>
        /// Reads a <see cref="bool"/> from the buffer as 1 byte
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool ReadBool() => ReadT<bool>();
        /// <summary>
        /// Reads a <see cref="bool"/> from the buffer as 1 byte from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="bool"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadBoolFromGML() => "buffer_read(resultBuffer, buffer_bool)";

        /// <summary>
        /// Reads a <see cref="string"/> from the buffer
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public unsafe string ReadString() {
            bool foundEnd = false;
            int end = Offset;
            while (!foundEnd && end < Length) {
                if (Buffer[end] == '\0')
                    foundEnd = true;
                else
                    end++;
            }

            if (!foundEnd)
                throw new IndexOutOfRangeException("Could not find end of string before end of buffer.");

            string result = new((sbyte*)Buffer, Offset, end - Offset, Encoding.UTF8);
            Offset = end + 1;
            return result;
        }
        /// <summary>
        /// Reads a <see cref="string"/> from the buffer from the gml side
        /// </summary>
        /// <returns>The string to read the <see cref="string"/> from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadStringFromGML() => "buffer_read(resultBuffer, buffer_string)";

        /// <summary>
        /// Reads an object of the type corresponding to the given <see cref="GMLInteropTypeId"/> from the buffer
        /// </summary>
        /// <param name="id">The <see cref="GMLInteropTypeId"/> corresponding to the type of the object</param>
        /// <returns>The object</returns>
        public object Read(GMLInteropTypeId id) {
            if (id.IsArray())
                return ReadArray(id.GetElementTypeId());
            return GMLInteropManager.GetRegisteredType(id).Read(this);
        }
        /// <summary>
        /// Reads an object of the type corresponding to the given <see cref="GMLInteropTypeId"/> from the buffer from the gml side
        /// </summary>
        /// <param name="id">The gml variable/expression of the <see cref="GMLInteropTypeId"/> corresponding to the type of the object</param>
        /// <returns>The string to read the object from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadFromGML(string id) => $"{GMLInteropManager.GML_gmlinterop_read}(resultBuffer, {id})";

        /// <summary>
        /// Reads an object from the buffer of the given type
        /// </summary>
        /// <param name="type">The type to read</param>
        /// <returns>The object</returns>
        public object Read(Type type) {
            if (type.IsArray)
                return ReadArray(type.GetElementType());
            return GMLInteropManager.GetRegisteredType(type).Read(this);
        }
        /// <summary>
        /// Reads an object from the buffer of the given type from the gml side
        /// </summary>
        /// <param name="type">The type to read</param>
        /// <returns>The string to read the object from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadFromGML(Type type) => ReadFromGML($"{GMLInteropManager.GetRegisteredType(type).Id.ToValue()}");

        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the buffer
        /// </summary>
        /// <typeparam name="T">The type to read</typeparam>
        /// <returns>The object</returns>
        public T Read<T>() {
            Type type = typeof(T);
            if (type.IsArray)
                // TODO: test if this actually works
                return (T)(object)ReadArray(type.GetElementType());
            return (T)GMLInteropManager.GetRegisteredType<T>().Read(this);
        }
        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the buffer from the gml side
        /// </summary>
        /// <typeparam name="T">The type to read</typeparam>
        /// <returns>The string to read the object from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadFromGML<T>() => ReadFromGML($"{GMLInteropManager.GetRegisteredType<T>().Id.ToValue()}");

        /// <summary>
        /// Reads an array with elements corresponding to the given <see cref="GMLInteropTypeId"/> from the buffer
        /// </summary>
        /// <param name="id">The type id of the array elements</param>
        /// <returns>The array</returns>
        public Array ReadArray(GMLInteropTypeId id) {
            GMLInteropManager.IRegisteredType register = GMLInteropManager.GetRegisteredType(id);
            int length = ReadInt();
            Array array = Array.CreateInstance(register.Type, length);
            for (int i = 0; i < length; i++)
                array.SetValue(Read(id), i);
            return array;
        }
        /// <summary>
        /// Reads an array with elements corresponding to the given <see cref="GMLInteropTypeId"/> from the buffer from the gml side
        /// </summary>
        /// <param name="id">The gml variable/expression of the type id of the array elements</param>
        /// <returns>The string to read the array from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadArrayFromGML(string id) => ReadFromGML($"int64({id}) | {GMLInteropTypeId.IsArray.ToValue()}");

        /// <summary>
        /// Reads an array with elements of the given <see cref="Type"/> from the buffer
        /// </summary>
        /// <param name="type">The type of the array elements</param>
        /// <returns>The array</returns>
        public Array ReadArray(Type type) {
            int length = ReadInt();
            Array array = Array.CreateInstance(type, length);
            for (int i = 0; i < length; i++)
                array.SetValue(Read(type), i);
            return array;
        }
        /// <summary>
        /// Reads an array with elements of the given <see cref="Type"/> from the buffer from the gml side
        /// </summary>
        /// <param name="type">The type of the array elements</param>
        /// <returns>The string to read the array from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadArrayFromGML(Type type) => ReadArrayFromGML($"{GMLInteropManager.GetRegisteredType(type).Id.ToValue()}");

        /// <summary>
        /// Reads an array with elements of type <typeparamref name="T"/> from the buffer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The array</returns>
        public T[] ReadArray<T>() {
            int length = ReadInt();
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
                array[i] = Read<T>();
            return array;
        }
        /// <summary>
        /// Reads an array with elements of type <typeparamref name="T"/> from the buffer from the gml side
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The string to read the array from gml</returns>
        /// <inheritdoc cref="ReadInteropTypeIdFromGML"/>
        public static string ReadArrayFromGML<T>() => ReadArrayFromGML($"{GMLInteropManager.GetRegisteredType<T>().Id.ToValue()}");
    }
}
