using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;



namespace CSMA_1_persistent
{
    public class Packet : Process
    {

        // Potwierdzenie odbioru.
        private bool ACK;

        //Czas nadania pakietu.
        private double startTime;

        // Czas transmisji pakietu.
        private int CTP;

        // Maksymalna liczba retransmisji.
        private const short LR = 5;

        // Numer retransmisji.
        private short r;

        // Faza procesu.
        private enum statement
        {
            waiting=0,
            polling=1,
            transmiting,
            ACKchecking
        }
        private statement phase;

        // Odwołanie do stacji bazowej.
        private Source baseStation;

        // Informacja o zakończeniu procesu.
        private bool _terminated;

        

        public bool terminated
        {
            get
            {
                return _terminated;
            }
            set
            {
                _terminated = value;
            }
        }

        public Packet(short identity, Process myBase, Space space)
        {
            ID = identity;
            ACK = false;
            startTime = -1.0;
            CTP = -1;
            r = 0;
            phase = statement.waiting;
            baseStation = myBase as Source;
            mySpace = space;
            terminated = false;
            file = space.file;
            myEvent = new Event(-1.0, this);
        }


        //
        // Metoda obsługi procesu.
        //
        public override void Execute()
        {
            Random random = new Random();
            bool active = true;
            double tempTime;
            while (active)
            {
                tempTime = WhatTimeIsIt();
                switch (phase)
                {
                    case statement.waiting: phase = statement.polling;break;
                    case statement.polling:// faza odpytywania
                        if (mySpace.ChannelIsEmpty() == true)
                        {
                            startTime = tempTime;
                            phase = statement.transmiting;
                            mySpace.AddToChannel(this);
                            WriteToFile(0);
                        }
                        else
                        {
                            List<Packet> collision = mySpace.SameTimeInChannel(tempTime);
                            if (collision.Count != 0)
                            {
                                foreach (Packet o in collision) o.ACK = false;
                                startTime = tempTime;
                                phase = statement.transmiting;
                                ACK = false;
                                mySpace.AddToChannel(this);
                                WriteToFile(0);
                            }
                            else
                            {
                                WriteToFile(0.5);
                                Plan(0.5);
                                active = false;
                            }
                        }
                        break;

                    case statement.transmiting:
                        if (CTP < 0)
                            CTP = random.Next()%10 + 1;// CTP losowy całkowity czas {1,2...10}
                        phase = statement.ACKchecking;
                        WriteToFile(CTP + 1);
                        Plan(CTP + 1);
                        active = false;
                        break;

                    case statement.ACKchecking:
                        mySpace.RemoveFromChannel(this);
                        if(ACK==false && r < LR)
                        {
                            r++;
                            phase = statement.polling;
                            double R = random.NextDouble()*MaxRangeOfR(r);// R należy do <0,2^r-1>
                            WriteToFile(R * CTP);
                            Plan(R * CTP);    
                         }
                         else
                         {
                            terminated = true;
                            Packet nextPacket = baseStation.DeleteFirstPacket();
                            nextPacket.Plan(tempTime+1);// +1 ponieważ początkowo _eventTime=-1
                                
                         }
                         active = false;
                         break;
                }
            }
        }
    
        
        public override void WriteToFile(double nextTime)
        {

            double time = WhatTimeIsIt();
            file.WriteLine(GetType().ToString() + "\t\t"
                            + ID.ToString() + "\t\t"
                            + phase.ToString() + "\t\t"
                            + r.ToString() + "\t\t"
                            + time.ToString() + "\t\t\t"
                            + (time + nextTime).ToString());
   
            //file.Flush();
        }
        //
        // Odczytanie aktualnego czasu zdarzenia pakietu.
        //
        public double WhatTimeIsIt()
        {
            return myEvent.eventTime;
        }

        //
        // Oblicza maksymalną wartość przedziału wartości R.
        //
        private double MaxRangeOfR(int r)
        {
            int max = 1;
            for (int i = 0; i < r; i++)
                max *= r;
            return max - 1;
        }


    }
}
