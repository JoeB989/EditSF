using System.IO;
namespace EsfLibrary
{
    public class FloatNode : DelegatingDecoderNode<float>
    {
        public FloatNode()
            : base((Converter<float>)float.Parse, (ValueReader<float>)EsfCodec.ReadFloat, (ValueWriter<float>)delegate(BinaryWriter writer, float f)
            {
                writer.Write(f);
            })
        {
            TypeCode = EsfType.SINGLE;
        }

        public override EsfNode CreateCopy()
        {
            return new FloatNode
            {
                Value = Value
            };
        }
    }
}
