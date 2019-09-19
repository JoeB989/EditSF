using System.IO;

namespace EsfLibrary
{
    public class SByteNode : DelegatingDecoderNode<sbyte>
    {
        public SByteNode()
            : base((Converter<sbyte>) sbyte.Parse, (ValueReader<sbyte>) EsfCodec.ReadSbyte,
                (ValueWriter<sbyte>) delegate(BinaryWriter writer, sbyte b) { writer.Write(b); })
        {
            TypeCode = EsfType.INT8;
        }

        public override EsfNode CreateCopy()
        {
            return new SByteNode
            {
                Value = Value
            };
        }
    }
}