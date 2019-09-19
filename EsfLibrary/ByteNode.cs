using System.IO;

namespace EsfLibrary
{
    public class ByteNode : DelegatingDecoderNode<byte>
    {
        public ByteNode()
            : base((Converter<byte>) byte.Parse, (ValueReader<byte>) EsfCodec.ReadByte,
                (ValueWriter<byte>) delegate(BinaryWriter writer, byte b) { writer.Write(b); })
        {
            TypeCode = EsfType.UINT8;
        }

        public override EsfNode CreateCopy()
        {
            return new ByteNode
            {
                Value = Value
            };
        }
    }
}