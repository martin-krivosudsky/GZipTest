using System;
using System.IO;
using System.IO.Compression;
using GZipTest.Helpers;

namespace GZipTest.Compress
{
    internal class Decompressor : IDecompressor
    {
        private readonly IFileHelper _fileHelper;

        public Decompressor(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
        }

        public void Decompress(string archiveFileName, string targetFileName)
        {
            var bytes = _fileHelper.ReadAllBytes(archiveFileName);
            using var compressedStream = new MemoryStream(bytes);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            var resultStream = new MemoryStream();
            zipStream.CopyTo(resultStream);

            _fileHelper.WriteAllBytes(targetFileName, resultStream.ToArray());
        }
    }
}
