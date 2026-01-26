using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    // --- USUÁRIO ---
    public class UserRegisterModel
    {
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("tipo_cadastro")] public int TipoCadastro { get; set; }
        [JsonProperty("validade")] public int Validade { get; set; }
        [JsonProperty("beginning_validity")] public int InicioValidade { get; set; } // Ajustado camelCase do JSON
        [JsonProperty("ignoreDoubleValidation")] public int IgnorarDuplaValidacao { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }
        [JsonProperty("creditos")] public int Creditos { get; set; }
        [JsonProperty("data_inicio")] public int DataInicio { get; set; }
        [JsonProperty("data_fim")] public int DataFim { get; set; }
        [JsonProperty("id_familia")] public int IdFamilia { get; set; }
        [JsonProperty("portas")] public List<string> Portas { get; set; }
        [JsonProperty("imagem")] public string ImagemBase64 { get; set; }
        [JsonProperty("codigo_acesso")] public List<AccessCodeUserModel> CodigoAcesso { get; set; }
        [JsonProperty("grupo_de_acesso")] public List<string> GrupoDeAcesso { get; set; }
    }

    public class UserUpdateModel : UserRegisterModel { } // Geralmente similar ao register

    public class UserIdModel
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
    }

    public class AccessCodeUserModel
    {
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }
        [JsonProperty("comeco")] public int Comeco { get; set; }
        [JsonProperty("fim")] public int Fim { get; set; }
        [JsonProperty("regra_de_tempo")] public string RegraDeTempo { get; set; }
        [JsonProperty("digito_verificador")] public string DigitoVerificador { get; set; }
        [JsonProperty("id_regra_de_pgm")] public int IdRegraPgm { get; set; }
    }

    // --- GRUPOS ---
    public class GroupModel
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("portas")] public List<string> Portas { get; set; }
        [JsonProperty("regras_de_tempo")] public List<string> RegrasTempo { get; set; }
        [JsonProperty("ids_regra_de_tempo")] public List<string> IdsRegrasTempo { get; set; }
        [JsonProperty("limite")] public int Limite { get; set; }
        [JsonProperty("grupos")] public List<int> Grupos { get; set; }
    }

    public class GroupUserModel
    {
        [JsonProperty("id_grupo")] public string IdGrupo { get; set; }
        [JsonProperty("id_usuario")] public int IdUsuario { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
    }
}