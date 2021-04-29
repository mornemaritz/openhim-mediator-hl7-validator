using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using WcPhdc.OpenHim.Mediator.Configuration;
using WcPhdc.OpenHim.Mediator.Models;
using WcPhdc.OpenHim.Mediator.Services;

namespace WcPhdc.OpenHim.Mediator.Tests.Services
{
    [TestFixture]
    public class OrchestratorTests
    {
        private OpenHimOrchestrator orchestratorUnderTest;
        private Mock<IHttpClientFactory> httpClientFactory;
        private Mock<HttpMessageHandler> httpMessageHandler;
        private Mock<HttpMessageHandler> httpMessageHandler2;

        private Fixture fixture;

        private MediatorConfig mediatorConfig;
        private readonly string bodyString = "PostThis";
        private Response primaryResponse;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new TypeRelay(typeof(HttpContent), typeof(StringContent)));
            mediatorConfig = fixture.Create<MediatorConfig>();
            primaryResponse = fixture.Create<Response>();

            httpMessageHandler = new Mock<HttpMessageHandler>();

            var responseMessage = fixture.Create<HttpResponseMessage>();

            httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var httpClient1 = new HttpClient(httpMessageHandler.Object);

            httpMessageHandler2 = new Mock<HttpMessageHandler>();

            var responseMessage2 = fixture.Create<HttpResponseMessage>();

            httpMessageHandler2.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage2);


            var httpClient2 = new HttpClient(httpMessageHandler2.Object);

            var clientQueue = new Queue<HttpClient>(new[] { httpClient1, httpClient2 });

            httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(() => clientQueue.Dequeue());

            orchestratorUnderTest = new OpenHimOrchestrator(Options.Create(mediatorConfig), httpClientFactory.Object);
        }

        [Test]
        public async Task Do_WhenOrchestrationsNull_ReturnsOpenHimResponseWithNullOrchestrations()
        {
            // Arrange
            mediatorConfig = fixture.Build<MediatorConfig>()
                .With(c => c.Orchestrations, (List<Orchestration>)null)
                .Create();

            orchestratorUnderTest = new OpenHimOrchestrator(Options.Create(mediatorConfig), httpClientFactory.Object);

            // Act
            var actualResult = await orchestratorUnderTest.Do(bodyString, primaryResponse);

            // Assert
            Assert.That(actualResult.Orchestrations, Is.Null);
        }

        [Test]
        public async Task Do_WhenOrchestrationsEmpty_ReturnsOpenHimResponseWithNullOrchestrations()
        {
            // Arrange
            mediatorConfig = fixture.Build<MediatorConfig>()
                .With(c => c.Orchestrations, Enumerable.Empty<Orchestration>().ToList())
                .Create();

            orchestratorUnderTest = new OpenHimOrchestrator(Options.Create(mediatorConfig), httpClientFactory.Object);

            // Act
            var actualResult = await orchestratorUnderTest.Do(bodyString, primaryResponse);

            // Assert
            Assert.That(actualResult.Orchestrations, Is.Null);
        }

        [Test]
        public async Task Do_PerformsHttpCallForEachOrchestration()
        {
            // Arrange
            var request = fixture.Build<Request>()
                .With(r => r.Method, "POST")
                .With(r => r.Host, "http://somewhere.com")
                .Create();

            var orchestrations = fixture.Build<Orchestration>()
                .With(o => o.Request, request)
                .CreateMany(2);

            mediatorConfig = fixture.Build<MediatorConfig>()
                .With(c => c.Orchestrations, orchestrations.ToList())
                .Create();

            orchestratorUnderTest = new OpenHimOrchestrator(Options.Create(mediatorConfig), httpClientFactory.Object);

            // Act
            await orchestratorUnderTest.Do(bodyString, primaryResponse);

            // Assert
            httpClientFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Exactly(2));
            httpMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            httpMessageHandler2.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }
    }
}
