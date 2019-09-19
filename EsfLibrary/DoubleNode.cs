using System.IO;

namespace EsfLibrary
{
    public class DoubleNode : DelegatingDecoderNode<double>
    {
        public DoubleNode()
            : base((Converter<double>) double.Parse, (ValueReader<double>) EsfCodec.ReadDouble,
                (ValueWriter<double>) delegate(BinaryWriter writer, double b) { writer.Write(b); })
        {
            TypeCode = EsfType.DOUBLE;
        }

        public override EsfNode CreateCopy()
        {
            return new DoubleNode
            {
                Value = Value
            };
        }
    }
}