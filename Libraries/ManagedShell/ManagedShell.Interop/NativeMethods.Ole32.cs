using System;
using System.Runtime.InteropServices;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.Interop;

public partial class NativeMethods
{
    private const string Ole32_DllName = "ole32.dll";

    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    /// </summary>
    /// <remarks>
    /// Must call Clear when finished to avoid memory leaks. If you get the value of
    /// a VT_UNKNOWN prop, an implicit AddRef is called, thus your reference will
    /// be active even after the PropVariant struct is cleared.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct PropVariant
    {
        #region struct fields

        // The layout of these elements needs to be maintained.
        //
        // NOTE: We could use LayoutKind.Explicit, but we want
        //       to maintain that the IntPtr may be 8 bytes on
        //       64-bit architectures, so we’ll let the CLR keep
        //       us aligned.
        //
        // NOTE: In order to allow x64 compat, we need to allow for
        //       expansion of the IntPtr. However, the BLOB struct
        //       uses a 4-byte int, followed by an IntPtr, so
        //       although the p field catches most pointer values,
        //       we need an additional 4-bytes to get the BLOB
        //       pointer. The p2 field provides this, as well as
        //       the last 4-bytes of an 8-byte value on 32-bit
        //       architectures.

        // This is actually a VarEnum value, but the VarEnum type
        // shifts the layout of the struct by 4 bytes instead of the
        // expected 2.
        ushort vt;
        ushort wReserved1;
        ushort wReserved2;
        ushort wReserved3;
        IntPtr p;
        int p2;

        #endregion // struct fields

        #region union members

#pragma warning disable IDE1006
        sbyte cVal
        {
            get { return (sbyte)GetDataBytes()[0]; }
        }

        byte bVal
        {
            get { return GetDataBytes()[0]; }
        }

        short iVal
        {
            get { return BitConverter.ToInt16(GetDataBytes(), 0); }
        }

        ushort uiVal
        {
            get { return BitConverter.ToUInt16(GetDataBytes(), 0); }
        }

        int lVal
        {
            get { return BitConverter.ToInt32(GetDataBytes(), 0); }
        }

        uint ulVal
        {
            get { return BitConverter.ToUInt32(GetDataBytes(), 0); }
        }

        long hVal
        {
            get { return BitConverter.ToInt64(GetDataBytes(), 0); }
        }

        ulong uhVal
        {
            get { return BitConverter.ToUInt64(GetDataBytes(), 0); }
        }

        float fltVal
        {
            get { return BitConverter.ToSingle(GetDataBytes(), 0); }
        }

        double dblVal
        {
            get { return BitConverter.ToDouble(GetDataBytes(), 0); }
        }

        bool boolVal
        {
            get { return (iVal != 0); }
        }

        int scode
        {
            get { return lVal; }
        }

        decimal cyVal
        {
            get { return decimal.FromOACurrency(hVal); }
        }

        DateTime date
        {
            get { return DateTime.FromOADate(dblVal); }
        }
#pragma warning restore IDE1006

        #endregion // union members

        /// <summary>
        /// Gets a byte array containing the data bits of the struct.
        /// </summary>
        /// <returns>A byte array that is the combined size of the data bits.</returns>
        private byte[] GetDataBytes()
        {
            byte[] ret = new byte[IntPtr.Size + sizeof(int)];
            if (IntPtr.Size == 4)
                BitConverter.GetBytes(p.ToInt32()).CopyTo(ret, 0);
            else if (IntPtr.Size == 8)
                BitConverter.GetBytes(p.ToInt64()).CopyTo(ret, 0);
            BitConverter.GetBytes(p2).CopyTo(ret, IntPtr.Size);
            return ret;
        }

        /// <summary>
        /// Called to properly clean up the memory referenced by a PropVariant instance.
        /// </summary>
        [DllImport(Ole32_DllName)]
        private extern static int PropVariantClear(ref PropVariant pvar);

        /// <summary>
        /// Called to clear the PropVariant’s referenced and local memory.
        /// </summary>
        /// <remarks>
        /// You must call Clear to avoid memory leaks.
        /// </remarks>
        public void Clear()
        {
            // Can’t pass “this” by ref, so make a copy to call PropVariantClear with
            PropVariant var = this;
            PropVariantClear(ref var);

            // Since we couldn’t pass “this” by ref, we need to clear the member fields manually
            // NOTE: PropVariantClear already freed heap data for us, so we are just setting
            //       our references to null.
            vt = (ushort)VarEnum.VT_EMPTY;
            wReserved1 = wReserved2 = wReserved3 = 0;
            p = IntPtr.Zero;
            p2 = 0;
        }

        /// <summary>
        /// Gets the variant type.
        /// </summary>
#pragma warning disable IDE0251 // Définir comme membre 'readonly'
        public VarEnum Type
        {
            get { return (VarEnum)vt; }
        }
#pragma warning restore IDE0251 // Définir comme membre 'readonly'

        /// <summary>
        /// Gets the variant value.
        /// </summary>
        public object Value
        {
            get
            {
                // TODO: Add support for reference types (ie. VT_REF | VT_I1)
                // TODO: Add support for safe arrays

                switch ((VarEnum)vt)
                {
                    case VarEnum.VT_I1:
                        return cVal;
                    case VarEnum.VT_UI1:
                        return bVal;
                    case VarEnum.VT_I2:
                        return iVal;
                    case VarEnum.VT_UI2:
                        return uiVal;
                    case VarEnum.VT_I4:
                    case VarEnum.VT_INT:
                        return lVal;
                    case VarEnum.VT_UI4:
                    case VarEnum.VT_UINT:
                        return ulVal;
                    case VarEnum.VT_I8:
                        return hVal;
                    case VarEnum.VT_UI8:
                        return uhVal;
                    case VarEnum.VT_R4:
                        return fltVal;
                    case VarEnum.VT_R8:
                        return dblVal;
                    case VarEnum.VT_BOOL:
                        return boolVal;
                    case VarEnum.VT_ERROR:
                        return scode;
                    case VarEnum.VT_CY:
                        return cyVal;
                    case VarEnum.VT_DATE:
                        return date;
                    case VarEnum.VT_FILETIME:
                        return DateTime.FromFileTime(hVal);
                    case VarEnum.VT_BSTR:
                        return Marshal.PtrToStringBSTR(p);
                    case VarEnum.VT_BLOB:
                        byte[] blobData = new byte[lVal];
                        IntPtr pBlobData;
                        if (IntPtr.Size == 4)
                        {
                            pBlobData = new IntPtr(p2);
                        }
                        else if (IntPtr.Size == 8)
                        {
                            // In this case, we need to derive a pointer at offset 12,
                            // because the size of the blob is represented as a 4-byte int
                            // but the pointer is immediately after that.
                            pBlobData = new IntPtr(BitConverter.ToInt64(GetDataBytes(), sizeof(int)));
                        }
                        else
                            throw new NotSupportedException();
                        Marshal.Copy(pBlobData, blobData, 0, lVal);
                        return blobData;
                    case VarEnum.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(p);
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(p);
                    case VarEnum.VT_UNKNOWN:
                        return Marshal.GetObjectForIUnknown(p);
                    case VarEnum.VT_DISPATCH:
                        return p;
                    default:
                        return "";
                }
            }
        }
    }

