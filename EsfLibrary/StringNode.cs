namespace EsfLibrary
{
    public class StringNode : DelegatingDecoderNode<string>
    {
        public StringNode(ValueReader<string> reader, ValueWriter<string> writer)
            : base((Converter<string>) ((string v) => v), reader, writer)
        {
        }

        public override EsfNode CreateCopy()
        {
            return new StringNode(Read, Write)
            {
                TypeCode = TypeCode,
                Value = Value
            };
        }
    }
}