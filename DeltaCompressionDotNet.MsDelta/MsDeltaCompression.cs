using System;
using System.ComponentModel;

namespace DeltaCompressionDotNet.MsDelta
{
    public sealed class MsDeltaCompression : IDeltaCompression
    {
        public void CreateDelta(string oldFilePath, string newFilePath, string deltaFilePath)
        {
            const int setFlags = 0;
            const int resetFlags = 0;
            const string sourceOptionsName = null;
            const string targetOptionsName = null;
            var globalOptions = new DeltaInput();
            var targetFileTime = IntPtr.Zero;

            if (!NativeMethods.CreateDelta(FileTypeSet.Executables, setFlags, resetFlags, oldFilePath, newFilePath,
                sourceOptionsName, targetOptionsName, globalOptions, targetFileTime, HashAlgId.Crc32, deltaFilePath))
            {
                throw new Win32Exception();
            }
        }

        public void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath)
        {
            if (!NativeMethods.ApplyDelta(ApplyFlags.AllowLegacy, oldFilePath, deltaFilePath, newFilePath))
                throw new Win32Exception();
        }
    }
}