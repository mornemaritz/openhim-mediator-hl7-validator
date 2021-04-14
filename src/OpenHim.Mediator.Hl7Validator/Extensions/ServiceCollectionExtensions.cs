using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;
using OpenHim.Mediator.Hl7Validator.Net;
using OpenHim.Mediator.Hl7Validator.Services;
using System;
using System.Net.Http;
using System.Net.Security;

namespace OpenHim.Mediator.Hl7Validator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenHimMediator(this IServiceCollection services, IConfiguration mediatorConfigurationSection)
        {
            services.AddOptions();

            services.Configure<MediatorConfig>(mediatorConfigurationSection);

            var clientBuilder = services.AddHttpClient<IOpenHimCoreClient, OpenHimCoreClient>();

            _ = bool.TryParse(mediatorConfigurationSection["openHimAuth:trustSelfSigned"], out bool trustSelfSignedCertificate);

            if (trustSelfSignedCertificate)
            {
                clientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                        {
                            //var certificateAccepted = errors == SslPolicyErrors.None || errors  == SslPolicyErrors.RemoteCertificateChainErrors;

                            return true;// certificateAccepted;
                        }
                    };

                    return handler;
                });
            }

            services.AddHostedService<OpenHimRegistrationService>();

            _ = bool.TryParse(mediatorConfigurationSection["mediatorCore:heartbeatEnabled"], out bool heartbeatEnabled);

            if (heartbeatEnabled)
            {
                services.AddHostedService<OpenHimHeartbeatService>();
            }

            return services;
        }
    }
}
