using System.IO;

namespace EsfLibrary
{
    public interface ICodecNode
    {
        void Decode(BinaryReader reader, EsfType readAs);

        void Encode(BinaryWriter writer);
    }
}