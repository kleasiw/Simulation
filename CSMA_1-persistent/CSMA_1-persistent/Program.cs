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


            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.ToString());

            StringBuilder pathStats =new StringBuilder( path.ToString()+"\\staty3_nowy pakiet o 01ms.txt");
            StreamWriter staty = new StreamWriter(pathStats.ToString(), false);
        /*    StringBuilder pathLambda = new StringBuilder( path.ToString()+ "\\lambdy3.txt");
            StreamWriter lam = new StreamWriter(pathLambda.ToString(), false);
            int []array = air.KernelGeneration(30, 123);
            foreach (int i in array)
                file.WriteLine(i);
            file.Flush();*/

            double start, time, lambda;
            int K, ans, licznik = 0, max = 10;
            bool decision = false;
            Space air = null;
            string msg;
            Kernels kern = new Kernels();
            
       //     lam.WriteLine(lambda.ToString()+"\n-----------------------");*/
            while (true)
            {
                Console.WriteLine("Wybierz długość trwania symulacji w ms:");
                time = double.Parse(Console.ReadLine());

                Console.WriteLine("Wybierz czas rozpoczęcia zbierania pomiarów w ms:");
                start = double.Parse(Console.ReadLine());

                Console.WriteLine("Jaką wartość ma przyjmować lambda(parametr rozkładu wykładniczego)?");
                lambda = double.Parse(Console.ReadLine());

                Console.WriteLine("Ile nadajników?");
                K = int.Parse(Console.ReadLine());

                Console.WriteLine("Wybierz tryb symulacji: 1- ciągły, 2- krokowy");
                while ((!int.TryParse(Console.ReadLine(), out ans)) || (ans != 1 && ans != 2)) ;
                if (ans == 2) decision = true;

                air = new Space(lambda, K, kern);
                air.Simulation(decision,start,time);

                msg = "\nLiczba nadajników: " + K.ToString()
                                    + "\nMaksymalny czas symulacji: " + time.ToString()
                                    + "\nWartość lambda: " + lambda.ToString();
                staty.WriteLine(msg);
                Console.WriteLine(msg);
                air.ShowStats(staty);
                air.Close();
                //lam.WriteLine(air.stats.errorProbability.ToString()+";");
                //lam.Flush();
                licznik++;

               
                if (licznik >= max)
                      break;
                Console.ReadKey();

                //Console.WriteLine("Czy chcesz powtórzyć symulację?");
                //Console.ReadKey();// nie ma innej opcji, oczywiście że chcesz
            }
            //Console.ReadKey();
        }
    }
}
