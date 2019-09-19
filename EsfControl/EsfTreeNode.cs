using EsfLibrary;
using System.Drawing;
using System.Windows.Forms;

namespace EsfControl
{
    public class EsfTreeNode : TreeNode
    {
        private bool showCode;

        public bool ShowCode
        {
            get
            {
                return showCode;
            }
            set
            {
                ParentNode parentNode = base.Tag as ParentNode;
                if (parentNode != null)
                {
                    string name = ((INamedNode)parentNode).GetName();
                    base.Text = (value ? $"{name} - {parentNode.TypeCode}" : name);
                    showCode = value;
                    foreach (TreeNode node in base.Nodes)
                    {
                        (node as EsfTreeNode).ShowCode = value;
                    }
                }
            }
        }

        public EsfTreeNode(ParentNode node, bool showC = false)
        {
            base.Tag = node;
            base.Text = ((INamedNode)node).GetName();
            node.ModifiedEvent += NodeChange;
            base.ForeColor = (node.Modified ? Color.Red : Color.Black);
            ShowCode = showC;
            node.RenameEvent += delegate
            {
                base.Text = node.Name;
            };
        }

        public void Fill()
        {
            if (base.Nodes.Count == 0)
            {
                ParentNode parentNode = base.Tag as ParentNode;
                foreach (ParentNode child in parentNode.Children)
                {
                    EsfTreeNode node = new EsfTreeNode(child, ShowCode);
                    base.Nodes.Add(node);
                }
            }
        }

        public void NodeChange(EsfNode n)
        {
            base.ForeColor = (n.Modified ? Color.Red : Color.Black);
            ParentNode parentNode = base.Tag as ParentNode;
            bool flag = parentNode.Children.Count == base.Nodes.Count;
            int num = 0;
            while (flag && num < parentNode.Children.Count)
            {
                flag &= parentNode.Children[num].Name.Equals(base.Nodes[num].Text);
                num++;
            }
            if (parentNode == null)
            {
                return;
            }
            if (!flag)
            {
                base.Nodes.Clear();
                Fill();
                if (base.IsExpanded)
                {
                    foreach (TreeNode node in base.Nodes)
                    {
                        (node as EsfTreeNode).Fill();
                    }
                }
            }
            else
            {
                for (int i = 0; i < parentNode.Children.Count; i++)
                {
                    base.Nodes[i].Text = parentNode.Children[i].Name;
                }
            }
        }
    }

}