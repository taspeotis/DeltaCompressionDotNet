using System;
using System.Runtime.InteropServices;

namespace DeltaCompressionDotNet.MsDelta
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DeltaInput
    {
        public IntPtr Start;

        public IntPtr Size;

        [MarshalAs(UnmanagedType.Bool)] public bool Editable;
    }
}