using System;
using System.Collections.Generic;
using System.IO;

namespace EsfLibrary
{
    public class RawDataNode : EsfValueNode<byte[]>, ICodecNode
    {
        public RawDataNode(EsfCodec codec)
            : base((Converter<byte[]>)delegate
            {
                throw new InvalidOperationException();
            })
        {
            base.Codec = codec;
            TypeCode = EsfType.UINT8_ARRAY;
        }

        public override EsfNode CreateCopy()
        {
            return new RawDataNode(base.Codec)
            {
                TypeCode = TypeCode,
                Value = Value
            };
        }

        public override void ToXml(TextWriter writer, string indent)
        {
            writer.WriteLine("{2}<{0} Length=\"{1}\"/>", TypeCode, Value.Length, indent);
        }

        public void Decode(BinaryReader reader, EsfType type)
        {
            int count = base.Codec.ReadSize(reader);
            Value = reader.ReadBytes(count);
        }

        public void Encode(BinaryWriter writer)
        {
            writer.Write((byte)TypeCode);
            base.Codec.WriteOffset(writer, Value.Length);
            writer.Write(Value);
        }

        public override bool Equals(object o)
        {
            RawDataNode rawDataNode = o as RawDataNode;
            if (rawDataNode != null)
            {
                return EqualityComparer<byte[]>.Default.Equals(Value, rawDataNode.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            string text = Value.ToString();
            return $"{text.Substring(0, text.Length - 1)}{Value.Length}]";
        }
    }
}
