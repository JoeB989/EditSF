namespace EsfLibrary
{
    public class ParentIterator : NodeIterator
    {
        public override bool Iterate(EsfNode node)
        {
            bool num = node != null && Visit(node);
            if (num)
            {
                Iterate(node.Parent as ParentNode);
            }

            return num;
        }
    }
}