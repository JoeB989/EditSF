// EsfLibrary.MemoryMappedRecordNode
using EsfLibrary;
using System.IO;
namespace EsfLibrary
{
}
public class MemoryMappedRecordNode : DelegatingNode
{
	private byte[] buffer;

	private int mapStart;

	private int byteCount;

	private bool invalid;

	public bool InvalidateSiblings
	{
		get;
		set;
	}

	public bool Invalid
	{
		get
		{
			return invalid;
		}
		set
		{
			if (invalid == value)
			{
				return;
			}
			invalid = value;
			if (value)
			{
				ParentIterator parentIterator = new ParentIterator();
				parentIterator.Visit = Invalidate;
				parentIterator.Iterate(base.Parent);
				if (InvalidateSiblings)
				{
					SiblingIterator siblingIterator = new SiblingIterator();
					siblingIterator.Visit = InvalidateAll;
					siblingIterator.Iterate(this);
					ChildIterator childIterator = new ChildIterator();
					childIterator.Visit = InvalidateAll;
					childIterator.Iterate(base.Decoded);
				}
			}
		}
	}

	public override bool Modified
	{
		get
		{
			return base.Modified;
		}
		set
		{
			base.Modified = value;
			Invalid |= Modified;
		}
	}

	public MemoryMappedRecordNode(EsfCodec codec, byte[] bytes, int start)
		: base(codec, bytes[start - 1])
	{
		base.Codec = codec;
		buffer = bytes;
		mapStart = start - 1;
		InvalidateSiblings = true;
	}

	private bool Invalidate(EsfNode node)
	{
		MemoryMappedRecordNode memoryMappedRecordNode = node as MemoryMappedRecordNode;
		bool flag = memoryMappedRecordNode == null;
		if (memoryMappedRecordNode != null)
		{
			memoryMappedRecordNode.Invalid = true;
		}
		return !flag;
	}

	private bool InvalidateAll(EsfNode node)
	{
		Invalidate(node);
		return true;
	}

	protected override RecordNode DecodeDelegate()
	{
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
		{
			binaryReader.BaseStream.Seek(mapStart + 1, SeekOrigin.Begin);
			return base.Codec.ReadRecordNode(binaryReader, base.OriginalTypeCode, forceDecode: true);
		}
	}

	public override void Decode(BinaryReader reader, EsfType type)
	{
		base.Codec.ReadRecordInfo(reader, base.OriginalTypeCode, out string name, out byte version);
		base.Name = name;
		base.Version = version;
		int num = base.Codec.ReadSize(reader);
		int num2 = (int)(reader.BaseStream.Position - mapStart);
		byteCount = num + num2;
		reader.BaseStream.Seek(num, SeekOrigin.Current);
	}

	public override void Encode(BinaryWriter writer)
	{
		if (Invalid)
		{
			base.Decoded.Encode(writer);
		}
		else
		{
			writer.Write(buffer, mapStart, byteCount);
		}
	}

	public override EsfNode CreateCopy()
	{
		if (!Invalid)
		{
			return new MemoryMappedRecordNode(base.Codec, buffer, mapStart + 1)
			{
				Name = base.Name
			};
		}
		return base.Decoded.CreateCopy();
	}
}
