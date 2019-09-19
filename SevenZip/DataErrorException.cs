using System;

namespace SevenZip.Compression
{
    internal class DataErrorException : ApplicationException
    {
        public DataErrorException()
            : base("Data Error")
        {
        }
    }
}