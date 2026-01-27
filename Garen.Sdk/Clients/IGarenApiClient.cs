using System.Threading.Tasks;
using Garen.Sdk.Contracts;
using Refit;
using System.IO;
using System.Net.Http;

namespace Garen.Sdk.Clients
{
    /// <summary>
    /// Cliente da API Garen (Placa Controladora).
    /// <para>Todas as chamadas são assíncronas e mapeiam os comandos do protocolo TCP/IP da placa.</para>
    /// </summary>
    public interface IGarenApiClient
    {
        #region API - Comandos de Acionamento (Hardware)

        /// <summary>
        /// Comando 20: Acionamento Remoto de Porta ou Relé.
        /// </summary>
        /// <param name="payload">
        /// <para>Porta: 1 ou 2.</para>
        /// <para>Tempo: Tempo em ms (0 = usa configuração da placa).</para>
        /// <para>Auxiliar: 1 = Relé Auxiliar, 0 = Porta Principal.</para>
        /// </param>
        [Post("/api/comando/acionamento_remoto")]
        Task<GenericResponse> OpenDoorAsync([Body] RemoteDoorTrigger payload);

        /// <summary>
        /// Comando 19: Inverter Fluxo de Usuário.
        /// <para>Troca o estado de "dentro/fora" para corrigir problemas de Intertravamento (Anti-Passback).</para>
        /// </summary>
        [Post("/api/comando/inverter_fluxo")]
        Task<GenericResponse> InvertFlowAsync([Body] ReverseFlow payload);

        /// <summary>
        /// Comando 59: Acionar Botão Virtual.
        /// <para>Simula via software o pressionamento físico de uma botoeira (REX).</para>
        /// </summary>
        [Post("/api/comando/botao_virtual")]
        Task<GenericResponse> VirtualButtonAsync([Body] RemoteTrigger payload);

        /// <summary>
        /// Comando 23: Atualizar Créditos de Acesso.
        /// <para>Adiciona ou remove créditos de acesso de um usuário (para controle de refeitório ou estacionamento).</para>
        /// </summary>
        [Post("/api/comando/atualizar_credito")]
        Task<GenericResponse> UpdateCreditAsync([Body] UpdateCreditModel payload);

        #endregion

        #region API - Usuários (CRUD)

        /// <summary>
        /// Comando 27: Cadastrar Novo Usuário.
        /// </summary>
        /// <remarks>
        /// <para>O 'Id' é opcional (se null, a placa gera automaticamente).</para>
        /// <para>'Validade': Timestamp Epoch em segundos (0 = sem validade).</para>
        /// <para>'TipoCadastro': 0=Visitante, 1=Morador/Fixo.</para>
        /// </remarks>
        [Post("/api/usuario")]
        Task<GenericResponse> CreateUserAsync([Body] UserRegisterModel payload);

        /// <summary>
        /// Comando 37: Atualizar Usuário Existente.
        /// <para>Envie apenas os campos que deseja alterar. Campos nulos no objeto payload serão ignorados.</para>
        /// </summary>
        /// <param name="payload">Objeto com os dados a serem alterados.</param>
        /// <param name="id">ID do usuário (Obrigatório se Hash não informado).</param>
        /// <param name="hash">Hash do usuário (Obrigatório se ID não informado).</param>
        [Put("/api/usuario")]
        Task<GenericResponse> UpdateUserAsync([Body] UserUpdateModel payload, [AliasAs("id")] string id = null, [AliasAs("hash")] string hash = null);

        /// <summary>
        /// Comando 2: Deletar Usuário.
        /// <para>Remove o cadastro e todas as credenciais, biometrias e regras associadas.</para>
        /// </summary>
        [Delete("/api/usuario")]
        Task<GenericResponse> DeleteUserAsync([Body] UserIdModel payload);

        /// <summary>
        /// Comando 28: Obter Detalhes Completos do Usuário.
        /// <para>Retorna portas, grupos, regras de tempo e lista de credenciais.</para>
        /// </summary>
        [Get("/api/usuario")]
        Task<UserSelectModel> GetUserAsync([AliasAs("id")] string id = null, [AliasAs("hash")] string hash = null);

        /// <summary>
        /// Comando 3: Listar Todos os Usuários (Resumido).
        /// <para>Retorna ID, Nome, Hash e status. Use para sincronização inicial.</para>
        /// </summary>
        [Get("/api/usuario/all")]
        Task<UserSelectModelAll> GetAllUsersAsync();

        /// <summary>
        /// Comando 43: Obter Quantidade Total de Usuários.
        /// </summary>
        [Get("/api/usuario/quantity")]
        Task<UserQuantitySelectModel> GetUserQuantityAsync();

        #endregion

        #region API - Regras de Acesso (Credenciais)

