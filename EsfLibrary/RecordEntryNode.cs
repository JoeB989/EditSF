using System.Diagnostics;
using System.IO;

namespace EsfLibrary
{
    [DebuggerDisplay("Record Entry: {Name}")]
    public class RecordEntryNode : ParentNode, ICodecNode, INamedNode
    {
        public RecordEntryNode(EsfCodec codec)
        {
            TypeCode = EsfType.RECORD_BLOCK_ENTRY;
            base.Codec = codec;
        }

        public void Encode(BinaryWriter writer)
        {
            base.Codec.EncodeSized(writer, base.AllNodes);
        }

        public void Decode(BinaryReader reader, EsfType unused)
        {
            base.Size = base.Codec.ReadSize(reader);
            Value = base.Codec.ReadToOffset(reader, reader.BaseStream.Position + base.Size);
        }

        public override EsfNode CreateCopy()
        {
            RecordEntryNode recordEntryNode = new RecordEntryNode(base.Codec);
            CopyMembers(recordEntryNode);
            return recordEntryNode;
        }
    }
}