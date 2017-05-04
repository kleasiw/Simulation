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
    /// Klasa symulacyjna systemu.
    /// </summary>
    public class Space
    {
        // Aktualny czas liczony w ms.
        public double globalTime;

        public StreamWriter file;


        // Liczba nadajników i odbiorników.
        private const int K = 4;

        private static List<Packet> channel;
        public List<Event> agenda;
        private List<Source> transmitters;

        public Space()
        {
            channel = new List<Packet>();
            agenda = new List<Event>();
            transmitters = new List<Source>();
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            string path = directory.FullName+"\\SimulationLogs.txt";
            file = new StreamWriter(path, false);
            file.WriteLine("Proces:\t\t\t\t Numer ID:\t\t"
                            + "Faza: \t Nr retransmisji:\t"
                            + "Czas aktualny:\t Czas następnego wystąpienia:");
           
            file.Flush();
            for (short i = 0; i < K; i++)
            { transmitters.Add(new Source(i, this)); }
            
        }

        /// <summary>
        /// Pętla główna symulacji.
        /// </summary>
        public void Simulation()
        {
            globalTime = -1.0;
            Event current;
            while (globalTime < 25)
            {
                current = agenda.Last();
                agenda.Remove(current);
                globalTime = current.eventTime;
                current.GetProc().Execute();
            }
        }

        public delegate void ChannelSetup(Packet x);

        public ChannelSetup RemoveFromChannel = new ChannelSetup(Remove);
        public ChannelSetup AddToChannel = new ChannelSetup(Add);

        private static void Add(Packet packet)
        {
            channel.Add(packet);
        }

        private static void Remove(Packet packet)
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
            return channel.FindAll(o => o.WhatTimeIsIt() == time).ToList();
        }

        //
        // Dodanie komunikatu zdarzenia w odpowiednie miejsce w agendzie.
        //
        public void AddToAgenda(Event ev)
        {
            int index = agenda.IndexOf(agenda.Find(el => el.eventTime <= ev.eventTime));
            if (index < 0) index = 0;
            agenda.Insert(index, ev);   
        }

       
    }
}
