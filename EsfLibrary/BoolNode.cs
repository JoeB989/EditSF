using System.IO;

namespace EsfLibrary
{
    public class BoolNode : DelegatingDecoderNode<bool>
    {
        public BoolNode()
            : base((Converter<bool>) bool.Parse, (ValueReader<bool>) EsfCodec.ReadBool,
                (ValueWriter<bool>) delegate(BinaryWriter writer, bool b) { writer.Write(b); })
        {
            TypeCode = EsfType.BOOL;
        }

        public override EsfNode CreateCopy()
        {
            return new BoolNode
            {
                Value = Value
            };
        }
    }
}