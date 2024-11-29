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