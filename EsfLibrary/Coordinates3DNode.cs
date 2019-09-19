using System;
using System.IO;

namespace EsfLibrary
{
    public class Coordinates3DNode : CodecNode<Tuple<float, float, float>>
    {
        private static Tuple<float, float, float> Parse(string value)
        {
            string[] array = value.Substring(1, value.Length - 2).Split(',');
            return new Tuple<float, float, float>(float.Parse(array[0].Trim()), float.Parse(array[1].Trim()),
                float.Parse(array[2].Trim()));
        }

        public Coordinates3DNode()
            : base((Converter<Tuple<float, float, float>>) Parse)
        {
        }

        protected override Tuple<float, float, float> ReadValue(BinaryReader reader, EsfType readAs)
        {
            return new Tuple<float, float, float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public override void WriteValue(BinaryWriter writer)
        {
            writer.Write(Value.Item1);
            writer.Write(Value.Item2);
            writer.Write(Value.Item3);
        }

        public override EsfNode CreateCopy()
        {
            return new Coordinates3DNode
            {
                Value = Value
            };
        }
    }
}