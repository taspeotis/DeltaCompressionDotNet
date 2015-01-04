using System;
using System.IO;
using System.Security.Cryptography;

namespace DeltaCompressionDotNet.Tests
{
    internal sealed class RandomFile : IDisposable
    {
        public string FileName { get; private set; }

        private string FilePath { get; set; }

        public RandomFile(string baseFolderPath)
        {
            FileName = Path.GetRandomFileName();
            FilePath = Path.Combine(baseFolderPath, FileName);

            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                using (var randomNumberGenerator = new RNGCryptoServiceProvider())
                {
                    var randomData = new byte[4096];

                    // 48 MB => Runs in a manageable amount of time yet still tests tests that we
                    // can create deltas for files whose size exceed DELTA_FILE_SIZE_LIMIT (32 MB)
                    for (var x = 0; x < 48*1024*1024; x += randomData.Length)
                    {
                        randomNumberGenerator.GetBytes(randomData);
                        fileStream.Write(randomData, 0, randomData.Length);
                    }
                }
            }
            catch (Exception)
            {
                Dispose();

                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                File.Delete(FilePath);
            }
            catch (IOException)
            {
            }
        }
    }
}