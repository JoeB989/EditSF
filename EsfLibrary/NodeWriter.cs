// EsfLibrary.NodeWriter

using System.IO;

namespace EsfLibrary
{
    public delegate void NodeWriter(BinaryWriter writer, EsfNode node);
}