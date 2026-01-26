using Newtonsoft.Json;
using System.Collections.Generic;

namespace Garen.Sdk.Contracts
{
    public class GenericResponse
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public int Codigo { get; set; }
    }

    public class RemoteDoorTrigger
    {
        [JsonProperty("porta")] public int Porta { get; set; }
        [JsonProperty("auxiliar")] public int Auxiliar { get; set; }
        [JsonProperty("tempo")] public int Tempo { get; set; }
    }
    
    public class RemoteTrigger
    {
        [JsonProperty("botao")] public int Botao { get; set; }
        [JsonProperty("auxiliar")] public bool Auxiliar { get; set; }
    }
    
    public class ReverseFlow
    {
        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)] public int? IdUsuario { get; set; }
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)] public string Hash { get; set; }
    }
    
    public class IpConfigRule
    {
        [JsonProperty("ip")] public string Ip { get; set; }
        [JsonProperty("mask")] public string Mask { get; set; }
        [JsonProperty("dns")] public string Dns { get; set; }
    }
    
    public class IpResponse : IpConfigRule 
    {
        [JsonProperty("mac")] public string Mac { get; set; }
    }

    public class TokenConfigRule
    {
        [JsonProperty("token")] public string Token { get; set; }
    }

    public class DateModel
    {
        [JsonProperty("epoch")] public int Epoch { get; set; }
    }
    
    public class DateResponse 
    {
        [JsonProperty("detalhes")] public List<DateModel> Detalhes { get; set; }
    }
    
    // Schedule e PGM
    public class ScheduleRuleWithName
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("inicio")] public string Inicio { get; set; } 
        [JsonProperty("fim")] public string Fim { get; set; } 
        [JsonProperty("gmt")] public int Gmt { get; set; }
        [JsonProperty("dias")] public List<int> Dias { get; set; }
    }

    public class ScheduleRule : ScheduleRuleWithName { } 

    public class RuleName { [JsonProperty("id")] public int Id { get; set; } }

    public class ScheduleRuleResponse
    {
        [JsonProperty("detalhes")] public List<ScheduleRuleWithName> Detalhes { get; set; }
    }

    public class PgmRuleModel
    {
        [JsonProperty("portas")] public List<string> Portas { get; set; }
        [JsonProperty("index_pgm")] public int IndexPgm { get; set; }
        [JsonProperty("tempo_aberto")] public int TempoAberto { get; set; }
        [JsonProperty("botao")] public int Botao { get; set; }
    }

    public class RemotePgmTrigger
    {
        [JsonProperty("pgm")] public int Pgm { get; set; }
    }
}