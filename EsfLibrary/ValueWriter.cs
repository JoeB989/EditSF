using System.IO;

namespace EsfLibrary
{
    public delegate void ValueWriter<T>(BinaryWriter writer, T value);
}