using System;
using System.Net.Http;
using System.Threading.Tasks;
using Garen.Sdk.Clients;
using Polly;
using Polly.CircuitBreaker;
using Refit;

namespace Garen.Sdk.Infrastructure
{
    public static class GarenApiFactory
    {
        private static IGarenApiClient _instance;
        private static readonly object _lock = new object();
        
        // Circuit Breaker state to protect the client UI from freezing when server is down
        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        /// <summary>
        /// Initializes the Singleton instance. Call this once at Program.cs.
        /// </summary>
        /// <param name="baseUrl">The API URL (e.g., http://192.168.0.100:5000)</param>
        /// <param name="token">The fixed authentication token</param>
        public static void Initialize(string baseUrl, string token)
        {
            if (_instance != null) return;

            lock (_lock)
            {
                if (_instance != null) return;

                // 1. Define Policy: "Fail Fast" logic
                // If 2 consecutive errors occur (server down), stop trying for 30 seconds.
                // This prevents the WinForms UI from locking up on repeated timeouts.
                _circuitBreakerPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<HttpRequestException>()
                    .Or<Exception>() // Catch-all for network layers
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 2, 
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, timespan) => 
                        {
                            // Optional: Log here that the circuit is open (Server likely down)
                            System.Diagnostics.Debug.WriteLine($"Circuit Broken! Server unreachable. Pausing for {timespan.TotalSeconds}s.");
                        },
                        onReset: () => System.Diagnostics.Debug.WriteLine("Circuit Reset! Trying again.")
                    );

                // 2. Configure HttpClient with short timeout
                var httpClient = new HttpClient(new ApiHandler(_circuitBreakerPolicy))
                {
                    BaseAddress = new Uri(baseUrl),
                    // IMPORTANT: Short timeout. If server is down, fail in 5s, don't wait 100s.
                    Timeout = TimeSpan.FromSeconds(5) 
                };

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // 3. Configure Refit with Newtonsoft.Json (Legacy compatibility)
                var refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer());

                _instance = RestService.For<IGarenApiClient>(httpClient, refitSettings);
            }
        }

        public static IGarenApiClient Client
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("You must call GarenApiFactory.Initialize() before using the Client.");
                
                return _instance;
            }
        }

        // Internal Handler to inject Polly into the HTTP pipeline
        private class ApiHandler : DelegatingHandler
        {
            private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _policy;

            public ApiHandler(AsyncCircuitBreakerPolicy<HttpResponseMessage> policy)
            {
                InnerHandler = new HttpClientHandler();
                _policy = policy;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                // Execute the request inside the Circuit Breaker
                return await _policy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
            }
        }
    }
}