namespace CSMA_1_persistent
{
     /// <summary>
     /// Klasa bazowa procesów.
     /// </summary>
    public abstract class Process
    {
        public short ID { get; set; }
        protected Event myEvent;
        protected System mySpace;
        protected Logs logsDocument;
        
        //
        // Ta metoda planuje kolejne zdarzenie.
        //
        public void Plan(int time)
        {
            if(myEvent.eventTime<0)
                myEvent.eventTime = time + 1;
            else
                myEvent.eventTime = time;
            mySpace.AddToAgenda(myEvent);
        }

        public virtual void Execute() { }

        public virtual void WriteToFile(int nextTime) { }

    }
}
