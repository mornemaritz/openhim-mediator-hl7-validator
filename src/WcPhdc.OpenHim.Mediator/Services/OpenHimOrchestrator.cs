using Microsoft.Extensions.Options;
using WcPhdc.OpenHim.Mediator.Configuration;
using WcPhdc.OpenHim.Mediator.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WcPhdc.OpenHim.Mediator.Services
{
    public class OpenHimOrchestrator : IOpenHimOrchestrator
    {
        private readonly MediatorConfig _mediatorConfig;
        private readonly IHttpClientFactory _clientFactory;

        public OpenHimOrchestrator(IOptions<MediatorConfig> mediatorConfigOptions,
            IHttpClientFactory clientFactory)
        {
            _mediatorConfig = mediatorConfigOptions.Value ?? throw new ArgumentException($"{nameof(mediatorConfigOptions)} does not contain a value of type {nameof(MediatorConfig)}");
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        // Don't like this method name. Thinking of a better one...
        public async Task<OpenHimResponse> Do(string requestContent, Response primaryOpenHimConsumerResponse, bool primaryOperationSuccessful = true)
        {
            var openHimResponse = new OpenHimResponse
            {
                MediatorUrn = _mediatorConfig.MediatorSetup.Urn,
                Status = primaryOperationSuccessful ? "Success" : "Completed with error(s)",
                Response = primaryOpenHimConsumerResponse,
                Orchestrations = null,
                Properties = null
            };

            if (!primaryOperationSuccessful || !_mediatorConfig.HasOrchestrations())
                return openHimResponse;

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_mediatorConfig.OpenHimAuth.ApiClientName}:{_mediatorConfig.OpenHimAuth.ApiClientPassword}"));

            foreach (var orchestration in _mediatorConfig.Orchestrations)
            {
                var orchestrationClient = _clientFactory.CreateClient(orchestration.Name);
                orchestrationClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                orchestrationClient.BaseAddress = new Uri(orchestration.Request.Host);

                var request = new HttpRequestMessage(new HttpMethod(orchestration.Request.Method),$"{orchestration.Request.Path}?{orchestration.Request.Querystring}")
                {
                    Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
                };

                foreach (var header in orchestration.Request.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var response = await orchestrationClient.SendAsync(request);

                openHimResponse.AddOrchestration(
                        new Orchestration
                        {
                            Name = orchestration.Name,
                            Request = new Request
                            {
                                Host = orchestration.Request.Host,
                                Path = orchestration.Request.Path,
                                Querystring = orchestration.Request.Querystring,
                                Headers = orchestration.Request.Headers,
                                Body = requestContent,
                                Method = orchestration.Request.Method,
                                Timestamp = DateTime.UtcNow.ToString("s")
                            },
                            Response = new Response
                            {
                                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value)),
                                Body = await response.Content.ReadAsStringAsync(),
                                Status = (short)response.StatusCode,
                                Timestamp = DateTime.UtcNow.ToString("s")
                            }
                        });
            }

            return openHimResponse;
        }
    }
}
