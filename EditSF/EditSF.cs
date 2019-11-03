using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CommonDialogs;
using EditSF.tw3k;
using EsfControl;
using EsfLibrary;

namespace EditSF
{
    public class EditSF : Form
    {
        private IContainer components;

        private MenuStrip menuStrip1;

        private ToolStripMenuItem _fileToolStripMenuItem; // File菜单

        private ToolStripMenuItem _openToolStripMenuItem; // File-Open选项

        private ToolStripMenuItem _saveToolStripMenuItem; // File-Save选项

        private ToolStripMenuItem _saveAsToolStripMenuItem; // File-SaveAs选项

        private ToolStripMenuItem _exitToolStripMenuItem; // File-Exit选项

        private ToolStripMenuItem _bookmarksToolStripMenuItem; // Bookmarks菜单

        private ToolStripMenuItem _addBookmarkToolStripMenuItem; // Bookmarks-AddBookmark选项

        private ToolStripMenuItem _editBookmarkToolStripMenuItem; // Bookmarks-EditBookmark选项

        private ToolStripSeparator _bookmarkSeparator; // Bookmarks-分隔线

        private ToolStripMenuItem _optionsToolStripMenuItem; // Options菜单

        private ToolStripMenuItem _writeLogFileToolStripMenuItem; // Options-WriteLogFile选项

        private ToolStripMenuItem _showNodeTypeToolStripMenuItem; // Options-ShowNodeType选项

        private ToolStripMenuItem _helpToolStripMenuItem; //  Help菜单

        private ToolStripMenuItem _aboutToolStripMenuItem; //  Help-About菜单

        private ToolStripMenuItem _testToolStripMenuItem;

        private ToolStripMenuItem _runTestsStripMenuItem;

        private ToolStripMenuItem _runSingleTestToolStripMenuItem;

        private StatusStrip _statusBar;

        private ToolStripProgressBar _progressBar;

        private ToolStripStatusLabel _statusLabel;

        private EditEsfComponent _editEsfComponent;

        private ProgressUpdater updater;

        public static string FILENAME = "testfiles.txt";

        private string _filename;

        private EsfFile _file;

        private List<string> bookmarks = new List<string>(); // 书签名列表

        private Dictionary<string, string> bookmarkToPath = new Dictionary<string, string>(); // 书签路径字典

        private static string BOOKMARKS_FILE_NAME = "bookmarks.txt"; // 书签名称

        private string BookmarkPath => Path.Combine(Application.UserAppDataPath, BOOKMARKS_FILE_NAME); // 书签路径

        public string FileName
        {
            get { return _filename; }
            set
            {
                Text = $"{Path.GetFileName(value)} - EditSF {Application.ProductVersion}";
                _statusLabel.Text = value;
                _filename = value;
            }
        }

