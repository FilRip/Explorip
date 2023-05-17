using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{

    /// <summary>
    /// Factory class for creating typed ShellProperties.
    /// Generates/caches expressions to create generic ShellProperties.
    /// </summary>
    internal static class ShellPropertyFactory
    {
        // Constructor cache.  It takes object as the third param so a single function will suffice for both constructors.
        private static readonly Dictionary<int, Func<PropertyKey, ShellPropertyDescription, object, IShellProperty>> _storeCache
            = new();

        /// <summary>
        /// Creates a generic ShellProperty.
        /// </summary>
        /// <param name="propKey">PropertyKey</param>
        /// <param name="shellObject">Shell object from which to get property</param>
        /// <returns>ShellProperty matching type of value in property.</returns>
        public static IShellProperty CreateShellProperty(PropertyKey propKey, ShellObject shellObject)
        {
            return GenericCreateShellProperty(propKey, shellObject);
        }

        /// <summary>
        /// Creates a generic ShellProperty.
        /// </summary>
        /// <param name="propKey">PropertyKey</param>
        /// <param name="store">IPropertyStore from which to get property</param>
        /// <returns>ShellProperty matching type of value in property.</returns>
        public static IShellProperty CreateShellProperty(PropertyKey propKey, IPropertyStore store)
        {
            return GenericCreateShellProperty(propKey, store);
        }

        private static IShellProperty GenericCreateShellProperty<T>(PropertyKey propKey, T thirdArg)
        {
            Type thirdType = (thirdArg is ShellObject) ? typeof(ShellObject) : typeof(T);

            ShellPropertyDescription propDesc = ShellPropertyDescriptionsCache.Cache.GetPropertyDescription(propKey);

            // Get the generic type
            Type type = typeof(ShellProperty<>).MakeGenericType(VarEnumToSystemType(propDesc.VarEnumType));

            // The hash for the function is based off the generic type and which type (constructor) we're using.
            int hash = GetTypeHash(type, thirdType);

            if (!_storeCache.TryGetValue(hash, out Func<PropertyKey, ShellPropertyDescription, object, IShellProperty> ctor))
            {
                Type[] argTypes = { typeof(PropertyKey), typeof(ShellPropertyDescription), thirdType };
                ctor = ExpressConstructor(type, argTypes);
                _storeCache.Add(hash, ctor);
            }

            return ctor(propKey, propDesc, thirdArg);
        }

        /// <summary>
        /// Converts VarEnum to its associated .net Type.
        /// </summary>
        /// <param name="VarEnumType">VarEnum value</param>
        /// <returns>Associated .net equivelent.</returns>
        public static Type VarEnumToSystemType(VarEnum VarEnumType)
        {
            return VarEnumType switch
            {
                (VarEnum.VT_EMPTY) or (VarEnum.VT_NULL) => typeof(Object),
                (VarEnum.VT_UI1) => typeof(Byte?),
                (VarEnum.VT_I2) => typeof(Int16?),
                (VarEnum.VT_UI2) => typeof(UInt16?),
                (VarEnum.VT_I4) => typeof(Int32?),
                (VarEnum.VT_UI4) => typeof(UInt32?),
                (VarEnum.VT_I8) => typeof(Int64?),
                (VarEnum.VT_UI8) => typeof(UInt64?),
                (VarEnum.VT_R8) => typeof(Double?),
                (VarEnum.VT_BOOL) => typeof(Boolean?),
                (VarEnum.VT_FILETIME) => typeof(DateTime?),
                (VarEnum.VT_CLSID) => typeof(IntPtr?),
                (VarEnum.VT_CF) => typeof(IntPtr?),
                (VarEnum.VT_BLOB) => typeof(Byte[]),
                (VarEnum.VT_LPWSTR) => typeof(String),
                (VarEnum.VT_UNKNOWN) => typeof(IntPtr?),
                (VarEnum.VT_STREAM) => typeof(IStream),
#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
                (VarEnum.VT_VECTOR | VarEnum.VT_UI1) => typeof(Byte[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_I2) => typeof(Int16[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_UI2) => typeof(UInt16[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_I4) => typeof(Int32[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_UI4) => typeof(UInt32[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_I8) => typeof(Int64[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_UI8) => typeof(UInt64[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_R8) => typeof(Double[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_BOOL) => typeof(Boolean[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME) => typeof(DateTime[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_CLSID) => typeof(IntPtr[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_CF) => typeof(IntPtr[]),
                (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR) => typeof(String[]),
#pragma warning restore S3265 // Non-flags enums should not be used in bitwise operations
                _ => typeof(Object),
            };
        }

        #region Private static helper functions

        // Creates an expression for the specific constructor of the given type.
        private static Func<PropertyKey, ShellPropertyDescription, object, IShellProperty> ExpressConstructor(Type type, Type[] argTypes)
        {
            int typeHash = GetTypeHash(argTypes);

            // Finds the correct constructor by matching the hash of the types.
#pragma warning disable S3011
            ConstructorInfo ctorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(x => typeHash == GetTypeHash(x.GetParameters().Select(a => a.ParameterType))) ?? throw new ArgumentException(LocalizedMessages.ShellPropertyFactoryConstructorNotFound, "type");
            var key = Expression.Parameter(argTypes[0], "propKey");
            var desc = Expression.Parameter(argTypes[1], "desc");
            var third = Expression.Parameter(typeof(object), "third"); //needs to be object to avoid casting later

            var create = Expression.New(ctorInfo, key, desc,
                Expression.Convert(third, argTypes[2]));

            return Expression.Lambda<Func<PropertyKey, ShellPropertyDescription, object, IShellProperty>>(
                create, key, desc, third).Compile();
        }

        private static int GetTypeHash(params Type[] types)
        {
            return GetTypeHash((IEnumerable<Type>)types);
        }

        // Creates a hash code, unique to the number and order of types.
        private static int GetTypeHash(IEnumerable<Type> types)
        {
            int hash = 0;
            foreach (Type type in types)
            {
                hash = hash * 31 + type.GetHashCode();
            }
            return hash;
        }

        #endregion
    }
}
