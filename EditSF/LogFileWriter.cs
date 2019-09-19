using EsfLibrary;
using System.IO;

namespace EditSF
{
    public class LogFileWriter
    {
        private TextWriter writer;

        public LogFileWriter(string logFileName)
        {
            writer = File.CreateText(logFileName);
        }

        public void WriteEntry(EsfNode node, long position)
        {
            bool flag = node is RecordNode;
        }

        public void WriteLogEntry(string entry)
        {
            writer.WriteLine(entry);
        }

        public void Close()
        {
            writer.Close();
        }
    }
}