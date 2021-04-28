using System.Linq;

namespace OpenHim.Mediator.HL7Validator.Extensions
{
    public static class StringExtensions
    {
        public static char nHapiHL7NewLine = '\r';

        public static bool IsHL7MessageHeaderOnly(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.StartsWith("MSH|")
                && !source.Contains(nHapiHL7NewLine); 
        }

        public static string GetHL7MessageHeader(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return default;

            var firstLine = source.Split(nHapiHL7NewLine).FirstOrDefault();

            if (firstLine.IsHL7MessageHeaderOnly())
                return firstLine;
            else
                return default;
        }

        public static bool IsHL7ApplicationAcceptAck(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.Split(nHapiHL7NewLine).Any(l => l.StartsWith("MSA|AA|"));
        }
    }
}
