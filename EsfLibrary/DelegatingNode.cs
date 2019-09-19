using System.Collections.Generic;

namespace EsfLibrary
{
    public abstract class DelegatingNode : RecordNode
    {
        protected RecordNode decoded;

        public override List<EsfNode> Value
        {
            get { return Decoded.Value; }
            set { Decoded.Value = value; }
        }

        protected RecordNode Decoded
        {
            get
            {
                if (decoded == null)
                {
                    decoded = DecodeDelegate();
                    decoded.Parent = this;
                    decoded.Modified = false;
                    decoded.ModifiedEvent += ModifiedDelegate;
                }

                return decoded;
            }
        }

        public override bool Modified
        {
            get { return base.Modified; }
            set
            {
                if (decoded != null)
                {
                    base.Modified = value;
                }
            }
        }

        protected DelegatingNode(EsfCodec codec, byte originalCode = 0)
            : base(codec, originalCode)
        {
        }

        protected abstract RecordNode DecodeDelegate();

        private void ModifiedDelegate(EsfNode node)
        {
            Modified = node.Modified;
        }
    }
}