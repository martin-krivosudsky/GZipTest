namespace GZipTest.Compress
{
    internal interface ICompressor
    {
        void Compress(string originalFileName, long originalFileSize, string archiveFileName);
    }
}
