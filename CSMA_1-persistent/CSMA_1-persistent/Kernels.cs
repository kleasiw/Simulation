using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMA_1_persistent
{
    public class Kernels
    {
        private StreamReader kernels;
        public Kernels()
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.FullName.ToString());
            StringBuilder pathKernel = new StringBuilder(path.ToString());
            pathKernel.Append("\\kernels.txt");
            kernels = new StreamReader(pathKernel.ToString());
        }
        public int GetNewKernel()
        {
            return int.Parse(kernels.ReadLine());
        }
        public void Close() { kernels.Close(); }
    }
}
