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