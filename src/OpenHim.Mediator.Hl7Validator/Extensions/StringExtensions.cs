using OpenHim.Mediator.Hl7Validator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenHim.Mediator.Hl7Validator.Extensions
{
    public static class StringExtensions
    {
        public static bool IsHL7MessageHeaderOnly(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.StartsWith("MSH|")
                && !source.Contains(Environment.NewLine); 
        }

        public static string GetHL7MessageHeader(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return default;

            var firstLine = source.Split(Environment.NewLine).FirstOrDefault();

            if (firstLine.IsHL7MessageHeaderOnly())
                return firstLine;
            else
                return default;
        }

        public static bool IsHL7ApplicationAcceptAck(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.Split(Environment.NewLine).Any(l => l.StartsWith("MSA|AA|"));
        }

        // This should be in a namespace that denotes the translation layer between HL7 related concepts and OpenHim related concepts (other extensions here are purely HL7).
        public static Response ToOpenHimConsumerResponse(this string responseBody)
        {
            return new Response
            {
                // Nothing in the ODS PIX Feed spec (it seems) that outlines expected http status codes.
                // My speculation is, that the expectation of receiving a higher level HL7 Ack message implies
                // that anything but a success response (even in the case of HL7 related errors)
                // may prevent the Ack message from passing through the lower HTTP layers.
                Status = 200,
                Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/hl7-v2" }
                    },
                Body = responseBody,
                Timestamp = DateTime.UtcNow.ToString("s")
            };
        }
    }
}
