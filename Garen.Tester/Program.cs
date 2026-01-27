using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Garen.Sdk.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;

namespace Garen.Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Garen SDK Tester v3.1 - Full Coverage";
            Console.WriteLine("=== Garen SDK Tester v3.1 ===");

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

            Console.WriteLine($"Conectado em: {baseUrl}");
            GarenApiFactory.Initialize(baseUrl, token);
            Console.WriteLine("SDK Pronta.\n");

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
                
                Console.WriteLine("6 - [ACESSO]   Listar Cartões/Senhas");
                Console.WriteLine("7 - [ACESSO]   Cadastrar Cartão/Senha");
                
                Console.WriteLine("8 - [GRUPOS]   Listar Grupos");
                Console.WriteLine("9 - [EVENTOS]  Ler Logs (Últimos 5)");
                
                Console.WriteLine("10- [FACIAL]   Última Foto (Log)");
                Console.WriteLine("11- [LPR]      Última Placa Lida");
                
                Console.WriteLine("12- [PGM]      Listar Regras");
                Console.WriteLine("13- [FACIAL]   Snapshot Ao Vivo (Câmera)");
                Console.WriteLine("14- [SISTEMA]  Ping e MAC Address");
                
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
                        case "6": await TesteListarAcessos(); break;
                        case "7": await TesteCadastrarAcesso(); break;
                        case "8": await TesteListarGrupos(); break;
                        case "9": await TesteLerEventos(); break;
                        case "10": await TesteFacialLastImage(); break;
                        case "11": await TesteLprLastPlate(); break;
                        case "12": await TesteListarPgm(); break;
                        case "13": await TesteFacialSnapshot(); break;
                        case "14": await TesteSistemaExtras(); break;
                        case "0": return;
                        default: Console.WriteLine("Opção inválida."); break;
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n[TIMEOUT] A placa não respondeu em 5 segundos.");
                    Console.ResetColor();
                }
                catch (ApiException apiEx)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERRO API] {apiEx.StatusCode} - {apiEx.Content}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ERRO] {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nPressione ENTER para voltar...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // ... (Mantenha os métodos de 1 a 11 iguais aos anteriores) ...
        
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
            Console.WriteLine($"Versão Firmware: {await GarenApiFactory.Client.GetVersionAsync()}");
            Console.WriteLine("\n[Configuração de IP]");
            PrintModel(await GarenApiFactory.Client.GetIpConfigAsync());
            Console.WriteLine("\n[Data/Hora Interna]");
            var dateResp = await GarenApiFactory.Client.GetDateAsync();
            if (dateResp?.Detalhes != null) PrintModel(dateResp.Detalhes);
        }

        // --- 3. LISTAR USUÁRIOS ---
        private static async Task TesteListarUsuarios()
        {
            Console.WriteLine("--- Usuários Cadastrados ---");
            var resp = await GarenApiFactory.Client.GetAllUsersAsync();
            if (resp?.Detalhes != null)
            {
                Console.WriteLine($"Total encontrados: {resp.Detalhes.Count}");
                PrintModel(resp.Detalhes.Take(5));
            }
            else Console.WriteLine("Nenhum usuário encontrado.");
        }

        // --- 4. CRIAR USUÁRIO ---
        private static async Task TesteCriarUsuario()
        {
            Console.WriteLine("--- Criar Usuário ---");
            Console.Write("Nome: ");
            var nome = Console.ReadLine();
            if (string.IsNullOrEmpty(nome)) nome = "User SDK Test";
            var novoUsuario = new UserRegisterModel { Nome = nome };
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

        // --- 6. LISTAR ACESSOS ---
        private static async Task TesteListarAcessos()
        {
            Console.WriteLine("--- Listar Credenciais ---");
            Console.Write("ID do Usuário (Enter para buscar todos): ");
            var id = Console.ReadLine();
            var resp = await GarenApiFactory.Client.GetAccessAsync(idUsuario: id);
            if (resp?.Detalhes != null) PrintModel(resp.Detalhes);
            else Console.WriteLine("Nenhuma credencial encontrada.");
        }

        // --- 7. CADASTRAR ACESSO ---
        private static async Task TesteCadastrarAcesso()
        {
            Console.WriteLine("--- Cadastrar Credencial ---");
            Console.Write("ID do Usuário: ");
            if (!int.TryParse(Console.ReadLine(), out int idUser)) return;
            Console.WriteLine("Tipo: 1-Cartão (RFID), 2-Senha, 3-QR Code");
            var tipoInput = Console.ReadLine();
            string tipo = "Cartao";
            if (tipoInput == "2") tipo = "Senha";
            if (tipoInput == "3") tipo = "Qr";
            Console.Write($"Digite o código ({tipo}): ");
            var codigo = Console.ReadLine();
            var payload = new AccessModel { IdUsuario = idUser, Tipo = tipo, Codigo = codigo };
            PrintResultado(await GarenApiFactory.Client.CreateAccessAsync(payload));
        }

        // --- 8. LISTAR GRUPOS ---
        private static async Task TesteListarGrupos()
        {
            Console.WriteLine("--- Grupos de Acesso ---");
            var resp = await GarenApiFactory.Client.GetAllGroupsAsync();
            PrintResultado(resp);
        }

        // --- 9. EVENTOS ---
        private static async Task TesteLerEventos()
        {
            Console.WriteLine("--- Ler Logs (Eventos Recentes) ---");
            var resp = await GarenApiFactory.Client.GetEventsAsync(limite: "5");
            if (resp?.Detalhes != null) PrintModel(resp.Detalhes);
            else Console.WriteLine("Nenhum evento encontrado.");
        }

        // --- 10. FACIAL (LOG) ---
        private static async Task TesteFacialLastImage()
        {
            Console.WriteLine("--- Última Foto (Log de Acesso) ---");
            Console.Write("ID do Facial (1 a 4): ");
            if (!int.TryParse(Console.ReadLine(), out int id)) id = 1;
            try 
            {
                var resp = await GarenApiFactory.Client.GetFacialLastImageAsync(id);
                SalvarImagem(resp?.ImageBase64, $"facial_log_{id}");
            }
            catch (Exception ex) { Console.WriteLine($"Erro: {ex.Message}"); }
        }

        // --- 11. LPR ---
        private static async Task TesteLprLastPlate()
        {
            Console.WriteLine("--- Última Placa Lida ---");
            Console.Write("ID do LPR (1 a 4): ");
            if (!int.TryParse(Console.ReadLine(), out int id)) id = 1;
            try 
            {
                string placa = await GarenApiFactory.Client.GetLprLastPlateAsync(id);
                Console.WriteLine(!string.IsNullOrEmpty(placa) ? $"PLACA: {placa}" : "Nenhuma placa encontrada.");
            }
            catch (Exception ex) { Console.WriteLine($"Erro: {ex.Message}"); }
        }

        // --- 12. PGM (NOVO) ---
        private static async Task TesteListarPgm()
        {
            Console.WriteLine("--- Regras de PGM ---");
            var resp = await GarenApiFactory.Client.GetPgmRulesAsync();
            if (resp?.Detalhes != null) PrintModel(resp.Detalhes);
            else Console.WriteLine("Nenhuma regra PGM encontrada.");
        }

        // --- 13. SNAPSHOT (NOVO) ---
        private static async Task TesteFacialSnapshot()
        {
            Console.WriteLine("--- Snapshot Facial (Ao Vivo) ---");
            Console.Write("ID da Câmera (1-4): ");
            if (!int.TryParse(Console.ReadLine(), out int id)) id = 1;
            Console.WriteLine("Capturando...");
            try 
            {
                var resp = await GarenApiFactory.Client.GetFacialSnapshotAsync(id);
                SalvarImagem(resp?.ImageBase64, $"snapshot_live_{id}");
            }
            catch (Exception ex) { Console.WriteLine($"Erro: {ex.Message}"); }
        }

        // --- 14. SISTEMA EXTRAS (NOVO) ---
        private static async Task TesteSistemaExtras()
        {
            Console.WriteLine("--- Diagnóstico ---");
            try 
            {
                var ping = await GarenApiFactory.Client.PingAsync();
                Console.WriteLine($"Ping API: {ping} (Esperado: 42)");
                var mac = await GarenApiFactory.Client.GetMacAddressAsync();
                Console.WriteLine($"MAC Address: {mac}");
            }
            catch (Exception ex) { Console.WriteLine($"Erro: {ex.Message}"); }
        }

        // --- HELPERS ---
        private static void SalvarImagem(string base64Data, string prefixo)
        {
            if (string.IsNullOrEmpty(base64Data))
            {
                Console.WriteLine("Nenhuma imagem recebida.");
                return;
            }
            if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
            byte[] bytes = Convert.FromBase64String(base64Data);
            string fileName = $"{prefixo}_{DateTime.Now:HHmmss}.jpg";
            File.WriteAllBytes(fileName, bytes);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCESSO: Imagem salva como '{fileName}' ({bytes.Length} bytes).");
            Console.ResetColor();
        }

        private static void PrintModel(object obj)
        {
            if (obj == null) { Console.WriteLine("(null)"); return; }
            if (obj is IEnumerable list && !(obj is string))
            {
                int count = 0;
                foreach (var item in list)
                {
                    Console.WriteLine($"\n--- Registro #{++count} ---");
                    PrintSingleObject(item);
                }
                if (count == 0) Console.WriteLine("Lista vazia.");
                return;
            }
            PrintSingleObject(obj);
        }

        private static void PrintSingleObject(object obj)
        {
            if (obj == null) return;
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                var val = prop.GetValue(obj);
                if (prop.Name.ToLower().Contains("data") || prop.Name.ToLower().Contains("epoch"))
                {
                    if (val is int epochInt && epochInt > 0) val = $"{epochInt} ({DateTimeOffset.FromUnixTimeSeconds(epochInt).ToLocalTime():dd/MM HH:mm:ss})";
                    if (val is double epochDbl && epochDbl > 0) val = $"{epochDbl} ({DateTimeOffset.FromUnixTimeSeconds((long)epochDbl).ToLocalTime():dd/MM HH:mm:ss})";
                }
                bool isSimple = val == null || val.GetType().IsPrimitive || val.GetType().IsEnum || val is string || val is decimal;
                if (isSimple) Console.WriteLine($"{prop.Name.PadRight(20)}: {val ?? "-"}");
            }
        }

        private static void PrintResultado(GenericResponse resp)
        {
            if (resp == null) { Console.WriteLine("Resposta vazia (null)."); return; }
            if (resp.Status?.ToLower() == "sucesso" || resp.Codigo == GarenReturnCode.Sucesso || (int)resp.Codigo == 200)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SUCESSO: {resp.Status}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"FALHA: {resp.Descricao} (Cod: {resp.Codigo})");
            }
            Console.ResetColor();
        }
    }
}