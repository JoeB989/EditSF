using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonDialogs
{
    public class ListEditor : Form
    {
        private delegate void MoveToList(string item);

        private IContainer components;

        private Button okButton;

        private Button cancelButton;

        private Panel panel1;

        private Panel panel2;

        private ListBox rightListBox;

        private Label rightListLabel;

        private Panel panel3;

        private ListBox leftListBox;

        private Label leftListLabel;

        private Panel panel4;

        private Button toTheLeftButton;

        private Button toTheRightButton;

        private BindingSource bindingSourceLeft;

        private BindingSource bindingSourceRight;

        private Button moveAllToTheLeftButton;

        private Button moveAllToTheRightButton;

        public List<string> LeftList
        {
            get { return FromListBox(leftListBox); }
            set { ToListBox(leftListBox, value); }
        }

        public List<string> RightList
        {
            get { return FromListBox(rightListBox); }
            set { ToListBox(rightListBox, value); }
        }

        public List<string> OriginalOrder { get; set; }

        public string LeftLabel
        {
            set { leftListLabel.Text = value; }
        }

        public string RightLabel
        {
            set { rightListLabel.Text = value; }
        }

        public ListEditor()
        {
            InitializeComponent();
            base.AcceptButton = okButton;
            base.CancelButton = cancelButton;
        }

        private void toTheRightButton_Click(object sender, EventArgs e)
        {
            MoveSelected(leftListBox, rightListBox);
        }

        private void toTheLeftButton_Click(object sender, EventArgs e)
        {
            MoveSelected(rightListBox, leftListBox);
        }

        private void MoveSelected(ListBox fromBox, ListBox toBox)
        {
            List<string> list = new List<string>(fromBox.SelectedItems.Count);
            foreach (object selectedItem in fromBox.SelectedItems)
            {
                list.Add(selectedItem.ToString());
            }

            list.ForEach(delegate(string i) { MoveBetweenLists(i.ToString(), fromBox, toBox); });
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
            Close();
        }

        private void moveAllToTheRightButton_Click(object sender, EventArgs e)
        {
            MoveAll(leftListBox, rightListBox);
        }

        private void moveAllToTheLeftButton_Click(object sender, EventArgs e)
        {
            MoveAll(rightListBox, leftListBox);
        }

        private void MoveAll(ListBox fromListBox, ListBox toListBox)
        {
            List<string> list = FromListBox(fromListBox);
            list.ForEach(delegate(string item) { MoveBetweenLists(item, fromListBox, toListBox); });
        }

        private void MoveBetweenLists(string item, ListBox fromList, ListBox toList)
        {
            fromList.Items.Remove(item);
            int index = FindInsertIndex(toList, item);
            toList.Items.Insert(index, item);
        }

        private int FindInsertIndex(ListBox listBox, string item)
        {
            int num = -1;
            if (OriginalOrder != null)
            {
                List<string> list = new List<string>(OriginalOrder);
                list.RemoveAll((string i) => !item.Equals(i) && !listBox.Items.Contains(i));
                num = list.IndexOf(item);
            }

            if (num != -1)
            {
                return num;
            }

            return listBox.Items.Count;
        }

        private static List<string> FromListBox(ListBox listBox)
        {
            List<string> list = new List<string>();
            foreach (object item in listBox.Items)
            {
                list.Add(item.ToString());
            }

            return list;
        }

        private static void ToListBox(ListBox listBox, List<string> items)
        {
            listBox.Items.Clear();
            items.ForEach(delegate(string i) { listBox.Items.Add(i); });
        }

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
            components = new Container();
            okButton = new Button();
            cancelButton = new Button();
            panel1 = new Panel();
            panel4 = new Panel();
            toTheLeftButton = new Button();
            toTheRightButton = new Button();
            panel2 = new Panel();
            rightListBox = new ListBox();
            rightListLabel = new Label();
            panel3 = new Panel();
            leftListBox = new ListBox();
            leftListLabel = new Label();
            bindingSourceLeft = new BindingSource(components);
            bindingSourceRight = new BindingSource(components);
            moveAllToTheRightButton = new Button();
            moveAllToTheLeftButton = new Button();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((ISupportInitialize) bindingSourceLeft).BeginInit();
            ((ISupportInitialize) bindingSourceRight).BeginInit();
            SuspendLayout();
            okButton.Anchor = AnchorStyles.Bottom;
            okButton.Location = new Point(168, 521);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(okButton_Click);
            cancelButton.Anchor = AnchorStyles.Bottom;
            cancelButton.Location = new Point(249, 521);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                             AnchorStyles.Left | AnchorStyles.Right);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(panel3);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(458, 503);
            panel1.TabIndex = 2;
            panel4.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                             AnchorStyles.Right);
            panel4.Controls.Add(moveAllToTheLeftButton);
            panel4.Controls.Add(moveAllToTheRightButton);
            panel4.Controls.Add(toTheLeftButton);
            panel4.Controls.Add(toTheRightButton);
            panel4.Location = new Point(188, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(44, 500);
            panel4.TabIndex = 8;
            toTheLeftButton.Location = new Point(3, 232);
            toTheLeftButton.Name = "toTheLeftButton";
            toTheLeftButton.Size = new Size(36, 29);
            toTheLeftButton.TabIndex = 11;
            toTheLeftButton.Text = "<";
            toTheLeftButton.UseVisualStyleBackColor = true;
            toTheLeftButton.Click += new EventHandler(toTheLeftButton_Click);
            toTheRightButton.Location = new Point(3, 195);
            toTheRightButton.Name = "toTheRightButton";
            toTheRightButton.Size = new Size(36, 29);
            toTheRightButton.TabIndex = 10;
            toTheRightButton.Text = ">";
            toTheRightButton.UseVisualStyleBackColor = true;
            toTheRightButton.Click += new EventHandler(toTheRightButton_Click);
            panel2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                             AnchorStyles.Right);
            panel2.Controls.Add(rightListBox);
            panel2.Controls.Add(rightListLabel);
            panel2.Location = new Point(238, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(224, 500);
            panel2.TabIndex = 7;
            rightListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                                   AnchorStyles.Left | AnchorStyles.Right);
            rightListBox.FormattingEnabled = true;
            rightListBox.Location = new Point(3, 21);
            rightListBox.Name = "rightListBox";
            rightListBox.SelectionMode = SelectionMode.MultiExtended;
            rightListBox.Size = new Size(214, 459);
            rightListBox.TabIndex = 1;
            rightListLabel.AutoSize = true;
            rightListLabel.Location = new Point(4, 4);
            rightListLabel.Name = "rightListLabel";
            rightListLabel.Size = new Size(51, 13);
            rightListLabel.TabIndex = 0;
            rightListLabel.Text = "Right List";
            panel3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                             AnchorStyles.Left | AnchorStyles.Right);
            panel3.Controls.Add(leftListBox);
            panel3.Controls.Add(leftListLabel);
            panel3.Location = new Point(5, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(179, 500);
            panel3.TabIndex = 6;
            leftListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom |
                                  AnchorStyles.Left | AnchorStyles.Right);
            leftListBox.FormattingEnabled = true;
            leftListBox.Location = new Point(7, 21);
            leftListBox.Name = "leftListBox";
            leftListBox.SelectionMode = SelectionMode.MultiExtended;
            leftListBox.Size = new Size(164, 459);
            leftListBox.TabIndex = 1;
            leftListLabel.AutoSize = true;
            leftListLabel.Location = new Point(4, 4);
            leftListLabel.Name = "leftListLabel";
            leftListLabel.Size = new Size(44, 13);
            leftListLabel.TabIndex = 0;
            leftListLabel.Text = "Left List";
            moveAllToTheRightButton.Location = new Point(3, 160);
            moveAllToTheRightButton.Name = "moveAllToTheRightButton";
            moveAllToTheRightButton.Size = new Size(36, 29);
            moveAllToTheRightButton.TabIndex = 12;
            moveAllToTheRightButton.Text = ">>";
            moveAllToTheRightButton.UseVisualStyleBackColor = true;
            moveAllToTheRightButton.Click += new EventHandler(moveAllToTheRightButton_Click);
            moveAllToTheLeftButton.Location = new Point(5, 267);
            moveAllToTheLeftButton.Name = "moveAllToTheLeftButton";
            moveAllToTheLeftButton.Size = new Size(36, 29);
            moveAllToTheLeftButton.TabIndex = 13;
            moveAllToTheLeftButton.Text = "<<";
            moveAllToTheLeftButton.UseVisualStyleBackColor = true;
            moveAllToTheLeftButton.Click += new EventHandler(moveAllToTheLeftButton_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(482, 556);
            base.Controls.Add(panel1);
            base.Controls.Add(cancelButton);
            base.Controls.Add(okButton);
            base.Name = "ListEditor";
            Text = "ListEditor";
            panel1.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((ISupportInitialize) bindingSourceLeft).EndInit();
            ((ISupportInitialize) bindingSourceRight).EndInit();
            ResumeLayout(false);
        }
    }
}