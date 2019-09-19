using System;

namespace EsfLibrary
{
    public class OptimizedArrayNode<T> : EsfArrayNode<T>
    {
        public delegate EsfValueNode<N> NodeFactory<N>(N val);

        private NodeFactory<T> CreateNode;

        private EsfType optimizedType;

        public override EsfType TypeCode
        {
            get
            {
                if (optimizedType == EsfType.INVALID)
                {
                    byte b = 0;
                    T[] value = Value;
                    foreach (T val in value)
                    {
                        EsfValueNode<T> esfValueNode = CreateNode(val);
                        b = Math.Max(b, (byte) esfValueNode.TypeCode);
                    }

                    optimizedType = (EsfType) (b + 64);
                }

                return optimizedType;
            }
            set { base.TypeCode = value; }
        }

        public override T[] Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                if (value.Length != 0)
                {
                    optimizedType = EsfType.INVALID;
                }
                else
                {
                    optimizedType = base.TypeCode;
                }
            }
        }

        public OptimizedArrayNode(EsfCodec codec, EsfType typeCode, NodeFactory<T> factory)
            : base(codec, typeCode)
        {
            CreateNode = factory;
            optimizedType = typeCode;
        }
    }
}