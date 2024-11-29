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