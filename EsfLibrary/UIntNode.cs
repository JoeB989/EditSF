using System.IO;

namespace EsfLibrary
{
    public class UIntNode : DelegatingDecoderNode<uint>
    {
        public UIntNode()
            : base((Converter<uint>) uint.Parse, (ValueReader<uint>) EsfCodec.ReadUInt,
                (ValueWriter<uint>) delegate(BinaryWriter writer, uint u) { writer.Write(u); })
        {
            TypeCode = EsfType.UINT32;
        }

        public override EsfNode CreateCopy()
        {
            return new UIntNode
            {
                Value = Value,
                Modified = false
            };
        }
    }
}