    [Flags()]
    public enum RegistryClasses
    {
        /// <summary>
        /// After an application is connected to a class object with CoGetClassObject, the class object is removed from public view so
        /// that no other applications can connect to it. This value is commonly used for single document interface (SDI) applications.
        /// Specifying this value does not affect the responsibility of the object application to call CoRevokeClassObject; it must
        /// always call CoRevokeClassObject when it is finished with an object class.
        /// </summary>
        REGCLS_SINGLEUSE = 0,

        /// <summary>
        /// Multiple applications can connect to the class object through calls to CoGetClassObject. If both the REGCLS_MULTIPLEUSE and
        /// CLSCTX_LOCAL_SERVER are set in a call to CoRegisterClassObject, the class object is also automatically registered as an
        /// in-process server, whether CLSCTX_INPROC_SERVER is explicitly set.
        /// </summary>
        REGCLS_MULTIPLEUSE = 1,

        /// <summary>
        /// Useful for registering separate CLSCTX_LOCAL_SERVER and CLSCTX_INPROC_SERVER class factories through calls to
        /// CoGetClassObject. If REGCLS_MULTI_SEPARATE is set, each execution context must be set separately; CoRegisterClassObject does
        /// not automatically register an out-of-process server (for which CLSCTX_LOCAL_SERVER is set) as an in-process server. This
        /// allows the EXE to create multiple instances of the object for in-process needs, such as self embeddings, without disturbing
        /// its CLSCTX_LOCAL_SERVER registration. If an EXE registers a REGCLS_MULTI_SEPARATE class factory and a CLSCTX_INPROC_SERVER
        /// class factory, instance creation calls that specify CLSCTX_INPROC_SERVER in the CLSCTX parameter executed by the EXE would
        /// be satisfied locally without approaching the SCM. This mechanism is useful when the EXE uses functions such as OleCreate and
        /// OleLoad to create embeddings, but at the same does not wish to launch a new instance of itself for the self-embedding case.
        /// The distinction is important for embeddings because the default handler aggregates the proxy manager by default and the
        /// application should override this default behavior by calling OleCreateEmbeddingHelper for the self-embedding case. If your
        /// application need not distinguish between the local and inproc case, you need not register your class factory using
        /// REGCLS_MULTI_SEPARATE. In fact, the application incurs an extra network round trip to the SCM when it registers its
        /// MULTIPLEUSE class factory as MULTI_SEPARATE and does not register another class factory as INPROC_SERVER.
        /// </summary>
        REGCLS_MULTI_SEPARATE = 2,

