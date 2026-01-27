using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    // --- ACESSO ---
    public class AccessModel
    {
        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)] 
        public int? IdUsuario { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)] 
        public string Hash { get; set; }

        [JsonProperty("codigo")] public string Codigo { get; set; }
        
        /// <summary>
        /// "Cartao", "Senha", "Qr".
        /// </summary>
        [JsonProperty("tipo")] public string Tipo { get; set; } 
        
        [JsonProperty("data_inicio", NullValueHandling = NullValueHandling.Ignore)] 
        public int? DataInicio { get; set; }

        [JsonProperty("data_fim", NullValueHandling = NullValueHandling.Ignore)] 
        public int? DataFim { get; set; }

        [JsonProperty("regra_de_tempo", NullValueHandling = NullValueHandling.Ignore)] 
        public string RegraDeTempo { get; set; }
        
        [JsonProperty("id_regra_de_pgm", NullValueHandling = NullValueHandling.Ignore)] 
        public int? IdRegraPgm { get; set; }
    }

    public class DelAccessModel
    {
        [JsonProperty("codigo", NullValueHandling = NullValueHandling.Ignore)] 
        public string Codigo { get; set; }

        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)] 
        public int? IdUsuario { get; set; }

        [JsonProperty("id_acesso", NullValueHandling = NullValueHandling.Ignore)] 
        public int? IdAcesso { get; set; }
    }

    public class AccessResponseModel
    {
        [JsonProperty("detalhes")] public List<AccessDetail> Detalhes { get; set; }
    }

    public class AccessDetail
    {
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }
    }

    // --- EVENTOS ---
    public class EventFilter
    {
        [JsonProperty("filtros")] public EventFilterDetails Filtros { get; set; }
    }

    public class EventFilterDetails
    {
        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)] public int? IdUsuario { get; set; }
        [JsonProperty("tipo_usuario", NullValueHandling = NullValueHandling.Ignore)] public int? TipoUsuario { get; set; }
        [JsonProperty("nome", NullValueHandling = NullValueHandling.Ignore)] public string Nome { get; set; }
        [JsonProperty("hora_inicial", NullValueHandling = NullValueHandling.Ignore)] public int? HoraInicial { get; set; }
        [JsonProperty("hora_final", NullValueHandling = NullValueHandling.Ignore)] public int? HoraFinal { get; set; }
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
        [JsonProperty("tipo", NullValueHandling = NullValueHandling.Ignore)] public string Tipo { get; set; }
    }
    
    public class EventResponse
    {
        [JsonProperty("detalhes")] public List<EventResponseDetails> Detalhes { get; set; }
    }
    
    public class EventResponseDetails
    {
        // Mapeia o campo "data" do JSON (Epoch timestamp)
        [JsonProperty("data")] 
        public int TimestampEpoch { get; set; }

        [JsonProperty("nome")] 
        public string Nome { get; set; }

        [JsonProperty("tipo")] 
        public string Tipo { get; set; } // Ex: "Cartao"

        [JsonProperty("status")] 
        public string Status { get; set; } // Ex: "AntiPassBack"

        [JsonProperty("id_usuario")] 
        public int? IdUsuario { get; set; }

        [JsonProperty("tipo_usuario")] 
        public int? TipoUsuario { get; set; }

        [JsonProperty("descricao")] 
        public string DescricaoRaw { get; set; } // JSON em string

        [JsonProperty("descricao_obj")] 
        public EventDescriptionObject DetalhesHardware { get; set; }
    }

    public class EventDescriptionObject
    {
        [JsonProperty("door")] public string Door { get; set; }
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("mac")] public long? Mac { get; set; }
    }

    public class httpEventServerModel
    {
        [JsonProperty("url")] public string Url { get; set; }
    }

    // --- FACIAL ---
    public class ConfigFacialModel
    {
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("user")] public string User { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
    }
    
    public class ConfigResponse 
    {
        [JsonProperty("detalhes")] public List<ConfigFacialModel> Detalhes { get; set; }
    }
}