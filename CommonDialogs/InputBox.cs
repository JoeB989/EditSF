using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonDialogs
{
    
public class InputBox : Form
{
	private IContainer components;

	private TextBox valueField;

	private Button okButton;

	private Button cancelButton;

	public string Input
	{
		get
		{
			return valueField.Text;
		}
		set
		{
			valueField.Text = value;
		}
	}

	public InputBox()
	{
		InitializeComponent();
		base.AcceptButton = okButton;
		base.CancelButton = cancelButton;
	}

	private void CloseWithOk(object sender = null, EventArgs e = null)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CloseWithCancel(object sender = null, EventArgs e = null)
	{
		base.DialogResult = DialogResult.Cancel;
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
		valueField = new System.Windows.Forms.TextBox();
		okButton = new System.Windows.Forms.Button();
		cancelButton = new System.Windows.Forms.Button();
		SuspendLayout();
		valueField.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
		valueField.Location = new System.Drawing.Point(13, 13);
		valueField.Name = "valueField";
		valueField.Size = new System.Drawing.Size(263, 20);
		valueField.TabIndex = 0;
		okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
		okButton.Location = new System.Drawing.Point(13, 48);
		okButton.Name = "okButton";
		okButton.Size = new System.Drawing.Size(75, 23);
		okButton.TabIndex = 1;
		okButton.Text = "OK";
		okButton.UseVisualStyleBackColor = true;
		okButton.Click += new System.EventHandler(CloseWithOk);
		cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
		cancelButton.Location = new System.Drawing.Point(201, 48);
		cancelButton.Name = "cancelButton";
		cancelButton.Size = new System.Drawing.Size(75, 23);
		cancelButton.TabIndex = 2;
		cancelButton.Text = "Cancel";
		cancelButton.UseVisualStyleBackColor = true;
		cancelButton.Click += new System.EventHandler(CloseWithCancel);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(288, 83);
		base.Controls.Add(cancelButton);
		base.Controls.Add(okButton);
		base.Controls.Add(valueField);
		base.Name = "InputBox";
		Text = "Enter value";
		ResumeLayout(false);
		PerformLayout();
	}
}

}