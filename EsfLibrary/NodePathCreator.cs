// EsfLibrary.NodePathCreator

namespace EsfLibrary
{
    public class NodePathCreator
    {
        private string path = "";

        private string pathSeparator = "/";

        public string Path => path;

        public string PathSeparator
        {
            get { return pathSeparator; }
            set { pathSeparator = value; }
        }

        public static string CreatePath(EsfNode node, string separator = "/")
        {
            NodePathCreator nodePathCreator = new NodePathCreator
            {
                PathSeparator = separator
            };
            ParentIterator parentIterator = new ParentIterator();
            parentIterator.Visit = nodePathCreator.Visit;
            parentIterator.Iterate(node);
            return nodePathCreator.Path;
        }

        public bool Visit(EsfNode node)
        {
            INamedNode namedNode = node as INamedNode;
            if (namedNode is CompressedNode)
            {
                path = path.Substring(path.IndexOf(PathSeparator) + 1);
            }

            if (!(namedNode is MemoryMappedRecordNode) || string.IsNullOrEmpty(path))
            {
                path = $"{namedNode.GetName()}{PathSeparator}{path}";
            }

            return true;
        }
    }
}