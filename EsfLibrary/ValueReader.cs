using System.IO;

namespace EsfLibrary
{
    public delegate T ValueReader<T>(BinaryReader reader);
}