using System;
using System.Threading;
using GZipTest.Compress;
using GZipTest.Helpers;

namespace GZipTest
{
    internal class App
    {
        private readonly ICompressor _compressor;
        private readonly IDecompressor _decompressor;
        private readonly IFileHelper _fileHelper;

        public App(ICompressor compressor, IDecompressor decompressor, IFileHelper fileHelper)
        {
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
            _decompressor = decompressor ?? throw new ArgumentNullException(nameof(decompressor));
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
        }

        public void Compress(string originalFileName, string archiveFileName)
        {
            var originalFileSize = _fileHelper.GetFileSize(originalFileName);
            _fileHelper.CreateEmptyFile(archiveFileName);

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var thread = new Thread(() => _compressor.Compress(originalFileName, originalFileSize, archiveFileName));
                try
                {
                    thread.Start();
                }
                catch
                {
                    Console.WriteLine("Thread can not be created");
                    throw;
                }
            }
        }

        public void Decompress(string archiveFileName, string targetFileName)
        {
            _decompressor.Decompress(archiveFileName, targetFileName);
        }
    }
}
