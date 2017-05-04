using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSMA_1_persistent
{
    /// <summary>
    /// Klasa określająca źródło generowania pakietów.
    /// </summary>
    class Source : Process 
    {
        private Queue<Packet> buffer;

        public Source(short number, Space space)
        {
            ID = number;
            mySpace = space;
            myEvent = new Event(0, this);
            mySpace.AddToAgenda(myEvent);
            buffer = new Queue<Packet>();
            file = space.file;
        }


        //
        // Metoda obsługi procesu.
        //
        public override void Execute()
        {
            Random random = new Random();
            buffer.Enqueue(new Packet(ID, this, mySpace));

            double time = random.NextDouble() * random.Next() % 10;// przykladowe losowanie czasu
            WriteToFile(time);
            Plan(time);
            

            if (buffer.Count == 1)
                buffer.First().Plan(1);// zaplanowanie zdarzenia pakietu

        }

        //
        // Metoda usuwa z bufora wysłany pakiet oraz zwraca referencje do nowego, 
        // pierwszego pakietu, który będzie "obudzony".
        //
        public Packet DeleteFirstPacket()
        {
            buffer.Dequeue();
            return buffer.First();
        }

        public override void WriteToFile(double nextTime)
        {
            double time = myEvent.eventTime;
            file.WriteLine(GetType().ToString() + "\t\t"
                            + ID.ToString() + "\t\t"
                            + "----\t\t"
                            + "----\t\t"
                            + time.ToString() + "\t\t\t"
                            + (time + nextTime).ToString());

            file.Flush();
        }
    }
}
