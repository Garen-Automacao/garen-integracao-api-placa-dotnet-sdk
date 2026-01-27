using System.Collections.Generic;
using Newtonsoft.Json;

namespace Garen.Sdk.Contracts
{
    // --- USUÁRIO ---
    public class UserRegisterModel
    {
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("tipo_cadastro")] public int? TipoCadastro { get; set; }
        [JsonProperty("validade")] public int? Validade { get; set; }
        [JsonProperty("beginning_validity")] public int? InicioValidade { get; set; }

        [JsonProperty("ignoreDoubleValidation")]
        public int? IgnorarDuplaValidacao { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }

        [JsonProperty("codigo", NullValueHandling = NullValueHandling.Ignore)]
        public string Codigo { get; set; }

        [JsonProperty("tipo", NullValueHandling = NullValueHandling.Ignore)]
        public string Tipo { get; set; }

        [JsonProperty("creditos")] public int? Creditos { get; set; }
        [JsonProperty("data_inicio")] public int? DataInicio { get; set; }
        [JsonProperty("data_fim")] public int? DataFim { get; set; }

        [JsonProperty("id_familia", NullValueHandling = NullValueHandling.Ignore)]
        public int? IdFamilia { get; set; }

        [JsonProperty("portas")] public List<string> Portas { get; set; }

        [JsonProperty("imagem", NullValueHandling = NullValueHandling.Ignore)]
        public string ImagemBase64 { get; set; }

        [JsonProperty("codigo_acesso")] public List<AccessCodeUserModel> CodigoAcesso { get; set; }
        [JsonProperty("grupo_de_acesso")] public List<string> GrupoDeAcesso { get; set; }
    }

    /// <summary>
    /// Modelo para Atualização Parcial.
    /// Inclui campos para atualizar regras de tempo específicas do usuário.
    /// </summary>
    public class UserUpdateModel
    {
        [JsonProperty("nome", NullValueHandling = NullValueHandling.Ignore)]
        public string Nome { get; set; }

        [JsonProperty("validity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Validade { get; set; }

        [JsonProperty("beginning_validity", NullValueHandling = NullValueHandling.Ignore)]
        public int? InicioValidade { get; set; }

        [JsonProperty("ignoreDoubleValidation", NullValueHandling = NullValueHandling.Ignore)]
        public int? IgnorarDuplaValidacao { get; set; }

        [JsonProperty("imagem", NullValueHandling = NullValueHandling.Ignore)]
        public string ImagemBase64 { get; set; }

        [JsonProperty("tipo_cadastro", NullValueHandling = NullValueHandling.Ignore)]
        public int? TipoCadastro { get; set; }

        [JsonProperty("creditos", NullValueHandling = NullValueHandling.Ignore)]
        public int? Creditos { get; set; }

        [JsonProperty("id_familia", NullValueHandling = NullValueHandling.Ignore)]
        public int? IdFamilia { get; set; }

        [JsonProperty("grupo_de_acesso", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> GrupoDeAcesso { get; set; }

        [JsonProperty("portas", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Portas { get; set; }

        // Campos para atualização de regra de tempo inline
        [JsonProperty("inicio", NullValueHandling = NullValueHandling.Ignore)]
        public string Inicio { get; set; }

        [JsonProperty("fim", NullValueHandling = NullValueHandling.Ignore)]
        public string Fim { get; set; }

        [JsonProperty("dias", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> Dias { get; set; }

        [JsonProperty("gmt", NullValueHandling = NullValueHandling.Ignore)]
        public int? Gmt { get; set; }
    }

    public class UserIdModel
    {
        [JsonProperty("id")] public int? Id { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
    }

    public class AccessCodeUserModel
    {
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("tipo")] public string Tipo { get; set; }

        [JsonProperty("comeco", NullValueHandling = NullValueHandling.Ignore)]
        public int? Comeco { get; set; }

        [JsonProperty("fim", NullValueHandling = NullValueHandling.Ignore)]
        public int? Fim { get; set; }

        [JsonProperty("regra_de_tempo", NullValueHandling = NullValueHandling.Ignore)]
        public string RegraDeTempo { get; set; }

        [JsonProperty("digito_verificador", NullValueHandling = NullValueHandling.Ignore)]
        public string DigitoVerificador { get; set; }

        [JsonProperty("id_regra_de_pgm", NullValueHandling = NullValueHandling.Ignore)]
        public int? IdRegraPgm { get; set; }
    }

    public class GroupModel
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("portas")] public List<string> Portas { get; set; }
        [JsonProperty("regras_de_tempo")] public List<string> RegrasTempo { get; set; }
        [JsonProperty("ids_regra_de_tempo")] public List<string> IdsRegrasTempo { get; set; }
        [JsonProperty("limite")] public int Limite { get; set; }
        [JsonProperty("grupos")] public List<int> Grupos { get; set; }
    }

    public class GroupUpdateModel : GroupModel
    {
        [JsonProperty("quantidade", NullValueHandling = NullValueHandling.Ignore)]
        public int? Quantidade { get; set; }
    }

    public class GroupUserModel
    {
        [JsonProperty("id_grupo")] public string IdGrupo { get; set; }

        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)]
        public int? IdUsuario { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }
    }

    public class UserSelectModelAll
    {
        [JsonProperty("detalhes")] public List<UserModelSimple> Detalhes { get; set; }
    }

    public class UserModelSimple
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("creditos")] public int? Creditos { get; set; }
        [JsonProperty("photo")] public bool Photo { get; set; }
        [JsonProperty("expirado")] public bool Expirado { get; set; }
        [JsonProperty("fluxo")] public string Fluxo { get; set; }
    }

    public class UserSelectModel
    {
        [JsonProperty("detalhes")] public UserRegisterModel Detalhes { get; set; }
    }

    public class UserQuantitySelectModel
    {
        [JsonProperty("detalhes")] public List<UserQuantityDetail> Detalhes { get; set; }
    }

    public class UserQuantityDetail
    {
        [JsonProperty("quantity")] public int Quantity { get; set; }
    }
}