using System;
using System.Windows.Forms;

namespace EsfControl
{
    public class BookmarkItem : ToolStripMenuItem
    {
        private string openPath;

        private EditEsfComponent component;

        public BookmarkItem(string label, string path, EditEsfComponent c)
            : base(label)
        {
            openPath = path;
            component = c;
            base.Click += OpenPath;
        }

        private void OpenPath(object sender, EventArgs args)
        {
            component.SelectedPath = openPath;
        }
    }

}