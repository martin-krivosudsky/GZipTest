namespace GZipTest.Compress
{
    internal interface IDecompressor
    {
        void Decompress(string archiveFileName, string targetFileName);
    }
}