        /// <summary>
        /// Suspends registration and activation requests for the specified CLSID until there is a call to CoResumeClassObjects. This is
        /// used typically to register the CLSIDs for servers that can register multiple class objects to reduce the overall
        /// registration time, and thus the server application startup time, by making a single call to the SCM, no matter how many
        /// CLSIDs are registered for the server.
        /// </summary>
        REGCLS_SUSPENDED = 4,

        /// <summary>
        /// The class object is a surrogate process used to run DLL servers. The class factory registered by the surrogate process is
        /// not the actual class factory implemented by the DLL server, but a generic class factory implemented by the surrogate. This
        /// generic class factory delegates instance creation and marshaling to the class factory of the DLL server running in the
        /// surrogate. For further information on DLL surrogates, see the DllSurrogate registry value.
        /// </summary>
        REGCLS_SURROGATE = 8,

        /// <summary>
        /// The class object aggregates the free-threaded marshaler and will be made visible to all inproc apartments. Can be used
        /// together with other flags. For example, REGCLS_AGILE | REGCLS_MULTIPLEUSE to register a class object that can be used
        /// multiple times from different apartments. Without other flags, behavior will retain REGCLS_SINGLEUSE semantics in that only
        /// one instance can be generated.
        /// </summary>
        REGCLS_AGILE = 0x10,
    }

    [Flags()]
    public enum ClassesContexts : uint
    {
        INPROC_SERVER = 0x1,
        INPROC_HANDLER = 0x2,
        LOCAL_SERVER = 0x4,
        INPROC_SERVER16 = 0x8,
        REMOTE_SERVER = 0x10,
        INPROC_HANDLER16 = 0x20,
        RESERVED1 = 0x40,
        RESERVED2 = 0x80,
        RESERVED3 = 0x100,
        RESERVED4 = 0x200,
        NO_CODE_DOWNLOAD = 0x400,
        RESERVED5 = 0x800,
        NO_CUSTOM_MARSHAL = 0x1000,
        ENABLE_CODE_DOWNLOAD = 0x2000,
        NO_FAILURE_LOG = 0x4000,
        DISABLE_AAA = 0x8000,
        ENABLE_AAA = 0x10000,
        FROM_DEFAULT_CONTEXT = 0x20000,
        CLSCTX_ACTIVATE_X86_SERVER = 0x40000,
        CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
        CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
        CLSCTX_ENABLE_CLOAKING = 0x100000,
        CLSCTX_APPCONTAINER = 0x400000,
        CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,
        CLSCTX_RESERVED6 = 0x1000000,
        CLSCTX_ACTIVATE_ARM32_SERVER = 0x2000000,
        CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION = 0x4000000,
        CLSCTX_PS_DLL = 0x80000000,
        INPROC = INPROC_SERVER | INPROC_HANDLER,
        SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
        ALL = SERVER | INPROC_HANDLER
    }

    [DllImport(Ole32_DllName)]
    internal static extern int CoRegisterClassObject(ref Guid rclsid, [MarshalAs(UnmanagedType.Interface)] object pUnk, ClassesContexts dwClsContext, RegistryClasses flags, out uint lpdwRegister);

    [DllImport(Ole32_DllName, SetLastError = false)]
    internal static extern int CoRevokeClassObject(uint dwRegister);
}
