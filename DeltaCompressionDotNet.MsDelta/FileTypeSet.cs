using System;

namespace DeltaCompressionDotNet.MsDelta
{
    /// <remarks>
    ///     http://msdn.microsoft.com/en-us/library/bb417345.aspx#filetypesets
    /// </remarks>
    [Flags]
    internal enum FileTypeSet : long
    {
        Executables = 0x0FL
    }
}