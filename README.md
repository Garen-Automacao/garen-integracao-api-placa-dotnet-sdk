# Garen.Sdk (.NET Standard 2.0)

Biblioteca oficial de integração .NET para as placas Garen.
Desenvolvida para ser leve, resiliente e compatível com .NET Framework 4.6.2+ (Legacy Windows Forms) e .NET 5/6/8.

## 🚀 Instalação

### Opção A: Arquivo Local (.nupkg)
1. Baixe o arquivo `Garen.Sdk.1.0.0.nupkg`.
2. No Visual Studio/Rider, configure uma fonte de pacote local apontando para a pasta onde salvou o arquivo.
3. Instale via Gerenciador de Pacotes NuGet.

### Opção B: Referência Direta
Adicione o projeto `.csproj` à sua solução e referencie-o.

**Dependências Necessárias:**
- `Refit` (v6.3.2)
- `Refit.Newtonsoft.Json` (v6.3.2)
- `Polly` (v7.2.4)

---

## ⚡ Guia de Uso Rápido

### 1. Inicialização (Singleton)
A biblioteca gerencia a conexão HTTP para evitar exaustão de sockets. Inicialize **uma única vez** no `Program.cs` ou `Global.asax`.

```csharp
using Garen.Sdk.Infrastructure;

// Configure com o IP da placa e o Token
GarenApiFactory.Initialize("http://192.168.0.100:5000", "SEU_TOKEN_AQUI");
```

### 2. Exemplo: Abrir Portão (Comando 20)
**Importante:** Sempre use `await`. Nunca use `.Result` em Windows Forms para evitar travamentos.

```csharp
using Garen.Sdk.Contracts;

public async Task AbrirPortaoPrincipal()
{
    try 
    {
        var comando = new RemoteDoorTrigger 
        { 
            Porta = 1,    // Número da porta/relé
            Tempo = 0     // 0 = Padrão configurado na placa
        };

        // O Polly gerencia tentativas automáticas em caso de falha de rede
        var resposta = await GarenApiFactory.Client.OpenDoorAsync(comando);
        
        if (resposta.Status == "success" || resposta.Codigo == 200)
        {
            Console.WriteLine("Comando aceito pela placa!");
        }
        else
        {
            Console.WriteLine($"Erro na placa: {resposta.Status}");
        }
    }
    catch (Exception ex)
    {
        // Tratamento de erro de rede (Cabo desconectado, Placa desligada)
        // Graças ao Circuit Breaker, se a placa estiver desligada, 
        // o erro retorna rápido (5s) sem travar a tela.
        MessageBox.Show($"Erro de conexão: {ex.Message}");
    }
}
```

### 3. Exemplo: Listar Usuários

```csharp
var usuarios = await GarenApiFactory.Client.GetAllUsersAsync();
// A resposta é dinâmica ou tipada conforme o contrato
```

---

## 🛡️ Resiliência (Polly Circuit Breaker)

Esta SDK implementa proteção contra falhas de infraestrutura:

1.  **Timeout Curto (5s):** Se a placa não responder em 5 segundos, a requisição é cancelada para não deixar o usuário esperando.
2.  **Circuit Breaker:** Se houver **2 falhas consecutivas** (ex: placa sem energia), o sistema entra em modo de proteção por **30 segundos**.
    * Durante esses 30s, qualquer chamada falha imediatamente sem tentar conectar na rede.
    * Isso previne o travamento da interface gráfica (UI Freeze) em sistemas Windows Forms.

## 📦 Estrutura do Projeto

* **Contracts/**: DTOs (Data Transfer Objects) baseados no Swagger da Garen.
* **Clients/**: Interface `IGarenApiClient` com o mapeamento das rotas REST.
* **Infrastructure/**: `GarenApiFactory` contendo a configuração do `HttpClient` e políticas do Polly.
---

## 🧪 Como usar o Projeto de Teste (Garen.Tester)

Esta solução inclui um projeto Console Application chamado `Garen.Tester`. Ele serve como **exemplo de implementação** e ferramenta de **diagnóstico** para validar a comunicação com a placa antes de iniciar a integração no seu sistema principal.

### 1. Configuração
Abra o arquivo `appsettings.json` no projeto **Garen.Tester** e insira o IP da sua placa e o Token (se houver):

```json
{
  "GarenApi": {
    "BaseUrl": "http://192.168.0.88:5000",
    "Token": "SEU_TOKEN_AQUI"
  }
}
```

### 2. Executando
Defina o `Garen.Tester` como **Startup Project** e inicie o Debug. Um menu interativo será exibido no console permitindo testar:

* **[HARDWARE]**: Teste de acionamento de relés e portas.
* **[SISTEMA]**: Validação de versão de firmware, IP e Data/Hora.
* **[USUÁRIOS]**: CRUD completo (Criar, Listar, Excluir usuários).
* **[ACESSO]**: Gestão de credenciais (Cartão/Senha).
* **[EVENTOS]**: Leitura de logs de acesso em tempo real.