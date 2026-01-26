using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Garen.Sdk.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json; // Certifique-se de que o tester tenha esse pacote para o PrintJson

namespace Garen.Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Garen SDK Tester v1.0";
            Console.WriteLine("=== Garen SDK Tester - Ambiente de Validação ===");

            // 1. Configuração (Appsettings)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            var baseUrl = config["GarenApi:BaseUrl"];
            var token = config["GarenApi:Token"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                Console.WriteLine("ERRO: Configure o BaseUrl no appsettings.json");
                return;
            }

            // 2. Inicializa a Factory
            Console.WriteLine($"Conectando em: {baseUrl}...");
            GarenApiFactory.Initialize(baseUrl, token);
            Console.WriteLine("SDK Inicializada.\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nSELECIONE UMA CATEGORIA DE TESTE:");
                Console.ResetColor();
                
                Console.WriteLine("1 - [HARDWARE] Acionar Portão / Relé");
                Console.WriteLine("2 - [SISTEMA]  Ver Status (Versão, Data, IP)");
                Console.WriteLine("3 - [USUÁRIOS] Listar Todos os Usuários");
                Console.WriteLine("4 - [USUÁRIOS] Criar Usuário de Teste");
                Console.WriteLine("5 - [USUÁRIOS] Deletar Usuário");
                Console.WriteLine("6 - [EVENTOS]  Ler Logs de Acesso");
                Console.WriteLine("7 - [RESILIÊNCIA] Simular Queda de Rede");
                Console.WriteLine("0 - Sair");
                Console.Write("> ");

                var opcao = Console.ReadLine();
                Console.Clear();

                try
                {
                    switch (opcao)
                    {
                        case "1": await TesteHardware(); break;
                        case "2": await TesteSistema(); break;
                        case "3": await TesteListarUsuarios(); break;
                        case "4": await TesteCriarUsuario(); break;
                        case "5": await TesteDeletarUsuario(); break;
                        case "6": await TesteLerEventos(); break;
                        case "7": await TesteResiliencia(); break;
                        case "0": return;
                        default: Console.WriteLine("Opção inválida."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[EXCEPTION] Ocorreu um erro não tratado: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    Console.ResetColor();
                }

                Console.WriteLine("\nPressione ENTER para voltar ao menu...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // --- 1. TESTES DE HARDWARE ---
        private static async Task TesteHardware()
        {
            Console.WriteLine("--- Acionamento Remoto (Cmd 20) ---");
            Console.Write("Digite o número da porta (Ex: 1): ");
            if (!int.TryParse(Console.ReadLine(), out int porta)) porta = 1;

            var comando = new RemoteDoorTrigger { Porta = porta, Tempo = 0, Auxiliar = 0 };
            
            Console.WriteLine("Enviando comando...");
            var resp = await GarenApiFactory.Client.OpenDoorAsync(comando);
            PrintResultado(resp);
        }

        // --- 2. TESTES DE SISTEMA ---
        private static async Task TesteSistema()
        {
            Console.WriteLine("--- Checagem de Sistema ---");
            
            Console.Write("Lendo Versão... ");
            var versao = await GarenApiFactory.Client.GetVersionAsync();
            Console.WriteLine(versao);

            Console.Write("Lendo Configuração de IP... ");
            var ipConfig = await GarenApiFactory.Client.GetIpConfigAsync();
            PrintJson(ipConfig);

            Console.Write("Lendo Data/Hora da Placa... ");
            var data = await GarenApiFactory.Client.GetDateAsync();
            PrintJson(data);
        }

        // --- 3. LISTAR USUÁRIOS ---
        private static async Task TesteListarUsuarios()
        {
            Console.WriteLine("--- Listagem de Usuários ---");
            // Nota: O retorno pode ser grande, cuidado no console
            var usuarios = await GarenApiFactory.Client.GetAllUsersAsync();
            PrintJson(usuarios);
        }

        // --- 4. CRIAR USUÁRIO ---
        private static async Task TesteCriarUsuario()
        {
            Console.WriteLine("--- Criar Usuário de Teste ---");
            Console.Write("Nome do Usuário: ");
            var nome = Console.ReadLine();
            if (string.IsNullOrEmpty(nome)) nome = "Teste SDK";

            // Cria um payload completo baseado no seu DTO
            var novoUsuario = new UserRegisterModel
            {
                Nome = nome,
                TipoCadastro = 1, // Exemplo
                Validade = 1,     // Habilitado
                Creditos = 0,
                DataInicio = 1700000000, // Exemplo Epoch
                DataFim = 1900000000,    // Exemplo Epoch futuro
                Portas = new List<string> { "1" },
                GrupoDeAcesso = new List<string> { "1" }, // Assumindo grupo 1 existe
                // Adicione outros campos obrigatórios conforme sua regra de negócio
            };

            Console.WriteLine($"Enviando cadastro de '{nome}'...");
            var resp = await GarenApiFactory.Client.CreateUserAsync(novoUsuario);
            PrintResultado(resp);
        }

        // --- 5. DELETAR USUÁRIO ---
        private static async Task TesteDeletarUsuario()
        {
            Console.WriteLine("--- Deletar Usuário ---");
            Console.Write("ID do Usuário para deletar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var payload = new UserIdModel { Id = id };
                var resp = await GarenApiFactory.Client.DeleteUserAsync(payload);
                PrintResultado(resp);
            }
            else
            {
                Console.WriteLine("ID Inválido.");
            }
        }

        // --- 6. LER EVENTOS ---
        private static async Task TesteLerEventos()
        {
            Console.WriteLine("--- Últimos Eventos ---");
            // Busca sem filtros (traz tudo ou os últimos N, dependendo da placa)
            var eventos = await GarenApiFactory.Client.GetEventsAsync();
            PrintJson(eventos);
        }

        // --- 7. RESILIÊNCIA ---
        private static async Task TesteResiliencia()
        {
            Console.WriteLine("--- Teste de Circuit Breaker ---");
            Console.WriteLine("1. Desconecte o cabo da placa ou desligue-a agora.");
            Console.WriteLine("2. Pressione ENTER para tentar conectar.");
            Console.ReadLine();

            for (int i = 1; i <= 5; i++)
            {
                Console.Write($"Tentativa {i}: ");
                try
                {
                    await GarenApiFactory.Client.GetVersionAsync();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Sucesso (Placa Online)");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    // Se o Circuit Breaker abrir, a mensagem será "The circuit is now open..."
                    // e a resposta será instantânea, sem esperar timeout.
                    Console.WriteLine($"Falha: {ex.Message}");
                }
                Console.ResetColor();
                await Task.Delay(1000); // Espera 1s entre tentativas
            }
        }

        // --- HELPER PARA FORMATAR JSON NO CONSOLE ---
        private static void PrintJson(object obj)
        {
            if (obj == null)
            {
                Console.WriteLine("(null)");
                return;
            }
            try 
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                Console.WriteLine(json);
            }
            catch 
            { 
                Console.WriteLine(obj.ToString()); 
            }
        }

        private static void PrintResultado(GenericResponse resp)
        {
            if (resp == null) 
            {
                Console.WriteLine("Resposta nula.");
                return;
            }

            if (resp.Status?.ToLower() == "success" || resp.Codigo == 200)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SUCESSO! Status: {resp.Status} | Código: {resp.Codigo}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"ERRO NA PLACA: {resp.Status} | Código: {resp.Codigo}");
            }
            Console.ResetColor();
        }
    }
}