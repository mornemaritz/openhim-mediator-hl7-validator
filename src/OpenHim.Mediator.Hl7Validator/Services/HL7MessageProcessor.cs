using Microsoft.Extensions.Options;
using NHapi.Base;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.validation.impl;
using NHapiTools.Base.Util;
using OpenHim.Mediator.HL7Validator.Configuration;
using OpenHim.Mediator.HL7Validator.Extensions;
using System;
using System.Threading.Tasks;

namespace OpenHim.Mediator.HL7Validator.Services
{
    public class HL7MessageProcessor : IHL7MessageProcessor
    {
        private PipeParser pipeParser = new PipeParser { ValidationContext = new StrictValidation() };

        private readonly HL7Config hl7Config;

        public HL7MessageProcessor(IOptions<HL7Config> hl7ConfigOptions)
        {
            hl7Config = hl7ConfigOptions.Value ?? throw new ArgumentNullException(nameof(hl7ConfigOptions));
        }

        public async Task<string> ParseAndReturnEncodedAck(string hl7Message, string parseError = default)
        {
            if (string.IsNullOrEmpty(hl7Message))
                throw new HL7Exception("No valid HL7 message segments found");

            try
            {
                var parsedMessage = await Task.Run(() => pipeParser.Parse(hl7Message));

                var ack = new Ack(hl7Config.Application, hl7Config.Facility);
                IMessage ackMessage;

                if (string.IsNullOrEmpty(parseError))
                    ackMessage = ack.MakeACK(parsedMessage);
                else
                    ackMessage = ack.MakeACK(parsedMessage, AckTypes.AE, parseError);

                return await Task.Run(() => pipeParser.Encode(ackMessage));
            }
            catch (HL7Exception hex) when (string.IsNullOrEmpty(parseError) && !hl7Message.IsHL7MessageHeaderOnly())
            {
                // If no parseError has been specified as a method parameter AND the hl7Message we've just failed to
                // parse is NOT just an HL7 Message Header, we can re-attempt this ParseAndReturnEncodedAck operation on
                // the header only, so that we can generate an Ack from the (possibly) successfully parsed header, in order to
                // return *this* exception message as the parseError in a valid HL7 Ack Message
                parseError = hex.Message;
            }

            return await ParseAndReturnEncodedAck(hl7Message.GetHL7MessageHeader(), parseError);
        }
    }
}
