using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMA_1_persistent
{
    public class Statistics
    {
        // Liczba retransmisji.
        private double retransmition;

        // Liczba odebranych pakietów.
        private double success;

        // Liczba nieodebranych pakietów.
        private double fail;

        // Liczba aktywnych pakietów.
        private double packets;

        // Suma czasów opóźnień.
        private double delay;
        
        // Suma czasów oczekiwania na opuszczenie buffora.
        private double waitingInBuf;

        public Statistics()
        {
            retransmition = success = fail = packets = delay = waitingInBuf = 0.0;
        }

        public void AddFail() { fail++; }
        public void AddSuccess() { success++; }
        public void AddDelay(double d) { delay += d; }
        public void AddWaiting(double w) { waitingInBuf += w; }
        public void AddRetrans(double R) { retransmition += R; }

        public void ShowStats()
        {
            Console.Write("\n");
            Console.WriteLine("Średnia liczba retransmisji: " + (retransmition / success).ToString());
            Console.WriteLine("Średni czas opóźnienia: " + (delay / (success + fail)).ToString());
            Console.WriteLine("Średnia stopa błędów: " + (fail / (success + fail)).ToString());
            Console.WriteLine("Średni czas oczekiwania: " + (waitingInBuf / (success + fail)).ToString());
            Console.WriteLine("Obsłużone pakiety: " + success.ToString());
            Console.WriteLine("Stracone pakiety: " + fail.ToString());
        }

    }
}
