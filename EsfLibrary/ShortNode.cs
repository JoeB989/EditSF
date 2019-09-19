using System.IO;

namespace EsfLibrary
{
    public class ShortNode : DelegatingDecoderNode<short>
    {
        public ShortNode()
            : base((Converter<short>) short.Parse, (ValueReader<short>) EsfCodec.ReadShort,
                (ValueWriter<short>) delegate(BinaryWriter writer, short b) { writer.Write(b); })
        {
            TypeCode = EsfType.INT16;
        }

        public override EsfNode CreateCopy()
        {
            return new ShortNode
            {
                Value = Value
            };
        }
    }
}