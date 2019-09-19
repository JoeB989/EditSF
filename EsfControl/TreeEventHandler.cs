using EsfLibrary;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace EsfControl
{
    public class TreeEventHandler
    {
        private List<ModificationColorizer> registeredEvents = new List<ModificationColorizer>();

        private EditEsfComponent component;

        private DataGridView nodeValueGridView;

        public TreeEventHandler(DataGridView view, EditEsfComponent c)
        {
            nodeValueGridView = view;
            component = c;
        }

        public void FillNode(object sender, TreeViewCancelEventArgs args)
        {
            foreach (TreeNode node in args.Node.Nodes)
            {
                (node as EsfTreeNode)?.Fill();
            }
        }

        public void TreeNodeSelected(object sender, TreeViewEventArgs args)
        {
            ParentNode parentNode = args.Node.Tag as ParentNode;
            try
            {
                nodeValueGridView.Rows.Clear();
                registeredEvents.ForEach(delegate(ModificationColorizer handler)
                {
                    (handler.row.Tag as EsfNode).ModifiedEvent -= handler.ChangeColor;
                });
                registeredEvents.Clear();
                foreach (EsfNode value in parentNode.Values)
                {
                    int index = nodeValueGridView.Rows.Add(value.ToString(), value.SystemType.ToString(), value.TypeCode.ToString());
                    DataGridViewRow dataGridViewRow = nodeValueGridView.Rows[index];
                    ModificationColorizer modificationColorizer = new ModificationColorizer(dataGridViewRow);
                    registeredEvents.Add(modificationColorizer);
                    foreach (DataGridViewCell cell in dataGridViewRow.Cells)
                    {
                        cell.Style.ForeColor = (value.Modified ? Color.Red : Color.Black);
                    }
                    value.ModifiedEvent += modificationColorizer.ChangeColor;
                    dataGridViewRow.Tag = value;
                }
                component.NotifySelection(parentNode);
            }
            catch
            {
            }
        }
    }

}