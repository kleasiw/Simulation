using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSMA_1_persistent
{
    class Program
    {
        
        static void Main(string[] args)
        {
          
            //
            // Przykładowa symulacja sprawdzająca działanie
            //
            Space air = new CSMA_1_persistent.Space();

            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder();

            StringBuilder pathK = new StringBuilder(path.ToString() + "\\kernels.txt");
            StreamWriter file = new StreamWriter(pathK.ToString(), false);
            int []array = air.KernelGeneration(10, 123);
            foreach (int i in array)
                file.WriteLine(i);
            file.Flush();

            path.Append(directory.FullName + "\\kernels.txt");
            StreamReader kernels = new StreamReader(path.ToString());

            /*Console.WriteLine("Wybierz tryb symulacji: 1- ciągły, 2- krokowy");
            int ans=9; bool decision = false;
            while((!int.TryParse(Console.ReadLine(),out ans)) || (ans!=1 && ans!=2));
            if (ans == 2) decision = true;
            air.Simulation(decision);*/
            Console.ReadKey();
        }
    }
}
