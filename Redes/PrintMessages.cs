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