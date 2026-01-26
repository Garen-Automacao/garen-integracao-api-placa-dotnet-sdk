using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    public class RemoteDoorTrigger
    {
        /// <summary>
        /// Número da porta (ex: 1).
        /// </summary>
        [JsonProperty("porta")]
        public int Porta { get; set; }

        /// <summary>
        /// 0 para desativar, 1 para ativar.
        /// </summary>
        [JsonProperty("auxiliar")]
        public int Auxiliar { get; set; }

        /// <summary>
        /// Tempo em milissegundos (ou conforme documentação da placa). 0 geralmente é o padrão.
        /// </summary>
        [JsonProperty("tempo")]
        public int Tempo { get; set; }
    }
}