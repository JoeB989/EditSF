using System.Collections.Generic;
using System.IO;
using SevenZip.Compression.LZMA;

namespace EsfLibrary
{
    public class CompressedNode : DelegatingNode
    {
        private RecordNode compressedNode;

        public static readonly string TAG_NAME = "COMPRESSED_DATA";

        public static readonly string INFO_TAG = "COMPRESSED_DATA_INFO";

        public CompressedNode(EsfCodec codec, RecordNode rootNode)
            : base(codec, 0)
        {
            base.Name = TAG_NAME;
            compressedNode = rootNode;
        }

        public void Decode(BinaryReader reader)
        {
        }

        protected override RecordNode DecodeDelegate()
        {
            byte[] value = (compressedNode.Values[0] as EsfValueNode<byte[]>).Value;
            ParentNode parentNode = compressedNode.Children[0];
            uint value2 = (parentNode.Values[0] as EsfValueNode<uint>).Value;
            byte[] value3 = (parentNode.Values[1] as EsfValueNode<byte[]>).Value;
            Decoder decoder = new Decoder();
            decoder.SetDecoderProperties(value3);
            byte[] array = new byte[value2];
            using (MemoryStream inStream = new MemoryStream(value, writable: false))
            {
                using (MemoryStream memoryStream = new MemoryStream(array))
                {
                    decoder.Code(inStream, memoryStream, value.Length, value2, null);
                    array = memoryStream.ToArray();
                }
            }

            AbcaFileCodec abcaFileCodec = new AbcaFileCodec();
            EsfNode esfNode = abcaFileCodec.Parse(array);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(array)))
            {
                esfNode = abcaFileCodec.Parse(reader);
            }

            return esfNode as RecordNode;
        }

        public override void Encode(BinaryWriter writer)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] array;
            using (BinaryWriter writer2 = new BinaryWriter(memoryStream))
            {
                base.Decoded.Codec.EncodeRootNode(writer2, base.Decoded);
                array = memoryStream.ToArray();
            }

            uint value = (uint) array.LongLength;
            MemoryStream memoryStream2 = new MemoryStream();
            Encoder encoder = new Encoder();
            using (memoryStream = new MemoryStream(array))
            {
                encoder.Code(memoryStream, memoryStream2, array.Length, long.MaxValue, null);
                array = memoryStream2.ToArray();
            }

            List<EsfNode> list = new List<EsfNode>();
            list.Add(new UIntNode
            {
                Value = value,
                TypeCode = EsfType.UINT32,
                Codec = base.Codec
            });
            using (MemoryStream memoryStream4 = new MemoryStream())
            {
                encoder.WriteCoderProperties(memoryStream4);
                list.Add(new RawDataNode(base.Codec)
                {
                    Value = memoryStream4.ToArray()
                });
            }

            List<EsfNode> list2 = new List<EsfNode>();
            list2.Add(new RawDataNode(base.Codec)
            {
                Value = array
            });
            list2.Add(new RecordNode(base.Codec, 0)
            {
                Name = INFO_TAG,
                Value = list
            });
            RecordNode recordNode = new RecordNode(base.Codec, 0);
            recordNode.Name = TAG_NAME;
            recordNode.Value = list2;
            recordNode.Encode(writer);
        }
    }
}