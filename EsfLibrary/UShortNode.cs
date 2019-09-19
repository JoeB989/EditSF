using System.IO;

namespace EsfLibrary
{
    public class UShortNode : DelegatingDecoderNode<ushort>
    {
        public UShortNode()
            : base((Converter<ushort>) ushort.Parse, (ValueReader<ushort>) EsfCodec.ReadUshort,
                (ValueWriter<ushort>) delegate(BinaryWriter writer, ushort b) { writer.Write(b); })
        {
            TypeCode = EsfType.UINT16;
        }

        public override EsfNode CreateCopy()
        {
            return new UShortNode
            {
                Value = Value
            };
        }
    }
}