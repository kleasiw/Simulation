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
            ACK = true;
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
                tempTime = myEvent.eventTime;
                switch (phase)
                {
                    case statement.waiting: WriteToFile(0);phase = statement.polling;break;
                    case statement.polling:// faza odpytywania
                        if (mySpace.ChannelIsEmpty() == true)
                        {
                            WriteToFile(0);
                            startTime = tempTime;
                            phase = statement.transmiting;
                            mySpace.AddToChannel(this);
                        }
                        else
                        {
                            List<Packet> collision = mySpace.SameTimeInChannel(tempTime);
                            if (collision.Count != 0)
                            {
                                foreach (Packet o in collision) o.ACK = false;
                                startTime = tempTime;
                                WriteToFile(0);
                                phase = statement.transmiting;
                                ACK = false;
                                mySpace.AddToChannel(this);
                                
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
                        WriteToFile(CTP + 1);
                        phase = statement.ACKchecking;
                        Plan(CTP + 1);
                        active = false;
                        break;

                    case statement.ACKchecking:

                        mySpace.RemoveFromChannel(this);
                        if(ACK==false && r < LR)
                        {
                            r++;
                            double R = random.NextDouble()*MaxRangeOfR(r);// R należy do <0,2^r-1>
                            WriteToFile(R * CTP);
                            phase = statement.polling;
                            ACK = true;
                            Plan(R * CTP);    
                         }
                         else
                         {
                            terminated = true;
                            WriteToFile(0);
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

            double time = myEvent.eventTime;
            string message = "Pakiet z ID:" + ID.ToString()
                            + " w chwili: " + time.ToString()
                            + " znajdujący się w fazie: " + phase.ToString()
                            + " (retrans.: "+ r.ToString()+")";
            file.Write(message);
            Console.Write(message);
            switch (this.phase)
            {
                case statement.waiting:
                    message = " oczekuje na obudzenie.";
                    file.WriteLine(message);
                    Console.WriteLine(message); break;

                case statement.polling:
                    message = " odpytuje dostępność kanału";
                    file.WriteLine(message);
                    Console.WriteLine(message); break;

                case statement.transmiting:
                    message = " rozpoczyna transmisję, która zakończy się w chwili: "
                            + (time + nextTime).ToString();
                    file.WriteLine(message);
                    Console.WriteLine(message); break;

                case statement.ACKchecking:
                    message = " sprawdza ACK: " + ACK.ToString();
                    file.Write(message);
                    Console.Write(message);
                    if (ACK == false)
                    {
                        message = " i rozpoczyna retransmisję z nr: " + r.ToString()
                            + ", w chwili: " + (time + nextTime).ToString();
                        file.WriteLine(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        file.WriteLine(" i kończy transmisję, wzbudzając nowy pakiet.");
                        Console.WriteLine(" i kończy transmisję, wzbudzając nowy pakiet.");
                    }
                    break;
            }
            file.Flush();
        }
        //
        // Odczytanie aktualnego czasu zdarzenia pakietu.
        //
        public double WhenItStarted()
        {
            return startTime;
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
