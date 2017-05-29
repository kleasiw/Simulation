namespace CSMA_1_persistent
{
    /// <summary>
    /// Ta klasa reprezentuje procesy rozpoczynające się
    /// w danej chwili eventTime.
    /// </summary>
    public class Event
    {
        private int _eventTime;
        private Process myProcess;

        public int eventTime
        {
            get { return _eventTime; }
            set { _eventTime += value; }
        }

        public Event(int time, Process proc)
        {
            _eventTime = time;
            myProcess = proc;
        }

        public Process GetProc() { return myProcess; }

    }
}
