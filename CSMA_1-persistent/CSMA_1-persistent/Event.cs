using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMA_1_persistent
{

    /// <summary>
    /// Ta klasa reprezentuje procesy rozpoczynające się
    /// w danej chwili eventTime.
    /// </summary>
    public class Event
    {

        public double eventTime
        {
            get { return _eventTime; }
            set { _eventTime += value; }
        }

        private double _eventTime;
        private Process myProcess;
        
        public Event(double time, Process proc)
        {
            _eventTime = time;
            myProcess = proc;
        }
        public Process GetProc() { return myProcess; }

    }
}
