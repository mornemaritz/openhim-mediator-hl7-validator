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
using NHapi.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace OpenHim.Mediator.Hl7Validator.Controllers
{
    [ApiController]
    [Route("api/hl7-validation-requests")]
    public class Hl7ValidationRequestsController : ControllerBase
    {
        private readonly ILogger<Hl7ValidationRequestsController> _logger;
        private readonly Hl7Config hl7Config;

        public Hl7ValidationRequestsController(IOptions<Hl7Config> hl7ConfigOptions, ILogger<Hl7ValidationRequestsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            hl7Config = hl7ConfigOptions.Value ?? throw new ArgumentNullException(nameof(hl7ConfigOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var hl7MessageString = string.Empty;
            IMessage hl7Message;
            HL7Exception hl7Exception = null;

            // TODO: Custom formatter bound to content-type=application/hl7-v2
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-5.0
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                hl7MessageString = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(hl7MessageString))
                return BadRequest("Request body may not be null or empty");

            var pipeParser = new PipeParser { ValidationContext = new StrictValidation() };
            var ack = new Ack(hl7Config.Application, hl7Config.Facility);
            IMessage ackMessage = null;

            try
            {
                hl7Message = await Task.Run(() => pipeParser.Parse(hl7MessageString));

                ackMessage = ack.MakeACK(hl7Message);
            }
            catch (HL7Exception hex)
            {
                _logger.LogError(hex, "HL7Exception encountered in Post");

                hl7Exception = hex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Exception encountered in Post");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (hl7Exception != null)
            {
                try
                {
                    // This double try-catch block is a bit hacky (better than nested try-catch though).
                    // We need a parsed message to be able to easily generate an Ack message, so
                    // let's take another bite at attempting to parse the message, this time just the header (1st line) which is
                    // all we need to generate a valid Ack message.
                    hl7Message = await Task.Run(() => pipeParser.Parse(hl7MessageString.Split(Environment.NewLine).First()));

                    ackMessage = ack.MakeACK(hl7Message, AckTypes.AE, hl7Exception.Message);
                }
                catch (HL7Exception hex)
                {
                    // Oh well, tried hard to return a valid ack message. ¯\_(ツ)_/¯
                    // Nothing to do now but return a BadRequest.
                    // At least we're including the HL7 parse error message.
                    return BadRequest(hex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected Exception encountered in Post");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

            return Ok(pipeParser.Encode(ackMessage));
        }
    }
}