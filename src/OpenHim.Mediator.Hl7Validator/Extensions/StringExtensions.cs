using System;
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
    }
}
