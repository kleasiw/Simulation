using System;
using System.Collections.Generic;
using System.Text;
using static CSMA_1_persistent.Generators;

namespace CSMA_1_persistent
{
    public class Packet : Process
    {
        private bool ACK;                       // Potwierdzenie odbioru.
        private int startTime;                  // Czas nadania pakietu.
        private int bufferTime;                 // Czas pojawienia się w buforze.
        private int CTP;                        // Czas transmisji pakietu.
        private const short LR = 2;             // Maksymalna liczba retransmisji.
        private statement phase;                // Faza procesu.
        private short r;                        // Numer retransmisji.
        private Source baseStation;             // Odwołanie do stacji bazowej.
        private bool terminated { get; set; }   // Informacja o zakończeniu procesu.
        private UniformGenerator uniformIntiger;// Generator równomierny z zakresu {0,1...10}
        private UniformGenerator uniformInterval;// Generator równomierny z przedziału.

        private enum statement          
        {
            waiting,
            polling,
            transmiting,
            ACKchecking
        }
       

        public Packet(short identity, int time, Process myBase, System space,Logs l, UniformGenerator[] newGenerators)
        {
            ID = identity;
            ACK = true;
            startTime = CTP = -1;
            bufferTime = time;
            r = 0;
            phase = statement.waiting;
            baseStation = myBase as Source;
            mySpace = space;
            logsDocument = l;
            terminated = false;
            myEvent = new Event(-1, this);

            uniformIntiger = newGenerators[0];
            uniformInterval = newGenerators[1];
            
        }


        //
        // Metoda obsługi procesu.
        //
        public override void Execute()
        {
            bool active = true;
            int tempTime;
            while (active)
            {
                tempTime = myEvent.eventTime;
                switch (phase)
                {
                    case statement.waiting:
                        WriteToFile(0);
                        phase = statement.polling;
                        break;

                    case statement.polling:
                        // Faza odpytywania
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
                                WriteToFile(5);
                                Plan(5);
                                active = false;
                            }
                        }
                        break;

                    case statement.transmiting:

                        if (CTP < 0)
                        {
                            // Nie było wcześniej transmisji, więc zapisuje opuszczenie bufora
                            if (mySpace.withPhase)
                                mySpace.stats.AddWaiting(tempTime - bufferTime);
                            else
                            {
                                if (!mySpace.IsStartingPhase())
                                    mySpace.stats.AddWaiting(tempTime - bufferTime);
                            } 
                        }

                        double x = uniformIntiger.Rand() * 10;
                        CTP = (int)x + 1;       // CTP losowy całkowity czas {1,2...10}
                        if (CTP == 11) CTP = 10;
                        CTP *= 10;

                        WriteToFile(CTP+10);    // 10 - czas powrotu ACK
                        phase = statement.ACKchecking;
                        Plan(CTP+10);
                        active = false;
                        break;

                    case statement.ACKchecking:

                        mySpace.RemoveFromChannel(this);
                        if(ACK==false && r < LR)
                        {
                            // Transmisja się nie powiodła, ale nie wykorzystano limitu retransmisji
                            double R = Math.Round(uniformInterval.Rand(0,MaxRangeOfR(r+1)),1);// R należy do <0,2^r-1>
                            WriteToFile((int)( R * CTP));
                            r++;
                            phase = statement.polling;
                            ACK = true;
                            Plan((int)(R * CTP));
                         }
                        else if( ACK==false && r >= LR)
                        {
                            // Transmisja się nie powiodła i nie można już retransmitować
                            terminated = true;
                            WriteToFile(0);
                            if (mySpace.withPhase)
                                mySpace.stats.AddFail(ID);
                            else
                            {
                                if (!mySpace.IsStartingPhase())
                                    mySpace.stats.AddFail(ID);
                            }
                                Packet nextPacket = baseStation.DeleteFirstPacket();
                            if (nextPacket != null)
                                nextPacket.Plan(tempTime+1);
                        }
                         else
                         {
                            // Transmisja się powiodła.
                            terminated = true;
                            WriteToFile(0);
                            if (mySpace.withPhase)
                            {
                                mySpace.stats.AddSuccess(ID);
                                mySpace.stats.AddRetrans(r);
                                mySpace.stats.AddDelay(tempTime - bufferTime);
                            }
                            else
                            {
                                if (!mySpace.IsStartingPhase())
                                {
                                    mySpace.stats.AddSuccess(ID);
                                    mySpace.stats.AddRetrans(r);
                                    mySpace.stats.AddDelay(tempTime - bufferTime);
                                }
                            }
                            mySpace.numOfPacket++;
                            Packet nextPacket = baseStation.DeleteFirstPacket();
                            if(nextPacket!=null)
                                nextPacket.Plan(tempTime+1);
                         }
                         active = false;
                         break;
                }
            }
        }
    
        
        public override void WriteToFile(int nextTime)
        {
            double time = myEvent.eventTime;
            StringBuilder message= new StringBuilder();
            if (!mySpace.mode || mySpace.logOn)
            {
                message.Append("Pakiet z ID: " + ID.ToString()
                                + " w chwili " + time.ToString()
                                + " będący w fazie: " + phase.ToString()
                                + " (retrans.: " + r.ToString() + ")");

                switch (this.phase)
                {
                    case statement.waiting:
                        message.Append(" został obudzony.");
                        Console.ResetColor();
                        break;

                    case statement.polling:
                        message.Append(" odpytuje kanał.");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;

                    case statement.transmiting:
                        message.Append(" rozpoczyna transmisję, która zakończy się w chwili: "
                                + (time + nextTime).ToString() + ".");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;

                    case statement.ACKchecking:
                        message.Append(" sprawdza ACK: " + ACK.ToString());
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (ACK == false)
                        {
                            if (r < LR)
                                message.Append(" i rozpoczyna retransmisję z nr: " + (r + 1).ToString()
                                + ", w chwili: " + (time + nextTime).ToString());
                            else
                                message.Append(" i zostaje odrzucony.");
                        }
                        else
                        {
                            message.Append(" i kończy transmisję, wzbudzając nowy pakiet.");
                        }
                        break;
                }
                if (!mySpace.mode)
                    Console.WriteLine(message);
                if (mySpace.logOn)
                    logsDocument.Write(message);
            }
            
            
        }


        //
        // Odczytanie czasu początku transmisji pakietu.
        //
        public double WhenItStarted() { return startTime; }


        //
        // Oblicza maksymalną wartość przedziału wartości R.
        //
        internal int MaxRangeOfR(int r)
        {
            int max = 1;
            for (int i = 0; i < r; i++)
                max *= 2;
            return max - 1;
        }
    }
}
