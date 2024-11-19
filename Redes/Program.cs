using System;
using System.Threading;

namespace TCPIPSimulation
{
    class PrintMessages
    {
        private static int verticalOffset = 0;

        public static void DisplayAnimation(string camada, string pdu, Computer actualNode)
        {

            Console.Clear();
            Console.WriteLine(@"
███╗░░██╗███████╗████████╗░██╗░░░░░░░██╗░█████╗░██████╗░██╗░░██╗
████╗░██║██╔════╝╚══██╔══╝░██║░░██╗░░██║██╔══██╗██╔══██╗██║░██╔╝
██╔██╗██║█████╗░░░░░██║░░░░╚██╗████╗██╔╝██║░░██║██████╔╝█████═╝░
██║╚████║██╔══╝░░░░░██║░░░░░████╔═████║░██║░░██║██╔══██╗██╔═██╗░
██║░╚███║███████╗░░░██║░░░░░╚██╔╝░╚██╔╝░╚█████╔╝██║░░██║██║░╚██╗
╚═╝░░╚══╝╚══════╝░░░╚═╝░░░░░░╚═╝░░░╚═╝░░░╚════╝░╚═╝░░╚═╝╚═╝░░╚═╝

░██████╗██╗███╗░░░███╗██╗░░░██╗██╗░░░░░░█████╗░████████╗██╗░█████╗░███╗░░██╗
██╔════╝██║████╗░████║██║░░░██║██║░░░░░██╔══██╗╚══██╔══╝██║██╔══██╗████╗░██║
╚█████╗░██║██╔████╔██║██║░░░██║██║░░░░░███████║░░░██║░░░██║██║░░██║██╔██╗██║
░╚═══██╗██║██║╚██╔╝██║██║░░░██║██║░░░░░██╔══██║░░░██║░░░██║██║░░██║██║╚████║
██████╔╝██║██║░╚═╝░██║╚██████╔╝███████╗██║░░██║░░░██║░░░██║╚█████╔╝██║░╚███║
╚═════╝░╚═╝╚═╝░░░░░╚═╝░╚═════╝░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚═╝░╚════╝░╚═╝░░╚══╝");
            Console.WriteLine();

        // Adiciona linhas em branco para o deslocamento vertical
        for (int k = 0; k < verticalOffset; k++)
        {
            Console.WriteLine();
        }

        Console.WriteLine($"{camada} Layer:");
        Console.WriteLine($"Node: {actualNode.Name}");
        Console.WriteLine($"MAC Address: {actualNode.MacAddress}");

        // Divide o PDU em linhas
        List<string> linhasPdu = pdu.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //Console.WriteLine(linhasPdu.Count);
        // Encontra a linha mais longa
        int maxLength = linhasPdu.Max(line => line.Length);

        // Calcule o tamanho do retângulo
        int width = maxLength + 6; // Adiciona espaço para a tabulação em ambos os lados

        // Adiciona uma linha com "----" antes da linha que começa com "Data:"
        for (int i = 0; i < linhasPdu.Count; i++)
        {
            if (linhasPdu[i].StartsWith("Data:"))
            {
                linhasPdu.Insert(i, new string('-', width - 2));
                break;
            }
        }

        int height = linhasPdu.Count + 2; // Adiciona 2 linhas vazias (uma acima e uma abaixo)

        // Imprime a borda superior do retângulo
        Console.WriteLine(new string('*', width));

        for (int i = 0; i < height; i++)
        {
            if (i == 0 || i == height - 1)
            {
                // Imprime uma linha vazia com bordas
                Console.Write("*");
                Console.Write(new string(' ', width - 2));
                Console.Write("*");
                Console.WriteLine();
            }
            else if (linhasPdu[i - 1].StartsWith("-"))
            {
                // Imprime a linha com "----" de borda a borda
                Console.Write("*");
                Console.Write(linhasPdu[i - 1]);
                Console.Write("*");
                Console.WriteLine();
            }
            else
            {
                // Imprime a linha do PDU com bordas e tabulação
                Console.Write("*  ");
                Console.Write(linhasPdu[i - 1].PadRight(maxLength));
                Console.Write("  *");
                Console.WriteLine();
            }
        }

        // Imprime a borda inferior do retângulo
        Console.WriteLine(new string('*', width));
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        Console.ReadKey(true);
    }

        public static void IncrementVerticalOffset()
        {
            verticalOffset += 2;
        }

        public static void DecrementVerticalOffset()
        {
            verticalOffset -= 2;
        }
    }  
    public class Computer
    {
        public string Name { get; }
        public string MacAddress { get; }
        public string IpAddress { get; }
        public Application Application { get; private set; }
        public Interface physical { get; private set; }

