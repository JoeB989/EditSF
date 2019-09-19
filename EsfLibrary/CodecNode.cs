using System.IO;

namespace EsfLibrary
{
    public abstract class CodecNode<T> : EsfValueNode<T>, ICodecNode
    {
        public CodecNode(Converter<T> conv)
            : base(conv)
        {
        }

        public void Decode(BinaryReader reader, EsfType readAs)
        {
            Value = ReadValue(reader, readAs);
        }

        protected abstract T ReadValue(BinaryReader reader, EsfType readAs);

        public void Encode(BinaryWriter writer)
        {
            if (TypeCode == EsfType.INVALID)
            {
                throw new InvalidDataException("Cannot encode without valid type code");
            }

            writer.Write((byte) TypeCode);
            WriteValue(writer);
        }

        public abstract void WriteValue(BinaryWriter writer);
    }
}