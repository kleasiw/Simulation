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


            /*DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.ToString());

            StringBuilder pathK = new StringBuilder(path.ToString() + "\\kernels.txt");
            StreamWriter file = new StreamWriter(pathK.ToString(), false);
            int []array = air.KernelGeneration(30, 123);
            foreach (int i in array)
                file.WriteLine(i);
            file.Flush();*/
            double time, lambda;
            int K,ans;
            bool decision = false;
            Space air = null;
            while (true)
            {
                Console.WriteLine("Wybierz długość trwania symulacji w ms:");
                time = double.Parse(Console.ReadLine());

                Console.WriteLine("Jaką wartość ma przyjmować lambda(parametr rozkładu wykładniczego)?");
                lambda = double.Parse(Console.ReadLine());

                Console.WriteLine("Ile nadajników?");
                K = int.Parse(Console.ReadLine());

                Console.WriteLine("Wybierz tryb symulacji: 1- ciągły, 2- krokowy");
                while ((!int.TryParse(Console.ReadLine(), out ans)) || (ans != 1 && ans != 2)) ;
                if (ans == 2) decision = true;

                air = new CSMA_1_persistent.Space(lambda, K);
                air.Simulation(decision, time);
                Console.WriteLine("\n\nLiczba nadajników: " + K.ToString()
                                    + "\nMaksymalny czas symulacji: " + time.ToString()
                                    + "\nWartość lambda: " + lambda.ToString());
                air.ShowStats();
                air.Close();
                Console.WriteLine("Czy chcesz powtórzyć symulację?");
                Console.ReadKey();// nie ma innej opcji, oczywiście że chcesz
            }
            Console.ReadKey();
        }
    }
}