        private static readonly List<Computer> AllComputers = new List<Computer>();

        public Computer(string name, string macAddress, string ipAddress)
        {
            Name = name;
            MacAddress = macAddress;
            IpAddress = ipAddress;
            Application = null!;
            physical = new Interface();
            AllComputers.Add(this);
        }

        public static List<Computer> GetAllComputers()
        {
            return AllComputers;
        }

        public void InitializeApplication(string message)
        {
            Application = new Application(message, this);
        }
    }

    public class ApplicationPDU
    {
        public string Data { get; set; }

        public ApplicationPDU(string data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public override string ToString()
        {
            return Data;
        }
    }

    public class Application
    {
        private readonly ApplicationPDU _data;
        private readonly Transport _transport;
        private readonly Computer _computer;

        public Application(string message, Computer computer)
        {
            _data = new ApplicationPDU(message);
            _transport = new Transport();
            _computer = computer;
        }

        public void Run(Computer targetComputer)
        {
            const int sourcePort = 80;
            const int destinationPort = 8100;
            PrintMessages.DisplayAnimation( "Application", _data.ToString(), _computer);
            PrintMessages.IncrementVerticalOffset();
            Transport.Pack(targetComputer, sourcePort, destinationPort, _data, _computer);
        }

        public static void Unpack(ApplicationPDU data, Computer computer)
        {
            PrintMessages.DecrementVerticalOffset();
            PrintMessages.DisplayAnimation("Application", data.ToString(), computer);
            Console.WriteLine("Application: Data received successfully.");
        }
    }

    public class TransportPDU
    {
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public ApplicationPDU Data { get; set; }

        public override string ToString()
        {
            return $"Source Port: {SourcePort}\nDestination Port: {DestinationPort}\nData: {Data}";
        }
    }

    public class Transport
    {
        public static void Pack(Computer targetComputer, int sourcePort, int destinationPort, ApplicationPDU applicationData, Computer sourceComputer)
        {
            Thread.Sleep(4000);
            TransportPDU segment = new TransportPDU();
            segment.SourcePort = sourcePort;
            segment.DestinationPort = destinationPort;
            segment.Data = applicationData;
            string destinationIp = targetComputer.IpAddress;
            PrintMessages.DisplayAnimation( "Transport", segment.ToString(), sourceComputer);
            PrintMessages.IncrementVerticalOffset();

            Network network = new Network();
            Network.Pack(destinationIp, segment, sourceComputer, targetComputer);
        }

        public static void Unpack(TransportPDU segment, Computer computer)
        {
            Thread.Sleep(3000);
            PrintMessages.DisplayAnimation( "Transport", segment.ToString(), computer);
            PrintMessages.DecrementVerticalOffset();
            Application app = new Application(segment.Data.Data, computer);
            Application.Unpack(segment.Data, computer);
        }
    }

    public class NetworkPDU
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public TransportPDU Data { get; set; }

        public override string ToString()
        {
            return $"Source IP: {SourceIp}\nDestination IP: {DestinationIp}\nData: {Data}";
        }

        public NetworkPDU(string sourceIp, string destinationIp, TransportPDU data)
        {
            SourceIp = sourceIp ?? throw new ArgumentNullException(nameof(sourceIp));
            DestinationIp = destinationIp ?? throw new ArgumentNullException(nameof(destinationIp));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }

