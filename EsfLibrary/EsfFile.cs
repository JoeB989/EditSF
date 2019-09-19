using System.IO;

namespace EsfLibrary
{
    public class EsfFile
    {
        public EsfNode RootNode { get; private set; }

        public EsfCodec Codec { get; set; }

        public EsfFile(EsfNode rootNode, EsfCodec codec)
        {
            Codec = codec;
            RootNode = rootNode;
        }

        public EsfFile(Stream stream, EsfCodec codec)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Codec = codec;
                RootNode = Codec.Parse(reader);
            }
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            EsfFile esfFile = obj as EsfFile;
            if (esfFile != null)
            {
                result = (Codec.ID == esfFile.Codec.ID);
                result &= (RootNode as ParentNode).Equals(esfFile.RootNode);
            }

            return result;
        }

        public override int GetHashCode()
        {
            return 2 * Codec.ID.GetHashCode() + 3 * RootNode.GetHashCode();
        }
    }
}