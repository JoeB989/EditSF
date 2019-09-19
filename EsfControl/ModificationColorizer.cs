using System.Drawing;
using System.Windows.Forms;
using EsfLibrary;

namespace EsfControl
{
    public class ModificationColorizer
    {
        public DataGridViewRow row;

        public ModificationColorizer(DataGridViewRow r)
        {
            row = r;
        }

        public void ChangeColor(EsfNode node)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.ForeColor = (node.Modified ? Color.Red : Color.Black);
            }
        }
    }
}