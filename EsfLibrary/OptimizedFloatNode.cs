// EsfLibrary.OptimizedFloatNode

using System.IO;

namespace EsfLibrary
{
    public class OptimizedFloatNode : CodecNode<float>
    {
        public override EsfType TypeCode
        {
            get
            {
                if (Value != 0f)
                {
                    return EsfType.SINGLE;
                }

                return EsfType.SINGLE_ZERO;
            }
            set { }
        }

        public OptimizedFloatNode()
            : base((Converter<float>) float.Parse)
        {
        }

        protected override float ReadValue(BinaryReader reader, EsfType readAs)
        {
            if (readAs != EsfType.SINGLE_ZERO)
            {
                return reader.ReadSingle();
            }

            return 0f;
        }

        public override void WriteValue(BinaryWriter writer)
        {
            if (TypeCode != EsfType.SINGLE_ZERO)
            {
                writer.Write(Value);
            }
        }

        public override EsfNode CreateCopy()
        {
            return new OptimizedFloatNode
            {
                Value = Value
            };
        }
    }
}