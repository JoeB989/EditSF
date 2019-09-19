using System;
using System.Collections.Generic;
using System.IO;

namespace EsfLibrary
{
    public class EsfArrayNode<T> : EsfValueNode<T[]>, ICodecNode
    {
        public Converter<T> ConvertItem { get; set; }

        protected virtual EsfType ContainedTypeCode => TypeCode - 64;

        public string Separator { get; set; }

        public EsfArrayNode(EsfCodec codec, EsfType code)
            : base((Converter<T[]>) delegate { throw new InvalidOperationException(); })
        {
            base.Codec = codec;
            Separator = " ";
            TypeCode = code;
            ConvertItem = DefaultFromString;
            Value = new T[0];
        }

        private static T DefaultFromString(string toConvert)
        {
            return (T) Convert.ChangeType(toConvert, typeof(T));
        }

        public override EsfNode CreateCopy()
        {
            return new EsfArrayNode<T>(base.Codec, TypeCode)
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
            EsfType containedTypeCode = ContainedTypeCode;
            int num = base.Codec.ReadSize(reader);
            List<T> list = new List<T>();
            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(reader.ReadBytes(num))))
            {
                while (binaryReader.BaseStream.Position < num)
                {
                    list.Add(ReadFromCodec(binaryReader, containedTypeCode));
                }
            }

            Value = list.ToArray();
        }

        public void Encode(BinaryWriter writer)
        {
            EsfType containedTypeCode = ContainedTypeCode;
            EsfType esfType = containedTypeCode + 64;
            writer.Write((byte) esfType);
            byte[] array = new byte[0];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer2 = new BinaryWriter(memoryStream))
                {
                    CodecNode<T> codecNode =
                        base.Codec.CreateValueNode(containedTypeCode, optimize: false) as CodecNode<T>;
                    codecNode.TypeCode = containedTypeCode;
                    T[] value = Value;
                    for (int i = 0; i < value.Length; i++)
                    {
                        T val2 = codecNode.Value = value[i];
                        codecNode.WriteValue(writer2);
                    }

                    array = memoryStream.ToArray();
                    base.Codec.WriteOffset(writer, array.Length);
                    writer.Write(array);
                }
            }
        }

        private T ReadFromCodec(BinaryReader reader, EsfType containedTypeCode)
        {
            return ((EsfValueNode<T>) base.Codec.ReadValueNode(reader, containedTypeCode)).Value;
        }

        public override void FromString(string value)
        {
            string[] array = value.Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<T> list = new List<T>(array.Length);
            string[] array2 = array;
            foreach (string value2 in array2)
            {
                list.Add(ConvertItem(value2));
            }

            if (base.Parent == null)
            {
                Console.WriteLine("No parent set! I'm sad.");
            }

            Value = (list.ToArray() ?? new T[0]);
        }

        public override bool Equals(object o)
        {
            EsfArrayNode<T> esfArrayNode = o as EsfArrayNode<T>;
            return (esfArrayNode != null) & ArraysEqual(Value, esfArrayNode.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            string result = "";
            try
            {
                if (Value != null)
                {
                    return string.Join(Separator, Value);
                }

                return result;
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
                result = Value.ToString();
                return $"{result.Substring(0, result.Length - 1)}{Value.Length}]";
            }
        }

        private static bool ArraysEqual<O>(O[] array1, O[] array2)
        {
            bool flag = array1.Length == array2.Length;
            if (flag)
            {
                for (int i = 0; i < array1.Length; i++)
                {
                    if (!EqualityComparer<O>.Default.Equals(array1[i], array2[i]))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return flag;
        }

        private static O[] ParseArray<O>(string value, Converter<O> convert)
        {
            string[] array = value.Split(' ');
            List<O> list = new List<O>(array.Length);
            string[] array2 = array;
            foreach (string value2 in array2)
            {
                list.Add(convert(value2));
            }

            return list.ToArray();
        }
    }
}