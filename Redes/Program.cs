using System;
using System.Threading;
using System.Linq;

namespace TCPIPSimulation
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            Database db = new Database("localhost", "gamefication", "root", "koalla");
            Console.Clear();
            Console.Write("Digite o nome de usuário: ");
            string userName = Console.ReadLine() ?? string.Empty;

            Computer nodeA = new("Node A", "5A:BD:19:19:3F:24", "192.168.1.1");
            Computer nodeB = new("Node B", "AA:B5:14:A9:3F:22", "192.168.1.7");
            Computer nodeC = new("Node C", "48:DE:30:AF:3A:D7", "192.168.1.9");
            Computer nodeD = new("Node D", "00:A0:34:AB:33:F3", "192.168.1.28");
            // Computer nodeE = new("Node E", "18:C5:A4:7A:35:FF", "192.168.1.15");
            // Computer nodeF = new("Node F", "B8:D5:3B:AA:35:AA", "192.168.1.2");
            // Computer nodeG = new("Node G", "38:E5:F6:6A:35:BB", "192.168.1.70");
            // Computer nodeH = new("Node H", "4A:15:B4:AA:3A:CC", "192.168.1.45");
            // Computer nodeI = new("Node I", "58:05:34:AC:A5:DD", "192.168.1.33");
            // Computer nodeJ = new("Node J", "6D:25:34:5A:3D:EE", "192.168.1.4");

            PrintComputersTable();

            string message = "Hello, World!";
            var (sourceComputer, targetComputer) = GetSourceAndTargetComputersFromUser();
            if (sourceComputer != null && targetComputer != null)
            {
                sourceComputer.InitializeApplication(message);
                sourceComputer.Application.Run(targetComputer);

                // Perguntas e respostas
                Console.Clear();
                Console.WriteLine(@"
░██████╗░██╗░░░██╗██╗███████╗
██╔═══██╗██║░░░██║██║╚════██║
██║██╗██║██║░░░██║██║░░███╔═╝
╚██████╔╝██║░░░██║██║██╔══╝░░
░╚═██╔═╝░╚██████╔╝██║███████╗
░░░╚═╝░░░░╚═════╝░╚═╝╚══════╝");
                Console.WriteLine("\nResponda as perguntas com true or false\n");
                var questions = db.GetQuestions();
                int totalScore = 0;
                foreach (var (question, correctAnswer) in questions)
                {
                    Console.WriteLine(question);
                    bool userAnswer;
                    while (!bool.TryParse(Console.ReadLine(), out userAnswer))
                    {
                        Console.WriteLine("Resposta inválida. Digite 'true' ou 'false':");
                    }
                    int score = userAnswer == correctAnswer ? 50 : 0;
                    totalScore += score;
                    db.InsertScore(userName, score);
                }

                Console.WriteLine($"Pontuação total: {totalScore}");

                 // Exibir todos os registros da tabela UserScores
                var allScores = db.GetAllScores();
                Console.WriteLine("\nRegistros da tabela UserScores:");
                foreach (var (user, score, date) in allScores)
                {
                    Console.WriteLine($"Usuário: {user}, Pontuação: {score}, Data: {date}");
                }
            }
            else
            {
                Console.WriteLine("Invalid source or target computer selected.");
            }
        }
        static void PrintComputersTable()
        {
            var allComputers = Computer.GetAllComputers();
            // Console.Clear();
            Console.WriteLine(@"
███╗░░██╗███████╗████████╗░██╗░░░░░░░██╗░█████╗░██████╗░██╗░░██╗
████╗░██║██╔════╝╚══██╔══╝░██║░░██╗░░██║██╔══██╗██╔══██╗██║░██╔╝
██╔██╗██║█████╗░░░░░██║░░░░╚██╗████╗██╔╝██║░░██║██████╔╝█████═╝░
██║╚████║██╔══╝░░░░░██║░░░░░████╔═████║░██║░░██║██╔══██╗██╔═██╗░
██║░╚███║███████╗░░░██║░░░░░╚██╔╝░╚██╔╝░╚█████╔╝██║░░██║██║░╚██╗
╚═╝░░╚══╝╚══════╝░░░╚═╝░░░░░░╚═╝░░░╚═╝░░░╚════╝░╚═╝░░╚═╝╚═╝░░╚═╝

░██████╗██╗███╗░░░███╗██╗░░░██╗██╗░░░░░░█████╗░████████╗██╗░█████��░███╗░░██╗
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
            string sourceName = Console.ReadLine() ?? string.Empty;
            Computer sourceComputer = allComputers.FirstOrDefault(c => c.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Enter the name of the target computer:");
            string targetName = Console.ReadLine();
            Computer targetComputer = allComputers.FirstOrDefault(c => c.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

            return (sourceComputer, targetComputer);
        }
    }
}
