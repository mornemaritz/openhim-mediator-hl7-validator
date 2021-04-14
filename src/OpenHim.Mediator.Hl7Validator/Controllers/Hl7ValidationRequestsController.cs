using Microsoft.AspNetCore.Mvc;
using NHapi.Base.Model;
using NHapiTools.Base.Util;
using NHapi.Base.Parser;
using NHapi.Base.validation.impl;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;

namespace OpenHim.Mediator.Hl7Validator.Controllers
{
    [ApiController]
    [Route("api/hl7-validation-requests")]
    public class Hl7ValidationRequestsController : ControllerBase
    {
        private readonly Hl7Config hl7Config;

        public Hl7ValidationRequestsController(IOptions<Hl7Config> hl7ConfigOptions)
        {
            hl7Config = hl7ConfigOptions.Value ?? throw new ArgumentNullException(nameof(hl7ConfigOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var hl7MessageString = string.Empty;
            IMessage hl7Message;

            // TODO: Custom formatter bound to content-type=application/hl7-v2
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-5.0
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                hl7MessageString = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(hl7MessageString))
                return BadRequest("Request body may not be null or empty");

            var pipeParser = new PipeParser { ValidationContext = new StrictValidation() };

            try
            {
                hl7Message = await Task.Run(() => pipeParser.Parse(hl7MessageString));
            }
            catch (Exception ex)
            {
                // How do I generate an Ack message from scratch from an
                // unparsed message string, i.e., without the convenience of
                // ack.MakeACK(IMessage)
                return BadRequest(ex.Message);
            }

            var ack = new Ack(hl7Config.Application, hl7Config.Facility);
            var ackMessage = ack.MakeACK(hl7Message);

            return Ok(pipeParser.Encode(ackMessage));
        }
    }
}