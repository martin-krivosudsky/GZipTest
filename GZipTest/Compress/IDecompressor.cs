using System;
using System.Collections.Generic;
using System.Text;

namespace GZipTest.Compress
{
    internal interface IDecompressor
    {
        void Decompress(string archiveFileName, string targetFileName);
    }
}
