using EsfLibrary;
using System;
using System.IO;
using System.Windows.Forms;

namespace EditSF
{
    public class FileTester
    {
        public int TestsRun
        {
            get;
            private set;
        }

        public int TestSuccesses
        {
            get;
            set;
        }

        public string RunTest(string file, ToolStripProgressBar progress, ToolStripStatusLabel statusLabel)
        {
            string text = statusLabel.Text;
            statusLabel.Text = $"Loading file {Path.GetFileName(file)}";
            Application.DoEvents();
            string result;
            using (Stream stream = File.OpenRead(file))
            {
                try
                {
                    EsfCodec codec = EsfCodecUtil.GetCodec(File.OpenRead(file));
                    if (codec != null)
                    {
                        TestsRun++;
                        EsfFile esfFile = new EsfFile(stream, codec);
                        string text2 = file + "_test";
                        EsfCodecUtil.WriteEsfFile(text2, esfFile);
                        statusLabel.Text = $"Saving file {Path.GetFileName(file)}";
                        Application.DoEvents();
                        using (Stream stream2 = File.OpenRead(text2))
                        {
                            EsfFile obj = new EsfFile(stream2, codec);
                            if (esfFile.Equals(obj))
                            {
                                TestSuccesses++;
                                result = $"success Test {file}";
                            }
                            else
                            {
                                result = $"FAIL Test {file}: Reload of save file different from original";
                            }
                        }
                        Application.DoEvents();
                        File.Delete(text2);
                    }
                    else
                    {
                        result = $"not running test on {file}";
                    }
                }
                catch (Exception arg)
                {
                    result = $"FAIL Test of {file}: {arg}";
                }
                stream.Close();
            }
            statusLabel.Text = text;
            return result;
        }
    }
}