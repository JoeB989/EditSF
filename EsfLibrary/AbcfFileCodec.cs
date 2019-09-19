using System.Collections.Generic;
using System.IO;

namespace EsfLibrary
{
    public class AbcfFileCodec : AbceCodec
    {
        protected int headerLength = 16;

        protected Dictionary<string, int> Utf16StringList = new Dictionary<string, int>();

        protected Dictionary<string, int> AsciiStringList = new Dictionary<string, int>();

        private static Dictionary<string, int> ReadStringList(BinaryReader reader, ValueReader<string> readString)
        {
            int num = reader.ReadInt32();
            Dictionary<string, int> dictionary = new Dictionary<string, int>(num);
            for (int i = 0; i < num; i++)
            {
                string key = readString(reader);
                dictionary.Add(key, reader.ReadInt32());
            }

            return dictionary;
        }

        private static void WriteStringList(BinaryWriter writer, Dictionary<string, int> stringList,
            ValueWriter<string> writeString)
        {
            writer.Write(stringList.Count);
            foreach (string key in stringList.Keys)
            {
                writeString(writer, key);
                writer.Write(stringList[key]);
            }
        }

        private void WriteStringReference(BinaryWriter writer, string toWrite, Dictionary<string, int> referenceList)
        {
            int i;
            if (referenceList.ContainsKey(toWrite))
            {
                i = referenceList[toWrite];
            }
            else
            {
                for (i = referenceList.Count; referenceList.ContainsValue(i); i++)
                {
                }

                referenceList.Add(toWrite, i);
            }

            writer.Write(i);
        }

        public AbcfFileCodec(uint id = 43983u)
            : base(id)
        {
        }

        protected override string ReadUtf16String(BinaryReader reader)
        {
            return ReadStringReference(reader, Utf16StringList);
        }

        protected override string ReadAsciiString(BinaryReader reader)
        {
            return ReadStringReference(reader, AsciiStringList);
        }

        protected void WriteAsciiReference(BinaryWriter w, string s)
        {
            WriteStringReference(w, s, AsciiStringList);
        }

        protected void WriteUtf16Reference(BinaryWriter w, string s)
        {
            WriteStringReference(w, s, Utf16StringList);
        }

        public override EsfNode CreateValueNode(EsfType typeCode, bool optimize = false)
        {
            StringNode stringNode;
            switch (typeCode)
            {
                case EsfType.UTF16:
                    stringNode = new StringNode(ReadUtf16String, WriteUtf16Reference);
                    break;
                case EsfType.ASCII:
                    stringNode = new StringNode(ReadAsciiString, WriteAsciiReference);
                    break;
                case EsfType.ASCII_W21:
                    stringNode = new StringNode(ReadAsciiString, WriteAsciiReference);
                    break;
                case EsfType.ASCII_W25:
                    stringNode = new StringNode(ReadAsciiString, WriteAsciiReference);
                    break;
                default:
                    return base.CreateValueNode(typeCode);
            }

            stringNode.TypeCode = typeCode;
            return stringNode;
        }

        protected override void ReadNodeNames(BinaryReader reader)
        {
            base.ReadNodeNames(reader);
            Utf16StringList = ReadStringList(reader, ReadUtf16);
            AsciiStringList = ReadStringList(reader, ReadAscii);
        }

        protected override void WriteNodeNames(BinaryWriter writer)
        {
            base.WriteNodeNames(writer);
            WriteStringList(writer, Utf16StringList, WriteUtf16);
            WriteStringList(writer, AsciiStringList, WriteAscii);
        }
    }
}