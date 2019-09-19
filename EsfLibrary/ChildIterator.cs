namespace EsfLibrary
{
    public class ChildIterator : NodeIterator
    {
        public override bool Iterate(EsfNode node)
        {
            bool num = Visit(node);
            if (num)
            {
                (node as ParentNode)?.AllNodes.ForEach(delegate(EsfNode n) { Iterate(n); });
            }

            return num;
        }
    }
}