using System.Diagnostics;
using System.IO;

namespace EsfLibrary
{
    [DebuggerDisplay("Record: {Name}")]
    public class RecordNode : ParentNode, ICodecNode
    {
        public override EsfType TypeCode
        {
            get { return EsfType.RECORD; }
            set { }
        }

        public RecordNode(EsfCodec codec, byte originalCode = 0)
            : base(originalCode)
        {
            base.Codec = codec;
        }

        public virtual void Encode(BinaryWriter writer)
        {
            base.Codec.WriteRecordInfo(writer, (byte) TypeCode, base.Name, base.Version);
            base.Codec.EncodeSized(writer, base.AllNodes);
        }

        public virtual void Decode(BinaryReader reader, EsfType unused)
        {
            base.Codec.ReadRecordInfo(reader, base.OriginalTypeCode, out string name, out byte version);
            base.Name = name;
            base.Version = version;
            base.Size = base.Codec.ReadSize(reader);
            Value = base.Codec.ReadToOffset(reader, reader.BaseStream.Position + base.Size);
        }

        public override string ToString()
        {
            return base.Name;
        }

        public override EsfNode CreateCopy()
        {
            RecordNode recordNode = new RecordNode(base.Codec, base.OriginalTypeCode);
            CopyMembers(recordNode);
            return recordNode;
        }
    }
}