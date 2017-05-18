using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static CSMA_1_persistent.RandGenerator;

namespace CSMA_1_persistent
{
    /// <summary>
    /// Klasa określająca źródło generowania pakietów.
    /// </summary>
    class Source : Process 
    {
        private Kernels kernel;
        private Queue<Packet> buffer;
        private ExponentialRandomGenerator myExpGenerator;
        // Generator równomierny z zakresu {0,1...10}
        public UniformRandomGenerator uniformIntiger;

        // Generator równomierny z przedziału.
        public UniformRandomGenerator uniformInterval;

        


        public Source(short number, Space space, Logs l, double lambda, Kernels k)
        {
            ID = number;
            mySpace = space;
            myEvent = new Event(0, this);
            mySpace.AddToAgenda(myEvent);
            buffer = new Queue<Packet>();
            kernel = k;
            logsDocument = l;
            uniformInterval = new UniformRandomGenerator(kernel.GetNewKernel());
            uniformIntiger = new UniformRandomGenerator(kernel.GetNewKernel());
            myExpGenerator = new ExponentialRandomGenerator(lambda,kernel.GetNewKernel());
        }


        //
        // Metoda obsługi procesu.
        //
        public override void Execute(double start)
        {
            UniformRandomGenerator[] uniform = { uniformIntiger, uniformInterval };
            buffer.Enqueue(new Packet(ID, myEvent.eventTime, this, mySpace,logsDocument, uniform));

            double time = Math.Round(myExpGenerator.Rand(),1);// użycie generatora wykładniczego
            WriteToFile(time);
            
            if (buffer.Count == 1)
            {
                buffer.First().Plan(this.myEvent.eventTime);// zaplanowanie zdarzenia pakietu
                StringBuilder message = new StringBuilder("Nadajnik nr: " + ID.ToString()
                            + " w chwili: " + this.myEvent.eventTime.ToString()
                            + " 'budzi' pakiet i ustala jego czas wykonania na: "
                            + this.myEvent.eventTime.ToString());
                logsDocument.Write(message);
                Console.WriteLine(message);
            }
            Plan(time);
        }

        //
        // Metoda usuwa z bufora wysłany pakiet oraz zwraca referencje do nowego, 
        // pierwszego pakietu, który będzie "obudzony".
        //
        public Packet DeleteFirstPacket()
        {
            buffer.Dequeue();
            if (buffer.Count == 0) return null;
            else return buffer.First();
        }

        public override void WriteToFile(double nextTime)
        {
            double time = myEvent.eventTime;
            StringBuilder message = new StringBuilder("Nadajnik nr: " + ID.ToString()
                            + " w chwili: " + time.ToString()
                            + " generuje pakiet i ustala czas nowej generacji pakietu: "
                            + (time + nextTime).ToString());
            logsDocument.Write(message);
            Console.WriteLine(message);

            //file.Flush();
        }
    }
}
