using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EsfLibrary
{
    [DebuggerDisplay("RecordArray: {Name}")]
    public class RecordArrayNode : ParentNode, ICodecNode
    {
        public override List<EsfNode> Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                for (int i = 0; i < value.Count; i++)
                {
                    (Value[i] as RecordEntryNode).Name = $"{base.Name} - {i}";
                }
            }
        }

        public override EsfType TypeCode
        {
            get { return EsfType.RECORD_BLOCK; }
            set { }
        }

        public RecordArrayNode(EsfCodec codec, byte originalCode = 0)
            : base(originalCode)
        {
            base.Codec = codec;
        }

        public void Decode(BinaryReader reader, EsfType unused)
        {
            base.Codec.ReadRecordInfo(reader, base.OriginalTypeCode, out string name, out byte version);
            base.Name = name;
            base.Version = version;
            base.Size = base.Codec.ReadSize(reader);
            int num = base.Codec.ReadCount(reader);
            List<EsfNode> list = new List<EsfNode>(num);
            for (int i = 0; i < num; i++)
            {
                RecordEntryNode recordEntryNode = new RecordEntryNode(base.Codec)
                {
                    Name = $"{base.Name} - {i}",
                    TypeCode = EsfType.RECORD_BLOCK_ENTRY
                };
                recordEntryNode.Decode(reader, EsfType.RECORD_BLOCK_ENTRY);
                list.Add(recordEntryNode);
            }

            Value = list;
        }

        public void Encode(BinaryWriter writer)
        {
            base.Codec.WriteRecordInfo(writer, (byte) TypeCode, base.Name, base.Version);
            base.Codec.EncodeSized(writer, base.AllNodes, writeCount: true);
        }

        public override EsfNode CreateCopy()
        {
            RecordArrayNode recordArrayNode = new RecordArrayNode(base.Codec, base.OriginalTypeCode);
            CopyMembers(recordArrayNode);
            return recordArrayNode;
        }
    }
}