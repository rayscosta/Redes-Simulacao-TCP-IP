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