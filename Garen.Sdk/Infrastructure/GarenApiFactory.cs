using System;
using System.Net.Http;
using System.Threading.Tasks;
using Garen.Sdk.Clients;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Refit;

namespace Garen.Sdk.Infrastructure
{
    public static class GarenApiFactory
    {
        private static IGarenApiClient _instance;
        private static readonly object _lock = new object();

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        public static void Initialize(string baseUrl, string token)
        {
            if (_instance != null) return;

            lock (_lock)
            {
                if (_instance != null) return;

                // 1. Configuração do Polly (Circuit Breaker)
                _circuitBreakerPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<HttpRequestException>()
                    .Or<Exception>()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 2,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, timespan) =>
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"Circuit Broken! Pausing for {timespan.TotalSeconds}s.");
                        },
                        onReset: () => System.Diagnostics.Debug.WriteLine("Circuit Reset!")
                    );

                // 2. Configuração do HttpClient
                var httpClient = new HttpClient(new ApiHandler(_circuitBreakerPolicy))
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromSeconds(5)
                };
                
                // Limpa cabeçalhos anteriores por segurança
                httpClient.DefaultRequestHeaders.Authorization = null;
                // Adiciona o token diretamente (ex: "seutoken123" em vez de "Bearer seutoken123")
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                // -----------------------------------------

                // 3. CONFIGURAÇÃO GLOBAL DO JSON  -----------------------
                var jsonSettings = new JsonSerializerSettings
                {
                    // Ignora propriedades nulas na serialização (Envio)
                    // Se você mandar um objeto com Nome="Teste" e Id=null, o JSON vai apenas {"nome": "Teste"}
                    NullValueHandling = NullValueHandling.Ignore,

                    // Ignora campos extras que a API retornar e você não mapeou (Evita erros futuros)
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                var refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer(jsonSettings));
                // ------------------------------------------------------------------------

                _instance = RestService.For<IGarenApiClient>(httpClient, refitSettings);
            }
        }

        public static IGarenApiClient Client
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "You must call GarenApiFactory.Initialize() before using the Client.");

                return _instance;
            }
        }

        private class ApiHandler : DelegatingHandler
        {
            private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _policy;

            public ApiHandler(AsyncCircuitBreakerPolicy<HttpResponseMessage> policy)
            {
                InnerHandler = new HttpClientHandler();
                _policy = policy;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                System.Threading.CancellationToken cancellationToken)
            {
                return await _policy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
            }
        }
    }
}