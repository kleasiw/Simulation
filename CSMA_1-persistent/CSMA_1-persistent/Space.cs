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
        private double globalTime;

        private Logs logsDocument;
        private Kernels kernel;


        // Liczba nadajników i odbiorników.
        private int K;

        // Lista reprezentująca transmitowane w kanale pakiety.
        private static List<Packet> channel;

        // Lista nadajników.
        private List<Source> transmitters;

        // Zbiór zaplanowanych zdarzeń.
        public List<Event> agenda;


        public Statistics stats;


        public Space(double lambda, int k, Kernels kern)
        {
            channel = new List<Packet>();
            agenda = new List<Event>();
            transmitters = new List<Source>();
            logsDocument = new Logs();
            kernel = kern;
            K = k;

            for (short i = 0; i < K; i++)
            { transmitters.Add(new Source(i, this, logsDocument, lambda, kernel)); }

            stats = new Statistics(K);
        }

        /// <summary>
        /// Pętla główna symulacji.
        /// </summary>
        /// <param name="decision">Wybór trybu symulacji: true - krokowy,
        /// false - ciągły.</param>
        public void Simulation(bool decision, double start, double time)
        {
            globalTime = -1.0;
            Event current;
            while (globalTime < time)
            {
                current = agenda.Last();// wybór najwcześniejszego zdarzenia
                agenda.Remove(current);// usunięcie go z listy zdarzeń(zostanie dodany w trakcie Execute)
                globalTime = current.eventTime;// ustalenie aktualnego czasu symulacji
                current.GetProc().Execute(start);// uruchomienie akrualnego procesu
                if (decision) Console.ReadKey();
            }
        }


        /// <summary>
        /// Generuje ziarna.
        /// </summary>
        /// <param name="number">Liczba przewidywanych symulacji</param>
        /// <param name="kernel">Ziarno początkowe</param>
        /// <returns>Tablica ziaren.</returns>
        public int[] KernelGeneration(int number, int kernel)
        {
            Generators.UniformGenerator gen = new Generators.UniformGenerator(kernel);
            int[] array = new int[number*3*K];
            for (int i = 0; i < K ; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    for(int k = 0; k < 3; k++)
                    {
                        for (int m = 0; m < 100000; m++)
                            gen.Rand(0, int.MaxValue);
                        array.SetValue(gen.get_kernel(), i * number*3 + j*3 +k);
                    }
                    
                }
            }
            return array;

        }

       // public delegate double Rand()

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

        public void ShowStats(StreamWriter f) { stats.ShowStats(f, globalTime); }

        public void Close()
        {
            logsDocument.Close();
       //     kernel.Close();
        }
       
    }
}
