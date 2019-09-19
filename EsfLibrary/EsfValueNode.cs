using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace EsfLibrary
{
    [DebuggerDisplay("ValueNode: {TypeCode}")]
    public class EsfValueNode<T> : EsfNode
    {
        public delegate S Converter<S>(string value);

        protected Converter<T> ConvertString;

        private T val;

        public virtual T Value
        {
            get
            {
                return val;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(val, value))
                {
                    val = value;
                    Modified = true;
                }
            }
        }

        public EsfValueNode(T value)
        {
            val = value;
        }

        public EsfValueNode()
            : this((Converter<T>)null)
        {
        }

        public EsfValueNode(Converter<T> converter)
        {
            base.SystemType = typeof(T);
            ConvertString = converter;
        }

        public override void FromString(string value)
        {
            Value = ConvertString(value);
        }

        public override string ToString()
        {
            return val.ToString();
        }

        public override bool Equals(object o)
        {
            bool result = false;
            try
            {
                T value = (o as EsfValueNode<T>).Value;
                result = (value != null && EqualityComparer<T>.Default.Equals(val, value));
                return result;
            }
            catch
            {
                return result;
            }
        }

        public override EsfNode CreateCopy()
        {
            return new EsfValueNode<T>
            {
                TypeCode = TypeCode,
                Value = Value
            };
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override void ToXml(TextWriter writer, string indent)
        {
            writer.WriteLine(string.Format("{2}<{0} Value=\"{1}\"/>", TypeCode, Value, indent));
        }
    }
}