        /// <summary>
        /// Comando 4: Cadastrar Credencial (Cartão/Senha/QR).
        /// </summary>
        /// <param name="payload">
        /// <para>Tipo: "Cartao", "Senha", "Qr" ou "RF".</para>
        /// <para>Codigo: O número do cartão ou a senha numérica.</para>
        /// </param>
        [Post("/api/acesso")]
        Task<GenericResponse> CreateAccessAsync([Body] AccessModel payload);

        /// <summary>
        /// Comando 69: Atualizar Credencial.
        /// </summary>
        /// <param name="idAcesso">ID único da credencial (obtido via GetAccessAsync).</param>
        [Put("/api/acesso")]
        Task<GenericResponse> UpdateAccessAsync([Body] AccessModel payload, [AliasAs("id")] string idAcesso = null);

        /// <summary>
        /// Comando 5: Deletar Credencial.
        /// <para>Remove um cartão ou senha específica sem excluir o usuário dono.</para>
        /// </summary>
        [Delete("/api/acesso")]
        Task<GenericResponse> DeleteAccessAsync([Body] DelAccessModel payload);

        /// <summary>
        /// Comando 6: Listar Credenciais do Usuário.
        /// </summary>
        [Get("/api/acesso")]
        Task<AccessResponseModel> GetAccessAsync([AliasAs("id")] string idUsuario = null, [AliasAs("hash")] string hash = null);

        #endregion

        #region API - Grupos de Acesso

        /// <summary>
        /// Comando 24: Criar Grupo de Acesso.
        /// <para>Grupos definem portas e horários permitidos para múltiplos usuários.</para>
        /// </summary>
        [Post("/api/grupo")]
        Task<GenericResponse> CreateGroupAsync([Body] GroupModel payload);

        /// <summary>
        /// Comando 35: Atualizar Grupo.
        /// </summary>
        [Put("/api/grupo")]
        Task<GenericResponse> UpdateGroupAsync([Body] GroupUpdateModel payload, [AliasAs("id")] string id);

        /// <summary>
        /// Comando 26: Deletar Grupo.
        /// </summary>
        [Delete("/api/grupo")]
        Task<GenericResponse> DeleteGroupAsync([AliasAs("id")] string id);

        /// <summary>
        /// Comando 31: Obter Detalhes do Grupo.
        /// </summary>
        /// <param name="gmt">Fuso horário para formatação da hora (Padrão -3).</param>
        [Get("/api/grupo")]
        Task<dynamic> GetGroupAsync([AliasAs("id")] string id, [AliasAs("gmt")] string gmt = "-3");

        /// <summary>
        /// Comando 32: Listar Todos os Grupos.
        /// </summary>
        [Get("/api/grupo/all")]
        Task<GenericResponse> GetAllGroupsAsync();

        /// <summary>
        /// Comando 25: Vincular Usuário a Grupo.
        /// </summary>
        [Post("/api/grupo/user")]
        Task<GenericResponse> AssociateUserToGroupAsync([Body] GroupUserModel payload);

        #endregion

        #region API - Família (Controle de Vagas)

        /// <summary>
        /// Comando 46: Criar Família (Agrupamento de Vagas).
        /// </summary>
        [Post("/api/familia")]
        Task<GenericResponse> CreateFamilyAsync([Body] FamilyModel payload);

        /// <summary>
        /// Comando 47: Atualizar Família.
        /// </summary>
        [Put("/api/familia")]
        Task<GenericResponse> UpdateFamilyAsync([Body] FamilyModel payload, [AliasAs("id")] string id);

        /// <summary>
        /// Comando 49: Deletar Família.
        /// </summary>
        [Delete("/api/familia")]
        Task<GenericResponse> DeleteFamilyAsync([AliasAs("id")] string id);

        /// <summary>
        /// Comando 48: Consultar Família.
        /// </summary>
        [Get("/api/familia")]
        Task<FamilySelectModel> GetFamilyAsync([AliasAs("id")] string id);

        /// <summary>
        /// Comando 50: Listar Todas as Famílias.
        /// </summary>
        [Get("/api/familia/all")]
        Task<GenericResponse> GetAllFamiliesAsync();

        /// <summary>
        /// Comando 44: Configuração Global de Famílias.
        /// <para>Define o total de vagas do condomínio e se o uso é obrigatório.</para>
        /// </summary>
        [Put("/api/familia/config")]
        Task<GenericResponse> UpdateFamilyConfigAsync([Body] FamilyConfigModel payload);

        /// <summary>
        /// Comando 45: Obter Configuração de Famílias.
        /// </summary>
        [Get("/api/familia/config")]
        Task<FamilyConfigSelectModel> GetFamilyConfigAsync();

        #endregion

        #region API - Eventos (Logs de Acesso)

