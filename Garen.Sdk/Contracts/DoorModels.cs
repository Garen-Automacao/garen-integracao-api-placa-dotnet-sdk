using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    public class CreditDoorModel
    {
        [JsonProperty("porta")] public string Porta { get; set; }
        [JsonProperty("dupla_validacao")] public bool? DuplaValidacao { get; set; }
        [JsonProperty("timeout_dupla_validacao")] public int? TimeoutDuplaValidacao { get; set; }
        [JsonProperty("creditos_consumidos")] public int? CreditosConsumidos { get; set; }
        [JsonProperty("fluxo")] public string Fluxo { get; set; } // Ex: "Entrada", "Saida", "Bidirecional"
        [JsonProperty("grupo_de_acesso")] public string GrupoDeAcesso { get; set; }
        [JsonProperty("activation_time")] public int? ActivationTime { get; set; }
        [JsonProperty("nome_customizado")] public string NomeCustomizado { get; set; }
    }
}