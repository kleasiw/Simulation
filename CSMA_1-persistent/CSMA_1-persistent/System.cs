using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CSMA_1_persistent
{
    /// <summary>
    /// Klasa symulacyjna systemu.
    /// </summary>
    public class System
    {
        private int globalTime;             // Aktualny czas liczony w ms.
        private int endTime;
        private Logs logsDocument;          // Uchwyt do dokumentu zapisu logów
        private const int K = 10;           // Liczba nadajników i odbiorników.
        private static List<Packet> channel;// Lista reprezentująca transmitowane w kanale pakiety.
        private List<Source> transmitters;  // Lista nadajników.
        private const int endOfStartingPhase = 130; // Numer pakietu kończącego fazę początkową.

        public List<Event> agenda;          // Zbiór zaplanowanych zdarzeń.
        public readonly bool logOn;         // Flaga wyświetlania logów.
        public readonly bool mode;          // Flaga zapisu logów.
        public readonly bool withPhase;     // Flaga uwzględniania fazy początkowej
        public Statistics stats;            // Uchwyt do zbierania statystyk.
        public int numOfPacket { get; set; }// Liczba pakietów popawnie odebranych.
        
        public bool IsStartingPhase()
        {
            bool result = numOfPacket < endOfStartingPhase ? true : false;
            if (numOfPacket == 130 && stats.StartTimeIsZero())
                    stats.SetStartTime(globalTime);
            return result;
        }

        public System(double lambda, Kernels kernel, bool logsFile, bool mode_, bool phase, int time)
        {
            channel = new List<Packet>();
            agenda = new List<Event>();
            transmitters = new List<Source>();
            if (logsFile)
                logsDocument = new Logs();
            else logsDocument = null;
            logOn = logsFile;
            mode = mode_;
            endTime = time;
            withPhase = phase;

            for (short i = 0; i < K; i++)
            { transmitters.Add(new Source(i, this, logsDocument, lambda, kernel)); }

            stats = new Statistics(K,lambda,endTime,this);
        }

        /// <summary>
        /// Pętla główna symulacji.
        /// </summary>
        /// <param name="decision">Wybór trybu symulacji: true - krokowy,
        /// false - ciągły.</param>
        public void Simulation()
        {
            globalTime = -1;
            Event current;
            while (globalTime < endTime)
            {
                current = agenda.Last();        // Wybór najwcześniejszego zdarzenia
                agenda.Remove(current);         // Usunięcie go z listy zdarzeń(zostanie dodany w trakcie Execute)
                globalTime = current.eventTime; // Ustalenie aktualnego czasu symulacji
                current.GetProc().Execute();   
                if (!mode) Console.ReadKey();
            }
        }

        public void AddToChannel(Packet packet)
        {
            channel.Add(packet);
        }

        public void RemoveFromChannel(Packet packet)
        {
            channel.Remove(packet);
        }

        public bool ChannelIsEmpty()
        {
            return (channel.Count == 0);
        }

        //
        // Znajduje pakiety w kanale z tym samym zadanym czasem.
        //
        public List<Packet> SameTimeInChannel(double time)
        {
            return channel.FindAll(o => o.WhenItStarted() == time).ToList();
        }

        //
        // Dodanie komunikatu zdarzenia w odpowiednie miejsce w agendzie.
        //
        public void AddToAgenda(Event ev)
        {
            int index = agenda.IndexOf(agenda.Find(el => el.eventTime <= ev.eventTime));
            if (index < 0) index = agenda.Count;
            agenda.Insert(index, ev);   
        }

        public void ShowStats(StreamWriter f) { stats.ShowStats(f); }

        public void Close()
        {
            try
            {
                logsDocument.Close();
            }
            catch (NullReferenceException) { return; }
        }   
    }
}
