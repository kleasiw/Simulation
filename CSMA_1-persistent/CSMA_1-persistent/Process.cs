using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;

namespace CSMA_1_persistent
{
     /// <summary>
     /// Klasa bazowa procesów.
     /// </summary>
    public abstract class Process
    {
        public short ID { get; set; }
        protected Event myEvent;
        protected Space mySpace;
        protected Logs logsDocument;
        //protected Kernels kernel;


        //
        // Ta metoda planuje kolejne zdarzenie.
        //
        public void Plan(double time)
        {
            if(myEvent.eventTime<0) // jesli jest pakietem z czasem -1
                myEvent.eventTime = time + 1;
            else
                myEvent.eventTime = time;
            mySpace.AddToAgenda(myEvent);
        }

        //
        // Metoda obsługi procesu.
        //
        public virtual void Execute() { }

        public virtual void WriteToFile(double nextTime) { }

    }
}