        /// <summary>
        /// Comando 21: Buscar Logs/Eventos.
        /// <para>Permite filtrar o histórico de acessos da placa.</para>
        /// </summary>
        /// <param name="tipo">Filtro: "Cartao", "Senha", "FACIAL", "Botoeira", "Qr".</param>
        /// <param name="status">Filtro: "Acesso", "Tentativa", "AntiPassBack", "Bloqueado", etc.</param>
        /// <param name="limite">Quantidade máxima de registros retornados (Padrão 1000).</param>
        /// <param name="horaInicial">Timestamp Epoch de início.</param>
        /// <param name="horaFinal">Timestamp Epoch de fim.</param>
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
        /// Comando 22: Deletar Logs da Memória.
        /// <para>Cuidado: Apaga permanentemente os registros que corresponderem ao filtro.</para>
        /// </summary>
        [Delete("/api/eventos")]
        Task<GenericResponse> DeleteEventsAsync([Body] EventFilter payload);

        /// <summary>
        /// Comando 98: Configurar Webhook (Push de Eventos).
        /// <para>Configura a URL para onde a placa enviará requisições POST em tempo real a cada novo evento.</para>
        /// </summary>
        [Post("/api/http-event-server")]
        Task SetHttpEventServerAsync([Body] httpEventServerModel payload);

        #endregion

        #region API - Configurações Gerais

        /// <summary>
        /// Comando 61: Configurar Rede (IP, Máscara, Gateway).
        /// </summary>
        [Post("/api/configuracao/ip")]
        Task<GenericResponse> SetIpConfigAsync([Body] IpConfigRule payload);

        /// <summary>
        /// Comando 62: Obter Configurações de Rede Atuais.
        /// </summary>
        [Get("/api/configuracao/ip")]
        Task<IpResponse> GetIpConfigAsync();

        /// <summary>
        /// Comando 63: Alterar Token de Autenticação da API.
        /// </summary>
        [Post("/api/configuracao/token")]
        Task<GenericResponse> SetTokenConfigAsync([Body] TokenConfigRule payload);

        /// <summary>
        /// Comando 30: Configurar Parâmetros de Porta.
        /// <para>Define tempo de relé, sensores, intertravamento, fluxo e tempos de validação.</para>
        /// </summary>
        [Put("/api/porta")]
        Task<GenericResponse> UpdateDoorAsync([Body] CreditDoorModel payload);

        /// <summary>
        /// Comando 33: Obter Configurações das Portas.
        /// </summary>
        [Get("/api/porta")]
        Task<dynamic> GetDoorConfigAsync();

        #endregion

        #region API - Reporte entre Placas (Server/Client)

        /// <summary>
        /// Comando 55: Cadastrar IP de Reporte.
        /// <para>Configura IPs de outras placas para comunicação entre elas.</para>
        /// </summary>
        [Post("/api/reporte")]
        Task<GenericResponse> CreateReportIpAsync([Body] ReportIpModel payload);

        /// <summary>
        /// Comando 56: Atualizar IP de Reporte.
        /// </summary>
        [Put("/api/reporte")]
        Task<GenericResponse> UpdateReportIpAsync([Body] ReportIpModel payload, [AliasAs("id")] string id);

        /// <summary>
        /// Comando 58: Deletar IP de Reporte.
        /// </summary>
        [Delete("/api/reporte")]
        Task<GenericResponse> DeleteReportIpAsync([AliasAs("id")] string id);

        /// <summary>
        /// Comando 57: Listar IPs de Reporte.
        /// </summary>
        [Get("/api/reporte")]
        Task<GenericResponse> GetReportIpAsync();

        #endregion

        #region API - Regras de Tempo (Horários)

        /// <summary>
        /// Comando 10: Criar Regra de Tempo (Horário).
        /// <para>Define intervalos de horário e dias da semana permitidos.</para>
        /// </summary>
        [Post("/api/tempo")]
        Task<GenericResponse> CreateScheduleRuleAsync([Body] ScheduleRuleWithName payload);

        /// <summary>
        /// Comando 34: Atualizar Regra de Tempo.
        /// </summary>
        [Put("/api/tempo")]
        Task<GenericResponse> UpdateScheduleRuleAsync([Body] ScheduleRule payload, [AliasAs("id")] string id, [AliasAs("nome")] string nome = null);

        /// <summary>
        /// Comando 11: Deletar Regra de Tempo.
        /// </summary>
        [Delete("/api/tempo")]
        Task<GenericResponse> DeleteScheduleRuleAsync([Body] RuleName payload);

        /// <summary>
        /// Comando 12: Listar Regras de Tempo.
        /// </summary>
        [Get("/api/tempo")]
        Task<ScheduleRuleResponse> GetScheduleRulesAsync([AliasAs("gmt")] string gmt = "-3");

        #endregion

        #region API - PGM, Facial e LPR

