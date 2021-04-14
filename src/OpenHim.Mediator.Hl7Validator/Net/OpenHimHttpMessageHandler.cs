using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Net
{
    public class OpenHimHttpMessageHandler : DelegatingHandler
    {
        public OpenHimHttpMessageHandler(IOptions<MediatorConfig> mediatorConfig)
        {
            if (mediatorConfig.Value.OpenHimAuth.TrustSelfSigned)
            {
            }
        }
    }
}
