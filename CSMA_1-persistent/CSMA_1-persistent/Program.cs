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
            Console.WriteLine("Wybierz tryb symulacji: 1- ciągły, 2- krokowy");
            int ans=9; bool decision = false;
            while((!int.TryParse(Console.ReadLine(),out ans)) || (ans!=1 && ans!=2));
            if (ans == 2) decision = true;
            air.Simulation(decision);
            Console.ReadKey();
        }
    }
}
