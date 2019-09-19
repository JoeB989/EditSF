// EsfLibrary.NodeIterator

namespace EsfLibrary
{
    public abstract class NodeIterator
    {
        public delegate bool Visitor(EsfNode node);

        public virtual Visitor Visit { get; set; }

        public abstract bool Iterate(EsfNode node);
    }
}