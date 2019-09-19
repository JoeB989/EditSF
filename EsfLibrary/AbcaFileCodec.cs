using System;
using System.Collections.Generic;
using System.IO;

namespace EsfLibrary
{
    public class AbcaFileCodec : AbcfFileCodec
    {
        private static byte RECORD_BIT = 128;

        private static ushort BLOCK_BIT = 64;

        private static byte LONG_INFO = 32;

        protected void WriteBoolNoop(BinaryWriter writer, bool value)
        {
        }

        protected void WriteFloatNoop(BinaryWriter writer, float value)
        {
        }

        public AbcaFileCodec()
            : base(43978u)
        {
        }

        public override EsfNode Decode(BinaryReader reader, byte typeCode)
        {
            EsfNode esfNode;
            if ((byte) (typeCode & RECORD_BIT) == 0 || reader.BaseStream.Position == headerLength + 1)
            {
                if (typeCode == 4 || (uint) (typeCode - 25) <= 3u)
                {
                    esfNode = new OptimizedIntNode();
                    (esfNode as OptimizedIntNode).Decode(reader, (EsfType) typeCode);
                }
                else
                {
                    esfNode = base.Decode(reader, typeCode);
                }
            }
            else
            {
                esfNode = (((typeCode & BLOCK_BIT) != 0)
                    ? ReadRecordArrayNode(reader, typeCode)
                    : ReadRecordNode(reader, typeCode));
            }

            return esfNode;
        }

        protected override EsfNode ReadRecordArrayNode(BinaryReader reader, byte typeCode)
        {
            RecordArrayNode recordArrayNode = new RecordArrayNode(this, typeCode);
            recordArrayNode.Decode(reader, EsfType.RECORD_BLOCK);
            return recordArrayNode;
        }

        public override EsfNode CreateValueNode(EsfType typeCode, bool optimize = true)
        {
            EsfNode esfNode;
            switch (typeCode)
            {
                case EsfType.BOOL:
                case EsfType.BOOL_TRUE:
                case EsfType.BOOL_FALSE:
                    if (optimize)
                    {
                        return new OptimizedBoolNode();
                    }

                    esfNode = new BoolNode();
                    break;
                case EsfType.UINT32:
                case EsfType.UINT32_ZERO:
                case EsfType.UINT32_ONE:
                case EsfType.UINT32_BYTE:
                case EsfType.UINT32_SHORT:
                case EsfType.UINT32_24BIT:
                    return new OptimizedUIntNode
                    {
                        SingleByteMin = !optimize
                    };
                case EsfType.INT32:
                case EsfType.INT32_ZERO:
                case EsfType.INT32_BYTE:
                case EsfType.INT32_SHORT:
                case EsfType.INT32_24BIT:
                    return new OptimizedIntNode
                    {
                        SingleByteMin = !optimize
                    };
                case EsfType.SINGLE:
                case EsfType.SINGLE_ZERO:
                    if (optimize)
                    {
                        return new OptimizedFloatNode();
                    }

                    esfNode = new FloatNode();
                    break;
                case EsfType.UNKNOWN_23:
                    esfNode = new SByteNode();
                    break;
                case EsfType.UNKNOWN_24:
                    esfNode = new ShortNode();
                    break;
                default:
                    return base.CreateValueNode(typeCode, optimize: false);
            }

            esfNode.TypeCode = typeCode;
            return esfNode;
        }

        protected override EsfNode CreateArrayNode(EsfType typeCode)
        {
            EsfNode esfNode;
            switch (typeCode)
            {
                case EsfType.BOOL_TRUE_ARRAY:
                case EsfType.BOOL_FALSE_ARRAY:
                case EsfType.UINT_ZERO_ARRAY:
                case EsfType.UINT_ONE_ARRAY:
                case EsfType.INT32_ZERO_ARRAY:
                case EsfType.SINGLE_ZERO_ARRAY:
                    throw new InvalidDataException($"Array {typeCode:x} of zero-byte entries makes no sense");
                case EsfType.UINT32_BYTE_ARRAY:
                case EsfType.UINT32_SHORT_ARRAY:
                case EsfType.UINT32_24BIT_ARRAY:
                    esfNode = new OptimizedArrayNode<uint>(this, typeCode, (uint val) => new OptimizedUIntNode
                    {
                        Value = val,
                        SingleByteMin = true
                    });
                    break;
                case EsfType.INT32_BYTE_ARRAY:
                case EsfType.INT32_SHORT_ARRAY:
                    esfNode = new OptimizedArrayNode<int>(this, typeCode, (int val) => new OptimizedIntNode
                    {
                        Value = val,
                        SingleByteMin = true
                    });
                    break;
                default:
                    return base.CreateArrayNode(typeCode);
            }

            esfNode.TypeCode = typeCode;
            return esfNode;
        }

