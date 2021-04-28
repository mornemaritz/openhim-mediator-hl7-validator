using System.Linq;
using System.Net;
using NUnit.Framework;
using WcPhdc.OpenHim.Mediator.Extensions;
using WcPhdc.OpenHim.Mediator.Models;

namespace WcPhdc.OpenHim.Mediator.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private const string responseBody = "this is the response body";

        [Test]
        public void ToOpenHimConsumerResponse_ReturnsOpenHimResponse()
        {
            // Act
            var actual = responseBody.ToOpenHimConsumerResponse(HttpStatusCode.OK);

            // Assert
            Assert.That(actual, Is.InstanceOf<Response>());
        }

        [Test]
        public void ToOpenHimConsumerResponse_ReturnsOpenHimResponse_WithSpecifiedStatus()
        {
            // Act
            var actual = responseBody.ToOpenHimConsumerResponse(HttpStatusCode.OK);

            // Assert
            Assert.That(actual.Status, Is.EqualTo(200));
        }

        [Test]
        public void ToOpenHimConsumerResponse_OpenHimResponse_ContainsSpecifiedResponseBody()
        {
            // Act
            var actual = responseBody.ToOpenHimConsumerResponse(HttpStatusCode.OK);

            // Assert
            Assert.That(actual.Body, Is.EqualTo(responseBody));
        }

        [Test]
        public void ToOpenHimConsumerResponse_ReturnsOpenHimResponse_WithOpenHimContentType()
        {
            // Act
            var actual = responseBody.ToOpenHimConsumerResponse(HttpStatusCode.OK);

            // Assert
            var contentTypeHeader = actual.Headers.SingleOrDefault(h => h.Key == "Content-Type");

            Assert.That(contentTypeHeader, Is.Not.Null);
            Assert.That(contentTypeHeader.Value, Is.EqualTo("application/hl7-v2"));
        }
    }
}
