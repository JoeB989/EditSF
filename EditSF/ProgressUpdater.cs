using EsfLibrary;
using System.IO;
using System.Windows.Forms;

namespace EditSF
{
    public class ProgressUpdater
    {
        private ToolStripProgressBar progress;

        private EsfCodec currentCodec;

        public ProgressUpdater(ToolStripProgressBar bar)
        {
            progress = bar;
        }

        public void StartLoading(string file, EsfCodec codec)
        {
            progress.Maximum = (int)new FileInfo(file).Length;
            currentCodec = codec;
            currentCodec.NodeReadFinished += Update;
        }

        public void LoadingFinished()
        {
            try
            {
                progress.Value = 0;
                currentCodec.NodeReadFinished -= Update;
            }
            catch
            {
            }
        }

        private void Update(EsfNode ignored, long position)
        {
            if (ignored is ParentNode)
            {
                try
                {
                    if ((int)position <= progress.Maximum)
                    {
                        progress.Value = (int)position;
                    }
                    Application.DoEvents();
                }
                catch
                {
                    progress.Value = 0;
                    currentCodec.NodeReadFinished -= Update;
                }
            }
        }
    }

}