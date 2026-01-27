namespace Garen.Sdk.Contracts
{
    /// <summary>
    /// Códigos de retorno padronizados da API Garen.
    /// </summary>
    public enum GarenReturnCode
    {
        /// <summary>0 - Operação realizada com sucesso.</summary>
        Sucesso = 0,

        /// <summary>1 - Já existe uma regra com este nome.</summary>
        RegraNomeDuplicado = 1,

        /// <summary>2 - Campos do JSON incompletos.</summary>
        CamposIncompletos = 2,

        /// <summary>3 - Comando inválido.</summary>
        ComandoInvalido = 3,

        /// <summary>4 - JSON inválido.</summary>
        JsonInvalido = 4,

        /// <summary>5 - Sem conteúdo no banco.</summary>
        SemConteudo = 5,

        /// <summary>6 - Falha ao cadastrar usuário.</summary>
        FalhaCadastroUsuario = 6,

        /// <summary>7 - Regra de porta não cadastrada.</summary>
        RegraPortaNaoEncontrada = 7,

        /// <summary>8 - Regra de tempo não cadastrada.</summary>
        RegraTempoNaoEncontrada = 8,

        /// <summary>9 - Authorization token não informado.</summary>
        TokenNaoInformado = 9,

        /// <summary>10 - Já existe um grupo vinculado a essa porta.</summary>
        GrupoPortaDuplicado = 10,

        /// <summary>11 - Authorization token invalido.</summary>
        TokenInvalido = 11,

        /// <summary>12 - Já existe este codigo de acesso.</summary>
        CodigoAcessoDuplicado = 12,

        /// <summary>13 - Configuração de família não encontrada.</summary>
        FamiliaNaoEncontrada = 13,

        /// <summary>14 - Não existem vagas livres sobrando.</summary>
        SemVagasLivres = 14,

        /// <summary>15 - Família usou mais vagas que o permitido.</summary>
        ExcessoVagasFamilia = 15
    }
}