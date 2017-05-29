using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CSMA_1_persistent.Generators;

namespace CSMA_1_persistent
{
    /// <summary>
    /// Klasa określająca źródło generowania pakietów.
    /// </summary>
    class Source : Process 
    {
        private Kernels kernel;
        private Queue<Packet> buffer;
        private ExpGenerator myExpGenerator;
        public UniformGenerator uniformIntiger;
        public UniformGenerator uniformInterval;
        

        public Source(short number, System space, Logs l, double lambda, Kernels k)
        {
            ID = number;
            mySpace = space;
            myEvent = new Event(0, this);
            mySpace.AddToAgenda(myEvent);
            buffer = new Queue<Packet>();
            kernel = k;
            logsDocument = l;
            uniformInterval = new UniformGenerator(kernel.GetNewKernel());
            uniformIntiger = new UniformGenerator(kernel.GetNewKernel());
            myExpGenerator = new ExpGenerator(lambda,new UniformGenerator(kernel.GetNewKernel()));
        }
        
        //
        // Metoda obsługi procesu.
        //
        public override void Execute()
        {
            UniformGenerator[] uniform = { uniformIntiger, uniformInterval };
            buffer.Enqueue(new Packet(ID, myEvent.eventTime, this, mySpace,logsDocument, uniform));

            double t = myExpGenerator.Rand();
            int time = (int)Math.Round(t*10,0);
            WriteToFile(time);
            
            if (buffer.Count == 1)
            {
                buffer.First().Plan(this.myEvent.eventTime);
                WriteToFileAwake(this.myEvent.eventTime); 
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

        public void WriteToFileAwake(int time)
        {
            StringBuilder message = new StringBuilder("Nadajnik nr: " + ID.ToString()
                            + " w chwili: " + time.ToString()
                            + " budzi pakiet.");
            if (!mySpace.mode)
                Console.WriteLine(message);
            if (mySpace.logOn)
                logsDocument.Write(message);
        }
        public override void WriteToFile(int nextTime)
        {
            
            int time = myEvent.eventTime;
            StringBuilder message = new StringBuilder("Nadajnik nr: " + ID.ToString()
                                + " w chwili: " + time.ToString()
                                + " generuje pakiet i ustala czas nowej generacji pakietu: "
                                + (time + nextTime).ToString());
            if (!mySpace.mode)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
            }
            if (mySpace.logOn)
                logsDocument.Write(message);
        }
    }
}
