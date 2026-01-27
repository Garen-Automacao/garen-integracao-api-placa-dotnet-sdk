using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    public class FamilyConfigModel
    {
        [JsonProperty("obrigatorio")] public bool? Obrigatorio { get; set; }
        [JsonProperty("vagas_totais")] public int? VagasTotais { get; set; }
        [JsonProperty("vagas_livres_usadas")] public int? VagasLivresUsadas { get; set; }
    }

    public class FamilyConfigSelectModel
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public int Codigo { get; set; }
        [JsonProperty("detalhes")] public List<FamilyConfigDetail> Detalhes { get; set; }
    }

    public class FamilyConfigDetail
    {
        [JsonProperty("obrigatorio")] public int Obrigatorio { get; set; }
        [JsonProperty("vagas_moradores_ocupadas")] public int VagasMoradoresOcupadas { get; set; }
        [JsonProperty("vagas_moradores_livres")] public int VagasMoradoresLivres { get; set; }
        [JsonProperty("vagas_visitantes_ocupadas")] public int VagasVisitantesOcupadas { get; set; }
        [JsonProperty("vagas_visitantes_livres")] public int VagasVisitantesLivres { get; set; }
    }

    public class FamilyModel
    {
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("vagas")] public int? Vagas { get; set; }
        [JsonProperty("vagas_usadas")] public int? VagasUsadas { get; set; }
    }

    public class FamilySelectModel
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public int Codigo { get; set; }
        [JsonProperty("detalhes")] public List<FamilyDetail> Detalhes { get; set; }
    }

    public class FamilyDetail
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("vagas")] public int Vagas { get; set; }
        [JsonProperty("vagas_usadas")] public int VagasUsadas { get; set; }
    }
}