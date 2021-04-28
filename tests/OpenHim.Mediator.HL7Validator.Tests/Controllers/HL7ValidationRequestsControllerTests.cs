using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpenHim.Mediator.HL7Validator.Controllers;
using OpenHim.Mediator.HL7Validator.Services;
using WcPhdc.OpenHim.Mediator.Models;
using WcPhdc.OpenHim.Mediator.Services;

namespace OpenHim.Mediator.HL7Validator.Tests.Controllers
{
    [TestFixture]
    public class HL7ValidationRequestsControllerTests
    {
        private HL7ValidationRequestsController controllerUnderTest;
        private Fixture fixture;

        private Mock<IHL7MessageProcessor> hl7MessageProcessor;
        private Mock<IOpenHimOrchestrator> orchestrator;
        private Mock<ILogger<HL7ValidationRequestsController>> logger;

        private const string hl7MessageData = @"MSH|^~\&|SENDING_APPLICATION|SENDING_FACILITY|RECEIVING_APPLICATION|RECEIVING_FACILITY|20110614075841||ACK|1407511|P|2.5.1||||||";
        private OpenHimResponse openHimResponse;

        [SetUp]
        public void SetUp()
        {
            hl7MessageProcessor = new Mock<IHL7MessageProcessor>();
            orchestrator = new Mock<IOpenHimOrchestrator>();
            logger = new Mock<ILogger<HL7ValidationRequestsController>>();

            fixture = new Fixture();
            openHimResponse = fixture.Create<OpenHimResponse>();
            orchestrator.Setup(o => o.Do(It.IsAny<string>(), It.IsAny<Response>(), It.IsAny<bool>()))
                .ReturnsAsync(openHimResponse);

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(hl7MessageData));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            controllerUnderTest = new HL7ValidationRequestsController(hl7MessageProcessor.Object, orchestrator.Object, logger.Object) { ControllerContext = controllerContext };
        }

        [Test]
        public async Task Post_WhenBodyEmpty_ReturnsBadRequest()
        {            
            //Arrange
            string emptyBodyData = string.Empty;
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(emptyBodyData));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            controllerUnderTest.ControllerContext = controllerContext;

            // Act
            var result = await controllerUnderTest.Post();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult).Value, Is.EqualTo("Request body may not be null or empty"));
        }

        [Test, Ignore("How do I simulate a null body with the DefaultHttpContext that uses a memory stream for the body which does not allow null as input")]
        public async Task Post_WhenBodyNull_ReturnsBadRequest()
        {
            // Act
            var result = await controllerUnderTest.Post();

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult).Value, Is.EqualTo("Request body may not be null or empty"));
        }

        [Test]
        public async Task Post_ReturnsOkResultWithOpenHimResponse()
        {
            // Act
            var actualResult = await controllerUnderTest.Post();

            // Assert
            Assert.That(actualResult, Is.InstanceOf<OkObjectResult>());

            Assert.That((actualResult as OkObjectResult).Value, Is.InstanceOf<OpenHimResponse>());
        }

        [Test]
        public async Task Post_SetsResponseHeaderTo()
        {
            // Act
            await controllerUnderTest.Post();

            // Assert
            var contentTypeHeaderValues = controllerUnderTest.Response.Headers.GetCommaSeparatedValues("Content-Type");

            Assert.That(contentTypeHeaderValues, Does.Contain("application/json+openhim"));
        }

        [Test]
        public async Task Post_Calls_HL7MessageProcessor_ParseAndReturnAck()
        {
            // Act
            await controllerUnderTest.Post();

            // Assert
            hl7MessageProcessor.Verify(p => p.ParseAndReturnEncodedAck(hl7MessageData, default), Times.Once);
        }

        [Test]
        public async Task Post_Calls_Orchsetrator_Do()
        {
            // Act
            await controllerUnderTest.Post();

            // Assert
            orchestrator.Verify(p => p.Do(It.IsAny<string>(), It.IsAny<Response>(), default), Times.Once);
        }
    }
}
