using System;

namespace TCPIPSimulation
{
    // 1. Camada de Aplicação
    public class Aplicacao
    {
        private Transporte transporte;

        public Aplicacao()
        {
            transporte = new Transporte();
        }

        public void EnviarDados(string dados)
        {
            Console.WriteLine("Aplicação: Preparando dados...");
            transporte.Enviar(dados);
        }

        public void ReceberDados(string dados)
        {
            Console.WriteLine($"Aplicação: Dados recebidos -> {dados}");
        }
    }

    // 2. Camada de Transporte
    public class Transporte
    {
        private Rede rede;

        public Transporte()
        {
            rede = new Rede();
        }

        public void Enviar(string dados)
        {
            Console.WriteLine("Transporte: Dividindo dados em segmentos...");
            string segmento = $"[Segmento]: {dados}";
            rede.Enviar(segmento);
        }

        public void Receber(string segmento)
        {
            Console.WriteLine("Transporte: Segmento recebido, reconstruindo dados...");
            string dados = segmento.Replace("[Segmento]: ", "");
            Aplicacao app = new Aplicacao();
            app.ReceberDados(dados);
        }
    }

    // 3. Camada de Rede
    public class Rede
    {
        public string EnderecoLogicoOrigem { get; private set; }
        public string EnderecoLogicoDestino { get; private set; }
        public string ProtocoloTransporte { get; private set; }
        public string Pacote { get; private set; }
        private Interface enlace;

        public Rede(string enderecoLogicoOrigem, string enderecoLogicoDestino, string protocoloTransporte, string segmento)
        {
            EnderecoLogicoOrigem = enderecoLogicoOrigem;
            EnderecoLogicoDestino = enderecoLogicoDestino;
            ProtocoloTransporte = protocoloTransporte;
            Pacote = $"PACOTE:\nIP Origem: {EnderecoLogicoOrigem}\nIP Destino: {EnderecoLogicoDestino}\nDados: {segmento}";
            enlace = new Interface("00:11:22:33:44:55", "66:77:88:99:AA:BB", Pacote);
        }

        public void Enviar(string segmento)
        {
            Console.WriteLine("Rede: Encapsulando em pacotes...");
            enlace.Enviar(Pacote);
        }

        public void Receber(string pacote)
        {
            Console.WriteLine("Rede: Pacote recebido, removendo encapsulamento...");
            string segmento = pacote.Replace("[Pacote]: ", "");
            Transporte transporte = new Transporte();
            transporte.Receber(segmento);
        }
    }

    // 4. Camada de Interface
    public class Interface
    {
        public string macOrigem { get; private set; }
        public string macDestino { get; private set; }
        public Rede dados { get; private set; }

        public Interface(string enderecoFisicoOrigem, string enderecoFisicoDestino, Rede pacote)
        {
            macOrigem = enderecoFisicoOrigem;
            macDestino = enderecoFisicoDestino;
            dados = pacote;
            string quadro = $"QUADRO:\nMAC Origem: {macOrigem}\nMAC Destino: {macOrigem}\nDados: {dados}";
        }

        public void Enviar(string pacote)
        {
            Console.WriteLine("Camada de Interface: Encapsulando em quadros e enviando pelo meio físico...");
            Console.WriteLine(Quadro);
            Thread.Sleep(2000);
            MeioFisicoEnviar(Quadro);
        }

        public void Receber(string quadro)
        {
            Console.WriteLine("Enlace: Quadro recebido, removendo encapsulamento...");
            string pacote = quadro.Replace("QUADRO:\nMAC Origem: {macOrigem}\nMAC Destino: {macOrigem}\nDados:", "");
            Thread.Sleep(2000);
            Rede rede = new Rede("192.168.0.1", "192.168.0.2", "B", Dados);
            rede.Receber(pacote);
        }

        private void MeioFisicoEnviar(string quadro)
        {
            Console.WriteLine("Meio Físico: Transmitindo dados...\n");
            // Simulação de dados sendo recebidos de volta para a camada de enlace.
            Thread.Sleep(2000);
            Receber(quadro);
        }
    }

    // Programa principal para testar a simulação
    class Program
    {
        static void Main(string[] args)
        {
            Aplicacao app = new Aplicacao();
            app.EnviarDados("Mensagem de Teste");
        }
    }
}

