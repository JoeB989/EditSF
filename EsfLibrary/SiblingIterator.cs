namespace EsfLibrary
{
    public class SiblingIterator : NodeIterator
    {
        public override bool Iterate(EsfNode fromNode)
        {
            ParentNode parentNode = fromNode.Parent as ParentNode;
            bool flag = parentNode != null;
            if (flag)
            {
                bool flag2 = false;
                {
                    foreach (EsfNode allNode in parentNode.AllNodes)
                    {
                        if (flag2)
                        {
                            if (!Visit(allNode))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            flag2 = (allNode == fromNode);
                        }
                    }

                    return flag;
                }
            }

            return flag;
        }
    }
}