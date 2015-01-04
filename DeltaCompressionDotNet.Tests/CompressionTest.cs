using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeltaCompressionDotNet.Tests
{
    internal sealed class CompressionTest
    {
        private readonly string _privateFile1Path;

        private readonly string _privateFile2Path;

        private readonly string _privateDeltaPath;

        private readonly string _privateFinalPath;

        private readonly byte[] _expectedHash;

        public CompressionTest(string baseFolderPath, string file1FileName, string file2FileName)
        {
            _privateFile1Path = Path.GetTempFileName();
            _privateFile2Path = Path.GetTempFileName();
            _privateDeltaPath = Path.GetTempFileName();
            _privateFinalPath = Path.GetTempFileName();

            var file1Path = Path.Combine(baseFolderPath, file1FileName);
            var file2Path = Path.Combine(baseFolderPath, file2FileName);

            CopyFile(file1Path, _privateFile1Path);
            CopyFile(file2Path, _privateFile2Path);

            _expectedHash = HashFile(file2Path);
        }

        public void CreateAndApplyDelta(IDeltaCompression deltaCompression)
        {
            deltaCompression.CreateDelta(_privateFile1Path, _privateFile2Path, _privateDeltaPath);
            deltaCompression.ApplyDelta(_privateDeltaPath, _privateFile1Path, _privateFinalPath);

            var actualHash = HashFile(_privateFinalPath);

            CollectionAssert.AreEqual(_expectedHash, actualHash);
        }

        public void TestCleanup()
        {
            TryDeleteFile(_privateFile1Path);
            TryDeleteFile(_privateFile2Path);
            TryDeleteFile(_privateDeltaPath);
            TryDeleteFile(_privateFinalPath);
        }

        private static void CopyFile(string sourceFileName, string destinationFileName)
        {
            File.Copy(sourceFileName, destinationFileName, true);
        }

        private static byte[] HashFile(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var hashAlgorithm = SHA1.Create())
            {
                return hashAlgorithm.ComputeHash(fileStream);
            }
        }

        [ExcludeFromCodeCoverage]
        private static void TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
            }
        }
    }
}