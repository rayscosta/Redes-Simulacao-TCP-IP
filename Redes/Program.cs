using System;
using System.Threading;

namespace TCPIPSimulation
{
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
        public string Data { get; }

        public ApplicationPDU(string data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
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
            Console.WriteLine($"Application: Sending data -> {_data.Data}");
            const int sourcePort = 80;
            const int destinationPort = 80;
            _transport.Pack(targetComputer, sourcePort, destinationPort, _data, _computer);
        }

        public void Unpack(ApplicationPDU data)
        {
            Console.WriteLine($"Application: Data received -> {data.Data}");
        }
    }

    public class TransportPDU
    {
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public ApplicationPDU Data { get; set; }
    }

    public class Transport
    {
        public void Pack(Computer targetComputer, int sourcePort, int destinationPort, ApplicationPDU applicationData, Computer sourceComputer)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Transport: Dividing data into segments...");
            TransportPDU segment = new TransportPDU();
            segment.SourcePort = sourcePort;
            segment.DestinationPort = destinationPort;
            segment.Data = applicationData;
            string destinationIp = targetComputer.IpAddress;

            Network network = new Network();
            network.Pack(destinationIp, segment, sourceComputer, targetComputer);
        }

        public void Unpack(TransportPDU segment, Computer computer)
        {
            Console.WriteLine("Transport: Segment received, reconstructing data...");
            Application app = new Application(segment.Data.Data, computer);
            app.Unpack(segment.Data);
        }
    }

    public class NetworkPDU
    {
        public string SourceIp { get; private set; }
        public string DestinationIp { get; private set; }
        public TransportPDU Data { get; private set; }

        public NetworkPDU(string sourceIp, string destinationIp, TransportPDU data)
        {
            SourceIp = sourceIp ?? throw new ArgumentNullException(nameof(sourceIp));
            DestinationIp = destinationIp ?? throw new ArgumentNullException(nameof(destinationIp));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }

    public class Network
    {
        public void Pack(string destinationIp, TransportPDU segment, Computer sourceComputer, Computer targetComputer)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Network: Encapsulating into packets...");
            NetworkPDU packet = new NetworkPDU(sourceComputer.IpAddress, destinationIp, segment);

            // Determinar o próximo salto (next hop)
            Computer nextHop = DetermineNextHop(destinationIp);

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

        public void Unpack(NetworkPDU packet, Computer computer)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Network: Packet received, removing encapsulation...");

            if (packet.DestinationIp == computer.IpAddress)
            {
                Transport transport = new Transport();
                transport.Unpack(packet.Data, computer);
            }
            else
            {
                Console.WriteLine("Network: Forwarding packet to the next hop...");

                // Determinar o próximo nó na rede (roteamento)
                Computer nextHop = DetermineNextHop(packet.DestinationIp);

                if (nextHop != null)
                {
                    Interface interfaceLayer = new Interface();
                    interfaceLayer.Pack(nextHop.MacAddress, packet, computer);
                }
                else
                {
                    Console.WriteLine("Network: No route to destination.");
                }
            }
        }

        private Computer DetermineNextHop(string destinationIp)
        {
            var allComputers = Computer.GetAllComputers();

            // Para simplificação, vamos selecionar o próximo salto aleatoriamente
            Random random = new Random();
            int index = random.Next(allComputers.Count);
            return allComputers[index];
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
    }

    public class Interface
    {
        public void Pack(string targetMacAdress, NetworkPDU packet, Computer sourceComputer)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Interface: Encapsulating into frames and sending over the physical medium...");
            InterfacePDU frame = new InterfacePDU(sourceComputer.MacAddress, targetMacAdress, packet);
            SendPhysical(frame);
        }

        public void Unpack(InterfacePDU interfacePdu, Computer computer)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Interface: Frame received, removing encapsulation...");
            Network network = new Network();
            network.Unpack(interfacePdu.Data, computer);
        }

        private void SendPhysical(InterfacePDU interfacePDU)
        {
            Console.WriteLine("Physical Medium: Transmitting data...\n");
            Thread.Sleep(2000);
            var allComputers = Computer.GetAllComputers();
            foreach (var computer in allComputers)
            {
                if (interfacePDU.DestinationMac == computer.MacAddress)
                {
                    computer.physical.Unpack(interfacePDU, computer);
                    break;
                }
            }
        }
    }

    class MainProgram
    {
        static void Main(string[] args)
        {
            Computer nodeA = new Computer("Node A", "5A:BD:19:19:3F:24", "192.168.1.1");
            Computer nodeB = new Computer("Node B", "AA:B5:14:A9:3F:22", "192.168.1.7");
            Computer nodeC = new Computer("Node C", "B8:D5:34:AA:35:D3", "192.168.1.20");
            nodeA.InitializeApplication("Hello, Node C!");
            nodeA.Application.Run(nodeC);
        }
    }
}
