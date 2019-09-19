using System;
using System.IO;

namespace EsfLibrary
{
    public class OptimizedIntNode : CodecNode<int>
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

                if (Value == int.MinValue)
                {
                    return EsfType.INT32;
                }

                EsfType result = EsfType.INT32_ZERO;
                int num = Math.Abs(Value);
                if ((num & 0x7F800000) != 0)
                {
                    result = EsfType.INT32;
                }
                else if ((num & 0x7F8000) != 0)
                {
                    result = EsfType.INT32_24BIT;
                }
                else if ((num & 0x7F80) != 0 || SingleByteMin)
                {
                    result = EsfType.INT32_SHORT;
                }
                else if (num > 0)
                {
                    result = EsfType.INT32_BYTE;
                }

                return result;
            }
            set { setType = value; }
        }

        public OptimizedIntNode()
            : base((Converter<int>) int.Parse)
        {
        }

        protected override int ReadValue(BinaryReader reader, EsfType readAs)
        {
            switch (readAs)
            {
                case EsfType.INT32_ZERO:
                    return 0;
                case EsfType.INT32_BYTE:
                    return reader.ReadSByte();
                case EsfType.INT32_SHORT:
                    return reader.ReadInt16();
                case EsfType.INT32_24BIT:
                    return ReadInt24(reader);
                case EsfType.INT32:
                    return reader.ReadInt32();
                default:
                    throw new InvalidOperationException();
            }
        }

        protected void WriteInt24(BinaryWriter writer)
        {
            uint num = (uint) Math.Abs(Value);
            if (Value < 0)
            {
                uint num2 = 8388608u;
                num += num2;
            }

            uint num3 = 16711680u;
            for (int num4 = 16; num4 >= 0; num4 -= 8)
            {
                byte value = (byte) ((num3 & num) >> num4);
                writer.Write(value);
                num3 >>= 8;
            }
        }

        private int ReadInt24(BinaryReader reader)
        {
            int num = reader.ReadByte();
            bool flag = (num & 0x80) != 0;
            num &= 0x7F;
            for (int i = 0; i < 2; i++)
            {
                num = (num << 8) + reader.ReadByte();
            }

            if (flag)
            {
                num = -num;
            }

            return num;
        }

        public override void WriteValue(BinaryWriter writer)
        {
            switch (TypeCode)
            {
                case EsfType.INT32_ZERO:
                    break;
                case EsfType.INT32_BYTE:
                    writer.Write((sbyte) Value);
                    break;
                case EsfType.INT32_SHORT:
                    writer.Write((short) Value);
                    break;
                case EsfType.INT32_24BIT:
                    WriteInt24(writer);
                    break;
                case EsfType.INT32:
                    writer.Write(Value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override EsfNode CreateCopy()
        {
            return new OptimizedIntNode
            {
                Value = Value
            };
        }
    }
}