using System;
using System.ComponentModel;

namespace DeltaCompressionDotNet.MsDelta
{
    public sealed class MsDeltaCompression : IDeltaCompression
    {
        public void CreateDelta(string oldFilePath, string newFilePath, string deltaFilePath)
        {
            if (!NativeMethods.CreateDelta(fileTypeSet: FileTypeSet.Executables,
                                           setFlags: 0,
                                           resetFlags: 0,
                                           sourceName: oldFilePath,
                                           targetName: newFilePath,
                                           sourceOptionsName: null,
                                           targetOptionsName: null,
                                           globalOptions: new DeltaInput(),
                                           targetFileTime: IntPtr.Zero,
                                           hashAlgId: HashAlgId.Crc32,
                                           deltaName: deltaFilePath))
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