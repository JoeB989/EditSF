using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CommonDialogs
{
    public class FileAlreadyExistsDialog : Form
{
	public enum Action
	{
		Ask,
		Overwrite,
		Skip,
		RenameExisting,
		RenameNew,
		Cancel
	}

	private Button cancelButton;

	private Action chosenAction;

	private IContainer components;

	private ComboBox defaultActionComboBox;

	private Label defaultActionLabel;

	private TextBox messageTextBox;

	private Button overwriteButton;

	private Button renameExistingButton;

	private Button renameNewButton;

	private Button skipButton;

	public bool CanRename
	{
		get
		{
			return renameExistingButton.Enabled;
		}
		set
		{
			if (renameExistingButton.Enabled != value)
			{
				if (value)
				{
					renameExistingButton.Enabled = true;
					renameNewButton.Enabled = true;
					defaultActionComboBox.Items.Add("Rename existing files");
					defaultActionComboBox.Items.Add("Rename new files");
				}
				else
				{
					renameExistingButton.Enabled = false;
					renameNewButton.Enabled = false;
					defaultActionComboBox.Items.Remove("Rename existing files");
					defaultActionComboBox.Items.Remove("Rename new files");
				}
			}
		}
	}

	public Action ChosenAction => chosenAction;

	public Action NextAction => (Action)defaultActionComboBox.SelectedIndex;

	public FileAlreadyExistsDialog(string filepath)
	{
		InitializeComponent();
		CanRename = true;
		defaultActionComboBox.SelectedIndex = 0;
		messageTextBox.Text = $"The file \"{Path.GetFileName(filepath)}\" already exists.\r\n\r\nDo you want to overwrite the existing file, skip this file, rename the existing file, or rename the new file?";
	}

	private void cancelButton_Click(object sender, EventArgs e)
	{
		chosenAction = Action.Cancel;
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
		overwriteButton = new System.Windows.Forms.Button();
		skipButton = new System.Windows.Forms.Button();
		messageTextBox = new System.Windows.Forms.TextBox();
		renameExistingButton = new System.Windows.Forms.Button();
		renameNewButton = new System.Windows.Forms.Button();
		defaultActionLabel = new System.Windows.Forms.Label();
		defaultActionComboBox = new System.Windows.Forms.ComboBox();
		cancelButton = new System.Windows.Forms.Button();
		SuspendLayout();
		overwriteButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		overwriteButton.Location = new System.Drawing.Point(305, 12);
		overwriteButton.Name = "overwriteButton";
		overwriteButton.Size = new System.Drawing.Size(75, 23);
		overwriteButton.TabIndex = 2;
		overwriteButton.Text = "Overwrite";
		overwriteButton.UseVisualStyleBackColor = true;
		overwriteButton.Click += new System.EventHandler(overwriteButton_Click);
		skipButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		skipButton.Location = new System.Drawing.Point(386, 12);
		skipButton.Name = "skipButton";
		skipButton.Size = new System.Drawing.Size(75, 23);
		skipButton.TabIndex = 3;
		skipButton.Text = "Skip";
		skipButton.UseVisualStyleBackColor = true;
		skipButton.Click += new System.EventHandler(skipButton_Click);
		messageTextBox.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
		messageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
		messageTextBox.Location = new System.Drawing.Point(12, 12);
		messageTextBox.Multiline = true;
		messageTextBox.Name = "messageTextBox";
		messageTextBox.ReadOnly = true;
		messageTextBox.Size = new System.Drawing.Size(287, 110);
		messageTextBox.TabIndex = 5;
		messageTextBox.Text = "The file \"foo\" already exists.\r\n\r\nDo you want to overwrite the existing file, skip this file, rename the existing file, or rename the new file?";
		renameExistingButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		renameExistingButton.Location = new System.Drawing.Point(305, 41);
		renameExistingButton.Name = "renameExistingButton";
		renameExistingButton.Size = new System.Drawing.Size(156, 23);
		renameExistingButton.TabIndex = 6;
		renameExistingButton.Text = "Rename Existing File";
		renameExistingButton.UseVisualStyleBackColor = true;
		renameExistingButton.Click += new System.EventHandler(renameExistingButton_Click);
		renameNewButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		renameNewButton.Location = new System.Drawing.Point(305, 70);
		renameNewButton.Name = "renameNewButton";
		renameNewButton.Size = new System.Drawing.Size(156, 23);
		renameNewButton.TabIndex = 7;
		renameNewButton.Text = "Rename New File";
		renameNewButton.UseVisualStyleBackColor = true;
		renameNewButton.Click += new System.EventHandler(renameNewButton_Click);
		defaultActionLabel.AutoSize = true;
		defaultActionLabel.Location = new System.Drawing.Point(12, 144);
		defaultActionLabel.Name = "defaultActionLabel";
		defaultActionLabel.Size = new System.Drawing.Size(271, 13);
		defaultActionLabel.TabIndex = 8;
		defaultActionLabel.Text = "You can change the default action for all remaining files:";
		defaultActionComboBox.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		defaultActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		defaultActionComboBox.FormattingEnabled = true;
		defaultActionComboBox.Items.AddRange(new object[5]
		{
			"Ask for each file",
			"Overwrite all files",
			"Skip all files",
			"Rename existing files",
			"Rename new files"
		});
		defaultActionComboBox.Location = new System.Drawing.Point(305, 141);
		defaultActionComboBox.Name = "defaultActionComboBox";
		defaultActionComboBox.Size = new System.Drawing.Size(156, 21);
		defaultActionComboBox.TabIndex = 9;
		cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
		cancelButton.Location = new System.Drawing.Point(305, 99);
		cancelButton.Name = "cancelButton";
		cancelButton.Size = new System.Drawing.Size(156, 23);
		cancelButton.TabIndex = 10;
		cancelButton.Text = "Cancel Extraction";
		cancelButton.UseVisualStyleBackColor = true;
		cancelButton.Click += new System.EventHandler(cancelButton_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(473, 173);
		base.Controls.Add(cancelButton);
		base.Controls.Add(defaultActionComboBox);
		base.Controls.Add(defaultActionLabel);
		base.Controls.Add(renameNewButton);
		base.Controls.Add(renameExistingButton);
		base.Controls.Add(messageTextBox);
		base.Controls.Add(skipButton);
		base.Controls.Add(overwriteButton);
		base.Name = "FileAlreadyExistsDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		Text = "File Already Exists";
		ResumeLayout(false);
		PerformLayout();
	}

	private void overwriteButton_Click(object sender, EventArgs e)
	{
		chosenAction = Action.Overwrite;
		Close();
	}

	private void renameExistingButton_Click(object sender, EventArgs e)
	{
		chosenAction = Action.RenameExisting;
		Close();
	}

	private void renameNewButton_Click(object sender, EventArgs e)
	{
		chosenAction = Action.RenameNew;
		Close();
	}

	private void skipButton_Click(object sender, EventArgs e)
	{
		chosenAction = Action.Skip;
		Close();
	}
}

}