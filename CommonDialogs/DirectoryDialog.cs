using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonDialogs
{
    public class DirectoryDialog : Form
    {
        private IContainer components;

        private TextBox directory;

        private Button browseButton;

        private Label label;

        private Button okButton;

        private Button cancelButton;

        public string Description
        {
            private get { return label.Text; }
            set { label.Text = value; }
        }

        public string SelectedPath
        {
            get { return directory.Text; }
            set { directory.Text = value; }
        }

        public DirectoryDialog()
        {
            InitializeComponent();
            okButton.NotifyDefault(value: true);
            base.AcceptButton = okButton;
            base.CancelButton = cancelButton;
        }

        private void Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = Description;
            folderBrowserDialog.SelectedPath = SelectedPath;
            FolderBrowserDialog folderBrowserDialog2 = folderBrowserDialog;
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog2.SelectedPath;
            }
        }

        private void CloseWithOk(object sender = null, EventArgs e = null)
        {
            base.DialogResult = DialogResult.OK;
            Close();
        }

        private void CloseWithCancel(object sender = null, EventArgs e = null)
        {
            base.DialogResult = DialogResult.Cancel;
            SelectedPath = "";
            Close();
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
            directory = new TextBox();
            browseButton = new Button();
            label = new Label();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            directory.Anchor = (AnchorStyles.Top | AnchorStyles.Left |
                                AnchorStyles.Right);
            directory.Location = new Point(12, 40);
            directory.Name = "directory";
            directory.Size = new Size(350, 20);
            directory.TabIndex = 0;
            browseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            browseButton.Location = new Point(368, 39);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(75, 23);
            browseButton.TabIndex = 1;
            browseButton.Text = "Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new EventHandler(Browse);
            label.AutoSize = true;
            label.Location = new Point(13, 13);
            label.Name = "label";
            label.Size = new Size(112, 13);
            label.TabIndex = 2;
            label.Text = "Please enter directory:";
            okButton.Anchor = AnchorStyles.Bottom;
            okButton.Location = new Point(142, 81);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 3;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(CloseWithOk);
            cancelButton.Anchor = AnchorStyles.Bottom;
            cancelButton.Location = new Point(235, 81);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += new EventHandler(CloseWithCancel);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(451, 119);
            base.Controls.Add(cancelButton);
            base.Controls.Add(okButton);
            base.Controls.Add(label);
            base.Controls.Add(browseButton);
            base.Controls.Add(directory);
            base.Name = "DirectoryDialog";
            Text = "Enter Directory";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}