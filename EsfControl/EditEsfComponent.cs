using EsfLibrary;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EsfControl
{
    public class EditEsfComponent : UserControl
    {
        public delegate void Selected(EsfNode node);

        private IContainer components;

        private SplitContainer splitContainer1;

        private TreeView esfNodeTree;

        private DataGridView nodeValueGridView;

        private DataGridViewTextBoxColumn valueColumn;

        private DataGridViewTextBoxColumn typeColumn;

        private DataGridViewTextBoxColumn Code;

        private TreeEventHandler treeEventHandler;

        private EsfTreeNode rootNode;

        private bool showCode;

        public EsfNode RootNode
        {
            get
            {
                if (rootNode == null)
                {
                    return null;
                }

                return rootNode.Tag as EsfNode;
            }
            set
            {
                esfNodeTree.Nodes.Clear();
                if (value != null)
                {
                    rootNode = new EsfTreeNode(value as ParentNode);
                    rootNode.ShowCode = ShowCode;
                    esfNodeTree.Nodes.Add(rootNode);
                    rootNode.Fill();
                    nodeValueGridView.Rows.Clear();
                    value.Modified = false;
                }
            }
        }

        public bool ShowCode
        {
            get { return showCode; }
            set
            {
                showCode = value;
                if (esfNodeTree.Nodes.Count > 0)
                {
                    (esfNodeTree.Nodes[0] as EsfTreeNode).ShowCode = value;
                    nodeValueGridView.Columns["Code"].Visible = value;
                }
            }
        }

        public string SelectedPath
        {
            get
            {
                string text = "";
                for (EsfNode esfNode = esfNodeTree.SelectedNode.Tag as EsfNode;
                    esfNode != null;
                    esfNode = esfNode.Parent)
                {
                    INamedNode namedNode = esfNode as INamedNode;
                    if (namedNode is CompressedNode)
                    {
                        text = text.Substring(text.IndexOf('/') + 1);
                    }

                    if (!(namedNode is MemoryMappedRecordNode) || string.IsNullOrEmpty(text))
                    {
                        text = $"{namedNode.GetName()}/{text}";
                        Console.WriteLine("node {0} - {1}", namedNode.GetName(), esfNode.GetType());
                    }
                }

                return text;
            }
            set
            {
                string[] array = value.Split(new char[1]
                {
                    '/'
                }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode treeNode = rootNode;
                rootNode.Expand();
                for (int i = 1; i < array.Length; i++)
                {
                    treeNode = FindNode(treeNode.Nodes, array[i]);
                    if (treeNode != null)
                    {
                        treeNode.Expand();
                        continue;
                    }

                    Console.WriteLine("Cannot find {0} in {1}", array[i], array[i - 1]);
                    break;
                }

                if (treeNode != null)
                {
                    esfNodeTree.SelectedNode = treeNode;
                }
            }
        }

        public event Selected NodeSelected;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            esfNodeTree = new System.Windows.Forms.TreeView();
            nodeValueGridView = new System.Windows.Forms.DataGridView();
            valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Panel1.Controls.Add(esfNodeTree);
            splitContainer1.Panel2.Controls.Add(nodeValueGridView);
            splitContainer1.Size = new System.Drawing.Size(465, 371);
            splitContainer1.SplitterDistance = 154;
            splitContainer1.TabIndex = 1;
            esfNodeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            esfNodeTree.Location = new System.Drawing.Point(0, 0);
            esfNodeTree.Name = "esfNodeTree";
            esfNodeTree.Size = new System.Drawing.Size(154, 371);
            esfNodeTree.TabIndex = 0;
            nodeValueGridView.AllowUserToAddRows = false;
            nodeValueGridView.Anchor = (System.Windows.Forms.AnchorStyles.Top |
                                        System.Windows.Forms.AnchorStyles.Bottom |
                                        System.Windows.Forms.AnchorStyles.Left |
                                        System.Windows.Forms.AnchorStyles.Right);
            nodeValueGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            nodeValueGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            nodeValueGridView.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            nodeValueGridView.Columns.AddRange(valueColumn, typeColumn, Code);
            nodeValueGridView.Location = new System.Drawing.Point(3, 3);
            nodeValueGridView.Name = "nodeValueGridView";
            nodeValueGridView.RowHeadersVisible = false;
            nodeValueGridView.Size = new System.Drawing.Size(301, 341);
            nodeValueGridView.TabIndex = 0;
            valueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            valueColumn.HeaderText = "Value";
            valueColumn.Name = "valueColumn";
            typeColumn.HeaderText = "Type";
            typeColumn.Name = "typeColumn";
            typeColumn.ReadOnly = true;
            typeColumn.Width = 56;
            Code.HeaderText = "TypeCode";
            Code.Name = "Code";
            Code.Visible = false;
            Code.Width = 81;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(splitContainer1);
            base.Name = "EditEsfComponent";
            base.Size = new System.Drawing.Size(465, 371);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        public EditEsfComponent()
        {
            InitializeComponent();
            nodeValueGridView.Rows.Clear();
            treeEventHandler = new TreeEventHandler(nodeValueGridView, this);
            esfNodeTree.BeforeExpand += treeEventHandler.FillNode;
            esfNodeTree.AfterSelect += treeEventHandler.TreeNodeSelected;
            nodeValueGridView.CellValidating += validateCell;
            nodeValueGridView.CellEndEdit += cellEdited;
            MouseHandler @object = new MouseHandler();
            esfNodeTree.MouseUp += @object.ShowContextMenu;
            nodeValueGridView.CellClick += CellClicked;
        }

        private void CellClicked(object sender, DataGridViewCellEventArgs args)
        {
            if (args.ColumnIndex == 1)
            {
                Console.WriteLine("editing {0}", nodeValueGridView.Rows[args.RowIndex].Cells[0].Value);
            }
        }

        private void validateCell(object sender, DataGridViewCellValidatingEventArgs args)
        {
            EsfNode esfNode = nodeValueGridView.Rows[args.RowIndex].Tag as EsfNode;
            if (esfNode != null)
            {
                string text = args.FormattedValue.ToString();
                try
                {
                    if (args.ColumnIndex == 0 && text != esfNode.ToString())
                    {
                        esfNode.FromString(text);
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid value {0}", text);
                    args.Cancel = true;
                }
            }
            else
            {
                nodeValueGridView.Rows[args.RowIndex].ErrorText = "Cannot edit this value";
            }
        }

        private void cellEdited(object sender, DataGridViewCellEventArgs args)
        {
            nodeValueGridView.Rows[args.RowIndex].ErrorText = string.Empty;
        }

        public void NotifySelection(EsfNode node)
        {
            if (this.NodeSelected != null)
            {
                this.NodeSelected(node);
            }
        }

        private TreeNode FindNode(TreeNodeCollection collection, string pathSegment)
        {
            foreach (TreeNode item in collection)
            {
                if (item.Text.Equals(pathSegment))
                {
                    return item;
                }
            }

            return null;
        }
    }
}