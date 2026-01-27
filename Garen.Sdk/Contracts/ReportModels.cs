using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    public class ReportIpModel
    {
        [JsonProperty("ip")] public string Ip { get; set; }
    }
}