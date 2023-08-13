using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonDialogs;
using EsfLibrary;

namespace EsfControl
{
    public class MouseHandler
    {
        public delegate void NodeAction(EsfNode node);

        public void ShowContextMenu(object sender, MouseEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (e.Button == MouseButtons.Right && treeView != null)
            {
                Point point = new Point(e.X, e.Y);
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                TreeNode nodeAt = treeView.GetNodeAt(point);
                ParentNode parentNode = (nodeAt != null) ? (nodeAt.Tag as ParentNode) : null;
                if (parentNode != null && (nodeAt.Tag as EsfNode).Parent is RecordArrayNode)
                {
                    treeView.SelectedNode = nodeAt;
                    ToolStripItem value = CreateMenuItem("Duplicate", parentNode, CopyNode);
                    contextMenuStrip.Items.Add(value);
                    value = CreateMenuItem("Delete", parentNode, DeleteNode);
                    contextMenuStrip.Items.Add(value);
                    value = CreateMenuItem("Move", parentNode, MoveNode);
                    contextMenuStrip.Items.Add(value);
                }
                else if ((nodeAt.Tag is RecordNode) && ((nodeAt.Tag as RecordNode).Name == "TRAITS"))
                {
                    var traitNode = parentNode.Children[0];
                    if ((traitNode != null) && (traitNode.Children.Count > 0))
                    {
                        treeView.SelectedNode = nodeAt;
                        ToolStripItem item = CreateMenuItem("Show Traits ...", traitNode, ShowTraits);
                        contextMenuStrip.Items.Add(item);
                    }
                }

                if (contextMenuStrip.Items.Count != 0)
                {
                    contextMenuStrip.Show(treeView, point);
                }
            }
        }

        private ToolStripMenuItem CreateMenuItem(string label, EsfNode node, NodeAction action)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(label);
            toolStripMenuItem.Click += delegate { action(node); };
            return toolStripMenuItem;
        }

        private void CopyNode(EsfNode node)
        {
            ParentNode parentNode = node as ParentNode;
            ParentNode parentNode2 = parentNode.CreateCopy() as ParentNode;
            if (parentNode2 != null)
            {
                ParentNode parentNode3 = parentNode.Parent as ParentNode;
                if (parentNode3 != null)
                {
                    List<EsfNode> list = new List<EsfNode>(parentNode3.Value);
                    int index = parentNode3.Children.IndexOf(parentNode) + 1;
                    list.Insert(index, parentNode2);
                    parentNode3.Value = list;
                    parentNode2.Modified = true;
                    parentNode2.AllNodes.ForEach(delegate(EsfNode n) { n.Modified = false; });
                }
            }
        }

        private void DeleteNode(EsfNode node)
        {
            RecordArrayNode recordArrayNode = node.Parent as RecordArrayNode;
            if (recordArrayNode != null)
            {
                List<EsfNode> list = new List<EsfNode>(recordArrayNode.Value);
                list.Remove(node);
                recordArrayNode.Value = list;
            }
        }

        private void MoveNode(EsfNode node)
        {
            RecordArrayNode recordArrayNode = node.Parent as RecordArrayNode;
            if (recordArrayNode == null)
            {
                return;
            }

            InputBox inputBox = new InputBox();
            inputBox.Input = "Move to index";
            InputBox inputBox2 = inputBox;
            if (inputBox2.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            int result = -1;
            List<EsfNode> list = new List<EsfNode>(recordArrayNode.Value);
            if (int.TryParse(inputBox2.Input, out result))
            {
                if (result >= 0 && result < list.Count)
                {
                    list.Remove(node);
                    list.Insert(result, node);
                    recordArrayNode.Value = list;
                }
                else
                {
                    MessageBox.Show($"Entry only valid between 0 and {list.Count - 1}", "Invalid input",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                MessageBox.Show($"Enter index (between 0 and {list.Count - 1})", "Invalid input", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
        }

        private void ShowTraits(EsfNode node)
        {
            var traitNode = (node as ParentNode);
            StringBuilder traits = new StringBuilder();
            foreach (RecordEntryNode trait in traitNode.Children)
			{
                traits.AppendFormat("{0} = {1}", trait.Values[0], trait.Values[1]);
                traits.AppendLine();
			}
            var ret = MessageBox.Show(traits.ToString(), "Click OK to copy traits to clipboard", MessageBoxButtons.OKCancel);
            if (ret == DialogResult.OK)
			{
                Clipboard.SetText(traits.ToString());
			}
        }
    }
}