using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeltaCompressionDotNet.Tests
{
    public class CompressionTests<TDeltaCompression>
        where TDeltaCompression : IDeltaCompression, new()
    {       
        private const string CalcFileName = "Calc.exe";
        private const string NotepadFileName = "Notepad.exe";
        private const string MediaFolderName = "Media";
        private const string Alarm01FileName = "Alarm01.wav";
        private const string Alarm02FileName = "Alarm02.wav";

        private readonly List<CompressionTest> _compressionTests = new List<CompressionTest>();

        [TestInitialize]
        public void TestInitialize()
        {
            _compressionTests.Clear();

            var systemFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

            _compressionTests.Add(new CompressionTest(systemFolderPath, CalcFileName, NotepadFileName));

            var windowsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var mediaFolderPath = Path.Combine(windowsFolderPath, MediaFolderName);

            _compressionTests.Add(new CompressionTest(mediaFolderPath, Alarm01FileName, Alarm02FileName));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (var compressionTest in _compressionTests)
                compressionTest.TestCleanup();
        }

        [ExpectedException(typeof(Win32Exception)), TestMethod]
        public void ApplyDelta_Calls_GetLastError()
        {
            var deltaCompression = new TDeltaCompression();

            try
            {
                deltaCompression.ApplyDelta(null, null, null);
            }
            catch (Win32Exception exception)
            {
                Assert.AreNotEqual(0, exception.NativeErrorCode);

                throw;
            }
        }

        [ExpectedException(typeof(Win32Exception)), TestMethod]
        public void CreateDelta_Calls_GetLastError()
        {
            var deltaCompression = new TDeltaCompression();

            try
            {
                deltaCompression.CreateDelta(null, null, null);
            }
            catch (Win32Exception exception)
            {
                Assert.AreNotEqual(0, exception.NativeErrorCode);

                throw;
            }
        }

        [TestMethod]
        public void CreateDelta_And_ApplyDelta_Creates_And_Applies_Delta()
        {
            var deltaCompression = new TDeltaCompression();

            foreach (var compressionTest in _compressionTests)
                compressionTest.CreateAndApplyDelta(deltaCompression);
        }
    }

    internal sealed class CompressionTest
    {
        public string PrivateFile1Path { get; }

        public string PrivateFile2Path { get; }

        public string PrivateDeltaPath { get; }

        public string PrivateFinalPath { get; }

        private byte[] ExpectedHash { get; }

        public CompressionTest(string baseFolderPath, string file1FileName, string file2FileName)
        {
            PrivateFile1Path = Path.GetTempFileName();
            PrivateFile2Path = Path.GetTempFileName();
            PrivateDeltaPath = Path.GetTempFileName();
            PrivateFinalPath = Path.GetTempFileName();

            var file1Path = Path.Combine(baseFolderPath, file1FileName);
            var file2Path = Path.Combine(baseFolderPath, file2FileName);

            CopyFile(file1Path, PrivateFile1Path);
            CopyFile(file2Path, PrivateFile2Path);

            ExpectedHash = HashFile(file2Path);
        }

        public void CreateAndApplyDelta(IDeltaCompression deltaCompression)
        {
            deltaCompression.CreateDelta(PrivateFile1Path, PrivateFile2Path, PrivateDeltaPath);
            deltaCompression.ApplyDelta(PrivateDeltaPath, PrivateFile1Path, PrivateFinalPath);

            var actualHash = HashFile(PrivateFinalPath);

            CollectionAssert.AreEqual(ExpectedHash, actualHash);
        }

        public void TestCleanup()
        {
            TryDeleteFile(PrivateFile1Path);
            TryDeleteFile(PrivateFile2Path);
            TryDeleteFile(PrivateDeltaPath);
            TryDeleteFile(PrivateFinalPath);
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