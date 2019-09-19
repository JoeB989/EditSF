using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EsfLibrary
{
    public abstract class EsfCodec
    {
        public delegate void NodeStarting(byte typeCode, long readerPosition);

        public delegate void NodeRead(EsfNode node, long readerPosition);

        public delegate void WriteLog(string log);

        protected byte[] buffer;

        public readonly uint ID;

        protected SortedList<int, string> nodeNames = new SortedList<int, string>();

        public SortedList<int, string> NodeNames
        {
            get { return nodeNames; }
            set { nodeNames = value; }
        }

        public EsfHeader Header { get; set; }

        public event NodeStarting NodeReadStarting;

        public event NodeRead NodeReadFinished;

        public EsfCodec(uint id)
        {
            ID = id;
        }

        public abstract void WriteHeader(BinaryWriter writer);

        public static bool ReadBool(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        public static sbyte ReadSbyte(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static short ReadShort(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        public static int ReadInt(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public static long ReadLong(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        public static byte ReadByte(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static ushort ReadUshort(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        public static uint ReadUInt(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public static ulong ReadUlong(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        public static float ReadFloat(BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        public static double ReadDouble(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        public static Tuple<float, float> ReadCoordinates2D(BinaryReader r)
        {
            return new Tuple<float, float>(r.ReadSingle(), r.ReadSingle());
        }

        public static Tuple<float, float, float> ReadCoordinates3D(BinaryReader r)
        {
            return new Tuple<float, float, float>(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        protected virtual string ReadUtf16String(BinaryReader reader)
        {
            return ReadUtf16(reader);
        }

        protected virtual string ReadAsciiString(BinaryReader reader)
        {
            return ReadAscii(reader);
        }

        public static string ReadUtf16(BinaryReader reader)
        {
            ushort num = reader.ReadUInt16();
            return Encoding.Unicode.GetString(reader.ReadBytes(num * 2));
        }

        public static void WriteUtf16(BinaryWriter writer, string toWrite)
        {
            writer.Write((ushort) toWrite.Length);
            writer.Write(Encoding.Unicode.GetBytes(toWrite));
        }

        public static string ReadAscii(BinaryReader reader)
        {
            ushort count = reader.ReadUInt16();
            return Encoding.ASCII.GetString(reader.ReadBytes(count));
        }

        public static void WriteAscii(BinaryWriter writer, string toWrite)
        {
            writer.Write((ushort) toWrite.Length);
            writer.Write(Encoding.ASCII.GetBytes(toWrite));
        }

        public static string ReadStringReference(BinaryReader reader, Dictionary<string, int> list)
        {
            int num = reader.ReadInt32();
            string result = null;
            foreach (string key in list.Keys)
            {
                if (list[key] == num)
                {
                    return key;
                }
            }

            return result;
        }

        public virtual EsfNode Parse(byte[] data)
        {
            return Parse(new MemoryStream(data));
        }

        public virtual EsfNode Parse(Stream stream)
        {
            MemoryStream memoryStream = stream as MemoryStream;
            if (memoryStream == null)
            {
                memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
            }

            buffer = memoryStream.ToArray();
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                return Parse(reader);
            }
        }

        public EsfNode Parse(BinaryReader reader)
        {
            reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            Header = ReadHeader(reader);
            uint num = reader.ReadUInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Seek(num, SeekOrigin.Begin);
            ReadNodeNames(reader);
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            EsfNode esfNode = Decode(reader);
            esfNode.Codec = this;
            return esfNode;
        }

        public abstract EsfHeader ReadHeader(BinaryReader reader);

        public EsfNode Decode(BinaryReader reader)
        {
            byte b = reader.ReadByte();
            if (this.NodeReadStarting != null)
            {
                this.NodeReadStarting(b, reader.BaseStream.Position - 1);
            }

            EsfNode esfNode = Decode(reader, b);
            if (this.NodeReadFinished != null)
            {
                this.NodeReadFinished(esfNode, reader.BaseStream.Position);
            }

            return esfNode;
        }

        public virtual EsfNode Decode(BinaryReader reader, byte code)
        {
            long position = reader.BaseStream.Position;
            EsfNode esfNode;
            if (code < 65)
            {
                esfNode = ReadValueNode(reader, (EsfType) code);
            }
            else if (code < 128)
            {
                esfNode = ReadArrayNode(reader, (EsfType) code);
            }
            else
            {
                switch (code)
                {
                    case 128:
                        esfNode = ReadRecordNode(reader, code);
                        break;
                    case 129:
                        esfNode = ReadRecordArrayNode(reader, code);
                        break;
                    default:
                        throw new InvalidDataException(
                            $"Type code {(EsfType) code:x} at {reader.BaseStream.Position - 1:x} invalid");
                }
            }

            if (this.NodeReadFinished != null)
            {
                this.NodeReadFinished(esfNode, position);
            }

            return esfNode;
        }

        public void EncodeRootNode(BinaryWriter writer, EsfNode rootNode)
        {
            WriteHeader(writer);
            long position = writer.BaseStream.Position;
            writer.Write(0);
            WriteRecordNode(writer, rootNode);
            long position2 = writer.BaseStream.Position;
            WriteNodeNames(writer);
            writer.BaseStream.Seek(position, SeekOrigin.Begin);
            writer.Write((uint) position2);
        }

        public void Encode(BinaryWriter writer, EsfNode node)
        {
            try
            {
                if (node.TypeCode == EsfType.RECORD_BLOCK_ENTRY)
                {
                    EncodeSized(writer, (node as ParentNode).AllNodes);
                }
                else if (node.TypeCode < EsfType.BOOL_ARRAY)
                {
                    WriteValueNode(writer, node);
                }
                else if (node.TypeCode < EsfType.RECORD)
                {
                    WriteArrayNode(writer, node);
                }
                else if (node.TypeCode == EsfType.RECORD)
                {
                    WriteRecordNode(writer, node);
                }
                else
                {
                    if (node.TypeCode != EsfType.RECORD_BLOCK)
                    {
                        throw new NotImplementedException(string.Format("Cannot write type code {0:x} at {1:x}",
                            node.TypeCode));
                    }

                    WriteRecordArrayNode(writer, node);
                }
            }
            catch
            {
                Console.WriteLine($"failed to write node {node}");
                throw;
            }
        }

        public virtual EsfNode CreateValueNode(EsfType typeCode, bool optimize = true)
        {
            EsfNode esfNode = null;
            switch (typeCode)
            {
                case EsfType.BOOL:
                    esfNode = new BoolNode();
                    break;
                case EsfType.INT8:
                    esfNode = new SByteNode();
                    break;
                case EsfType.INT16:
                    esfNode = new ShortNode();
                    break;
                case EsfType.INT32:
                    esfNode = new IntNode();
                    break;
                case EsfType.INT64:
                    esfNode = new LongNode();
                    break;
                case EsfType.UINT8:
                    esfNode = new ByteNode();
                    break;
                case EsfType.UINT16:
                    esfNode = new UShortNode();
                    break;
                case EsfType.UINT32:
                    esfNode = new UIntNode();
                    break;
                case EsfType.UINT64:
                    esfNode = new ULongNode();
                    break;
                case EsfType.SINGLE:
                    esfNode = new FloatNode();
                    break;
                case EsfType.DOUBLE:
                    esfNode = new DoubleNode();
                    break;
                case EsfType.COORD2D:
                    esfNode = new Coordinate2DNode();
                    break;
                case EsfType.COORD3D:
                    esfNode = new Coordinates3DNode();
                    break;
                case EsfType.UTF16:
                    esfNode = new StringNode(ReadUtf16, WriteUtf16);
                    break;
                case EsfType.ASCII:
                    esfNode = new StringNode(ReadAscii, WriteAscii);
                    break;
                case EsfType.ANGLE:
                    esfNode = new UShortNode();
                    break;
                default:
                    esfNode = new BoolNode();
                    break;
            }

            esfNode.TypeCode = typeCode;
            return esfNode;
        }

        public EsfNode ReadValueNode(BinaryReader reader, EsfType typeCode)
        {
            EsfNode esfNode = CreateValueNode(typeCode);
            (esfNode as ICodecNode).Decode(reader, typeCode);
            return esfNode;
        }

        public virtual void WriteValueNode(BinaryWriter writer, EsfNode node)
        {
            if (node is ICodecNode)
            {
                (node as ICodecNode).Encode(writer);
                return;
            }

            throw new InvalidDataException($"Invalid type code {node.TypeCode:x} at {writer.BaseStream.Position:x}");
        }

        protected virtual EsfNode CreateArrayNode(EsfType typeCode)
        {
            EsfNode esfNode = null;
            switch (typeCode)
            {
                case EsfType.BOOL_ARRAY:
                    esfNode = new EsfArrayNode<bool>(this, typeCode);
                    break;
                case EsfType.INT8_ARRAY:
                    esfNode = new EsfArrayNode<sbyte>(this, typeCode);
                    break;
                case EsfType.INT16_ARRAY:
                    esfNode = new EsfArrayNode<short>(this, typeCode);
                    break;
                case EsfType.INT32_ARRAY:
                    esfNode = new EsfArrayNode<int>(this, typeCode);
                    break;
                case EsfType.INT64_ARRAY:
                    esfNode = new EsfArrayNode<long>(this, typeCode);
                    break;
                case EsfType.UINT8_ARRAY:
                    esfNode = new EsfArrayNode<byte>(this, typeCode);
                    break;
                case EsfType.UINT16_ARRAY:
                    esfNode = new EsfArrayNode<ushort>(this, typeCode);
                    break;
                case EsfType.UINT32_ARRAY:
                    esfNode = new EsfArrayNode<uint>(this, typeCode);
                    break;
                case EsfType.UINT64_ARRAY:
                    esfNode = new EsfArrayNode<ulong>(this, typeCode);
                    break;
                case EsfType.SINGLE_ARRAY:
                    esfNode = new EsfArrayNode<float>(this, typeCode);
                    break;
                case EsfType.DOUBLE_ARRAY:
                    esfNode = new EsfArrayNode<double>(this, typeCode);
                    break;
                case EsfType.COORD2D_ARRAY:
                    esfNode = new EsfArrayNode<Tuple<float, float>>(this, typeCode)
                    {
                        Separator = "-"
                    };
                    break;
                case EsfType.COORD3D_ARRAY:
                    esfNode = new EsfArrayNode<Tuple<float, float>>(this, typeCode)
                    {
                        Separator = "-"
                    };
                    break;
                case EsfType.UTF16_ARRAY:
                    esfNode = new EsfArrayNode<string>(this, EsfType.UTF16_ARRAY);
                    break;
                case EsfType.ASCII_ARRAY:
                    esfNode = new EsfArrayNode<string>(this, EsfType.ASCII_ARRAY);
                    break;
                case EsfType.ANGLE_ARRAY:
                    esfNode = new EsfArrayNode<ushort>(this, typeCode);
                    break;
                default:
                    throw new InvalidDataException($"Unknown array type code {typeCode}");
            }

            esfNode.TypeCode = typeCode;
            return esfNode;
        }

        protected virtual EsfNode ReadArrayNode(BinaryReader reader, EsfType typeCode)
        {
            EsfNode esfNode;
            try
            {
                esfNode = CreateArrayNode(typeCode);
            }
            catch (Exception)
            {
                throw new InvalidDataException(string.Format("{0} at {1:x}", reader.BaseStream.Position));
            }

            (esfNode as ICodecNode).Decode(reader, typeCode);
            return esfNode;
        }

        protected virtual byte[] ReadArray(BinaryReader reader)
        {
            int count = ReadSize(reader);
            return reader.ReadBytes(count);
        }

        protected void WriteArrayNode(BinaryWriter writer, EsfNode arrayNode)
        {
            (arrayNode as ICodecNode).Encode(writer);
        }

        public virtual RecordNode ReadRecordNode(BinaryReader reader, byte typeCode, bool forceDecode = false)
        {
            RecordNode recordNode = (forceDecode || buffer == null)
                ? new RecordNode(this, typeCode)
                : new MemoryMappedRecordNode(this, buffer, (int) reader.BaseStream.Position);
            recordNode.Decode(reader, EsfType.RECORD);
            return recordNode;
        }

        protected virtual void WriteRecordNode(BinaryWriter writer, EsfNode node)
        {
            (node as RecordNode).Encode(writer);
        }

        protected virtual EsfNode ReadRecordArrayNode(BinaryReader reader, byte typeCode)
        {
            RecordArrayNode recordArrayNode = new RecordArrayNode(this, 0);
            recordArrayNode.Decode(reader, EsfType.RECORD_BLOCK);
            return recordArrayNode;
        }

        protected void WriteRecordArrayNode(BinaryWriter writer, EsfNode node)
        {
            RecordArrayNode recordArrayNode = node as RecordArrayNode;
            if (recordArrayNode != null)
            {
                recordArrayNode.Encode(writer);
                return;
            }

            throw new InvalidOperationException();
        }

        protected virtual void ReadNodeNames(BinaryReader reader)
        {
            int num = reader.ReadInt16();
            nodeNames = new SortedList<int, string>(num);
            for (ushort num2 = 0; num2 < num; num2 = (ushort) (num2 + 1))
            {
                nodeNames.Add(num2, ReadAscii(reader));
            }
        }

        protected virtual void WriteNodeNames(BinaryWriter writer)
        {
            writer.Write((short) nodeNames.Count);
            for (int i = 0; i < nodeNames.Count; i++)
            {
                WriteAscii(writer, nodeNames[i]);
            }
        }

        public virtual int ReadCount(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public virtual int ReadSize(BinaryReader reader)
        {
            return (int) (ReadCount(reader) - reader.BaseStream.Position);
        }

        public virtual void WriteSize(BinaryWriter writer, long size)
        {
            writer.Write((uint) size);
        }

        public virtual void WriteOffset(BinaryWriter writer, long offset)
        {
            WriteSize(writer, offset + writer.BaseStream.Position + 4);
        }

        public virtual void ReadRecordInfo(BinaryReader reader, byte typeCode, out string name, out byte version)
        {
            ushort nameIndex = reader.ReadUInt16();
            name = GetNodeName(nameIndex);
            version = reader.ReadByte();
        }

        public virtual void WriteRecordInfo(BinaryWriter writer, byte typeCode, string name, byte version)
        {
            writer.Write(typeCode);
            ushort nodeNameIndex = GetNodeNameIndex(name);
            writer.Write(nodeNameIndex);
            writer.Write(version);
        }

        public virtual List<EsfNode> ReadToOffset(BinaryReader reader, long targetOffset)
        {
            List<EsfNode> list = new List<EsfNode>();
            while (reader.BaseStream.Position < targetOffset)
            {
                list.Add(Decode(reader));
            }

            return list;
        }

        public virtual void EncodeSized(BinaryWriter writer, List<EsfNode> nodes, bool writeCount = false)
        {
            long position = writer.BaseStream.Position;
            writer.Seek(4, SeekOrigin.Current);
            if (writeCount)
            {
                WriteSize(writer, nodes.Count);
            }

            Encode(writer, nodes);
            long position2 = writer.BaseStream.Position;
            writer.BaseStream.Seek(position, SeekOrigin.Begin);
            WriteSize(writer, position2);
            writer.BaseStream.Seek(position2, SeekOrigin.Begin);
        }

        public void Encode(BinaryWriter writer, List<EsfNode> nodes)
        {
            foreach (EsfNode node in nodes)
            {
                Encode(writer, node);
            }
        }

        public string GetNodeName(ushort nameIndex)
        {
            try
            {
                return nodeNames[nameIndex];
            }
            catch
            {
                Console.WriteLine($"Exception: invalid node index {nameIndex}");
                throw;
            }
        }

        public ushort GetNodeNameIndex(string nodename)
        {
            return (ushort) nodeNames.IndexOfValue(nodename);
        }

        public NodeRead CreateEventDelegate()
        {
            NodeRead result = null;
            if (this.NodeReadFinished != null)
            {
                result = delegate(EsfNode node, long position) { this.NodeReadFinished(node, position); };
            }

            return result;
        }
    }
}