
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