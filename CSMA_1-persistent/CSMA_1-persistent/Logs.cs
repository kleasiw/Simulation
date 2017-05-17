using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CSMA_1_persistent
{
    public class Logs
    {
        public StreamWriter file;
        public Logs()
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.FullName.ToString());

            StringBuilder pathLog = new StringBuilder(path.ToString() + "\\SimulationLogs.txt");
            file = new StreamWriter(pathLog.ToString(), false);
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
