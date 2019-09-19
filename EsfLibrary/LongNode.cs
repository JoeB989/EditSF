using System.IO;

namespace EsfLibrary
{
    public class LongNode : DelegatingDecoderNode<long>
    {
        public LongNode()
            : base((Converter<long>) long.Parse, (ValueReader<long>) EsfCodec.ReadLong,
                (ValueWriter<long>) delegate(BinaryWriter writer, long b) { writer.Write(b); })
        {
            TypeCode = EsfType.INT64;
        }

        public override EsfNode CreateCopy()
        {
            return new LongNode
            {
                Value = Value
            };
        }
    }
}