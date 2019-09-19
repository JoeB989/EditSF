using System.IO;

namespace EsfLibrary
{
    public abstract class DelegatingDecoderNode<T> : CodecNode<T>
    {
        protected ValueReader<T> Read;

        protected ValueWriter<T> Write;

        public DelegatingDecoderNode(Converter<T> conv, ValueReader<T> reader, ValueWriter<T> writer)
            : base(conv)
        {
            Read = reader;
            Write = writer;
        }

        protected override T ReadValue(BinaryReader reader, EsfType readAs)
        {
            return Read(reader);
        }

        public override void WriteValue(BinaryWriter writer)
        {
            Write(writer, Value);
        }
    }
}