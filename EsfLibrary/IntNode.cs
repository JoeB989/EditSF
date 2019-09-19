using System.IO;

namespace EsfLibrary
{
    public class IntNode : DelegatingDecoderNode<int>
    {
        public IntNode()
            : base((Converter<int>) int.Parse, (ValueReader<int>) EsfCodec.ReadInt,
                (ValueWriter<int>) delegate(BinaryWriter writer, int v) { writer.Write(v); })
        {
            TypeCode = EsfType.INT32;
        }

        public override EsfNode CreateCopy()
        {
            return new IntNode
            {
                Value = Value
            };
        }
    }
}