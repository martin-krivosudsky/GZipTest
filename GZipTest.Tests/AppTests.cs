using System;
using GZipTest.Compress;
using GZipTest.Helpers;
using Moq;
using NUnit.Framework;

namespace GZipTest.Tests
{
    [TestFixture]
    public class AppTests
    {
        private App _app;
        private Mock<ICompressor> _compressorMock;
        private Mock<IDecompressor> _decompressorMock;
        private Mock<IFileHelper> _fileHelperMock;

        [SetUp]
        public void Setup()
        {
            _compressorMock = new Mock<ICompressor>(MockBehavior.Strict);
            _decompressorMock = new Mock<IDecompressor>(MockBehavior.Strict);
            _fileHelperMock = new Mock<IFileHelper>(MockBehavior.Strict);

            _app = new App(_compressorMock.Object, _decompressorMock.Object, _fileHelperMock.Object);
        }

        [Test]
        public void CompressCalledOnEveryProcessor()
        {
            const string originalFileName = "someFile.txt";
            const string archiveFileName = "archiveFile.gz";
            const long originalFileSize = 4096;

            _fileHelperMock.Setup(fh => fh.GetFileSize(originalFileName)).Returns(originalFileSize);
            _fileHelperMock.Setup(fh => fh.CreateEmptyFile(archiveFileName));
            _compressorMock.Setup(c => c.Compress(originalFileName, originalFileSize, archiveFileName));

            _app.Compress(originalFileName, archiveFileName);

            _compressorMock.Verify(c => c.Compress(originalFileName, originalFileSize, archiveFileName), Times.Exactly(Environment.ProcessorCount));
        }

        [Test]
        public void DecompressCalledCorrectly()
        {
            const string originalFileName = "someFile.txt";
            const string archiveFileName = "archiveFile.gz";

            _decompressorMock.Setup(d => d.Decompress(archiveFileName, originalFileName));

            _app.Decompress(archiveFileName, originalFileName);

            _decompressorMock.Verify(d => d.Decompress(archiveFileName, originalFileName), Times.Once);
        }
    }
}
