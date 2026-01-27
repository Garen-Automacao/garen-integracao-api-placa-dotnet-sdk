using Newtonsoft.Json;
using System.Collections.Generic;

namespace Garen.Sdk.Contracts
{
    public class GenericResponse
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("codigo")] public GarenReturnCode Codigo { get; set; }

        /// <summary>
        /// Retorna a descrição legível do erro baseada no código.
        /// (Propriedade auxiliar, não vem do JSON da API).
        /// </summary>
        [JsonIgnore] // Não tenta ler do JSON
        public string Descricao
        {
            get
            {
                switch (Codigo)
                {
                    case GarenReturnCode.Sucesso: return "Sucesso";
                    case GarenReturnCode.RegraNomeDuplicado: return "Já existe uma regra com este nome.";
                    case GarenReturnCode.CamposIncompletos: return "Campos do JSON incompletos.";
                    case GarenReturnCode.ComandoInvalido: return "Comando inválido.";
                    case GarenReturnCode.JsonInvalido: return "JSON inválido.";
                    case GarenReturnCode.SemConteudo: return "Sem conteúdo no banco.";
                    case GarenReturnCode.FalhaCadastroUsuario: return "Falha ao cadastrar usuário.";
                    case GarenReturnCode.RegraPortaNaoEncontrada: return "Regra de porta não cadastrada.";
                    case GarenReturnCode.RegraTempoNaoEncontrada: return "Regra de tempo não cadastrada.";
                    case GarenReturnCode.TokenNaoInformado: return "Authorization token não informado.";
                    case GarenReturnCode.GrupoPortaDuplicado: return "Já existe um grupo vinculado a essa porta.";
                    case GarenReturnCode.TokenInvalido: return "Authorization token inválido.";
                    case GarenReturnCode.CodigoAcessoDuplicado: return "Já existe este código de acesso.";
                    case GarenReturnCode.FamiliaNaoEncontrada: return "Configuração de família não encontrada.";
                    case GarenReturnCode.SemVagasLivres: return "Não existem vagas livres sobrando.";
                    case GarenReturnCode.ExcessoVagasFamilia: return "Família usou mais vagas que o permitido.";
                    default: return "Código de erro desconhecido.";
                }
            }
        }
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
        [JsonProperty("id_usuario", NullValueHandling = NullValueHandling.Ignore)]
        public int? IdUsuario { get; set; }

        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }
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
        [JsonProperty("date")] 
        public string DateString { get; set; }

        [JsonProperty("epoch")] 
        public double Epoch { get; set; } 
    }

    public class DateResponse
    {
        [JsonProperty("status")] 
        public string Status { get; set; }

        [JsonProperty("codigo")] 
        public GarenReturnCode Codigo { get; set; }

        [JsonProperty("detalhes")] 
        public DateModel Detalhes { get; set; } 
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

    public class ScheduleRule : ScheduleRuleWithName
    {
    }

    public class RuleName
    {
        [JsonProperty("id")] public int Id { get; set; }
    }

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
    
    public class UpdateCreditModel
    {
        [JsonProperty("id_usuario")] public int IdUsuario { get; set; }
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)] public string Hash { get; set; }
        [JsonProperty("quantidade")] public int Quantidade { get; set; }
    }

    public class ImageModel
    {
        [JsonProperty("imagem")] public string Imagem { get; set; }
    }

    public class GenericFacialResponse
    {
        [JsonProperty("facialA")] public string FacialA { get; set; }
        [JsonProperty("facialB")] public string FacialB { get; set; }
    }
}