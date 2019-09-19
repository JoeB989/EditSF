using System;
using System.IO;
namespace EsfLibrary
{
    public class Coordinate2DNode : CodecNode<Tuple<float, float>>
    {
        private static Tuple<float, float> Parse(string value)
        {
            string[] array = value.Substring(1, value.Length - 2).Split(',');
            Console.WriteLine("Trying to parse [{0}] - [{1}]", array[0].Trim(), array[1].Trim());
            return new Tuple<float, float>(float.Parse(array[0].Trim()), float.Parse(array[1].Trim()));
        }

        public Coordinate2DNode()
            : base((Converter<Tuple<float, float>>) Parse)
        {
            TypeCode = EsfType.COORD2D;
        }

        protected override Tuple<float, float> ReadValue(BinaryReader reader, EsfType readAs)
        {
            return new Tuple<float, float>(reader.ReadSingle(), reader.ReadSingle());
        }

        public override void WriteValue(BinaryWriter writer)
        {
            writer.Write(Value.Item1);
            writer.Write(Value.Item2);
        }

        public override EsfNode CreateCopy()
        {
            return new Coordinate2DNode
            {
                Value = Value
            };
        }
    }
}
