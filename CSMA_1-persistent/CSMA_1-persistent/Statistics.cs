using System;
using System.Linq;
using System.IO;

namespace CSMA_1_persistent
{
    public class Statistics
    {
        private System mySpace;
        private int K;
        private double lambda;
        private int endTime;                // Czas zakończenia symulacji.
        private int startMeasurment;        // Czas rozpoczęcia pomiarów.
        private int retransmition;          // Liczba retransmisji.
        private int success;                // Liczba odebranych pakietów.
        private int[] successPerTransmitter;// Tablica opisująca ilość odebranych pakietów dla danego nadajnika(ID=indeks)      
        private int fail;                   // Liczba nieodebranych pakietów.
        private int[] failPerTransmitter;   // Tablica opisująca ilość straconych pakietów dla danego nadajnika(ID=indeks)
        private long delay;                 // Suma czasów opóźnień.
        private long waitingInBuf;          // Suma czasów oczekiwania na opuszczenie bufora.

        public double[] errorProbability;   // Stopy błędu per nadajnik.


        public Statistics(int k, double lambda_, int end, System s)
        {
            mySpace = s;
            delay = waitingInBuf = 0;
            success = fail = retransmition = 0;
            K = k;
            failPerTransmitter = new int[K];
            successPerTransmitter = new int[K];
            errorProbability = new double[K];
            
            lambda = lambda_;
            endTime = end;
        }

        public void SetStartTime(int time) { startMeasurment = time; }
        public bool StartTimeIsZero() { return startMeasurment == 0 ? true : false; }
        public void AddFail(int id) { failPerTransmitter[id]++; }
        public void AddSuccess(int id) { successPerTransmitter[id]++; }
        public void AddDelay(int d) { delay += d; }
        public void AddWaiting(int w) { waitingInBuf += w; }
        public void AddRetrans(int R) { retransmition += R; }

        public void ShowStats(StreamWriter f)
        {
            double avgError, maxError;
            GetStats(out avgError, out maxError);
            Random rand = new Random();
            Console.ForegroundColor = (ConsoleColor)(rand.Next()%15 + 1 );
            String msg = "\nLiczba nadajników: " + K.ToString()
                            + "\nMaksymalny czas symulacji: " + endTime.ToString()
                            + "\nWartość lambda: " + lambda.ToString();
            f.WriteLine(msg);
            Console.WriteLine(msg);
            f.Flush();
       
            f.Write("\n//---------------------------------------------------------//\n");
            f.WriteLine("Obsłużone pakiety: " + success.ToString());
            f.WriteLine("Stracone pakiety: " + fail.ToString());
        
            f.WriteLine("\nŚrednia stopa błędów: " + avgError.ToString());
            f.WriteLine("Maksymalna stopa błędów: " + maxError.ToString());

            f.WriteLine("\nŚrednie opóźnienie: " + (delay/10 / (double)(success + fail)).ToString("0.0000") + "ms.");
            f.WriteLine("Średni czas oczekiwania: " + (waitingInBuf/10 / (double)(success + fail)).ToString("0.0000") + "ms.");
            f.WriteLine("Średnia liczba retransmisji: " + (retransmition / (double)success).ToString("0.000000"));

            f.WriteLine("\nPrzepływność systemu: " + (success * 10000 / (double)(endTime-startMeasurment)).ToString("0.000000") + " pakietów na sek.");
            f.Write("\n//---------------------------------------------------------//\n");
            f.Flush();

           Console.Write("\n//---------------------------------------------------------//\n");
           Console.WriteLine("Obsłużone pakiety: " + success.ToString());
           Console.WriteLine("Stracone pakiety: " + fail.ToString());

           Console.WriteLine("\nŚrednia stopa błędów: " + avgError.ToString());
           Console.WriteLine("Maksymalna stopa błędów: " + maxError.ToString());

           Console.WriteLine("\nŚrednie opóźnienie: " + (delay / 10 / (double)(success + fail)).ToString("0.0000") + "ms.");
           Console.WriteLine("Średni czas oczekiwania: " + (waitingInBuf / 10 / (double)(success + fail)).ToString("0.0000") + "ms.");
           Console.WriteLine("Średnia liczba retransmisji: " + (retransmition / (double)success).ToString("0.000000"));

           Console.WriteLine("\nPrzepływność systemu: " + (success * 10000 / (double)(endTime - startMeasurment)).ToString("0.000000") + " pakietów na sek.");
           Console.Write("//---------------------------------------------------------//\n");
            
        }

        internal void GetStats(out double avgE, out double maxE)
        {
            for (int i = 0; i < errorProbability.Length; i++)
            {
                errorProbability[i] = failPerTransmitter[i] / 
                                        (double)(failPerTransmitter[i] + successPerTransmitter[i]);
                success += successPerTransmitter[i];
                fail += failPerTransmitter[i];
            }
            avgE = fail / (double)(success + fail);
            maxE = errorProbability.Max();
        }
    }
}
