using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Refit;
using System.IO;

namespace Garen.Sdk.Clients
{
    /// <summary>
    /// Interface de comunicação com a API REST da Placa Controladora Garen.
    /// <para>Baseado no Manual da API v1.17.1.</para>
    /// </summary>
    public interface IGarenApiClient
    {
        #region API - Comandos Básicos (Acionamento)

        /// <summary>
        /// Acionamento Remoto de Porta (Comando 20).
        /// Abre instantaneamente uma porta ou aciona um relé.
        /// </summary>
        /// <param name="payload">
        /// Configure 'Porta' (1 ou 2) e 'Tempo' (em ms). 
        /// Use tempo=0 para usar a configuração padrão da placa.
        /// </param>
        [Post("/api/comando/acionamento_remoto")]
        Task<GenericResponse> OpenDoorAsync([Body] RemoteDoorTrigger payload);

        /// <summary>
        /// Inverter Fluxo (Comando 19).
        /// Inverte o estado de "dentro/fora" de um usuário para controle de Intertravamento/Anti-Dupla.
        /// </summary>
        [Post("/api/comando/inverter_fluxo")]
        Task<GenericResponse> InvertFlowAsync([Body] ReverseFlow payload);

        /// <summary>
        /// Acionar Botão Virtual (Comando 59).
        /// Simula o pressionamento físico de uma botoeira de saída (REX).
        /// </summary>
        [Post("/api/comando/botao_virtual")]
        Task<GenericResponse> VirtualButtonAsync([Body] RemoteTrigger payload);

        #endregion

        #region API - Usuários (CRUD)

        /// <summary>
        /// Cadastrar Usuário (Comando 27).
        /// </summary>
        /// <param name="payload">
        /// - 'validade': Timestamp Epoch (segundos).
        /// - 'tipo_cadastro': 0=Visitante, 1=Morador/Fixo (usado apenas para relatório).
        /// </param>
        [Post("/api/usuario")]
        Task<GenericResponse> CreateUserAsync([Body] UserRegisterModel payload);

        /// <summary>
        /// Atualizar Usuário (Comando 37).
        /// <para>Envie apenas os campos que deseja alterar. Campos nulos serão ignorados.</para>
        /// </summary>
        /// <param name="id">ID do usuário (Obrigatório se Hash não informado).</param>
        /// <param name="hash">Hash do usuário (Opcional).</param>
        [Put("/api/usuario")]
        Task<GenericResponse> UpdateUserAsync([Body] UserUpdateModel payload, [AliasAs("id")] string id = null, [AliasAs("hash")] string hash = null);

        /// <summary>
        /// Deletar Usuário (Comando 2).
        /// Remove o usuário, suas credenciais, biometrias e regras.
        /// </summary>
        [Delete("/api/usuario")]
        Task<GenericResponse> DeleteUserAsync([Body] UserIdModel payload);

        /// <summary>
        /// Obter Detalhes do Usuário (Comando 28).
        /// Retorna todas as informações, incluindo cartões, senhas e grupos vinculados.
        /// </summary>
        [Get("/api/usuario")]
        Task<UserSelectModel> GetUserAsync([AliasAs("id")] string id = null, [AliasAs("hash")] string hash = null);

        /// <summary>
        /// Listar Todos os Usuários (Comando 3).
        /// Lista simplificada (ID, Nome, Hash) de todos os cadastros.
        /// </summary>
        [Get("/api/usuario/all")]
        Task<UserSelectModelAll> GetAllUsersAsync();

        /// <summary>
        /// Obter Quantidade de Usuários (Comando 43).
        /// </summary>
        [Get("/api/usuario/quantity")]
        Task<UserQuantitySelectModel> GetUserQuantityAsync();

        #endregion

        #region API - Regras de Acesso (Credenciais)

        /// <summary>
        /// Cadastrar Credencial (Comando 4).
        /// Vincula Cartão (RFID), Senha ou QR Code a um usuário existente.
        /// </summary>
        /// <param name="payload">
        /// - Tipo: "Cartao", "Senha", "Qr".
        /// - Codigo: O valor do cartão ou senha.
        /// </param>
        [Post("/api/acesso")]
        Task<GenericResponse> CreateAccessAsync([Body] AccessModel payload);

        /// <summary>
        /// Atualizar Credencial (Comando 69).
        /// </summary>
        [Put("/api/acesso")]
        Task<GenericResponse> UpdateAccessAsync([Body] AccessModel payload, [AliasAs("id")] string idAcesso = null);

        /// <summary>
        /// Deletar Credencial (Comando 5).
        /// Remove um cartão/senha específico.
        /// </summary>
        [Delete("/api/acesso")]
        Task<GenericResponse> DeleteAccessAsync([Body] DelAccessModel payload);

        /// <summary>
        /// Listar Credenciais do Usuário (Comando 6).
        /// </summary>
        [Get("/api/acesso")]
        Task<AccessResponseModel> GetAccessAsync([AliasAs("id")] string idUsuario = null, [AliasAs("hash")] string hash = null);

        #endregion

        #region API - Grupos

        /// <summary>
        /// Cadastrar Grupo (Comando 24).
        /// Grupos definem em quais portas e horários o usuário pode entrar.
        /// </summary>
        [Post("/api/grupo")]
        Task<GenericResponse> CreateGroupAsync([Body] GroupModel payload);

        /// <summary>
        /// Atualizar Grupo (Comando 35).
        /// </summary>
        [Put("/api/grupo")]
        Task<GenericResponse> UpdateGroupAsync([Body] GroupUpdateModel payload, [AliasAs("id")] string id);

