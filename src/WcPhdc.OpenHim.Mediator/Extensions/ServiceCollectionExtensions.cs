using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WcPhdc.OpenHim.Mediator.Configuration;
using WcPhdc.OpenHim.Mediator.Net;
using WcPhdc.OpenHim.Mediator.Services;

namespace WcPhdc.OpenHim.Mediator.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddOpenHimMediator(this IServiceCollection services, IConfiguration mediatorConfigurationSection)
		{
			services.AddOptions();

			services.Configure<MediatorConfig>(mediatorConfigurationSection);

			var clientBuilder = services.AddHttpClient<IOpenHimCoreClient, OpenHimCoreClient>();

			if (bool.TryParse(mediatorConfigurationSection["openHimAuth:trustSelfSigned"], out bool trustSelfSignedCertificate)
				&& trustSelfSignedCertificate)
			{
				clientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
				{
					var handler = new HttpClientHandler
					{
						ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
						{
							return true; // certificateAccepted;
						}
					};

					return handler;
				});
			}

			services.AddHostedService<OpenHimRegistrationService>();

			if (bool.TryParse(mediatorConfigurationSection["mediatorCore:heartbeatEnabled"], out bool heartbeatEnabled)
				&& heartbeatEnabled)
			{
				services.AddHostedService<OpenHimHeartbeatService>();
			}

			return services;
		}
	}
}
