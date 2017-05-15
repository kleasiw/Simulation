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

        private StreamWriter file;

        // Liczba nadajników i odbiorników.
        private const int K = 4;

        // Lista reprezentująca transmitowane w kanale pakiety.
        private static List<Packet> channel;

        // Lista nadajników.
        private List<Source> transmitters;

        // Generator równomierny z zakresu {0,1...10}
        private RandGenerator.UniformRandomGenerator uniformIntiger;

        // Generator równomierny z przedziału.
        private RandGenerator.UniformRandomGenerator uniformInterval;

        // Zbiór zaplanowanych zdarzeń.
        public List<Event> agenda;



        public Space()
        {
            channel = new List<Packet>();
            agenda = new List<Event>();
            transmitters = new List<Source>();

            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            StringBuilder path = new StringBuilder(directory.FullName.ToString());

            StringBuilder pathLog= new StringBuilder(path.ToString() +"\\SimulationLogs.txt");
            file = new StreamWriter(pathLog.ToString(), false);
            file.WriteLine("Lista logów:");
            file.Flush();
            
            StringBuilder pathKernel = new StringBuilder(path.ToString());
            pathKernel.Append(path.ToString() + "\\kernels.txt");
            StreamReader kernels = new StreamReader(pathKernel.ToString());
            
            Console.WriteLine("Wybierz lambdę dla generatorów wykladniczych");
            double lambda = double.Parse(Console.ReadLine());
            for (short i = 0; i < K; i++)
            { transmitters.Add(new Source(i, this, file,lambda,int.Parse(kernels.ReadLine()))); }
            uniformInterval = new RandGenerator.UniformRandomGenerator(int.Parse(kernels.ReadLine()));
            uniformIntiger = new RandGenerator.UniformRandomGenerator(int.Parse(kernels.ReadLine()));
        }

        /// <summary>
        /// Pętla główna symulacji.
        /// </summary>
        /// <param name="decision">Wybór trybu symulacji: true - krokowy,
        /// false - ciągły.</param>
        public void Simulation(bool decision)
        {
            globalTime = -1.0;
            Event current;
            while (globalTime < 40)
            {
                current = agenda.Last();// wybór najwcześniejszego zdarzenia
                agenda.Remove(current);// usunięcie go z listy zdarzeń(zostanie dodany w trakcie Execute)
                globalTime = current.eventTime;// ustalenie aktualnego czasu symulacji
                current.GetProc().Execute();// uruchomienie akrualnego procesu
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
            int[] array = new int[(K+1)*number];
            for (int i = 0; i < K + 1; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    for (int m = 0; m < 100000; m++)
                        gen.Rand(0, int.MaxValue);
                    array.SetValue(gen.get_kernel(),  i*number+j);
                }
            }
            return array;

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

       
    }
}
