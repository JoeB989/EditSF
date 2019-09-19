// EsfLibrary.OptimizedUIntNode

using System;
using System.IO;

namespace EsfLibrary
{
    public class OptimizedUIntNode : CodecNode<uint>
    {
        private EsfType setType;

        public bool SingleByteMin { get; set; }

        public override EsfType TypeCode
        {
            get
            {
                if (setType != 0)
                {
                    return setType;
                }

                if (Value > 16777215)
                {
                    return EsfType.UINT32;
                }

                if (Value > 65535)
                {
                    return EsfType.UINT32_24BIT;
                }

                if (Value > 255)
                {
                    return EsfType.UINT32_SHORT;
                }

                if (Value > 1 || SingleByteMin)
                {
                    return EsfType.UINT32_BYTE;
                }

                return (Value == 1) ? EsfType.UINT32_ONE : EsfType.UINT32_ZERO;
            }
            set { setType = value; }
        }

        public OptimizedUIntNode()
            : base((Converter<uint>) uint.Parse)
        {
        }

        protected override uint ReadValue(BinaryReader reader, EsfType readAs)
        {
            switch (readAs)
            {
                case EsfType.UINT32_ZERO:
                    return 0u;
                case EsfType.UINT32_ONE:
                    return 1u;
                case EsfType.UINT32_BYTE:
                    return reader.ReadByte();
                case EsfType.UINT32_SHORT:
                    return reader.ReadUInt16();
                case EsfType.UINT32_24BIT:
                    return ReadUInt24(reader);
                case EsfType.UINT32:
                    return reader.ReadUInt32();
                default:
                    throw new InvalidOperationException();
            }
        }

        protected void WriteUInt24(BinaryWriter writer)
        {
            uint num = 16711680u;
            for (int num2 = 16; num2 >= 0; num2 -= 8)
            {
                byte value = (byte) ((num & Value) >> num2);
                writer.Write(value);
                num >>= 8;
            }
        }

        private uint ReadUInt24(BinaryReader reader)
        {
            uint num = 0u;
            for (int i = 0; i < 3; i++)
            {
                num = (num << 8) + reader.ReadByte();
            }

            return num;
        }

        public override void WriteValue(BinaryWriter writer)
        {
            switch (TypeCode)
            {
                case EsfType.UINT32_ZERO:
                case EsfType.UINT32_ONE:
                    break;
                case EsfType.UINT32_BYTE:
                    writer.Write((byte) Value);
                    break;
                case EsfType.UINT32_SHORT:
                    writer.Write((ushort) Value);
                    break;
                case EsfType.UINT32_24BIT:
                    WriteUInt24(writer);
                    break;
                case EsfType.UINT32:
                    writer.Write(Value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override EsfNode CreateCopy()
        {
            return new OptimizedUIntNode
            {
                Value = Value
            };
        }
    }
}