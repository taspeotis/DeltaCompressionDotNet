using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeltaCompressionDotNet.PatchApi.Tests
{
    [TestClass]
    public class PatchApiCompressionTests
    {
        private const string NotepadPath = @"C:\Windows\System32\Notepad.exe";
        private const string CalcPath = @"C:\Windows\System32\Calc.exe";

        private readonly string _privateCalcPath = Path.GetTempFileName();
        private readonly string _privateDeltaPath = Path.GetTempFileName();
        private readonly string _privateFinalPath = Path.GetTempFileName();
        private readonly string _privateNotepadPath = Path.GetTempFileName();
        private byte[] _calcHash;

        [TestInitialize]
        public void TestInitialize()
        {
            CopyFile(CalcPath, _privateCalcPath);
            CopyFile(NotepadPath, _privateNotepadPath);

            // Hash Calc.exe so we can be sure that the applied patch turns Notepad.exe into Calc.exe
            _calcHash = HashFile(_privateCalcPath);
        }

        private static void CopyFile(string sourceFileName, string destinationFileName)
        {
            File.Copy(sourceFileName, destinationFileName, true);
        }

        private static byte[] HashFile(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var hashAlgorithm = SHA1.Create())
            {
                return hashAlgorithm.ComputeHash(fileStream);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TryDeleteFile(_privateCalcPath);
            TryDeleteFile(_privateDeltaPath);
            TryDeleteFile(_privateFinalPath);
            TryDeleteFile(_privateNotepadPath);
        }

        private static void TryDeleteFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
            }
        }

        [TestMethod]
        public void CreateAndApplyDelta()
        {
            var compression = new PatchApiCompression();

            compression.CreateDelta(_privateNotepadPath, _privateCalcPath, _privateDeltaPath);
            compression.ApplyDelta(_privateDeltaPath, _privateNotepadPath, _privateFinalPath);

            var finalHash = HashFile(_privateFinalPath);
            CollectionAssert.AreEqual(_calcHash, finalHash);
        }
    }
}