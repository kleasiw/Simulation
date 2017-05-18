using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSMA_1_persistent
{
    public class Statistics
    {
        // Liczba retransmisji.
        private double retransmition;

        // Liczba odebranych pakietów.
        private int success;

        // Liczba nieodebranych pakietów.
        private int fail;

        // Suma czasów opóźnień.
        private double delay;
        
        // Suma czasów oczekiwania na opuszczenie buffora.
        private double waitingInBuf;

        // Stopa błędu
        public double[] errorProbability;

        // Tablica opisująca ilość straconych pakietów dla danego nadajnika(ID=indeks)
        private int[] failPerTrans;

        // Tablica opisująca ilość straconych pakietów dla danego nadajnika(ID=indeks)
        private int[] successPerTrans;

        public Statistics(int K)
        {
            retransmition = delay = waitingInBuf = 0.0;
            success = fail = 0;
            failPerTrans = new int[K];
            successPerTrans = new int[K];
            errorProbability = new double[K];
        }

        public void AddFail(int id) { failPerTrans[id]++; }
        public void AddSuccess(int id) { successPerTrans[id]++; }
        public void AddDelay(double d) { delay += d; }
        public void AddWaiting(double w) { waitingInBuf += w; }
        public void AddRetrans(double R) { retransmition += R; }

        public void ShowStats(StreamWriter f,double globalTime)
        {
            double avgError, maxError;
            GetStats(out avgError,out maxError);
            
            f.Write("\n//---------------------------------------------------------//\n");
            f.WriteLine("Obsłużone pakiety: " + success.ToString());
            f.WriteLine("Stracone pakiety: " + fail.ToString());

            f.WriteLine("\nŚrednia stopa błędów: " + avgError.ToString());
            f.WriteLine("Maksymalna stopa błędów: " + maxError.ToString());

            f.WriteLine("\nŚrednie opóźnienie: " + (delay / (success + fail)).ToString() + "ms.");
            f.WriteLine("Średni czas oczekiwania: " + (waitingInBuf / (success + fail)).ToString() + "ms.");
            f.WriteLine("Średnia liczba retransmisji: " + (retransmition / success).ToString());

            f.WriteLine("\nPrzepływność systemu: " + (success*1000 / globalTime).ToString() + " pakietów na sek.");
            f.Write("\n//---------------------------------------------------------//\n");
            f.Flush();
   
            Console.Write("\n//---------------------------------------------------------//\n");
            Console.WriteLine("Obsłużone pakiety: " + success.ToString());
            Console.WriteLine("Stracone pakiety: " + fail.ToString());

            Console.WriteLine("\nŚrednia stopa błędów: " + avgError.ToString());
            Console.WriteLine("Maksymalna stopa błędów: " + maxError.ToString());

            Console.WriteLine("\nŚrednie opóźnienie: " + (delay / (success + fail)).ToString() + "ms.");
            Console.WriteLine("Średni czas oczekiwania: " + (waitingInBuf / (success + fail)).ToString() + "ms.");
            Console.WriteLine("Średnia liczba retransmisji: " + (retransmition / success).ToString());

            Console.WriteLine("\nPrzepływność systemu: " + (success*1000 / globalTime).ToString() + " pakietów na sek.");
            Console.Write("//---------------------------------------------------------//\n");

        }

        private void GetStats(out double avgE, out  double maxE)
        {
            for(int i=0;i<errorProbability.Length;i++)
            {
                errorProbability[i] = failPerTrans[i] / (double)(failPerTrans[i] + successPerTrans[i]);
                success += successPerTrans[i];
                fail += failPerTrans[i];
            }
            avgE = fail/(double)(success+fail);
            maxE = errorProbability.Max();
        }
    }
}
