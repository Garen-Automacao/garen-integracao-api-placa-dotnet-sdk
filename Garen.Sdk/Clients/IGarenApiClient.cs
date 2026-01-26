using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Refit;
using System.IO;

namespace Garen.Sdk.Clients
{
    public interface IGarenApiClient
    {
        #region API - Comandos Básicos (Acionamento)

        // Comando 20: Acionamento Remoto (Porta)
        [Post("/api/comando/acionamento_remoto")]
        Task<GenericResponse> OpenDoorAsync([Body] RemoteDoorTrigger payload);

        // Comando 19: Inverter Fluxo
        [Post("/api/comando/inverter_fluxo")]
        Task<GenericResponse> InvertFlowAsync([Body] object payload); // Criar DTO ReverseFlow se necessário

        // Comando 59: Botão Virtual
        [Post("/api/comando/botao_virtual")]
        Task<GenericResponse> VirtualButtonAsync([Body] object payload); // DTO RemoteTrigger

        #endregion

        #region API - Usuários (CRUD)

        // Comando 27: Criar Usuário
        [Post("/api/usuario")]
        Task<GenericResponse> CreateUserAsync([Body] UserRegisterModel payload);

        // Comando 37: Atualizar Usuário
        [Put("/api/usuario")]
        Task<GenericResponse> UpdateUserAsync([Body] UserUpdateModel payload, [AliasAs("id")] string id,
            [AliasAs("hash")] string hash);

        // Comando 2: Deletar Usuário
        [Delete("/api/usuario")]
        Task<GenericResponse> DeleteUserAsync([Body] UserIdModel payload);

        // Obter Usuário Específico
        [Get("/api/usuario")]
        Task<dynamic> GetUserAsync([AliasAs("id")] string id, [AliasAs("hash")] string hash);

        // Obter Todos Usuários
        [Get("/api/usuario/all")]
        Task<dynamic> GetAllUsersAsync();

        // Comando 43: Quantidade de Usuários
        [Get("/api/usuario/quantity")]
        Task<dynamic> GetUserQuantityAsync();

        #endregion

        #region API - Grupos

        // Comando 24: Criar Grupo
        [Post("/api/grupo")]
        Task<GenericResponse> CreateGroupAsync([Body] GroupModel payload);

        // Comando 35: Atualizar Grupo
        [Put("/api/grupo")]
        Task<GenericResponse> UpdateGroupAsync([Body] GroupModel payload, [AliasAs("id")] string id);

        // Comando 26: Deletar Grupo
        [Delete("/api/grupo")]
        Task<GenericResponse> DeleteGroupAsync([AliasAs("id")] string id);

        // Comando 31: Obter Grupo
        [Get("/api/grupo")]
        Task<dynamic> GetGroupAsync([AliasAs("id")] string id);

        // Comando 32: Listar todos grupos
        [Get("/api/grupo/all")]
        Task<GenericResponse> GetAllGroupsAsync();

        // Comando 25: Associar Usuário a Grupo
        [Post("/api/grupo/user")]
        Task<GenericResponse> AssociateUserToGroupAsync([Body] GroupUserModel payload);

        #endregion

        #region API - Configurações (IP, Token, Date)

        // Comando 61: Configurar IP
        [Post("/api/configuracao/ip")]
        Task<GenericResponse> SetIpConfigAsync([Body] IpConfigRule payload);

        // Comando 62: Obter Config IP
        [Get("/api/configuracao/ip")]
        Task<IpResponse> GetIpConfigAsync();

        // Comando 63: Configurar Token
        [Post("/api/configuracao/token")]
        Task<GenericResponse> SetTokenConfigAsync([Body] TokenConfigRule payload);

        // Comando 39: Setar Data/Hora
        [Post("/api/date")]
        Task<GenericResponse> SetDateAsync([Body] object payload); // DTO DateModel

        // Comando 36: Obter Data/Hora
        [Get("/api/date")]
        Task<dynamic> GetDateAsync();

        #endregion

        #region API - Regras de Tempo (Schedule)

        // Comando 10: Criar Regra de Tempo
        [Post("/api/tempo")]
        Task<GenericResponse> CreateScheduleRuleAsync([Body] ScheduleRuleWithName payload);

        // Comando 34: Atualizar Regra de Tempo
        [Put("/api/tempo")]
        Task<GenericResponse> UpdateScheduleRuleAsync([Body] ScheduleRule payload, [AliasAs("id")] string id,
            [AliasAs("nome")] string nome);

