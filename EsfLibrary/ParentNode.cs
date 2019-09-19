using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EsfLibrary
{
    [DebuggerDisplay("ParentNode: {Name}")]
    public abstract class ParentNode : EsfValueNode<List<EsfNode>>, INamedNode
    {
        private string name;
        private byte originalCode;

        public string Name
        {
            get { return name; }
            set
            {
                bool num = !value.Equals(name);
                name = value;
                if (num && this.RenameEvent != null)
                {
                    this.RenameEvent(this);
                }
            }
        }

        public byte Version { get; protected set; }

        public int Size { get; set; }

        public override bool Modified
        {
            get { return modified; }
            set
            {
                base.Modified = value;
                if (!Modified)
                {
                    Value.ForEach(delegate(EsfNode node) { node.Modified = false; });
                }
            }
        }

        public byte OriginalTypeCode
        {
            get
            {
                if (originalCode != 0)
                {
                    return originalCode;
                }

                return (byte) TypeCode;
            }
            set { originalCode = value; }
        }

        public override List<EsfNode> Value
        {
            get { return base.Value; }
            set
            {
                if (!Value.Equals(value))
                {
                    Value.ForEach(delegate(EsfNode node) { node.Parent = null; });
                    base.Value = value;
                    Value.ForEach(delegate(EsfNode node) { node.Parent = this; });
                    if (!Modified)
                    {
                        Modified = true;
                    }
                    else
                    {
                        RaiseModifiedEvent();
                    }
                }
            }
        }

        public List<EsfNode> AllNodes => Value;

        public List<ParentNode> Children
        {
            get
            {
                List<ParentNode> result = new List<ParentNode>();
                Value.ForEach(delegate(EsfNode node)
                {
                    if (node is ParentNode)
                    {
                        result.Add(node as ParentNode);
                    }
                });
                return result;
            }
        }

        public List<EsfNode> Values
        {
            get
            {
                List<EsfNode> result = new List<EsfNode>();
                Value.ForEach(delegate(EsfNode node)
                {
                    if (!(node is ParentNode))
                    {
                        result.Add(node);
                    }
                });
                return result;
            }
        }

        public ParentNode this[string key]
        {
            get
            {
                ParentNode result = null;
                Children.ForEach(delegate(ParentNode child)
                {
                    if (child.Name == key)
                    {
                        result = child;
                    }
                });
                if (result == null)
                {
                    throw new IndexOutOfRangeException($"Unknown child {key}");
                }

                return result;
            }
        }

        public event Modification RenameEvent;

        public ParentNode()
            : base(new List<EsfNode>())
        {
        }

        public ParentNode(byte code)
            : this()
        {
            originalCode = code;
        }

        public string GetName()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            ParentNode parentNode = obj as ParentNode;
            if (parentNode != null)
            {
                flag = parentNode.Name.Equals(Name);
                flag &= (parentNode.AllNodes.Count == Value.Count);
                if (flag)
                {
                    for (int i = 0; i < parentNode.AllNodes.Count; i++)
                    {
                        flag &= parentNode.AllNodes[i].Equals(Value[i]);
                        if (!flag)
                        {
                            break;
                        }
                    }
                }
            }

            if (!flag)
            {
                return false;
            }

            return flag;
        }

        public override int GetHashCode()
        {
            return 2 * Name.GetHashCode() + 3 * AllNodes.GetHashCode();
        }

        public override string ToString()
        {
            return $"{TypeCode} {Name}";
        }

        public override void ToXml(TextWriter writer, string indent)
        {
            writer.WriteLine(string.Format("{2}<{0} name=\"{1}\">", TypeCode, Name, indent));
            string indent2 = indent + " ";
            foreach (ParentNode child in Children)
            {
                child.ToXml(writer, indent2);
            }

            foreach (EsfNode value in Values)
            {
                value.ToXml(writer, indent2);
            }

            writer.WriteLine(string.Format("{1}</{0}>", TypeCode, indent));
        }

        public virtual string ToXml(bool end)
        {
            if (!end)
            {
                return $"<{TypeCode} name=\"{Name}\">";
            }

            return $"</{TypeCode}>";
        }

        protected void CopyMembers(ParentNode node)
        {
            node.Name = Name;
            node.OriginalTypeCode = OriginalTypeCode;
            node.Size = Size;
            node.Version = Version;
            List<EsfNode> nodeCopy = new List<EsfNode>();
            Value.ForEach(delegate(EsfNode n) { nodeCopy.Add(n.CreateCopy()); });
            node.Value = nodeCopy;
        }
    }
}