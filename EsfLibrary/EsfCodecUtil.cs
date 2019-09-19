using System;
using System.IO;

namespace EsfLibrary
{
    public class EsfCodecUtil
    {
        public static EsfCodec GetCodec(Stream stream)
        {
            EsfCodec esfCodec = null;
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
            {
                return CodecFromCode(binaryReader.ReadUInt32());
            }
        }

        private static EsfCodec CodecFromCode(uint code)
        {
            EsfCodec result = null;
            switch (code)
            {
                case 43982u:
                    result = new AbceCodec();
                    break;
                case 43983u:
                    result = new AbcfFileCodec();
                    break;
                case 43978u:
                    result = new AbcaFileCodec();
                    break;
            }

            return result;
        }

        public static void WriteEsfFile(string filename, EsfFile file)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(filename)))
            {
                file.Codec.EncodeRootNode(writer, file.RootNode);
            }
        }

        public static EsfFile LoadEsfFile(string filename)
        {
            byte[] array = File.ReadAllBytes(filename);
            EsfCodec codec;
            using (MemoryStream stream = new MemoryStream(array))
            {
                codec = GetCodec(stream);
            }

            return new EsfFile(codec.Parse(array), codec);
        }

        public static void LogReadNode(EsfNode node, long position)
        {
            Console.WriteLine("{1:x}: read {0}", node, position);
        }
    }
}