using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    // --- ACESSO (Comandos 4, 5, 6, 69) ---
    public class AccessModel
    {
        [JsonProperty("id_usuario")] public int IdUsuario { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; } // ex: "senha", "card"
        [JsonProperty("data_inicio")] public int DataInicio { get; set; } // Epoch timestamp
        [JsonProperty("data_fim")] public int DataFim { get; set; }
        [JsonProperty("regra_de_tempo")] public string RegraDeTempo { get; set; }
        [JsonProperty("digito_verificador")] public string DigitoVerificador { get; set; }
        [JsonProperty("id_regra_de_pgm")] public int IdRegraPgm { get; set; }
    }

    public class DelAccessModel
    {
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("id_usuario")] public int IdUsuario { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("id_acesso")] public int IdAcesso { get; set; }
    }

    public class AccessResponseModel
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public int Codigo { get; set; }
        [JsonProperty("detalhes")] public List<AccessDetail> Detalhes { get; set; }
    }

    public class AccessDetail
    {
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }
    }

    // --- EVENTOS (Logs) ---
    public class EventFilter
    {
        [JsonProperty("filtros")] public EventFilterDetails Filtros { get; set; }
    }

    public class EventFilterDetails
    {
        [JsonProperty("id_usuario")] public int? IdUsuario { get; set; }
        [JsonProperty("tipo_usuario")] public int? TipoUsuario { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("hora_inicial")] public int? HoraInicial { get; set; }
        [JsonProperty("hora_final")] public int? HoraFinal { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }
    }

    // --- FACIAL & LPR ---
    public class ConfigFacialModel
    {
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("user")] public string User { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
    }
}