        /// <summary>
        /// Deletar Grupo (Comando 26).
        /// </summary>
        [Delete("/api/grupo")]
        Task<GenericResponse> DeleteGroupAsync([AliasAs("id")] string id);

        /// <summary>
        /// Obter Detalhes do Grupo (Comando 31).
        /// </summary>
        [Get("/api/grupo")]
        Task<dynamic> GetGroupAsync([AliasAs("id")] string id, [AliasAs("gmt")] string gmt = "-3");

        /// <summary>
        /// Listar Todos os Grupos (Comando 32).
        /// </summary>
        [Get("/api/grupo/all")]
        Task<GenericResponse> GetAllGroupsAsync();

        /// <summary>
        /// Vincular Usuário a Grupo (Comando 25).
        /// </summary>
        [Post("/api/grupo/user")]
        Task<GenericResponse> AssociateUserToGroupAsync([Body] GroupUserModel payload);

        #endregion

        #region API - Eventos (Logs)

        /// <summary>
        /// Buscar Eventos / Logs (Comando 21).
        /// Retorna o histórico de acessos.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="nome">Nome parcial do usuário.</param>
        /// <param name="tipo">Filtro: "Cartao", "Senha", "FACIAL", "Botoeira", "Qr".</param>
        /// <param name="status">Filtro: "Acesso", "Tentativa", "Desconhecido".</param>
        /// <param name="horaInicial">Epoch timestamp inicio.</param>
        /// <param name="horaFinal">Epoch timestamp fim.</param>
        /// <param name="limite">Máximo de registros (Padrão 1000).</param>
        [Get("/api/eventos")]
        Task<EventResponse> GetEventsAsync(
            [AliasAs("id")] string id = null,
            [AliasAs("tipo_usuario")] string tipoUsuario = null,
            [AliasAs("hash")] string hash = null,
            [AliasAs("nome")] string nome = null,
            [AliasAs("hora_inicial")] string horaInicial = null,
            [AliasAs("hora_final")] string horaFinal = null,
            [AliasAs("limite")] string limite = null,
            [AliasAs("status")] string status = null,
            [AliasAs("tipo")] string tipo = null);

        /// <summary>
        /// Deletar Eventos (Comando 22).
        /// Filtra e apaga logs da memória da placa.
        /// </summary>
        [Delete("/api/eventos")]
        Task<GenericResponse> DeleteEventsAsync([Body] EventFilter payload);

        /// <summary>
        /// Configurar Monitoramento HTTP (Comando 98).
        /// A placa enviará POSTs para esta URL a cada novo evento.
        /// </summary>
        [Post("/api/http-event-server")]
        Task SetHttpEventServerAsync([Body] httpEventServerModel payload);

        #endregion

        #region API - Configurações Gerais

        [Post("/api/configuracao/ip")]
        Task<GenericResponse> SetIpConfigAsync([Body] IpConfigRule payload);

        [Get("/api/configuracao/ip")]
        Task<IpResponse> GetIpConfigAsync();

        [Post("/api/configuracao/token")]
        Task<GenericResponse> SetTokenConfigAsync([Body] TokenConfigRule payload);

        /// <summary>
        /// Ajustar Relógio (Comando 39).
        /// </summary>
        [Post("/api/date")]
        Task<GenericResponse> SetDateAsync([Body] DateModel payload);

        [Get("/api/date")]
        Task<DateResponse> GetDateAsync();

        #endregion

        #region API - Regras de Tempo (Horários)

        [Post("/api/tempo")]
        Task<GenericResponse> CreateScheduleRuleAsync([Body] ScheduleRuleWithName payload);

        [Put("/api/tempo")]
        Task<GenericResponse> UpdateScheduleRuleAsync([Body] ScheduleRule payload, [AliasAs("id")] string id, [AliasAs("nome")] string nome = null);

        [Delete("/api/tempo")]
        Task<GenericResponse> DeleteScheduleRuleAsync([Body] RuleName payload);

        [Get("/api/tempo")]
        Task<ScheduleRuleResponse> GetScheduleRulesAsync([AliasAs("gmt")] string gmt = "-3");

        #endregion

        #region API - PGM, Facial e LPR

        [Post("/api/pgm")]
        Task<GenericResponse> CreatePgmRuleAsync([Body] PgmRuleModel payload);

        [Post("/api/pgm/acionamento_remoto")]
        Task<GenericResponse> TriggerPgmAsync([Body] RemotePgmTrigger payload);

        /// <summary>
        /// Upload de Imagem (Backup ou Facial).
        /// </summary>
        [Multipart]
        [Post("/api/upload/bin")]
        Task<GenericResponse> UploadImageBinAsync([AliasAs("file")] StreamPart file);

        [Get("/api/facial/config")]
        Task<ConfigResponse> GetFacialConfigAsync();

        /// <summary>
        /// Sincronizar Facial (Comando Sync).
        /// </summary>
        [Put("/api/facial/sync")]
        Task<GenericResponse> SyncFacialAsync();

        [Get("/api/facial/{id}/last_image")]
        Task<GenericResponse> GetFacialLastImageAsync([AliasAs("id")] int id);

        [Get("/api/lpr/{id}/last_plate")]
        Task<GenericResponse> GetLprLastPlateAsync([AliasAs("id")] int id);

        #endregion

        #region API - Sistema

        /// <summary>
        /// Versão do Firmware (Comando 94).
        /// </summary>
        [Get("/versao")]
        Task<string> GetVersionAsync();

        [Get("/versao/update")]
        Task<GenericResponse> CheckUpdateAsync();

        /// <summary>
        /// Reset de Fábrica (Comando 99).
        /// </summary>
        [Delete("/api/reset")]
        Task<GenericResponse> FactoryResetAsync();

        #endregion
    }
}