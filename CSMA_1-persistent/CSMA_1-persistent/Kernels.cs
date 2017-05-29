using System;
using System.IO;
using System.Text;

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
            try
            {
                kernels = new StreamReader(pathKernel.ToString());
            }
            catch (FileNotFoundException)
            {
                KernelGeneration(10, 123, 10);
                kernels = new StreamReader(pathKernel.ToString());
            }
        }
        public int GetNewKernel()
        {
            return int.Parse(kernels.ReadLine());
        }
        public void Close() { kernels.Close(); }

        private void KernelGeneration(int simulations, int kernel, int K)
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.FullName.ToString());
            path.Append("\\kernels.txt");
            StreamWriter save = new StreamWriter(path.ToString(), false);
            Generators.UniformGenerator gen = new Generators.UniformGenerator(kernel);
            for (int i = 0; i < K; i++)
            {
                for (int j = 0; j < simulations; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int m = 0; m < 100000; m++)
                            gen.Rand(0, int.MaxValue);
                        save.WriteLine(gen.get_kernel());
                        save.Flush();
                    }
                }
            }
            save.Close();
        }
    }
}
