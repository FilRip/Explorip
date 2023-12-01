//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Resources;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace MS.WindowsAPICodePack.Internal
{
    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    /// This class is intended for internal use only.
    /// </summary>
    /// <remarks>
    /// Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx
    /// and modified to support additional types including vectors and ability to set values
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class PropVariant : IDisposable
    {
        #region Vector Action Cache

        // A static dictionary of delegates to get data from array's contained within PropVariants
        private static Dictionary<Type, Action<PropVariant, Array, uint>> _vectorActions = null;

        private static Dictionary<Type, Action<PropVariant, Array, uint>> GenerateVectorActions()
        {
            Dictionary<Type, Action<PropVariant, Array, uint>> cache = new()
            {
                {
                    typeof(Int16),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt16Elem(pv, i, out short val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(UInt16),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt16Elem(pv, i, out ushort val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(Int32),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt32Elem(pv, i, out int val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(UInt32),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt32Elem(pv, i, out uint val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(Int64),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt64Elem(pv, i, out long val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(UInt64),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt64Elem(pv, i, out ulong val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(DateTime),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetFileTimeElem(pv, i, out System.Runtime.InteropServices.ComTypes.FILETIME val);

                        long fileTime = GetFileTimeAsLong(ref val);

                        array.SetValue(DateTime.FromFileTime(fileTime), i);
                    }
                },

                {
                    typeof(Boolean),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetBooleanElem(pv, i, out bool val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(Double),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetDoubleElem(pv, i, out double val);
                        array.SetValue(val, i);
                    }
                },

                {
                    typeof(Single),
                    (pv, array, i) => // float
                    {
                        float[] val = new float[1];
                        Marshal.Copy(pv._ptr2, val, (int)i, 1);
                        array.SetValue(val[0], (int)i);
                    }
                },

                {
                    typeof(Decimal),
                    (pv, array, i) =>
                    {
                        int[] val = new int[4];
                        for (int a = 0; a < val.Length; a++)
                        {
                            val[a] = Marshal.ReadInt32(pv._ptr2,
                                (int)i * sizeof(decimal) + a * sizeof(int)); //index * size + offset quarter
                        }
                        array.SetValue(new decimal(val), i);
                    }
                },

                {
                    typeof(String),
                    (pv, array, i) =>
                    {
                        string val = string.Empty;
                        PropVariantNativeMethods.PropVariantGetStringElem(pv, i, ref val);
                        array.SetValue(val, i);
                    }
                }
            };

            return cache;
        }
        #endregion

        #region Dynamic Construction / Factory (Expressions)

        /// <summary>
        /// Attempts to create a PropVariant by finding an appropriate constructor.
        /// </summary>
        /// <param name="value">Object from which PropVariant should be created.</param>
        public static PropVariant FromObject(object value)
        {
            if (value == null)
            {
                return new PropVariant();
            }
            else
            {
                Func<object, PropVariant> func = GetDynamicConstructor(value.GetType());
                return func(value);
            }
        }

        // A dictionary and lock to contain compiled expression trees for constructors
        private static readonly Dictionary<Type, Func<object, PropVariant>> _cache = [];
        private static readonly object _padlock = new();

        // Retrieves a cached constructor expression.
        // If no constructor has been cached, it attempts to find/add it.  If it cannot be found
        // an exception is thrown.
        // This method looks for a public constructor with the same parameter type as the object.
        private static Func<object, PropVariant> GetDynamicConstructor(Type type)
        {
            lock (_padlock)
            {
                // initial check, if action is found, return it
                if (!_cache.TryGetValue(type, out Func<object, PropVariant> action))
                {
                    // iterates through all constructors
                    ConstructorInfo constructor = typeof(PropVariant)
                        .GetConstructor([type]);

                    if (constructor == null)
                    { // if the method was not found, throw.
                        throw new ArgumentException(LocalizedMessages.PropVariantTypeNotSupported);
                    }
                    else // if the method was found, create an expression to call it.
                    {
                        // create parameters to action                    
                        ParameterExpression arg = Expression.Parameter(typeof(object), "arg");

                        // create an expression to invoke the constructor with an argument cast to the correct type
                        NewExpression create = Expression.New(constructor, Expression.Convert(arg, type));

                        // compiles expression into an action delegate
                        action = Expression.Lambda<Func<object, PropVariant>>(create, arg).Compile();
                        _cache.Add(type, action);
                    }
                }
                return action;
            }
        }

        #endregion

        #region Fields

        [FieldOffset(0)]
        readonly decimal _decimal;

        // This is actually a VarEnum value, but the VarEnum type
        // requires 4 bytes instead of the expected 2.
        [FieldOffset(0)]
        ushort _valueType;

#pragma warning disable S125 // Sections of code should not be commented out
        // Reserved Fields
        //[FieldOffset(2)]
        //ushort _wReserved1;
        //[FieldOffset(4)]
        //ushort _wReserved2;
        //[FieldOffset(6)]
        //ushort _wReserved3;
#pragma warning restore S125 // Sections of code should not be commented out

        // In order to allow x64 compat, we need to allow for
        // expansion of the IntPtr. However, the BLOB struct
        // uses a 4-byte int, followed by an IntPtr, so
        // although the valueData field catches most pointer values,
        // we need an additional 4-bytes to get the BLOB
        // pointer. The valueDataExt field provides this, as well as
        // the last 4-bytes of an 8-byte value on 32-bit
        // architectures.
        [FieldOffset(12)]
        readonly IntPtr _ptr2;
        [FieldOffset(8)]
        IntPtr _ptr;
        [FieldOffset(8)]
        readonly Int32 _int32;
        [FieldOffset(8)]
        readonly UInt32 _uint32;
        [FieldOffset(8)]
        readonly byte _byte;
        [FieldOffset(8)]
        readonly sbyte _sbyte;
        [FieldOffset(8)]
        readonly short _short;
        [FieldOffset(8)]
        readonly ushort _ushort;
        [FieldOffset(8)]
        readonly long _long;
        [FieldOffset(8)]
        readonly ulong _ulong;
        [FieldOffset(8)]
        readonly double _double;
        [FieldOffset(8)]
        readonly float _float;

        #endregion // struct fields

        #region Constructors

        /// <summary>
        /// Default constrcutor
        /// </summary>
        public PropVariant()
        {
            // left empty
        }

        /// <summary>
        /// Set a string value
        /// </summary>
        public PropVariant(string value)
        {
            if (value == null)
            {
                throw new ArgumentException(LocalizedMessages.PropVariantNullString, "value");
            }

            _valueType = (ushort)VarEnum.VT_LPWSTR;
            _ptr = Marshal.StringToCoTaskMemUni(value);
        }

        /// <summary>
        /// Set a string vector
        /// </summary>
        public PropVariant(string[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromStringVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a bool vector
        /// </summary>
        public PropVariant(bool[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromBooleanVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        public PropVariant(short[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromInt16Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        public PropVariant(ushort[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromUInt16Vector(value, (uint)value.Length, this);

        }

        /// <summary>
        /// Set an int vector
        /// </summary>
        public PropVariant(int[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set an uint vector
        /// </summary>
        public PropVariant(uint[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromUInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a long vector
        /// </summary>
        public PropVariant(long[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a ulong vector
        /// </summary>
        public PropVariant(ulong[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>>
        /// Set a double vector
        /// </summary>
        public PropVariant(double[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            PropVariantNativeMethods.InitPropVariantFromDoubleVector(value, (uint)value.Length, this);
        }


        /// <summary>
        /// Set a DateTime vector
        /// </summary>
        public PropVariant(DateTime[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            System.Runtime.InteropServices.ComTypes.FILETIME[] fileTimeArr =
                new System.Runtime.InteropServices.ComTypes.FILETIME[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                fileTimeArr[i] = DateTimeToFileTime(value[i]);
            }

            PropVariantNativeMethods.InitPropVariantFromFileTimeVector(fileTimeArr, (uint)fileTimeArr.Length, this);
        }

        /// <summary>
        /// Set a bool value
        /// </summary>
        public PropVariant(bool value)
        {
            _valueType = (ushort)VarEnum.VT_BOOL;
            _int32 = value ? -1 : 0;
        }

        /// <summary>
        /// Set a DateTime value
        /// </summary>
        public PropVariant(DateTime value)
        {
            _valueType = (ushort)VarEnum.VT_FILETIME;

            System.Runtime.InteropServices.ComTypes.FILETIME ft = DateTimeToFileTime(value);
            PropVariantNativeMethods.InitPropVariantFromFileTime(ref ft, this);
        }


        /// <summary>
        /// Set a byte value
        /// </summary>
        public PropVariant(byte value)
        {
            _valueType = (ushort)VarEnum.VT_UI1;
            _byte = value;
        }

        /// <summary>
        /// Set a sbyte value
        /// </summary>
        public PropVariant(sbyte value)
        {
            _valueType = (ushort)VarEnum.VT_I1;
            _sbyte = value;
        }

        /// <summary>
        /// Set a short value
        /// </summary>
        public PropVariant(short value)
        {
            _valueType = (ushort)VarEnum.VT_I2;
            _short = value;
        }

        /// <summary>
        /// Set an unsigned short value
        /// </summary>
        public PropVariant(ushort value)
        {
            _valueType = (ushort)VarEnum.VT_UI2;
            _ushort = value;
        }

        /// <summary>
        /// Set an int value
        /// </summary>
        public PropVariant(int value)
        {
            _valueType = (ushort)VarEnum.VT_I4;
            _int32 = value;
        }

        /// <summary>
        /// Set an unsigned int value
        /// </summary>
        public PropVariant(uint value)
        {
            _valueType = (ushort)VarEnum.VT_UI4;
            _uint32 = value;
        }

        /// <summary>
        /// Set a decimal  value
        /// </summary>
        public PropVariant(decimal value)
        {
            _decimal = value;

            // It is critical that the value type be set after the decimal value, because they overlap.
            // If valuetype is written first, its value will be lost when _decimal is written.
            _valueType = (ushort)VarEnum.VT_DECIMAL;
        }

        /// <summary>
        /// Create a PropVariant with a contained decimal array.
        /// </summary>
        /// <param name="value">Decimal array to wrap.</param>
        public PropVariant(decimal[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
            _valueType = (ushort)(VarEnum.VT_DECIMAL | VarEnum.VT_VECTOR);
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
            _int32 = value.Length;

            // allocate required memory for array with 128bit elements
            _ptr2 = Marshal.AllocCoTaskMem(value.Length * sizeof(decimal));
            for (int i = 0; i < value.Length; i++)
            {
                int[] bits = decimal.GetBits(value[i]);
                Marshal.Copy(bits, 0, _ptr2, bits.Length);
            }
        }

        /// <summary>
        /// Create a PropVariant containing a float type.
        /// </summary>        
        public PropVariant(float value)
        {
            _valueType = (ushort)VarEnum.VT_R4;

            _float = value;
        }

        /// <summary>
        /// Creates a PropVariant containing a float[] array.
        /// </summary>        
        public PropVariant(float[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
            _valueType = (ushort)(VarEnum.VT_R4 | VarEnum.VT_VECTOR);
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
            _int32 = value.Length;

            _ptr2 = Marshal.AllocCoTaskMem(value.Length * sizeof(float));

            Marshal.Copy(value, 0, _ptr2, value.Length);
        }

        /// <summary>
        /// Set a long
        /// </summary>
        public PropVariant(long value)
        {
            _long = value;
            _valueType = (ushort)VarEnum.VT_I8;
        }

        /// <summary>
        /// Set a ulong
        /// </summary>
        public PropVariant(ulong value)
        {
            _valueType = (ushort)VarEnum.VT_UI8;
            _ulong = value;
        }

        /// <summary>
        /// Set a double
        /// </summary>
        public PropVariant(double value)
        {
            _valueType = (ushort)VarEnum.VT_R8;
            _double = value;
        }

        #endregion

        #region Uncalled methods - These are currently not called, but I think may be valid in the future.

        /// <summary>
        /// Set an IUnknown value
        /// </summary>
        /// <param name="value">The new value to set.</param>
        internal void SetIUnknown(object value)
        {
            _valueType = (ushort)VarEnum.VT_UNKNOWN;
            _ptr = Marshal.GetIUnknownForObject(value);
        }


        /// <summary>
        /// Set a safe array value
        /// </summary>
        /// <param name="array">The new value to set.</param>
        internal void SetSafeArray(Array array)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            const ushort vtUnknown = 13;
            IntPtr psa = PropVariantNativeMethods.SafeArrayCreateVector(vtUnknown, 0, (uint)array.Length);

            IntPtr pvData = PropVariantNativeMethods.SafeArrayAccessData(psa);
            try // to remember to release lock on data
            {
                for (int i = 0; i < array.Length; ++i)
                {
                    object obj = array.GetValue(i);
                    IntPtr punk = (obj != null) ? Marshal.GetIUnknownForObject(obj) : IntPtr.Zero;
                    Marshal.WriteIntPtr(pvData, i * IntPtr.Size, punk);
                }
            }
            finally
            {
                PropVariantNativeMethods.SafeArrayUnaccessData(psa);
            }

            _valueType = (ushort)VarEnum.VT_ARRAY | (ushort)VarEnum.VT_UNKNOWN;
            _ptr = psa;
        }

        #endregion

        #region public Properties

        /// <summary>
        /// Gets or sets the variant type.
        /// </summary>
        public VarEnum VarType
        {
            get { return (VarEnum)_valueType; }
            set { _valueType = (ushort)value; }
        }

        /// <summary>
        /// Checks if this has an empty or null value
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty
        {
            get
            {
                return (_valueType == (ushort)VarEnum.VT_EMPTY || _valueType == (ushort)VarEnum.VT_NULL);
            }
        }

        /// <summary>
        /// Gets the variant value.
        /// </summary>
        public object Value
        {
            get
            {
                return (VarEnum)_valueType switch
                {
                    VarEnum.VT_I1 => _sbyte,
                    VarEnum.VT_UI1 => _byte,
                    VarEnum.VT_I2 => _short,
                    VarEnum.VT_UI2 => _ushort,
                    VarEnum.VT_I4 or VarEnum.VT_INT => _int32,
                    VarEnum.VT_UI4 or VarEnum.VT_UINT => _uint32,
                    VarEnum.VT_I8 => _long,
                    VarEnum.VT_UI8 => _ulong,
                    VarEnum.VT_R4 => _float,
                    VarEnum.VT_R8 => _double,
                    VarEnum.VT_BOOL => _int32 == -1,
                    VarEnum.VT_ERROR => _long,
                    VarEnum.VT_CY => _decimal,
                    VarEnum.VT_DATE => DateTime.FromOADate(_double),
                    VarEnum.VT_FILETIME => DateTime.FromFileTime(_long),
                    VarEnum.VT_BSTR => Marshal.PtrToStringBSTR(_ptr),
                    VarEnum.VT_BLOB => GetBlobData(),
                    VarEnum.VT_LPSTR => Marshal.PtrToStringAnsi(_ptr),
                    VarEnum.VT_LPWSTR => Marshal.PtrToStringUni(_ptr),
                    VarEnum.VT_UNKNOWN => Marshal.GetObjectForIUnknown(_ptr),
                    VarEnum.VT_DISPATCH => Marshal.GetObjectForIUnknown(_ptr),
                    VarEnum.VT_DECIMAL => _decimal,
#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
                    VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN => CrackSingleDimSafeArray(_ptr),
                    (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR) => GetVector<string>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_I2) => GetVector<Int16>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_UI2) => GetVector<UInt16>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_I4) => GetVector<Int32>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_UI4) => GetVector<UInt32>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_I8) => GetVector<Int64>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_UI8) => GetVector<UInt64>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_R4) => GetVector<float>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_R8) => GetVector<Double>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_BOOL) => GetVector<Boolean>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME) => GetVector<DateTime>(),
                    (VarEnum.VT_VECTOR | VarEnum.VT_DECIMAL) => GetVector<Decimal>(),
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
                    _ => null,// if the value cannot be marshaled
                };
            }
        }

        #endregion

        #region Private Methods

        private static long GetFileTimeAsLong(ref System.Runtime.InteropServices.ComTypes.FILETIME val)
        {
            return (((long)val.dwHighDateTime) << 32) + val.dwLowDateTime;
        }

        private static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFileTime(DateTime value)
        {
            long hFT = value.ToFileTime();
            System.Runtime.InteropServices.ComTypes.FILETIME ft = new()
            {
                dwLowDateTime = (int)(hFT & 0xFFFFFFFF),
                dwHighDateTime = (int)(hFT >> 32)
            };
            return ft;
        }

        private object GetBlobData()
        {
            byte[] blobData = new byte[_int32];

            IntPtr pBlobData = _ptr2;
            Marshal.Copy(pBlobData, blobData, 0, _int32);

            return blobData;
        }

        private static void SetVectorActions(Dictionary<Type, Action<PropVariant, Array, uint>> newValue)
        {
            _vectorActions = newValue;
        }
        private Array GetVector<T>()
        {
            int count = PropVariantNativeMethods.PropVariantGetElementCount(this);
            if (count <= 0) { return null; }

            lock (_padlock)
            {
                if (_vectorActions == null)
                {
                    SetVectorActions(GenerateVectorActions());
                }
            }

            if (!_vectorActions.TryGetValue(typeof(T), out Action<PropVariant, Array, uint> action))
            {
                throw new InvalidCastException(LocalizedMessages.PropVariantUnsupportedType);
            }

            Array array = new T[count];
            for (uint i = 0; i < count; i++)
            {
                action(this, array, i);
            }

            return array;
        }

        private static Array CrackSingleDimSafeArray(IntPtr psa)
        {
            uint cDims = PropVariantNativeMethods.SafeArrayGetDim(psa);
            if (cDims != 1)
                throw new ArgumentException(LocalizedMessages.PropVariantMultiDimArray, "psa");

            int lBound = PropVariantNativeMethods.SafeArrayGetLBound(psa, 1U);
            int uBound = PropVariantNativeMethods.SafeArrayGetUBound(psa, 1U);

            int n = uBound - lBound + 1; // uBound is inclusive

            object[] array = new object[n];
            for (int i = lBound; i <= uBound; ++i)
            {
                array[i] = PropVariantNativeMethods.SafeArrayGetElement(psa, ref i);
            }

            return array;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, calls the clear function.
        /// </summary>
        public void Dispose()
        {
            PropVariantNativeMethods.PropVariantClear(this);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PropVariant()
        {
            Dispose();
        }

        #endregion

        /// <summary>
        /// Provides an simple string representation of the contained data and type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0}: {1}", Value, VarType.ToString());
        }

    }

}
