using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Necessário para .First() e .Any()
using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Garen.Sdk.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Garen.Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Garen SDK Tester v2.0 - Full Features";
            Console.WriteLine("=== Garen SDK Tester v2.0 ===");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            var baseUrl = config["GarenApi:BaseUrl"];
            var token = config["GarenApi:Token"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                Console.WriteLine("ERRO: Configure o BaseUrl no appsettings.json");
                return;
            }

            Console.WriteLine($"Target: {baseUrl}");
            GarenApiFactory.Initialize(baseUrl, token);
            Console.WriteLine("Factory Inicializada.\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- MENU PRINCIPAL ---");
                Console.ResetColor();
                
                Console.WriteLine("1 - [HARDWARE] Acionar Portão / Relé");
                Console.WriteLine("2 - [SISTEMA]  Status (Versão, Data, IP)");
                Console.WriteLine("3 - [USUÁRIOS] Listar Todos");
                Console.WriteLine("4 - [USUÁRIOS] Criar Usuário (Teste)");
                Console.WriteLine("5 - [USUÁRIOS] Deletar Usuário");
                Console.WriteLine("6 - [ACESSO]   Listar Cartões/Senhas de um Usuário");
                Console.WriteLine("7 - [ACESSO]   Cadastrar Cartão/Senha");
                Console.WriteLine("8 - [GRUPOS]   Listar Grupos");
                Console.WriteLine("9 - [EVENTOS]  Ler Logs");
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
                        case "6": await TesteListarAcessos(); break; // Novo
                        case "7": await TesteCadastrarAcesso(); break; // Novo
                        case "8": await TesteListarGrupos(); break; // Novo
                        case "9": await TesteLerEventos(); break;
                        case "0": return;
                        default: Console.WriteLine("Opção inválida."); break;
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n[TIMEOUT] A placa não respondeu em 5 segundos.");
                    Console.WriteLine("Verifique se ela está ligada e conectada na rede.");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nPressione ENTER...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // --- 1. HARDWARE ---
        private static async Task TesteHardware()
        {
            Console.WriteLine("--- Acionamento Remoto (Cmd 20) ---");
            Console.Write("Porta (1 ou 2): ");
            if (!int.TryParse(Console.ReadLine(), out int porta)) porta = 1;

            var comando = new RemoteDoorTrigger { Porta = porta, Tempo = 0, Auxiliar = 0 };
            PrintResultado(await GarenApiFactory.Client.OpenDoorAsync(comando));
        }

        // --- 2. SISTEMA ---
        private static async Task TesteSistema()
        {
            Console.WriteLine("--- Status do Sistema ---");
            Console.WriteLine($"Versão: {await GarenApiFactory.Client.GetVersionAsync()}");
            
            Console.WriteLine("\nConfiguração de IP:");
            PrintJson(await GarenApiFactory.Client.GetIpConfigAsync());

            Console.WriteLine("\nData/Hora Interna:");
            PrintJson(await GarenApiFactory.Client.GetDateAsync());
        }

        // --- 3. LISTAR USUÁRIOS ---
        private static async Task TesteListarUsuarios()
        {
            Console.WriteLine("--- Usuários Cadastrados ---");
            var resp = await GarenApiFactory.Client.GetAllUsersAsync();
            
            if (resp.Detalhes != null)
            {
                Console.WriteLine($"Total encontrados: {resp.Detalhes.Count}");
                // Mostra só os 5 primeiros para não poluir
                PrintJson(resp.Detalhes.Take(5)); 
            }
            else
            {
                Console.WriteLine("Nenhum usuário encontrado ou erro na estrutura.");
            }
        }

        // --- 4. CRIAR USUÁRIO ---
        private static async Task TesteCriarUsuario()
        {
            Console.WriteLine("--- Criar Usuário ---");
            Console.Write("Nome: ");
            var nome = Console.ReadLine();
            if (string.IsNullOrEmpty(nome)) nome = "User SDK Test";

            var novoUsuario = new UserRegisterModel
            {
                Nome = nome,
                TipoCadastro = 1, 
                Validade = 0, // 0 = sem validade (eterno)
                Creditos = 0,
                DataInicio = 0,
                DataFim = 0,
                Portas = new List<string> { "1", "2" }, // Acesso a ambas as portas
                GrupoDeAcesso = new List<string> { "1" } // Grupo padrão
            };

            Console.WriteLine("Enviando...");
            PrintResultado(await GarenApiFactory.Client.CreateUserAsync(novoUsuario));
        }

        // --- 5. DELETAR USUÁRIO ---
        private static async Task TesteDeletarUsuario()
        {
            Console.Write("ID do Usuário para deletar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var payload = new UserIdModel { Id = id };
                PrintResultado(await GarenApiFactory.Client.DeleteUserAsync(payload));
            }
        }

        // --- 6. LISTAR ACESSOS (NOVO) ---
        private static async Task TesteListarAcessos()
        {
            Console.WriteLine("--- Listar Credenciais ---");
            Console.Write("ID do Usuário: ");
            var id = Console.ReadLine();

            var resp = await GarenApiFactory.Client.GetAccessAsync(idUsuario: id);
            PrintJson(resp);
        }

        // --- 7. CADASTRAR ACESSO (NOVO) ---
        private static async Task TesteCadastrarAcesso()
        {
            Console.WriteLine("--- Cadastrar Credencial (Cartão/Senha) ---");
            Console.Write("ID do Usuário dono da credencial: ");
            if (!int.TryParse(Console.ReadLine(), out int idUser)) return;

            Console.WriteLine("Tipo: 1-Cartão (RFID), 2-Senha");
            var tipoInput = Console.ReadLine();
            string tipo = tipoInput == "2" ? "Senha" : "Cartao";

            Console.Write($"Digite o código ({tipo}): ");
            var codigo = Console.ReadLine();

            var payload = new AccessModel
            {
                IdUsuario = idUser,
                Tipo = tipo,
                Codigo = codigo,
                // Opcionais deixados como null/padrão
            };

            PrintResultado(await GarenApiFactory.Client.CreateAccessAsync(payload));
        }

        // --- 8. LISTAR GRUPOS (NOVO) ---
        private static async Task TesteListarGrupos()
        {
            Console.WriteLine("--- Grupos de Acesso ---");
            var resp = await GarenApiFactory.Client.GetAllGroupsAsync();
            PrintJson(resp);
        }

        // --- 9. EVENTOS ---
        private static async Task TesteLerEventos()
        {
            Console.WriteLine("--- Ler Logs (Eventos) ---");
            // Exemplo de filtro: Trazer apenas os últimos 5
            var resp = await GarenApiFactory.Client.GetEventsAsync(limite: "5");
            PrintJson(resp);
        }

        // --- HELPERS ---
        private static void PrintJson(object obj)
        {
            if (obj == null) { Console.WriteLine("(null)"); return; }
            try {
                Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
            } catch { Console.WriteLine(obj.ToString()); }
        }

        private static void PrintResultado(GenericResponse resp)
        {
            if (resp == null) { Console.WriteLine("Resposta vazia."); return; }

            if (resp.Status?.ToLower() == "success" || resp.Codigo == 200)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SUCESSO: {resp.Status} (Cod {resp.Codigo})");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"FALHA: {resp.Status} (Cod {resp.Codigo})");
            }
            Console.ResetColor();
        }
    }
}