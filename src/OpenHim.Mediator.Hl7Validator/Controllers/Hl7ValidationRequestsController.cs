using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenHim.Mediator.Hl7Validator.Services;
using OpenHim.Mediator.Hl7Validator.Extensions;
using WcPhdc.OpenHim.Mediator.Extensions;
using WcPhdc.OpenHim.Mediator.Services;
using System.Net;

namespace OpenHim.Mediator.Hl7Validator.Controllers
{
    [ApiController]
    [Route("api/hl7-validation-requests")]
    public class Hl7ValidationRequestsController : ControllerBase
    {
        private readonly IHL7MessageProcessor _hl7MessageProcessor;
        private readonly IOpenHimOrchestrator _orchestrator;
        private readonly ILogger<Hl7ValidationRequestsController> _logger;

        public Hl7ValidationRequestsController(IHL7MessageProcessor hl7MessageProcessor,
            IOpenHimOrchestrator orchestrator,
            ILogger<Hl7ValidationRequestsController> logger)
        {
            _hl7MessageProcessor = hl7MessageProcessor ?? throw new ArgumentNullException(nameof(hl7MessageProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var hl7MessageString = string.Empty;

            // TODO: Custom formatter bound to content-type=application/hl7-v2
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-5.0
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                hl7MessageString = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(hl7MessageString))
                return BadRequest("Request body may not be null or empty");

            try
            {
                var encodedAck = await _hl7MessageProcessor.ParseAndReturnEncodedAck(hl7MessageString);

                // Nothing in the ODS PIX Feed spec (it seems) that outlines expected http status codes.
                // My speculation is, that the expectation of receiving a higher level HL7 Ack message implies
                // that anything but a success response (even in the case of HL7 related errors)
                // may prevent the Ack message from passing through the lower HTTP layers.
                var openHimConsumerResponse = encodedAck.ToOpenHimConsumerResponse(HttpStatusCode.OK);

                var openHimResponse = await _orchestrator.Do(hl7MessageString, openHimConsumerResponse, encodedAck.IsHL7ApplicationAcceptAck());

                Response.Headers.Add("Content-Type", "application/json+openhim");

                return Ok(openHimResponse);
            }
            catch (HL7Exception hex)
            {
                _logger.LogError(hex, "HL7Exception encountered in Post");

                return BadRequest(hex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Exception encountered in Post");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}