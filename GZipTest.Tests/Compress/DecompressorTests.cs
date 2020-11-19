using GZipTest.Compress;
using GZipTest.Helpers;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests.Compress
{
    [TestFixture]
    public class DecompressorTests
    {
        private IDecompressor _decompressor;
        private Mock<IFileHelper> _fileHelperMock;

        private readonly byte[] _expectedResult = {0, 1, 2, 3, 4, 5, 6, 7};
        private readonly byte[] _compressedBytes = { 31, 139, 8, 0, 0, 0, 0, 0, 0, 10, 99, 96, 100, 98, 102, 97, 101, 99, 7, 0, 159, 104, 170, 136, 8, 0, 0, 0 };

        [SetUp]
        public void Setup()
        {
            _fileHelperMock = new Mock<IFileHelper>();

            _decompressor = new Decompressor(_fileHelperMock.Object);
        }

        [Test]
        public void DecompressBytes()
        {
            const string targetFileName = "someFile.txt";
            const string archiveFileName = "archive.gz";

            _fileHelperMock.Setup(fh => fh.ReadAllBytes(archiveFileName)).Returns(_compressedBytes);

            _decompressor.Decompress(archiveFileName, targetFileName);

            _fileHelperMock.Verify(fh =>fh.WriteAllBytes(targetFileName, _expectedResult), Times.Once);
        }
    }
}
