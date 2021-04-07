using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using OpenHim.Mediator.Hl7Validator.Controllers;

namespace OpenHim.Mediator.Hl7Validator.Tests.Controllers
{
    [TestFixture]
    public class Hl7ValidationRequestsControllerTests
    {
        private Hl7ValidationRequestsController controllerUnderTest;

        [SetUp]
        public void SetUp()
        {
            controllerUnderTest = new Hl7ValidationRequestsController();
        }

        [Test]
        public void Post_ReturnsOkResult()
        {
            // Act & Assert
            Assert.That(controllerUnderTest.Post(""), Is.InstanceOf<OkResult>());
        }
    }
}
