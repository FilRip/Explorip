using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    public enum DATADIR
    {
        DATADIR_GET = 1,
        DATADIR_SET = 2
    }

    [ComImport(),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("0000010E-0000-0000-C000-000000000046")]
    public interface IDataObject
    {
        void GetData([In()] ref FormatEtc format, out StgMedium medium);
        void GetDataHere([In()] ref FormatEtc format, ref StgMedium medium);
        [PreserveSig()]
        int QueryGetData([In()] ref FormatEtc format);
        [PreserveSig()]
        int GetCanonicalFormatEtc([In()] ref FormatEtc formatIn, out FormatEtc formatOut);
        void SetData([In()] ref FormatEtc formatIn, [In()] ref StgMedium medium, [MarshalAs(UnmanagedType.Bool)] bool release);
        IEnumFORMATETC EnumFormatEtc(DATADIR direction);
        [PreserveSig()]
        int DAdvise([In()] ref FormatEtc pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);
        void DUnadvise(int connection);
        [PreserveSig()]
        int EnumDAdvise(out IEnumSTATDATA enumAdvise);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid("0000010F-0000-0000-C000-000000000046")]
    [ComImport()]
    public interface IAdviseSink
    {
        void OnDataChange(FormatEtc pFormatetc, StgMedium pStgmed);
        void OnViewChange([MarshalAs(UnmanagedType.U4)] int dwAspect, [MarshalAs(UnmanagedType.I4)] int lindex);
        void OnRename([MarshalAs(UnmanagedType.Interface)] object pmk);
        void OnSave();
        void OnClose();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StgMedium
    {
        [MarshalAs(UnmanagedType.U4)]
        public int tymed;
        public IntPtr data;
        [MarshalAs(UnmanagedType.IUnknown)]
        public object pUnkForRelease;
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000103-0000-0000-C000-000000000046")]
    public interface IEnumFORMATETC
    {
        [PreserveSig()]
        int Next(int celt, [Out(), MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FormatEtc[] rgelt, [Out(), MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);
        [PreserveSig()]
        int Skip(int celt);
        [PreserveSig()]
        int Reset();
        void Clone(out IEnumFORMATETC newEnum);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FormatEtc
    {
        public short cfFormat;
        public IntPtr ptd;
        [MarshalAs(UnmanagedType.U4)]
        public DVASPECT dwAspect;
        public int lindex;
        [MarshalAs(UnmanagedType.U4)]
        public TYMED tymed;
    };

    ///// <summary>
    ///// The DVASPECT enumeration values specify the desired data or view aspect of the object when drawing or getting data.
    ///// </summary>
    [Flags()]
    public enum DVASPECT
    {
        DVASPECT_CONTENT = 1,
        DVASPECT_THUMBNAIL = 2,
        DVASPECT_ICON = 4,
        DVASPECT_DOCPRINT = 8
    }

    // Summary:
    //     Provides the managed definition of the TYMED structure.
#pragma warning disable S2346 // Flags enumerations zero-value members should be named "None"
    [Flags()]
    public enum TYMED
    {
        // Summary:
        //     No data is being passed.
        TYMED_NULL = 0,
        //
        // Summary:
        //     The storage medium is a global memory handle (HGLOBAL). Allocate the global
        //     handle with the GMEM_SHARE flag. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is null, the destination process should use GlobalFree to release
        //     the memory.
        TYMED_HGLOBAL = 1,
        //
        // Summary:
        //     The storage medium is a disk file identified by a path. If the STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is null, the destination process should use OpenFile to delete the
        //     file.
        TYMED_FILE = 2,
        //
        // Summary:
        //     The storage medium is a stream object identified by an IStream pointer. Use
        //     ISequentialStream::Read to read the data. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is not null, the destination process should use IStream::Release to
        //     release the stream component.
        TYMED_ISTREAM = 4,
        //
        // Summary:
        //     The storage medium is a storage component identified by an IStorage pointer.
        //     The data is in the streams and storages contained by this IStorage instance.
        //     If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is not null, the destination process should use IStorage::Release
        //     to release the storage component.
        TYMED_ISTORAGE = 8,
        //
        // Summary:
        //     The storage medium is a Graphics Device Interface (GDI) component (HBITMAP).
        //     If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is null, the destination process should use DeleteObject to delete
        //     the bitmap.
        TYMED_GDI = 16,
        //
        // Summary:
        //     The storage medium is a metafile (HMETAFILE). Use the Windows or WIN32 functions
        //     to access the metafile's data. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is null, the destination process should use DeleteMetaFile to delete
        //     the bitmap.
        TYMED_MFPICT = 32,
        //
        // Summary:
        //     The storage medium is an enhanced metafile. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
        //     member is null, the destination process should use DeleteEnhMetaFile to delete
        //     the bitmap.
        TYMED_ENHMF = 64,
    }
#pragma warning restore S2346 // Flags enumerations zero-value members should be named "None"
}
