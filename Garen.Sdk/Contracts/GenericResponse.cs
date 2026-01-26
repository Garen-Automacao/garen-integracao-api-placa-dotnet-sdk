using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    public class GenericResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("codigo")]
        public int Codigo { get; set; }
    }
}