        /// <summary>
        /// Comando 65: Criar Regra de PGM.
        /// <para>Configura saídas programáveis para automação.</para>
        /// </summary>
        [Post("/api/pgm")]
        Task<GenericResponse> CreatePgmRuleAsync([Body] PgmRuleModel payload);

        /// <summary>
        /// Comando 64: Acionamento Remoto de PGM.
        /// </summary>
        [Post("/api/pgm/acionamento_remoto")]
        Task<GenericResponse> TriggerPgmAsync([Body] RemotePgmTrigger payload);

        /// <summary>
        /// Upload de Binário (Update de Firmware).
        /// <para>Requer Stream do arquivo .bin.</para>
        /// </summary>
        [Multipart]
        [Post("/api/upload/bin")]
        Task<GenericResponse> UploadBinAsync([AliasAs("file")] StreamPart file);

        /// <summary>
        /// Comando 92: Enviar Foto Facial (Base64).
        /// <para>Vincula uma foto a um usuário para reconhecimento facial.</para>
        /// </summary>
        [Post("/api/upload")]
        Task<GenericFacialResponse> UploadUserImageAsync([Body] ImageModel payload, [AliasAs("user_id")] string userId, [AliasAs("validity")] string validity = null, [AliasAs("beginning_validity")] string beginningValidity = null);

        /// <summary>
        /// Comando 93 (PUT): Atualizar Foto Facial.
        /// </summary>
        [Put("/api/upload")]
        Task<GenericFacialResponse> UpdateUserImageAsync([Body] ImageModel payload, [AliasAs("user_id")] string userId, [AliasAs("validity")] string validity = null, [AliasAs("beginning_validity")] string beginningValidity = null);

        /// <summary>
        /// Comando 93 (DELETE): Remover Foto Facial.
        /// </summary>
        [Delete("/api/upload")]
        Task<GenericResponse> DeleteUserImageAsync([AliasAs("user_id")] string userId);

        /// <summary>
        /// Comando 91: Baixar Foto Facial do Usuário.
        /// </summary>
        [Get("/api/upload")]
        Task<ImageModel> GetUserImageAsync([AliasAs("user_id")] string userId);

        /// <summary>
        /// Comando 96: Obter Configuração de Terminais Faciais.
        /// </summary>
        [Get("/api/facial/config")]
        Task<ConfigResponse> GetFacialConfigAsync();

        /// <summary>
        /// Sincronizar Facial (Comando Sync).
        /// <para>Força a sincronização de usuários com os terminais faciais.</para>
        /// </summary>
        [Put("/api/facial/sync")]
        Task<GenericResponse> SyncFacialAsync();

        /// <summary>
        /// Obter Última Imagem Capturada (Facial).
        /// </summary>
        [Get("/api/facial/{id}/last_image")]
        Task<FacialImageResponse> GetFacialLastImageAsync([AliasAs("id")] int id);

        /// <summary>
        /// Obter Última Placa Lida (LPR).
        /// </summary>
        [Get("/api/lpr/{id}/last_plate")]
        Task<GenericResponse> GetLprLastPlateAsync([AliasAs("id")] int id);

        #endregion

        #region API - Sistema e Backup

        /// <summary>
        /// Comando 39: Ajustar Relógio da Placa.
        /// </summary>
        [Post("/api/date")]
        Task<GenericResponse> SetDateAsync([Body] DateModel payload);

        /// <summary>
        /// Comando 36: Ler Data/Hora da Placa.
        /// </summary>
        [Get("/api/date")]
        Task<DateResponse> GetDateAsync();

        /// <summary>
        /// Comando 94: Ler Versão do Firmware.
        /// </summary>
        [Get("/versao")]
        Task<string> GetVersionAsync();

        /// <summary>
        /// Comando 95: Verificar Atualização Online.
        /// </summary>
        [Get("/versao/update")]
        Task<GenericResponse> CheckUpdateAsync();

        /// <summary>
        /// Comando 99: Reset de Fábrica.
        /// <para>ATENÇÃO: Apaga TODOS os dados (Usuários, Logs, Configurações).</para>
        /// </summary>
        [Delete("/api/reset")]
        Task<GenericResponse> FactoryResetAsync();

        /// <summary>
        /// Download do Backup Completo (.bin).
        /// <para>Retorna o arquivo binário criptografado contendo o banco de dados.</para>
        /// </summary>
        [Get("/api/backup")]
        Task<HttpContent> DownloadBackupAsync();

        /// <summary>
        /// Restaurar Backup (.bin).
        /// <para>Restaura o banco de dados a partir de um arquivo previamente baixado.</para>
        /// </summary>
        [Multipart]
        [Post("/api/backup")]
        Task<GenericResponse> RestoreBackupAsync([AliasAs("file")] StreamPart file);

        #endregion
    }
}