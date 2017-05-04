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
            air.Simulation();
            
        }
    }
}
