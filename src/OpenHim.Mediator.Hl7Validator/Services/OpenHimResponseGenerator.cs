using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;
using OpenHim.Mediator.Hl7Validator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Services
{
    public class OpenHimResponseGenerator : IOpenHimResponseGenerator
    {
        private MediatorConfig _mediatorConfig;

        public OpenHimResponseGenerator(IOptions<MediatorConfig> mediatorConfigOptions)
        {
            _mediatorConfig = mediatorConfigOptions.Value ?? throw new ArgumentException($"${mediatorConfigOptions} does not container a MediatorConfig instance");
        }

        public Task<OpenHimResponse> PrimaryResponse(string body)
        {
            var openHimResponse = new OpenHimResponse
            {
                MediatorUrn = _mediatorConfig.MediatorSetup.Urn,
                Status = "Success",
                Response = new Response
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/hl7-v2" }
                    },
                    Body = body,
                    Timestamp = DateTime.UtcNow.ToString("s")
                },
                Orchestrations = null,
                Properties = null
            };

            return Task.FromResult(openHimResponse);
        }
    }
}
