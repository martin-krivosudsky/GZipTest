using System;
using System.IO;
using System.Security;

namespace GZipTest.Helpers
{
    internal class FileHelper : IFileHelper
    {
        public long GetFileSize(string fileName)
        {
            long fileSize = 0;

            ExecuteSafe(() =>
            {
                fileSize = new FileInfo(fileName).Length;
            }, fileName);

            return fileSize;
        }

        public void CreateEmptyFile(string fileName)
        {
            ExecuteSafe(() =>
            {
                File.CreateText(fileName).Close();
            }, fileName);
        }

        public void AppendToFile(string fileName, byte[] bytes)
        {
            ExecuteSafe(() =>
            {
                using var stream = new FileStream(fileName, FileMode.Append);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }, fileName);
        }

        public byte[] ReadBytes(string fileName, long readPosition, int readSize)
        {
            var bytes = new byte[] {};

            ExecuteSafe(() =>
            {
                using var reader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                reader.BaseStream.Seek(readPosition, SeekOrigin.Begin);
                bytes = reader.ReadBytes(readSize);
            }, fileName);

            return bytes;
        }

        public byte[] ReadAllBytes(string fileName)
        {
            var bytes = new byte[] { };

            ExecuteSafe(() => bytes = File.ReadAllBytes(fileName), fileName);

            return bytes;
        }

        public void WriteAllBytes(string fileName, byte[] bytes)
        {
            ExecuteSafe(() => File.WriteAllBytes(fileName, bytes), fileName);
        }

        private static void ExecuteSafe(Action action, string fileName)
        {
            ExecuteSafe(() => { action(); return 0; }, fileName);
        }

        private static void ExecuteSafe<T>(Func<T> action, string fileName)
        {
            try
            {
                action();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File does not exist. FileName: {0}", fileName);
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access to file denied. FileName: {0}", fileName);
                throw;
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Path is too long. FileName: {0}", fileName);
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory was not found. FileName: {0}", fileName);
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("File name is invalid. FileName: {0}", fileName);
                throw;
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("File name is in invalid format. FileName: {0}", fileName);
                throw;
            }
            catch (IOException)
            {
                Console.WriteLine("Something went wrong while accessing file. FileName: {0}", fileName);
                throw;
            }
            catch (SecurityException)
            {
                Console.WriteLine("Program does not have rights to access file. FileName: {0}", fileName);
                throw;
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("File stream can not be accessed because it was disposed. FileName: {0}", fileName);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Exception: {0}", ex.Message);
                throw;
            }
        }
    }
}