    public class Network
    {
        private static List<Computer> visitedComputers = new List<Computer>();
        public static void Pack(string destinationIp, TransportPDU segment, Computer sourceComputer, Computer targetComputer)
        {
            Thread.Sleep(4000);
            NetworkPDU packet = new NetworkPDU(sourceComputer.IpAddress, destinationIp, segment);
            PrintMessages.DisplayAnimation( "Network", packet.ToString(), sourceComputer);
            PrintMessages.IncrementVerticalOffset();
            visitedComputers.Add(sourceComputer);
            // Determinar o próximo salto (next hop)
            Computer nextHop = DetermineNextHop();

            if (nextHop != null)
            {
                string targetMacAddress = nextHop.MacAddress;
                Interface interfaceLayer = new Interface();
                interfaceLayer.Pack(targetMacAddress, packet, sourceComputer);
            }
            else
            {
                Console.WriteLine("Network: No route to destination.");
            }
        }

        public static void Unpack(NetworkPDU packet, Computer computer)
        {
            Thread.Sleep(3000);
            PrintMessages.DisplayAnimation( "Network", packet.ToString(), computer);
            visitedComputers.Add(computer);
            if (packet.DestinationIp == computer.IpAddress)
            {
                PrintMessages.DecrementVerticalOffset();
                Transport.Unpack(packet.Data, computer);
            }
            else
            {
                Console.WriteLine($"\nNetwork: Forwarding packet to the next hop...");

                // Determinar o próximo nó na rede (roteamento)
                Computer nextHop = DetermineNextHop();

                if (nextHop != null)
                {
                    Interface interfaceLayer = new Interface();
                    PrintMessages.IncrementVerticalOffset();
                    interfaceLayer.Pack(nextHop.MacAddress, packet, computer);
                }
                else
                {
                    Console.WriteLine("Network: No route to destination.");
                }
            }
        }

        private static Computer DetermineNextHop()
        {
            var allComputers = Computer.GetAllComputers();
            var availableComputers = allComputers.Except(visitedComputers).ToList();

            if (availableComputers.Count == 0)
            {
                throw new InvalidOperationException("Não há mais computadores disponíveis para o próximo salto.");
            }

            // Selecionar o próximo salto aleatoriamente
            Random random = new();
            int index = random.Next(availableComputers.Count);
            return availableComputers[index];
        }
    }

    public class InterfacePDU
    {
        public string SourceMac { get; private set; }
        public string DestinationMac { get; private set; }
        public NetworkPDU Data { get; private set; }

        public InterfacePDU(string sourceMac, string destinationMac, NetworkPDU data)
        {
            SourceMac = sourceMac ?? throw new ArgumentNullException(nameof(sourceMac));
            DestinationMac = destinationMac ?? throw new ArgumentNullException(nameof(destinationMac));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public override string ToString()
        {
            return $"Source MAC: {SourceMac}\nDestination MAC: {DestinationMac}\nData: {Data}";
        }
    }

    public class Interface
    {
        public void Pack(string targetMacAdress, NetworkPDU packet, Computer sourceComputer)
        {
            Thread.Sleep(4000);
            InterfacePDU frame = new InterfacePDU(sourceComputer.MacAddress, targetMacAdress, packet);
            PrintMessages.DisplayAnimation( "Interface", frame.ToString(), sourceComputer);
            SendPhysical(frame);
        }

        public static void Unpack(InterfacePDU interfacePdu, Computer computer)
        {
            Thread.Sleep(3000);
            PrintMessages.DisplayAnimation( "Interface", interfacePdu.ToString(), computer);
            PrintMessages.DecrementVerticalOffset();
            Network.Unpack(interfacePdu.Data, computer);
        }

        private static void SendPhysical(InterfacePDU frame)
        {
            //Console.Clear();
            Console.WriteLine("Physical Medium: Transmitting data...\n");
            Console.WriteLine("🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫 🡫\n");
            Thread.Sleep(2000);
            var allComputers = Computer.GetAllComputers();
            foreach (var computer in allComputers)
            {
                if (frame.DestinationMac == computer.MacAddress)
                {
                    Unpack(frame, computer);
                    break;
                }
            }
        }
    }

