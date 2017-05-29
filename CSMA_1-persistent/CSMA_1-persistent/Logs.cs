using System;
using System.Text;
using System.IO;

namespace CSMA_1_persistent
{
    public class Logs
    {
        public StreamWriter file;

        public Logs()
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.FullName.ToString());
            path.Append("\\SimulationLogs.txt");
            file = new StreamWriter(path.ToString(), false);
            file.WriteLine("Lista logów:");
            file.Flush();
        }

        public void Close() { file.Close(); }

        public void Write(StringBuilder msg) {
            file.WriteLine(msg.ToString());
            file.Flush();
        }
    }
}
