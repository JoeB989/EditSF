using CommonDialogs;
using EsfControl;
using EsfLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EditSF
{
    public class EditSF : Form
    {
        private IContainer components;

        private MenuStrip menuStrip1;

        private ToolStripMenuItem fileToolStripMenuItem;

        private ToolStripMenuItem openToolStripMenuItem;

        private ToolStripMenuItem saveAsToolStripMenuItem;

        private ToolStripMenuItem bookmarksToolStripMenuItem;

        private ToolStripMenuItem optionsToolStripMenuItem;

        private ToolStripMenuItem writeLogFileToolStripMenuItem;

        private ToolStripMenuItem testToolStripMenuItem;

        private ToolStripMenuItem runTestsStripMenuItem;

        private ToolStripMenuItem runSingleTestToolStripMenuItem;

        private ToolStripSeparator bookmarkSeparator;

        private StatusStrip statusBar;

        private ToolStripProgressBar progressBar;

        private ToolStripStatusLabel statusLabel;

        private ToolStripMenuItem saveToolStripMenuItem;

        private ToolStripMenuItem helpToolStripMenuItem;

        private ToolStripMenuItem aboutToolStripMenuItem;

        private ToolStripMenuItem exitToolStripMenuItem;

        private ToolStripMenuItem addBookmarkToolStripMenuItem;

        private ToolStripMenuItem editBookmarkToolStripMenuItem;

        private ToolStripMenuItem showNodeTypeToolStripMenuItem;

        private EditEsfComponent editEsfComponent;

        private ProgressUpdater updater;

        public static string FILENAME = "testfiles.txt";

        private string filename;

        private EsfFile file;

        private List<string> bookmarks = new List<string>();

        private Dictionary<string, string> bookmarkToPath = new Dictionary<string, string>();

        private static string BOOKMARKS_FILE_NAME = "bookmarks.txt";

        public string FileName
        {
            get { return filename; }
            set
            {
                Text = $"{Path.GetFileName(value)} - EditSF {Application.ProductVersion}";
                statusLabel.Text = value;
                filename = value;
            }
        }

        private EsfFile EditedFile
        {
            get { return file; }
            set
            {
                file = value;
                editEsfComponent.RootNode = value.RootNode;
                editEsfComponent.RootNode.Modified = false;
                saveAsToolStripMenuItem.Enabled = (file != null);
                saveToolStripMenuItem.Enabled = (file != null);
                showNodeTypeToolStripMenuItem.Enabled = (file != null);
            }
        }

        private string BookmarkPath => Path.Combine(Application.UserAppDataPath, BOOKMARKS_FILE_NAME);

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
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            bookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            writeLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showNodeTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            bookmarkSeparator = new System.Windows.Forms.ToolStripSeparator();
            testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runSingleTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runTestsStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusBar = new System.Windows.Forms.StatusStrip();
            progressBar = new System.Windows.Forms.ToolStripProgressBar();
            statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            editEsfComponent = new EsfControl.EditEsfComponent();
            menuStrip1.SuspendLayout();
            statusBar.SuspendLayout();
            SuspendLayout();
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[5]
            {
                fileToolStripMenuItem,
                bookmarksToolStripMenuItem,
                optionsToolStripMenuItem,
                testToolStripMenuItem,
                helpToolStripMenuItem
            });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(789, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4]
            {
                openToolStripMenuItem,
                saveToolStripMenuItem,
                saveAsToolStripMenuItem,
                exitToolStripMenuItem
            });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += new System.EventHandler(openToolStripMenuItem_Click);
            saveToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += new System.EventHandler(saveToolStripMenuItem1_Click);
            saveAsToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            saveAsToolStripMenuItem.Text = "Save As...";
            saveAsToolStripMenuItem.Click += new System.EventHandler(saveToolStripMenuItem_Click);
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += new System.EventHandler(exitToolStripMenuItem_Click);
            bookmarksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3]
            {
                addBookmarkToolStripMenuItem,
                editBookmarkToolStripMenuItem,
                bookmarkSeparator
            });
            bookmarksToolStripMenuItem.Name = "bookmarksToolStripMenuItem";
            bookmarksToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            bookmarksToolStripMenuItem.Text = "Bookmarks";
            addBookmarkToolStripMenuItem.Enabled = false;
            addBookmarkToolStripMenuItem.Name = "addBookmarkToolStripMenuItem";
            addBookmarkToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            addBookmarkToolStripMenuItem.Text = "Add Bookmark";
            addBookmarkToolStripMenuItem.Click += new System.EventHandler(AddBookmark);
            editBookmarkToolStripMenuItem.Enabled = true;
            editBookmarkToolStripMenuItem.Name = "editBookmarkToolStripMenuItem";
            editBookmarkToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            editBookmarkToolStripMenuItem.Text = "Edit Bookmarks";
            editBookmarkToolStripMenuItem.Click += new System.EventHandler(EditBookmarks);
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2]
            {
                writeLogFileToolStripMenuItem,
                showNodeTypeToolStripMenuItem
            });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            writeLogFileToolStripMenuItem.CheckOnClick = true;
            writeLogFileToolStripMenuItem.Name = "writeLogFileToolStripMenuItem";
            writeLogFileToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            writeLogFileToolStripMenuItem.Text = "Write Log File";
            showNodeTypeToolStripMenuItem.CheckOnClick = true;
            showNodeTypeToolStripMenuItem.Enabled = false;
            showNodeTypeToolStripMenuItem.Name = "showNodeTypeToolStripMenuItem";
            showNodeTypeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            showNodeTypeToolStripMenuItem.Text = "Show Node Type";
            showNodeTypeToolStripMenuItem.Click += new System.EventHandler(showNodeTypeToolStripMenuItem_Click);
            testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2]
            {
                runSingleTestToolStripMenuItem,
                runTestsStripMenuItem
            });
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            testToolStripMenuItem.Text = "Tests";
            testToolStripMenuItem.Visible = false;
            runSingleTestToolStripMenuItem.Name = "runSingleTestToolStripMenuItem";
            runSingleTestToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            runSingleTestToolStripMenuItem.Text = "Run Load/Save Test";
            runSingleTestToolStripMenuItem.Click += new System.EventHandler(runSingleTestToolStripMenuItem_Click);
            runTestsStripMenuItem.Name = "runTestsStripMenuItem";
            runTestsStripMenuItem.Size = new System.Drawing.Size(178, 22);
            runTestsStripMenuItem.Text = "Multiple Tests";
            runTestsStripMenuItem.Click += new System.EventHandler(runTestsToolStripMenuItem_Click);
            helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1]
            {
                aboutToolStripMenuItem
            });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += new System.EventHandler(aboutToolStripMenuItem_Click);
            statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[2]
            {
                progressBar,
                statusLabel
            });
            statusBar.Location = new System.Drawing.Point(0, 769);
            statusBar.Name = "statusBar";
            statusBar.Size = new System.Drawing.Size(789, 22);
            statusBar.TabIndex = 2;
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(100, 16);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(86, 17);
            statusLabel.Text = "No File Loaded";
            editEsfComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            editEsfComponent.Location = new System.Drawing.Point(0, 24);
            editEsfComponent.Name = "editEsfComponent";
            editEsfComponent.RootNode = null;
            editEsfComponent.ShowCode = false;
            editEsfComponent.Size = new System.Drawing.Size(789, 745);
            editEsfComponent.TabIndex = 3;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(789, 791);
            base.Controls.Add(editEsfComponent);
            base.Controls.Add(statusBar);
            base.Controls.Add(menuStrip1);
            base.MainMenuStrip = menuStrip1;
            base.Name = "EditSF";
            Text = "EditSF";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public EditSF()
        {
            InitializeComponent();
            updater = new ProgressUpdater(progressBar);
            Text = $"EditSF {Application.ProductVersion}";
            editEsfComponent.NodeSelected += NodeSelected;
            if (File.Exists(BookmarkPath))
            {
                string[] array = File.ReadAllLines(BookmarkPath);
                foreach (string text in array)
                {
                    string[] array2 = text.Split(Path.PathSeparator);
                    AddBookmark(array2[0], array2[1], enable: false);
                }

                editBookmarkToolStripMenuItem.Enabled = (bookmarks.Count > 0);
            }
        }

        private void promptOpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            OpenFileDialog openFileDialog2 = openFileDialog;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenFile(openFileDialog2.FileName);
                }
                catch (Exception arg)
                {
                    MessageBox.Show($"Could not open {openFileDialog2.FileName}: {arg}");
                    updater.LoadingFinished();
                }
            }
        }

        private void OpenFile(string openFilename)
        {
            string text = statusLabel.Text;
            try
            {
                fileToolStripMenuItem.Enabled = false;
                optionsToolStripMenuItem.Enabled = false;
                statusLabel.Text = $"Loading {openFilename}";
                LogFileWriter logFileWriter = null;
                if (writeLogFileToolStripMenuItem.Checked)
                {
                    logFileWriter = new LogFileWriter(openFilename + ".xml");
                }

                EditedFile = EsfCodecUtil.LoadEsfFile(openFilename);
                FileName = openFilename;
                logFileWriter?.Close();
                Text = $"{Path.GetFileName(openFilename)} - EditSF {Application.ProductVersion}";
                foreach (ToolStripItem dropDownItem in bookmarksToolStripMenuItem.DropDownItems)
                {
                    if (dropDownItem is BookmarkItem)
                    {
                        dropDownItem.Enabled = true;
                    }
                }
            }
            catch (Exception value)
            {
                statusLabel.Text = text;
                Console.WriteLine(value);
            }
            finally
            {
                fileToolStripMenuItem.Enabled = true;
                optionsToolStripMenuItem.Enabled = true;
            }
        }

        private void promptSaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            SaveFileDialog saveFileDialog2 = saveFileDialog;
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Save(saveFileDialog2.FileName);
                FileName = saveFileDialog2.FileName;
            }
        }

        private void NodeSelected(EsfNode node)
        {
            bookmarksToolStripMenuItem.Enabled = (node != null);
            addBookmarkToolStripMenuItem.Enabled = (node != null);
        }

        private void AddBookmark(object sender, EventArgs args)
        {
            InputBox inputBox = new InputBox();
            inputBox.Text = "Enter bookmark name";
            inputBox.Input = editEsfComponent.SelectedPath;
            InputBox inputBox2 = inputBox;
            if (inputBox2.ShowDialog() == DialogResult.OK && !bookmarks.Contains(inputBox2.Input))
            {
                AddBookmark(inputBox2.Input, editEsfComponent.SelectedPath);
                SaveBookmarks();
            }
        }

        public void EditBookmarks(object sender, EventArgs args)
        {
            List<string> leftList = new List<string>(bookmarks);
            List<string> rightList = new List<string>();
            ListEditor listEditor = new ListEditor();
            listEditor.LeftLabel = "Current Bookmarks";
            listEditor.LeftList = leftList;
            listEditor.RightLabel = "Delete Bookmarks";
            listEditor.RightList = rightList;
            ListEditor listEditor2 = listEditor;
            if (listEditor2.ShowDialog() == DialogResult.OK)
            {
                foreach (string right in listEditor2.RightList)
                {
                    bookmarks.Remove(right);
                    bookmarkToPath.Remove(right);
                    foreach (ToolStripItem dropDownItem in bookmarksToolStripMenuItem.DropDownItems)
                    {
                        if (dropDownItem is BookmarkItem && dropDownItem.Text.Equals(right))
                        {
                            bookmarksToolStripMenuItem.DropDownItems.Remove(dropDownItem);
                            break;
                        }
                    }
                }

                SaveBookmarks();
            }
        }

        private void SaveBookmarks()
        {
            using (StreamWriter streamWriter = File.CreateText(BookmarkPath))
            {
                foreach (string bookmark in bookmarks)
                {
                    streamWriter.WriteLine("{0}{1}{2}", bookmark, Path.PathSeparator, bookmarkToPath[bookmark]);
                }
            }
        }

        private void AddBookmark(string label, string path, bool enable = true)
        {
            bookmarks.Add(label);
            bookmarkToPath[label] = path;
            bookmarksToolStripMenuItem.DropDownItems.Add(new BookmarkItem(label, path, editEsfComponent)
            {
                Enabled = enable
            });
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promptOpenFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promptSaveFile();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (filename != null)
            {
                Save(filename);
            }
        }

        private void runTestsToolStripMenuItem_Click(object sender, EventArgs eventArgs)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderBrowserDialog.Dispose();
                string path = Path.Combine(folderBrowserDialog.SelectedPath, "EditSF_test.txt");
                FileTester fileTester = new FileTester();
                using (TextWriter textWriter = File.CreateText(path))
                {
                    foreach (string item in Directory.EnumerateFiles(folderBrowserDialog.SelectedPath))
                    {
                        if (!item.EndsWith("EditSF_test.txt"))
                        {
                            string value = fileTester.RunTest(item, progressBar, statusLabel);
                            textWriter.WriteLine(value);
                            textWriter.Flush();
                        }
                    }
                }

                MessageBox.Show($"Test successes {fileTester.TestSuccesses}/{fileTester.TestsRun}", "Tests finished");
            }
        }

        private void runSingleTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            OpenFileDialog openFileDialog2 = openFileDialog;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string text = new FileTester().RunTest(openFileDialog2.FileName, progressBar, statusLabel);
                MessageBox.Show(text, "Test Finished");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"EditSF {Application.ProductVersion}\nCreated by daniu", "About EditSF");
        }

        private void Save(string filename)
        {
            try
            {
                EsfCodecUtil.WriteEsfFile(filename, EditedFile);
                editEsfComponent.RootNode.Modified = false;
            }
            catch (Exception arg)
            {
                MessageBox.Show($"Could not save {filename}: {arg}");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showNodeTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editEsfComponent.ShowCode = true;
        }
    }
}