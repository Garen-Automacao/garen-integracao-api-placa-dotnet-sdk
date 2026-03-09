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

                // 1. Configure Polly Circuit Breaker
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

                // 2. Configure HttpClient
                var httpClient = new HttpClient(new ApiHandler(_circuitBreakerPolicy))
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromSeconds(5)
                };
                
                // Clear headers and add raw token
                httpClient.DefaultRequestHeaders.Authorization = null;
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);

                // 3. Global JSON configuration for Refit 5.2.4
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                var refitSettings = new RefitSettings
                {
                    ContentSerializer = new JsonContentSerializer(jsonSettings)
                };

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