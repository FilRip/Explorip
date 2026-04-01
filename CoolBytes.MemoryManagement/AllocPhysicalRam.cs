using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ManagedShell.Interop;

namespace CoolBytes.MemoryManagement;

public sealed unsafe class AllocPhysicalRam : MemoryManager<byte>
{
    private IntPtr _pointer;
    private readonly int _length;
    private bool _disposableValue;

    public AllocPhysicalRam(int requestedSize)
    {
        UIntPtr requestSize = (UIntPtr)requestedSize;
        NativeMethods.AllocationTypes allocType = NativeMethods.AllocationTypes.Commit | NativeMethods.AllocationTypes.Reserve;

        uint largePageSize = NativeMethods.GetLargePageMinimum();
        if (largePageSize != 0)
        {
            long aligned = (requestedSize + largePageSize - 1) / largePageSize * largePageSize;

            if (aligned > int.MaxValue)
                throw new Win32Exception("RequestedSize too big");
            requestSize = (UIntPtr)aligned;
            allocType |= NativeMethods.AllocationTypes.LargePages;
        }

        _pointer = NativeMethods.VirtualAlloc(
            IntPtr.Zero,
            requestSize,
            allocType,
            NativeMethods.MemoryProtections.ReadWrite);

        if (_pointer == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        if (!NativeMethods.VirtualLock(_pointer, requestSize))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        _length = (int)requestSize;
    }

    public override Span<byte> GetSpan()
    {
        if (_disposableValue)
            throw new ObjectDisposedException(nameof(AllocPhysicalRam));

        return new Span<byte>(_pointer.ToPointer(), _length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if (_disposableValue)
            throw new ObjectDisposedException(nameof(AllocPhysicalRam));
        if ((uint)elementIndex > (uint)_length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));

        byte* ptr = (byte*)_pointer + elementIndex;
        return new MemoryHandle(ptr);
    }

    public override void Unpin() { /* Nothing to do here */ }

    protected override void Dispose(bool disposing)
    {
        if (!_disposableValue)
        {
            if (_pointer != IntPtr.Zero)
            {
                NativeMethods.VirtualUnlock(_pointer, (UIntPtr)_length);
                NativeMethods.VirtualFree(_pointer, UIntPtr.Zero, NativeMethods.AllocationTypes.Release);
                _pointer = IntPtr.Zero;
            }

            _disposableValue = true;
        }
    }
}