        // Comando 11: Deletar Regra de Tempo
        [Delete("/api/tempo")]
        Task<GenericResponse> DeleteScheduleRuleAsync([Body] object payload); // DTO RuleName

        // Comando 12: Listar Regras de Tempo
        [Get("/api/tempo")]
        Task<GenericListResponse<ScheduleRuleWithName>> GetScheduleRulesAsync([AliasAs("gmt")] string gmt = "-3");

        #endregion

        #region API - PGM (Saídas Programáveis)

        // Comando 65: Criar Regra PGM
        [Post("/api/pgm")]
        Task<GenericResponse> CreatePgmRuleAsync([Body] PgmRuleModel payload);

        // Comando 64: Acionamento Remoto PGM
        [Post("/api/pgm/acionamento_remoto")]
        Task<GenericResponse> TriggerPgmAsync([Body] RemotePgmTrigger payload);

        #endregion

        #region API - Uploads e Arquivos (Multipart)

        // Upload de Backup (Requer StreamPart do Refit)
        [Multipart]
        [Post("/api/backup")]
        Task<GenericResponse> UploadBackupAsync([AliasAs("file")] StreamPart file);

        // Upload de Imagem (Facial/Cadastro)
        [Multipart]
        [Post("/api/upload/bin")]
        Task<GenericResponse> UploadImageBinAsync([AliasAs("file")] StreamPart file);

        #endregion

        #region API - Sistema e Versão

        // Comando 94: Versão
        [Get("/versao")]
        Task<string> GetVersionAsync();

        // Comando 95: Update Firmware
        [Get("/versao/update")]
        Task<GenericResponse> CheckUpdateAsync();

        // Comando 99: Reset Total (CUIDADO!)
        [Delete("/api/reset")]
        Task<GenericResponse> FactoryResetAsync();

        // MAC Address
        [Get("/mac")]
        Task<dynamic> GetMacAddressAsync();

        #endregion

        #region API - Regras de Acesso (Credenciais)

        // Comando 4: Criar Acesso
        [Post("/api/acesso")]
        Task<GenericResponse> CreateAccessAsync([Body] AccessModel payload);

        // Comando 69: Atualizar Acesso
        [Put("/api/acesso")]
        Task<GenericResponse> UpdateAccessAsync([Body] AccessModel payload, [AliasAs("id")] string id);

        // Comando 5: Deletar Acesso
        [Delete("/api/acesso")]
        Task<GenericResponse> DeleteAccessAsync([Body] DelAccessModel payload);

        // Comando 6: Obter Acessos
        [Get("/api/acesso")]
        Task<AccessResponseModel> GetAccessAsync([AliasAs("id")] string idUsuario, [AliasAs("hash")] string hash);

        #endregion

        #region API - Eventos (Logs)

        // Comando 21: Buscar Eventos
        [Get("/api/eventos")]
        Task<GenericResponse> GetEventsAsync(
            [AliasAs("id")] string id = null,
            [AliasAs("nome")] string nome = null,
            [AliasAs("hora_inicial")] string horaInicial = null,
            [AliasAs("hora_final")] string horaFinal = null,
            [AliasAs("tipo")] string tipo = null);

        // Comando 22: Deletar Eventos (Limpar Logs)
        [Delete("/api/eventos")]
        Task<GenericResponse> DeleteEventsAsync([Body] EventFilter payload);

        // Comando 98: Configurar Servidor de Eventos HTTP (Monitoramento)
        [Post("/api/http-event-server")]
        Task SetHttpEventServerAsync([Body] object payload); // Requer httpEventServerModel

        #endregion

        #region API - Facial e LPR

        // Comando 96: Configuração Facial
        [Get("/api/facial/config")]
        Task<GenericResponse> GetFacialConfigAsync();

        [Post("/api/facial/{id}/config")]
        Task<GenericResponse> SetFacialConfigAsync([AliasAs("id")] int id, [Body] ConfigFacialModel payload);

        [Get("/api/facial/{id}/last_image")]
        Task<GenericResponse> GetFacialLastImageAsync([AliasAs("id")] int id);

        [Get("/api/facial/{id}/snapshot")]
        Task<GenericResponse> GetFacialSnapshotAsync([AliasAs("id")] int id);

        // LPR (Leitura de Placa)
        [Get("/api/lpr/{id}/last_plate")]
        Task<GenericResponse> GetLprLastPlateAsync([AliasAs("id")] int id);

        #endregion
    }
}