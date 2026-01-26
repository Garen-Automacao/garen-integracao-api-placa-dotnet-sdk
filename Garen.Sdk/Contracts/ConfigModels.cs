using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
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

    public class ScheduleRuleWithName
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("inicio")] public string Inicio { get; set; } // "07:10"
        [JsonProperty("fim")] public string Fim { get; set; } // "18:30"
        [JsonProperty("gmt")] public int Gmt { get; set; }
        [JsonProperty("dias")] public List<int> Dias { get; set; }
    }

    public class ScheduleRule : ScheduleRuleWithName { } // Estrutura similar

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
    
    // Usado para respostas de listas (FamilySelectModel, UserSelectModel, etc)
    // Como o Refit retorna o JSON puro, podemos usar generics ou dynamic se preferir,
    // mas classes tipadas são melhores. Segue exemplo genérico para listas:
    public class GenericListResponse<T>
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public int Codigo { get; set; }
        [JsonProperty("detalhes")] public List<T> Detalhes { get; set; }
    }
}