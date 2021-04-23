using System;
using System.Collections.Generic;
using System.Net;
using WcPhdc.OpenHim.Mediator.Models;

namespace WcPhdc.OpenHim.Mediator.Extensions
{
    public static class StringExtensions
    {
        public static Response ToOpenHimConsumerResponse(this string responseBody, HttpStatusCode httpStatusCode)
        {
            return new Response
            {
                Status = (short)httpStatusCode,
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