        private EsfFile EditedFile
        {
            get { return _file; }
            set
            {
                _file = value;
                _editEsfComponent.RootNode = value.RootNode;
                _editEsfComponent.RootNode.Modified = false;
                _saveAsToolStripMenuItem.Enabled = (_file != null);
                _saveToolStripMenuItem.Enabled = (_file != null);
                _showNodeTypeToolStripMenuItem.Enabled = (_file != null);
            }
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
            menuStrip1 = new MenuStrip();
            _fileToolStripMenuItem = new ToolStripMenuItem();
            _openToolStripMenuItem = new ToolStripMenuItem();
            _saveToolStripMenuItem = new ToolStripMenuItem();
            _saveAsToolStripMenuItem = new ToolStripMenuItem();
            _exitToolStripMenuItem = new ToolStripMenuItem();
            _optionsToolStripMenuItem = new ToolStripMenuItem();
            _bookmarksToolStripMenuItem = new ToolStripMenuItem();
            _writeLogFileToolStripMenuItem = new ToolStripMenuItem();
            _showNodeTypeToolStripMenuItem = new ToolStripMenuItem();
            _addBookmarkToolStripMenuItem = new ToolStripMenuItem();
            _editBookmarkToolStripMenuItem = new ToolStripMenuItem();
            _bookmarkSeparator = new ToolStripSeparator();
            _testToolStripMenuItem = new ToolStripMenuItem();
            _runSingleTestToolStripMenuItem = new ToolStripMenuItem();
            _runTestsStripMenuItem = new ToolStripMenuItem();
            _helpToolStripMenuItem = new ToolStripMenuItem();
            _aboutToolStripMenuItem = new ToolStripMenuItem();
            _statusBar = new StatusStrip();
            _progressBar = new ToolStripProgressBar();
            _statusLabel = new ToolStripStatusLabel();
            _editEsfComponent = new EditEsfComponent();
            menuStrip1.SuspendLayout();
            _statusBar.SuspendLayout();
            SuspendLayout();
            menuStrip1.Items.AddRange(new ToolStripItem[5]
            {
                _fileToolStripMenuItem,
                _bookmarksToolStripMenuItem,
                _optionsToolStripMenuItem,
                _testToolStripMenuItem,
                _helpToolStripMenuItem
            });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(789, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            _fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[4]
            {
                _openToolStripMenuItem,
                _saveToolStripMenuItem,
                _saveAsToolStripMenuItem,
                _exitToolStripMenuItem
            });
            _fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
            _fileToolStripMenuItem.Size = new Size(37, 20);
            _fileToolStripMenuItem.Text = "File";
            _openToolStripMenuItem.Name = "_openToolStripMenuItem";
            _openToolStripMenuItem.Size = new Size(123, 22);
            _openToolStripMenuItem.Text = "Open";
            _openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);
            _saveToolStripMenuItem.Enabled = false;
            _saveToolStripMenuItem.Name = "_saveToolStripMenuItem";
            _saveToolStripMenuItem.Size = new Size(123, 22);
            _saveToolStripMenuItem.Text = "Save";
            _saveToolStripMenuItem.Click += new EventHandler(saveToolStripMenuItem1_Click);
            _saveAsToolStripMenuItem.Enabled = false;
            _saveAsToolStripMenuItem.Name = "_saveAsToolStripMenuItem";
            _saveAsToolStripMenuItem.Size = new Size(123, 22);
            _saveAsToolStripMenuItem.Text = "Save As...";
            _saveAsToolStripMenuItem.Click += new EventHandler(saveToolStripMenuItem_Click);
            _exitToolStripMenuItem.Name = "_exitToolStripMenuItem";
            _exitToolStripMenuItem.Size = new Size(123, 22);
            _exitToolStripMenuItem.Text = "Exit";
            _exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            _bookmarksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[3]
            {
                _addBookmarkToolStripMenuItem,
                _editBookmarkToolStripMenuItem,
                _bookmarkSeparator
            });
            _bookmarksToolStripMenuItem.Name = "_bookmarksToolStripMenuItem";
            _bookmarksToolStripMenuItem.Size = new Size(61, 20);
            _bookmarksToolStripMenuItem.Text = "Bookmarks";
            _addBookmarkToolStripMenuItem.Enabled = false;
            _addBookmarkToolStripMenuItem.Name = "_addBookmarkToolStripMenuItem";
            _addBookmarkToolStripMenuItem.Size = new Size(164, 22);
            _addBookmarkToolStripMenuItem.Text = "Add Bookmark";
            _addBookmarkToolStripMenuItem.Click += new EventHandler(AddBookmark_Click);
            _editBookmarkToolStripMenuItem.Enabled = true;
            _editBookmarkToolStripMenuItem.Name = "_editBookmarkToolStripMenuItem";
            _editBookmarkToolStripMenuItem.Size = new Size(164, 22);
            _editBookmarkToolStripMenuItem.Text = "Edit Bookmarks";
            _editBookmarkToolStripMenuItem.Click += new EventHandler(EditBookmarks_Click);
            _optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
            {
                _writeLogFileToolStripMenuItem,
                _showNodeTypeToolStripMenuItem
            });
            _optionsToolStripMenuItem.Name = "_optionsToolStripMenuItem";
            _optionsToolStripMenuItem.Size = new Size(61, 20);
            _optionsToolStripMenuItem.Text = "Options";
            _writeLogFileToolStripMenuItem.CheckOnClick = true;
            _writeLogFileToolStripMenuItem.Name = "_writeLogFileToolStripMenuItem";
            _writeLogFileToolStripMenuItem.Size = new Size(164, 22);
            _writeLogFileToolStripMenuItem.Text = "Write Log File";
            _showNodeTypeToolStripMenuItem.CheckOnClick = true;
            _showNodeTypeToolStripMenuItem.Enabled = false;
            _showNodeTypeToolStripMenuItem.Name = "_showNodeTypeToolStripMenuItem";
            _showNodeTypeToolStripMenuItem.Size = new Size(164, 22);
            _showNodeTypeToolStripMenuItem.Text = "Show Node Type";
            _showNodeTypeToolStripMenuItem.Click += new EventHandler(showNodeTypeToolStripMenuItem_Click);
            _testToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
            {
                _runSingleTestToolStripMenuItem,
                _runTestsStripMenuItem
            });
            _testToolStripMenuItem.Name = "_testToolStripMenuItem";
            _testToolStripMenuItem.Size = new Size(46, 20);
            _testToolStripMenuItem.Text = "Tests";
            _testToolStripMenuItem.Visible = false;
            _runSingleTestToolStripMenuItem.Name = "_runSingleTestToolStripMenuItem";
            _runSingleTestToolStripMenuItem.Size = new Size(178, 22);
            _runSingleTestToolStripMenuItem.Text = "Run Load/Save Test";
            _runSingleTestToolStripMenuItem.Click += new EventHandler(runSingleTestToolStripMenuItem_Click);
            _runTestsStripMenuItem.Name = "_runTestsStripMenuItem";
            _runTestsStripMenuItem.Size = new Size(178, 22);
            _runTestsStripMenuItem.Text = "Multiple Tests";
            _runTestsStripMenuItem.Click += new EventHandler(runTestsToolStripMenuItem_Click);
            _helpToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            _helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[1]
            {
                _aboutToolStripMenuItem
            });
            _helpToolStripMenuItem.Name = "_helpToolStripMenuItem";
            _helpToolStripMenuItem.Size = new Size(44, 20);
            _helpToolStripMenuItem.Text = "Help";
            _aboutToolStripMenuItem.Name = "_aboutToolStripMenuItem";
            _aboutToolStripMenuItem.Size = new Size(107, 22);
            _aboutToolStripMenuItem.Text = "About";
            _aboutToolStripMenuItem.Click += new EventHandler(aboutToolStripMenuItem_Click);
            _statusBar.Items.AddRange(new ToolStripItem[2]
            {
                _progressBar,
                _statusLabel
            });
            _statusBar.Location = new Point(0, 769);
            _statusBar.Name = "_statusBar";
            _statusBar.Size = new Size(789, 22);
            _statusBar.TabIndex = 2;
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(100, 16);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(86, 17);
            _statusLabel.Text = "No File Loaded";
            _editEsfComponent.Dock = DockStyle.Fill;
            _editEsfComponent.Location = new Point(0, 24);
            _editEsfComponent.Name = "_editEsfComponent";
            _editEsfComponent.RootNode = null;
            _editEsfComponent.ShowCode = false;
            _editEsfComponent.Size = new Size(789, 745);
            _editEsfComponent.TabIndex = 3;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(789, 791);
            base.Controls.Add(_editEsfComponent);
            base.Controls.Add(_statusBar);
            base.Controls.Add(menuStrip1);
            base.MainMenuStrip = menuStrip1;
            base.Name = "EditSF";
            Text = "EditSF";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            _statusBar.ResumeLayout(false);
            _statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public EditSF()
        {
            InitializeComponent();
            updater = new ProgressUpdater(_progressBar);
            Text = $"EditSF {Application.ProductVersion}";
            _editEsfComponent.NodeSelected += NodeSelected;
            if (File.Exists(BookmarkPath))
            {
                string[] array = File.ReadAllLines(BookmarkPath);
                foreach (string text in array)
                {
                    string[] array2 = text.Split(Path.PathSeparator);
                    AddBookmark(array2[0], array2[1], enable: false);
                }

                _editBookmarkToolStripMenuItem.Enabled = (bookmarks.Count > 0);
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

        // 打开文件
        private void OpenFile(string openFilename)
        {
            string text = _statusLabel.Text;
            try
            {
                _fileToolStripMenuItem.Enabled = false;
                _optionsToolStripMenuItem.Enabled = false;
                _statusLabel.Text = $"Loading {openFilename}";
                LogFileWriter logFileWriter = null;
                if (_writeLogFileToolStripMenuItem.Checked)
                {
                    logFileWriter = new LogFileWriter(openFilename + ".xml");
                }

                EditedFile = EsfCodecUtil.LoadEsfFile(openFilename);
                FileName = openFilename;
                logFileWriter?.Close();
                // 自定义检索
                try
                {
                    Tw3kSearch.SearchFaction(EditedFile);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }

                // 自定义换词条
//                ChangePersonality(EditedFile, 101, CeoCategory.Personality,
//                    "3k_main_ceo_trait_personality_kind", "3k_main_ceo_trait_personality_humble",
//                    "3k_main_ceo_trait_personality_fraternal");
                Text = $"{Path.GetFileName(openFilename)} - EditSF {Application.ProductVersion}";
                foreach (ToolStripItem dropDownItem in _bookmarksToolStripMenuItem.DropDownItems)
                {
                    if (dropDownItem is BookmarkItem)
                    {
                        dropDownItem.Enabled = true;
                    }
                }
            }
            catch (Exception value)
            {
                _statusLabel.Text = text;
                Console.WriteLine(value);
            }
            finally
            {
                _fileToolStripMenuItem.Enabled = true;
                _optionsToolStripMenuItem.Enabled = true;
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
            _bookmarksToolStripMenuItem.Enabled = (node != null);
            _addBookmarkToolStripMenuItem.Enabled = (node != null);
        }

        // 添加书签——点击事件
        private void AddBookmark_Click(object sender, EventArgs args)
        {
            InputBox inputBox = new InputBox();
            inputBox.Text = "Enter bookmark name";
            inputBox.Input = _editEsfComponent.SelectedPath;
            InputBox inputBox2 = inputBox;
            if (inputBox2.ShowDialog() == DialogResult.OK && !bookmarks.Contains(inputBox2.Input))
            {
                AddBookmark(inputBox2.Input, _editEsfComponent.SelectedPath);
                SaveBookmarks();
            }
        }

        // 编辑书签——点击事件
        public void EditBookmarks_Click(object sender, EventArgs args)
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
                    foreach (ToolStripItem dropDownItem in _bookmarksToolStripMenuItem.DropDownItems)
                    {
                        if (dropDownItem is BookmarkItem && dropDownItem.Text.Equals(right))
                        {
                            _bookmarksToolStripMenuItem.DropDownItems.Remove(dropDownItem);
                            break;
                        }
                    }
                }

                SaveBookmarks();
            }
        }

        private void AddBookmark(string label, string path, bool enable = true)
        {
            bookmarks.Add(label);
            bookmarkToPath[label] = path;
            _bookmarksToolStripMenuItem.DropDownItems.Add(new BookmarkItem(label, path, _editEsfComponent)
            {
                Enabled = enable
            });
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
            if (_filename != null)
            {
                Save(_filename);
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
                            string value = fileTester.RunTest(item, _progressBar, _statusLabel);
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
                string text = new FileTester().RunTest(openFileDialog2.FileName, _progressBar, _statusLabel);
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
                _editEsfComponent.RootNode.Modified = false;
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
            _editEsfComponent.ShowCode = true;
        }
    }
}