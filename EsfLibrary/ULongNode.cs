using System.IO;

namespace EsfLibrary
{
    public class ULongNode : DelegatingDecoderNode<ulong>
    {
        public ULongNode()
            : base((Converter<ulong>) ulong.Parse, (ValueReader<ulong>) EsfCodec.ReadUlong,
                (ValueWriter<ulong>) delegate(BinaryWriter writer, ulong b) { writer.Write(b); })
        {
            TypeCode = EsfType.UINT64;
        }

        public override EsfNode CreateCopy()
        {
            return new ULongNode
            {
                Value = Value
            };
        }
    }
}