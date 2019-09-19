using System;
using System.IO;

namespace EsfLibrary
{
    public abstract class EsfNode
    {
        public delegate void Modification(EsfNode node);

        private EsfNode parent;

        private bool deleted;

        protected bool modified;

        public EsfCodec Codec { get; set; }

        public virtual EsfType TypeCode { get; set; }

        public EsfNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Type SystemType { get; set; }

        public bool Deleted
        {
            get { return deleted; }
            set
            {
                deleted = value;
                Modified = true;
            }
        }

        public virtual bool Modified
        {
            get { return modified; }
            set
            {
                if (modified != value)
                {
                    modified = value;
                    RaiseModifiedEvent();
                    if (modified && Parent != null)
                    {
                        Parent.Modified = value;
                    }
                }
            }
        }

        public event Modification ModifiedEvent;

        protected void RaiseModifiedEvent()
        {
            if (this.ModifiedEvent != null)
            {
                this.ModifiedEvent(this);
            }
        }

        public virtual void FromString(string value)
        {
            throw new InvalidOperationException();
        }

        public abstract void ToXml(TextWriter writer, string indent);

        public abstract EsfNode CreateCopy();
    }
}