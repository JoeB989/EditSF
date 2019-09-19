// EsfLibrary.OptimizedBoolNode

using System;
using System.IO;

namespace EsfLibrary
{
    public class OptimizedBoolNode : CodecNode<bool>
    {
        public override EsfType TypeCode
        {
            get
            {
                if (!Value)
                {
                    return EsfType.BOOL_FALSE;
                }

                return EsfType.BOOL_TRUE;
            }
            set { }
        }

        public OptimizedBoolNode()
            : base((Converter<bool>) bool.Parse)
        {
        }

        protected override bool ReadValue(BinaryReader reader, EsfType readAs)
        {
            switch (readAs)
            {
                case EsfType.BOOL:
                    return reader.ReadBoolean();
                case EsfType.BOOL_TRUE:
                    return true;
                case EsfType.BOOL_FALSE:
                    return false;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override void WriteValue(BinaryWriter writer)
        {
        }

        public override EsfNode CreateCopy()
        {
            return new OptimizedBoolNode
            {
                Value = Value
            };
        }
    }
}