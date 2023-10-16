using System;

namespace SubModLoader.GMLInterop.Enums {
    // TODO: possibly convert this into a struct
    /// <summary>
    /// All the types available for GML to CSharp interop.
    /// </summary>
    /// <remarks>The bottom 16 bits are the type, the upper 16 bits are a bitfield of flags</remarks>
    public enum GMLInteropTypeId : uint {
        #region Lower 16 bits
        #region Types supported natively with GML buffers
        /// <summary>
        /// Type <see cref="void"/>, only for returns
        /// </summary>
        Void = 0,
        /// <summary>
        /// Type <see cref="byte"/>
        /// </summary>
        Byte,
        /// <summary>
        /// Type <see cref="sbyte"/>
        /// </summary>
        SByte,
        /// <summary>
        /// Type <see cref="ushort"/>
        /// </summary>
        UShort,
        /// <summary>
        /// Type <see cref="short"/>
        /// </summary>
        Short,
        /// <summary>
        /// Type <see cref="uint"/>
        /// </summary>
        UInt,
        /// <summary>
        /// Type <see cref="int"/>
        /// </summary>
        Int,
        /// <summary>
        /// Type <see cref="long"/>
        /// </summary>
        Long,
        /// <summary>
        /// Type <see cref="System.Half"/>
        /// </summary>
        Half,
        /// <summary>
        /// Type <see cref="float"/>
        /// </summary>
        Float,
        /// <summary>
        /// Type <see cref="double"/>
        /// </summary>
        Double,
        /// <summary>
        /// Type <see cref="bool"/>
        /// </summary>
        Bool,
        /// <summary>
        /// Type <see cref="string"/>
        /// </summary>
        String,
        #endregion
        #region Types with added support
        /// <summary>
        /// Type <see cref="char"/>, is a <see cref="string"/> in gml
        /// </summary>
        Char = 100,
        /// <summary>
        /// Type <see cref="IntPtr"/>
        /// </summary>
        IntPtr,
        #endregion
        /// <summary>
        /// Where the ids start for types registered with <see cref="GMLInteropManager.RegisterType{T}"/>
        /// </summary>
        RegisteredTypesStart = 1000,
        #endregion
        #region Upper 16 bits
        /// <summary>
        /// Represents a 1d array type
        /// </summary>
        IsArray = 1 << 16,
        /* For later ;)
        /// <summary>
        /// Represents an <see langword="out"/> type
        /// </summary>
        IsOut = 1 << 17,
        /// <summary>
        /// Represents a <see langword="ref"/> type
        /// </summary>
        IsRef = 1 << 18,*/
        #endregion
    }

    /// <summary>
    /// Adds some useful extension methods for for <see cref="GMLInteropTypeId"/>
    /// </summary>
    public static class GMLInteropTypeIdExt {
        /// <summary>
        /// Gets the respecitive <see cref="Type"/> of the <see cref="GMLInteropTypeId"/>
        /// </summary>
        /// <param name="id">The given <see cref="GMLInteropTypeId"/></param>
        /// <returns>The corresponding <see cref="Type"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Type ToType(this GMLInteropTypeId id) {
            Type result = GMLInteropManager.GetRegisteredType(id).Type;
            if (id.IsArray())
                result = result.MakeArrayType();
            return result;
        }

        /// <summary>
        /// Gets the respective <see cref="Type"/> of the array element of the <see cref="GMLInteropTypeId"/>
        /// </summary>
        /// <param name="id">The given <see cref="GMLInteropTypeId"/></param>
        /// <returns>The corresponding <see cref="Type"/></returns>
        public static Type GetElementType(this GMLInteropTypeId id) => id.ToType().GetElementType();

        /// <summary>
        /// Gets the respective <see cref="GMLInteropTypeId"/> of the array element of the <see cref="GMLInteropTypeId"/>
        /// </summary>
        /// <param name="id">The given <see cref="GMLInteropTypeId"/></param>
        /// <returns>The corresponding <see cref="GMLInteropTypeId"/></returns>
        public static GMLInteropTypeId GetElementTypeId(this GMLInteropTypeId id) => id & ~GMLInteropTypeId.IsArray;

        /// <summary>
        /// Gets the respective <see cref="GMLInteropTypeId"/> of the <see cref="Type"/>
        /// </summary>
        /// <param name="type">The given <see cref="Type"/></param>
        /// <returns>The corresponding <see cref="GMLInteropTypeId"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static GMLInteropTypeId ToGMLInteropTypeId(this Type type) {
            if (type.IsArray)
                return type.GetElementType().ToGMLInteropTypeId() | GMLInteropTypeId.IsArray;
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            return GMLInteropManager.GetRegisteredType(type).Id;
        }

        /// <summary>
        /// Gets the respective <see cref="GMLInteropTypeId"/> of the array element of the <see cref="Type"/>
        /// </summary>
        /// <param name="type">The given <see cref="Type"/></param>
        /// <returns>The corresponding <see cref="GMLInteropTypeId"/></returns>
        public static GMLInteropTypeId GetElementGMLInteropTypeId(this Type type) => type.ToGMLInteropTypeId().GetElementTypeId();

        /// <summary>
        /// Gets the <see cref="byte"/> value of the <see cref="GMLInteropTypeId"/>
        /// </summary>
        /// <param name="id">The <see cref="GMLInteropTypeId"/></param>
        /// <returns>The <see cref="byte"/> value</returns>
        public static uint ToValue(this GMLInteropTypeId id) => (uint)id;

        /// <summary>
        /// Checks if the <see cref="GMLInteropTypeId.IsArray"/> bit is set
        /// </summary>
        /// <param name="id">The type id</param>
        /// <returns>True if it is an array, false otheriwse</returns>
        public static bool IsArray(this GMLInteropTypeId id) => (id & GMLInteropTypeId.IsArray) != 0;
    }
}
