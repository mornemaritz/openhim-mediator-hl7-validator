using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WcPhdc.OpenHim.Mediator.Configuration;

namespace WcPhdc.OpenHim.Mediator.Net
{
    public class OpenHimCoreClient : IOpenHimCoreClient
    {
        private readonly HttpClient _openHimCoreHttpClient;
        private readonly MediatorConfig _mediatorConfig;
        private readonly ILogger<OpenHimCoreClient> _logger;

        public OpenHimCoreClient(HttpClient httpClient, IOptions<MediatorConfig> mediatorConfig, ILogger<OpenHimCoreClient> logger)
        {
            _mediatorConfig = mediatorConfig.Value ?? throw new ArgumentNullException(nameof(mediatorConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var openHimCoreHost = _mediatorConfig.MediatorCore.OpenHimCoreHost ?? throw new ArgumentException($"{nameof(mediatorConfig)} has no value for MediatorCore.OpenHimCoreHost");

            httpClient.BaseAddress = new Uri(openHimCoreHost);

            _openHimCoreHttpClient = httpClient;
        }

        public async Task RegisterAsync(CancellationToken cancellationToken)
        {
            var mediatorConfigStringContent = await AuthenticatedContentRequest(JsonSerializer.Serialize(_mediatorConfig.MediatorSetup), cancellationToken);

            var registrationResponse = await _openHimCoreHttpClient.PostAsync(_mediatorConfig.MediatorCore.OpenHimRegisterMediatorPath, mediatorConfigStringContent, cancellationToken);

            await ValidateResponse(registrationResponse, mediatorConfigStringContent.Headers.GetValues("auth-ts").FirstOrDefault());
        }

        public async void PingOpenHim(object state)
        {
            var uptimeStringContent = await AuthenticatedContentRequest(JsonSerializer.Serialize(new { uptime = DateTime.UtcNow.Ticks }), CancellationToken.None);

            var heartbeatResponse = await _openHimCoreHttpClient.PostAsync($"{_mediatorConfig.MediatorCore.OpenHimRegisterMediatorPath}/{_mediatorConfig.MediatorSetup.Urn}/{_mediatorConfig.MediatorCore.OpenHimHeartbeatPath}", uptimeStringContent, CancellationToken.None);

            await ValidateResponse(heartbeatResponse, uptimeStringContent.Headers.GetValues("auth-ts").FirstOrDefault());
        }

        private async Task<string> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var authenticationResponse = await _openHimCoreHttpClient.GetAsync($"{_mediatorConfig.MediatorCore.OpenHimCoreAuthPath}/{_mediatorConfig.OpenHimAuth.CoreUsername}", cancellationToken);

            await ValidateResponse(authenticationResponse);

            var authResult = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await authenticationResponse.Content.ReadAsStreamAsync(), default, cancellationToken);

            return authResult["salt"];
        }

        private async Task ValidateResponse(HttpResponseMessage httpResponse, string authTimestamp = default)
        {
            if (!_mediatorConfig.OpenHimAuth.IgnoreOutgoingOpenHimAuthFailures)
			{
				httpResponse.EnsureSuccessStatusCode();
			}
			else if (!httpResponse.IsSuccessStatusCode)
            {
                var passwordLength = _mediatorConfig.OpenHimAuth.CorePassword?.Length;
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                _logger.LogWarning($"Auth Failure: {responseContent}. auth-ts: {authTimestamp}. passwordLength: {passwordLength}. 'IgnoreOutgoingOpenHimAuthFailures' set to true. Ignoring");
            }
        }

        private async Task<HttpContent> AuthenticatedContentRequest(string content, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow.ToString("s");
            var mySalt = Guid.NewGuid().ToString();

            var theirSalt = await AuthenticateAsync(cancellationToken);

            var sha512 = SHA512.Create();

            var passwordBytes = sha512.ComputeHash(Encoding.Default.GetBytes($"{theirSalt}{_mediatorConfig.OpenHimAuth.CorePassword}"));
            var passwordHexHash = string.Join(string.Empty, passwordBytes.Select(b => $"{b:x2}"));

            var authTokenBytes = sha512.ComputeHash(Encoding.Default.GetBytes($"{passwordHexHash}{mySalt}{now}"));
            var authTokenHex = string.Join(string.Empty, authTokenBytes.Select(b => $"{b:x2}"));

            var stringContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json");

            stringContent.Headers.Add("auth-username", _mediatorConfig.OpenHimAuth.CoreUsername);
            stringContent.Headers.Add("auth-ts", now);
            stringContent.Headers.Add("auth-salt", mySalt);
            stringContent.Headers.Add("auth-token", authTokenHex);

            return stringContent;
        }
    }
}