        protected override byte[] ReadArray(BinaryReader reader)
        {
            long num = ReadSize(reader);
            return reader.ReadBytes((int) num);
        }

        public override RecordNode ReadRecordNode(BinaryReader reader, byte typeCode, bool forceDecode = false)
        {
            RecordNode recordNode = base.ReadRecordNode(reader, typeCode, forceDecode);
            if (forceDecode && recordNode.Name == CompressedNode.TAG_NAME)
            {
                recordNode = new CompressedNode(this, recordNode);
            }

            if (recordNode is MemoryMappedRecordNode)
            {
                (recordNode as MemoryMappedRecordNode).InvalidateSiblings = false;
            }

            return recordNode;
        }

        public override int ReadSize(BinaryReader reader)
        {
            byte b = reader.ReadByte();
            long num = 0L;
            while ((b & 0x80) != 0)
            {
                num = (num << 7) + (b & 0x7F);
                b = reader.ReadByte();
            }

            num = (num << 7) + (b & 0x7F);
            return (int) num;
        }

        public override int ReadCount(BinaryReader reader)
        {
            return ReadSize(reader);
        }

        public override void WriteSize(BinaryWriter writer, long size)
        {
            if (size == 0L)
            {
                writer.Write((byte) 0);
                return;
            }

            byte b = 128;
            byte b2 = 127;
            Stack<byte> stack = new Stack<byte>();
            while (size != 0L)
            {
                byte item = (byte) (size & b2);
                stack.Push(item);
                size >>= 7;
            }

            while (stack.Count != 0)
            {
                byte b3 = stack.Pop();
                b3 = (byte) (b3 | ((stack.Count != 0) ? b : 0));
                writer.Write(b3);
            }
        }

        public override void WriteOffset(BinaryWriter writer, long offset)
        {
            WriteSize(writer, offset);
        }

        public override void ReadRecordInfo(BinaryReader reader, byte encoded, out string name, out byte version)
        {
            if (reader.BaseStream.Position == headerLength + 1 || (encoded & LONG_INFO) != 0)
            {
                base.ReadRecordInfo(reader, encoded, out name, out version);
                return;
            }

            version = (byte) ((encoded & 0x1F) >> 1);
            ushort num = (ushort) ((encoded & 1) << 8);
            num = (ushort) (num + reader.ReadByte());
            name = GetNodeName(num);
        }

        public override void WriteRecordInfo(BinaryWriter writer, byte typeCode, string name, byte version)
        {
            ushort nodeNameIndex = GetNodeNameIndex(name);
            if ((nodeNameIndex != 0 && nodeNameIndex < 512) & (version < 16))
            {
                ushort num = encodeShortRecordInfo(typeCode, nodeNameIndex, version);
                byte value = (byte) ((num >> 8) & 0xFF);
                writer.Write(value);
                writer.Write((byte) num);
                return;
            }

            switch (typeCode)
            {
                case 128:
                    typeCode = (byte) ((nodeNameIndex == 0) ? 128 : 160);
                    break;
                case 129:
                    typeCode = 224;
                    break;
                default:
                    throw new InvalidDataException($"Trying to encode record info for wrong type code {typeCode}");
            }

            base.WriteRecordInfo(writer, typeCode, name, version);
        }

        public static ushort encodeShortRecordInfo(byte typeCode, ushort nameIndex, byte version)
        {
            return (ushort) ((ushort) ((ushort) ((ushort) (version << 9) | nameIndex) |
                                       ((typeCode == 129) ? ((ushort) (BLOCK_BIT << 8)) : 0)) |
                             (ushort) (RECORD_BIT << 8));
        }

        public override void EncodeSized(BinaryWriter writer, List<EsfNode> nodes, bool writeCount = false)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] array;
            using (BinaryWriter writer2 = new BinaryWriter(memoryStream))
            {
                foreach (EsfNode node in nodes)
                {
                    Encode(writer2, node);
                }

                array = memoryStream.ToArray();
            }

            WriteSize(writer, array.LongLength);
            if (writeCount)
            {
                WriteSize(writer, nodes.Count);
            }

            writer.Write(array);
            array = null;
            GC.Collect();
        }
    }
}