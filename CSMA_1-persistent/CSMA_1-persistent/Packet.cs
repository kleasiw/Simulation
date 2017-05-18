using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;
using static CSMA_1_persistent.RandGenerator;

namespace CSMA_1_persistent
{
    public class Packet : Process
    {

        // Potwierdzenie odbioru.
        private bool ACK;

        // Czas nadania pakietu.
        private double startTime;

        // Czas pojawienia się w buforze.
        private double bufferTime;
    
        // Czas transmisji pakietu.
        private int CTP;

        // Maksymalna liczba retransmisji.
        private const short LR = 5;

        // Numer retransmisji.
        private short r;

        // Faza procesu.
        private enum statement
        {
            waiting,
            polling,
            transmiting,
            ACKchecking
        }
        private statement phase;

        // Odwołanie do stacji bazowej.
        private Source baseStation;

        // Informacja o zakończeniu procesu.
        private bool _terminated;

        // Generator równomierny z zakresu {0,1...10}
        private UniformRandomGenerator uniformIntiger;

        // Generator równomierny z przedziału.
        private UniformRandomGenerator uniformInterval;


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

        public Packet(short identity, double time, Process myBase, Space space,Logs l, UniformRandomGenerator[] newGenerators)
        {
            ID = identity;
            ACK = true;
            startTime = -1.0;
            CTP = -1;
            bufferTime = time;
            r = 0;
            phase = statement.waiting;
            baseStation = myBase as Source;
            mySpace = space;
            logsDocument = l;
            terminated = false;
            myEvent = new Event(-1.0, this);

            uniformIntiger = newGenerators[0];
            uniformInterval = newGenerators[1];
            
        }


        //
        // Metoda obsługi procesu.
        //
        public override void Execute(double start)
        {
            //Random random = new Random();
            bool active = true;
            double tempTime;
            while (active)
            {
                tempTime = myEvent.eventTime;
                switch (phase)
                {
                    case statement.waiting: //WriteToFile(0);
                        phase = statement.polling;break;
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
                        {
                            if (tempTime > start)
                                // nie było wcześniej transmisji więc zapisuje opuszczenie bufora
                                mySpace.stats.AddWaiting(tempTime - bufferTime); // dodaj czas oczekiwania
                        }

                        double x = uniformIntiger.Rand() * 10;
                        CTP = (int)x + 1;// CTP losowy całkowity czas {1,2...10}
                        if (CTP == 11) CTP = 10;

                        WriteToFile(CTP+1);
                        phase = statement.ACKchecking;
                        Plan(CTP+1);// 1 - czas powrotu ACK
                        active = false;
                        break;

                    case statement.ACKchecking:

                        mySpace.RemoveFromChannel(this);
                        if(ACK==false && r < LR)
                        {
                            // transmisja się nie powiodła, ale nie wykorzystano limitu retransmisji
                            r++;
                            double R = Math.Round(uniformInterval.Rand(0,MaxRangeOfR(r)),1);// R należy do <0,2^r-1>
                            WriteToFile(R * CTP);
                            phase = statement.polling;
                            ACK = true;
                            Plan(R * CTP);
                         }
                        else if( ACK==false && r >= LR)
                        {
                            terminated = true;
                            if(tempTime>start)
                                // transmisja się nie powiodła i nie można już retransmitować
                                mySpace.stats.AddFail(ID); // zwiększ liczbę porażek
                            Packet nextPacket = baseStation.DeleteFirstPacket();
                            if (nextPacket != null)
                                nextPacket.Plan(tempTime+0.1);
                        }
                         else
                         {
                            terminated = true;
                            WriteToFile(0);
                            if (tempTime > start)
                            {
                                mySpace.stats.AddSuccess(ID); // zwiększ liczbę sukcesów
                                mySpace.stats.AddRetrans(r); // zwiększ sumę retransmisji
                                mySpace.stats.AddDelay(tempTime - bufferTime); // dodaj opóźnienie
                            }
                            Packet nextPacket = baseStation.DeleteFirstPacket();
                            if(nextPacket!=null)
                                nextPacket.Plan(tempTime+0.1);
                         }
                         active = false;
                         break;
                }
            }
        }
    
        
        public override void WriteToFile(double nextTime)
        {

            double time = myEvent.eventTime;
            StringBuilder message = new StringBuilder("Pakiet z ID: " + ID.ToString()
                            + " w " + time.ToString() + "ms"
                            + " będący w fazie: " + phase.ToString()
                            + " (retrans.: "+ r.ToString()+")");
            logsDocument.Write(message);
            Console.Write(message);
            message.Clear();
            switch (this.phase)
            {
                case statement.waiting:
                    message.Append(" został obudzony.");
                    logsDocument.Write(message);
                    Console.WriteLine(message); break;

                case statement.polling:
                    message.Append(" odpytuje kanał.");
                    logsDocument.Write(message);
                    Console.WriteLine(message); break;

                case statement.transmiting:
                    message.Append(" rozpoczyna transmisję, która zakończy się w chwili: "
                            + (time + nextTime).ToString());
                    logsDocument.Write(message);
                    Console.WriteLine(message); break;

                case statement.ACKchecking:
                    message.Append(" sprawdza ACK: " + ACK.ToString() + "\n");
                    logsDocument.Write(message);
                    Console.Write(message);
                    message.Clear();

                    //---------------------------//nalezy usprawnić logi, co jeśli ack ==false i r>=LR
                    //---------------------------//
                    if (ACK == false)
                    {
                        message.Append(" i rozpoczyna retransmisję z nr: " + r.ToString()
                            + ", w chwili: " + (time + nextTime).ToString());
                        logsDocument.Write(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        logsDocument.Write(message.Append(" i kończy transmisję, wzbudzając nowy pakiet."));
                        Console.WriteLine(" i kończy transmisję, wzbudzając nowy pakiet.\n");
                    }
                    break;
            }
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
        private int MaxRangeOfR(int r)
        {
            int max = 1;
            for (int i = 0; i < r; i++)
                max *= 2;
            return max - 1;
        }


    }
}
