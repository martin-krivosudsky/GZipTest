namespace GZipTest.Helpers
{
    internal interface IFileHelper
    {
        long GetFileSize(string fileName);
        void CreateEmptyFile(string fileName);
        void AppendToFile(string fileName, byte[] bytes);
        byte[] ReadBytes(string fileName, long readPosition, int readSize);
        byte[] ReadAllBytes(string fileName);
        void WriteAllBytes(string fileName, byte[] bytes);
    }
}
