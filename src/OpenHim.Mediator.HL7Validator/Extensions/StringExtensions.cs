﻿using System.Linq;

namespace OpenHim.Mediator.HL7Validator.Extensions
{
    public static class StringExtensions
    {
        public static readonly char nHapiHL7NewLine = '\r';

        public static bool IsHL7MessageHeaderOnly(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.StartsWith("MSH|")
                && !source.Contains(nHapiHL7NewLine);
        }

        public static string GetHL7MessageHeader(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return default;
            }

            var firstLine = source.Split(nHapiHL7NewLine).FirstOrDefault();

            return firstLine.IsHL7MessageHeaderOnly() ? firstLine : default;
        }

        public static bool IsHL7ApplicationAcceptAck(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.Split(nHapiHL7NewLine).Any(l => l.StartsWith("MSA|AA|"));
        }
    }
}
