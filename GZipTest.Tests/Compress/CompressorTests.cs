using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipTest.Compress;
using GZipTest.Helpers;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests.Compress
{
    [TestFixture]
    public class CompressorTests
    {
        private ICompressor _compressor;
        private Mock<IFileHelper> _fileHelperMock;

        private byte[] _bytesToCompress;
        private byte[] _expectedCompressedBytes;
        private byte[] _gZipHeader;

        [SetUp]
        public void Setup()
        {
            _fileHelperMock = new Mock<IFileHelper>();

            _bytesToCompress = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            _gZipHeader = new byte[] {31, 139, 8, 0, 0, 0, 0, 0, 0, 10};
            _expectedCompressedBytes = new byte[] { 99, 96, 100, 98, 102, 97, 101, 99, 7, 0, 159, 104, 170, 136, 8, 0, 0, 0 };
        }

        [Test]
        public void CompressBytesOnSingleThread()
        {
            _compressor = new Compressor(_fileHelperMock.Object, 8);

            const string originalFileName = "someFile.txt";
            long originalFileSize = _bytesToCompress.Length;
            const string archiveFileName = "archive.gz";

            _fileHelperMock.Setup(fh => fh.ReadBytes(originalFileName, 0, (int) originalFileSize)).Returns(_bytesToCompress);

            _compressor.Compress(originalFileName, originalFileSize, archiveFileName);

            var expectedBytes = Concat(_gZipHeader, _expectedCompressedBytes);
            _fileHelperMock.Verify(fh => fh.AppendToFile(archiveFileName, expectedBytes));
        }

        [Test]
        public void CompressBytesOnTwoThreads()
        {
            _compressor = new Compressor(_fileHelperMock.Object, 5);

            const string originalFileName = "someFile.txt";
            long originalFileSize = _bytesToCompress.Length;
            const string archiveFileName = "archive.gz";

            _fileHelperMock.Setup(fh => fh.ReadBytes(originalFileName, 0, (int)originalFileSize)).Returns(_bytesToCompress.Take(5).ToArray);
            _fileHelperMock.Setup(fh => fh.ReadBytes(originalFileName, 5, (int)originalFileSize)).Returns(_bytesToCompress.TakeLast(3).ToArray);

            for (int i = 0; i < 2; i++)
            {
                var thread = new Thread(() => _compressor.Compress(originalFileName, originalFileSize, archiveFileName));
                thread.Start();
            }

            var expectedResult1 = Concat(_gZipHeader, new byte[] {3, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            var expectedResult2 = Concat(_gZipHeader, new byte[] {3, 0, 0, 0, 0, 0, 0, 0, 0, 0});

            _fileHelperMock.Verify(fh => fh.AppendToFile(archiveFileName, expectedResult1));
            _fileHelperMock.Verify(fh => fh.AppendToFile(archiveFileName, expectedResult2));
        }

        private static byte[] Concat(IEnumerable<byte> x, IEnumerable<byte> y)
        {
            var list = new List<byte>();
            list.AddRange(x);
            list.AddRange(y);
            return list.ToArray();
        }
    }
}
