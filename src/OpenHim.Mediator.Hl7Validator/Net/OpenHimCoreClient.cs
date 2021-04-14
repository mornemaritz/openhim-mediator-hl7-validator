using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Net
{
    public class OpenHimCoreClient : IOpenHimCoreClient
    {
        private readonly HttpClient _httpClient;
        private readonly MediatorConfig _mediatorConfig;

        public OpenHimCoreClient(HttpClient httpClient, IOptions<MediatorConfig> mediatorConfig)
        {
            _mediatorConfig = mediatorConfig.Value ?? throw new ArgumentNullException(nameof(mediatorConfig));

            httpClient.BaseAddress = new Uri(_mediatorConfig.MediatorCore.OpenHimCoreHost);

            _httpClient = httpClient;
        }

        public async Task RegisterAsync(CancellationToken cancellationToken)
        {
            var mediatorConfigStringContent = await AuthenticatedContentRequest(JsonSerializer.Serialize(_mediatorConfig.MediatorSetup), cancellationToken);

            var registrationResponse = await _httpClient.PostAsync(_mediatorConfig.MediatorCore.OpenHimRegisterMediatorPath, mediatorConfigStringContent, cancellationToken);

            registrationResponse.EnsureSuccessStatusCode();
        }

        public async void PingOpenHim(object state)
        {
            var uptimeStringContent = await AuthenticatedContentRequest(JsonSerializer.Serialize(new { uptime = DateTime.UtcNow.Ticks }), CancellationToken.None);

            var heartbeatResponse = await _httpClient.PostAsync($"{_mediatorConfig.MediatorCore.OpenHimRegisterMediatorPath}/{_mediatorConfig.MediatorSetup.Urn}/{_mediatorConfig.MediatorCore.OpenHimHeartbeatPath}", uptimeStringContent, CancellationToken.None);

            heartbeatResponse.EnsureSuccessStatusCode();
        }

        private async Task<string> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var authenticationResponse = await _httpClient.GetAsync($"{_mediatorConfig.MediatorCore.OpenHimCoreAuthPath}/{_mediatorConfig.OpenHimAuth.Username}", cancellationToken);

            authenticationResponse.EnsureSuccessStatusCode();

            var authResult = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await authenticationResponse.Content.ReadAsStreamAsync(cancellationToken), default, cancellationToken);

            return authResult["salt"];
        }

        private async Task<HttpContent> AuthenticatedContentRequest(string content, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.ToString("s");
            var mySalt = Guid.NewGuid().ToString();

            var theirSalt = await AuthenticateAsync(cancellationToken);

            var sha512 = SHA512.Create();

            var passwordBytes = sha512.ComputeHash(Encoding.Default.GetBytes($"{theirSalt}{_mediatorConfig.OpenHimAuth.Password}"));
            var passwordHexHash = string.Join(string.Empty, passwordBytes.Select(b => $"{b:x2}"));

            var authTokenBytes = sha512.ComputeHash(Encoding.Default.GetBytes($"{passwordHexHash}{mySalt}{now}"));
            var authTokenHex = string.Join(string.Empty, authTokenBytes.Select(b => $"{b:x2}"));

            var stringContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json");

            stringContent.Headers.Add("auth-username", _mediatorConfig.OpenHimAuth.Username);
            stringContent.Headers.Add("auth-ts", now);
            stringContent.Headers.Add("auth-salt", mySalt);
            stringContent.Headers.Add("auth-token", authTokenHex);

            return stringContent;
        }
    }
}