    class MainProgram
    {
        static void Main(string[] args)
        {
            Computer nodeA = new("Node A", "5A:BD:19:19:3F:24", "192.168.1.1");
            Computer nodeB = new("Node B", "AA:B5:14:A9:3F:22", "192.168.1.7");
            Computer nodeC = new("Node C", "48:DE:30:AF:3A:D7", "192.168.1.9");
            Computer nodeD = new("Node D", "00:A0:34:AB:33:F3", "192.168.1.28");
            Computer nodeE = new("Node E", "18:C5:A4:7A:35:FF", "192.168.1.15");
            Computer nodeF = new("Node F", "B8:D5:3B:AA:35:AA", "192.168.1.2");
            Computer nodeG = new("Node G", "38:E5:F6:6A:35:BB", "192.168.1.70");
            Computer nodeH = new("Node H", "4A:15:B4:AA:3A:CC", "192.168.1.45");
            Computer nodeI = new("Node I", "58:05:34:AC:A5:DD", "192.168.1.33");
            Computer nodeJ = new("Node J", "6D:25:34:5A:3D:EE", "192.168.1.4");

            PrintComputersTable();

            string httpRequestHeader = "Hello, World!";
            var (sourceComputer, targetComputer) = GetSourceAndTargetComputersFromUser();
            if (sourceComputer != null && targetComputer != null)
            {
                sourceComputer.InitializeApplication(httpRequestHeader);
                sourceComputer.Application.Run(targetComputer);
            }
            else
            {
                Console.WriteLine("Invalid source or target computer selected.");
            }
        }
        static void PrintComputersTable()
        {
            var allComputers = Computer.GetAllComputers();
            Console.Clear();
            Console.WriteLine(@"
███╗░░██╗███████╗████████╗░██╗░░░░░░░██╗░█████╗░██████╗░██╗░░██╗
████╗░██║██╔════╝╚══██╔══╝░██║░░██╗░░██║██╔══██╗██╔══██╗██║░██╔╝
██╔██╗██║█████╗░░░░░██║░░░░╚██╗████╗██╔╝██║░░██║██████╔╝█████═╝░
██║╚████║██╔══╝░░░░░██║░░░░░████╔═████║░██║░░██║██╔══██╗██╔═██╗░
██║░╚███║███████╗░░░██║░░░░░╚██╔╝░╚██╔╝░╚█████╔╝██║░░██║██║░╚██╗
╚═╝░░╚══╝╚══════╝░░░╚═╝░░░░░░╚═╝░░░╚═╝░░░╚════╝░╚═╝░░╚═╝╚═╝░░╚═╝

░██████╗██╗███╗░░░███╗██╗░░░██╗██╗░░░░░░█████╗░████████╗██╗░█████╗░███╗░░██╗
██╔════╝██║████╗░████║██║░░░██║██║░░░░░██╔══██╗╚══██╔══╝██║██╔══██╗████╗░██║
╚█████╗░██║██╔████╔██║██║░░░██║██║░░░░░███████║░░░██║░░░██║██║░░██║██╔██╗██║
░╚═══██╗██║██║╚██╔╝██║██║░░░██║██║░░░░░██╔══██║░░░██║░░░██║██║░░██║██║╚████║
██████╔╝██║██║░╚═╝░██║╚██████╔╝███████╗██║░░██║░░░██║░░░██║╚█████╔╝██║░╚███║
╚═════╝░╚═╝╚═╝░░░░░╚═╝░╚═════╝░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚═╝░╚════╝░╚═╝░░╚══╝");

            Console.WriteLine($"\n            𝑳𝒊𝒔𝒕 𝒐𝒇 𝑪𝒐𝒎𝒑𝒖𝒕𝒆𝒓𝒔:\n");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("| Name     | MAC Address       | IP Address      |");
            Console.WriteLine("--------------------------------------------------");

            foreach (var computer in allComputers)
            {
                Console.WriteLine($"| {computer.Name,-8} | {computer.MacAddress,-17} | {computer.IpAddress,-15} |");
            }

            Console.WriteLine("--------------------------------------------------");
        }

        static (Computer sourceComputer, Computer targetComputer) GetSourceAndTargetComputersFromUser()
    {
        var allComputers = Computer.GetAllComputers();

        Console.WriteLine("Enter the name of the source computer:");
        string sourceName = Console.ReadLine();
        Computer sourceComputer = allComputers.FirstOrDefault(c => c.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine("Enter the name of the target computer:");
        string targetName = Console.ReadLine();
        Computer targetComputer = allComputers.FirstOrDefault(c => c.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

        return (sourceComputer, targetComputer);
    }
    }
}
