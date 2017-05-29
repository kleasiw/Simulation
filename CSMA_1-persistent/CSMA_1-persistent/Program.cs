using System;
using System.Text;
using System.IO;

namespace CSMA_1_persistent
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.ToString());
            path.Append("\\stats.txt");
            StreamWriter stats = new StreamWriter(path.ToString(), false);

            int time, licznik = 0; ;
            double lambda, max = 10;
            System space = null;
            Kernels kern = new Kernels();
            bool mode, logsFileON, phase;

            GetParam(out lambda, out time, out logsFileON, out phase, out mode);

            while (licznik<max)
            {
                space = new System(lambda, kern, logsFileON, mode, phase, time);
                space.Simulation();
                
                space.ShowStats(stats);
                space.Close();
                licznik++;
                Console.ReadKey();
            }
            kern.Close();
        }

        static void GetParam(out double lambda, out int time, out bool logsON, out bool phase, out bool mode)
        {
            int ans;
            logsON = phase = mode = false;

            Console.WriteLine("PROGRAM PRZEWIDUJE WYKONANIE 10 SYMULACJI DLA PARAMETRÓW KTÓRE ZARAZ WYBIERZESZ\n\n"
                                + "Wybierz długość trwania symulacji w ms:");
            time = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Jaką wartość ma przyjmować lambda(parametr rozkładu wykładniczego)?");
            lambda = double.Parse(Console.ReadLine());

            Console.WriteLine("Czy pominąc fazę początkową? 1 - tak, 2 - nie");
            while ((!int.TryParse(Console.ReadLine(), out ans)) || (ans != 1 && ans != 2)) ;
            if (ans == 2) phase = true;

            Console.WriteLine("Wybierz tryb symulacji: 1 - ciągły(brak wyświetlania logów), 2 - krokowy(wyświetlenie logów w konsoli)");
            while ((!int.TryParse(Console.ReadLine(), out ans)) || (ans != 1 && ans != 2)) ;
            if (ans == 1) mode = true;

            Console.WriteLine("Czy chcesz zapisywać logi do pliku? 1 - tak, 2 - nie");
            while ((!int.TryParse(Console.ReadLine(), out ans)) || (ans != 1 && ans != 2)) ;
            if (ans == 1) logsON = true;
        }
    }
}
