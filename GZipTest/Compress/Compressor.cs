using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using GZipTest.Helpers;

namespace GZipTest.Compress
{
    internal class Compressor : ICompressor
    {
        private readonly IFileHelper _fileHelper;

        private readonly object _lock = new object();
        private long _currentReadPosition;
        private readonly ConcurrentDictionary<long, byte[]> _chunks;
        private long _chunkIndex;
        private long _expectedChunkIndex;

        public Compressor(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));

            _chunks = new ConcurrentDictionary<long, byte[]>();
            _currentReadPosition = 0;
            _chunkIndex = 0;
            _expectedChunkIndex = 0;
        }

        public void Compress(string originalFileName, long originalFileSize, string archiveFileName)
        {
            while (_chunkIndex >= 0)
            {
                var chunkIndex = ReadChunk(originalFileName, originalFileSize);
                if (chunkIndex >= 0)
                {
                    var compressedBytes = CompressChunk(chunkIndex);
                    WriteCompressedChunk(compressedBytes, chunkIndex, archiveFileName);
                }
            }
        }

        private void WriteCompressedChunk(byte[] compressedBytes,long chunkIndex, string archiveFileName)
        {
            while (_expectedChunkIndex != chunkIndex)
            {
            }

            _fileHelper.AppendToFile(archiveFileName, compressedBytes);

            _expectedChunkIndex++;
        }

        private byte[] CompressChunk(long chunkIndex)
        {
            using var memoryStream = new MemoryStream();
            using var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal);

            if (_chunks.TryGetValue(chunkIndex, out var bytes))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
                gzipStream.Close();
            }

            return memoryStream.ToArray();
        }

        private long ReadChunk(string originalFileName, long originalFileSize)
        {
            long readPosition;
            long chunkIndex;

            lock (_lock)
            {
                readPosition = _currentReadPosition;
                chunkIndex = _chunkIndex;

                _currentReadPosition += Constants.ChunkSize;

                if (readPosition > originalFileSize)
                {
                    _chunkIndex = -1;
                    return _chunkIndex;
                }

                _chunkIndex++;
            }

            var readSize = readPosition + Constants.ChunkSize > originalFileSize
                ? originalFileSize - readPosition
                : Constants.ChunkSize;


            var bytes = _fileHelper.ReadBytes(originalFileName, readPosition, (int)readSize);

            _chunks.TryAdd(chunkIndex, bytes);

            return chunkIndex;
        }
    }
}
