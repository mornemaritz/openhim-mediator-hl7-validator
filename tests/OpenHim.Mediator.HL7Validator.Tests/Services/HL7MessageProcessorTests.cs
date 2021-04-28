using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Options;
using NHapi.Base;
using NUnit.Framework;
using OpenHim.Mediator.HL7Validator.Configuration;
using OpenHim.Mediator.HL7Validator.Services;

namespace OpenHim.Mediator.HL7Validator.Tests.Services
{
    [TestFixture]
    public class HL7MessageProcessorTests
    {
        private HL7MessageProcessor hl7MessageProcessorUnderTest;

        private Fixture fixture;
        private HL7Config hl7Config;

        private readonly string validHL7Message = @"MSH|^~\&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL
EVN|A08|20200702234543|||||
PID|||010-536-7189^^^DOH&www.health.gov.za&DNS^NH ~5812185170084^^^DHA^NNZAF||DYSSEL^JACOB^^^MR^^L||19581218|M|||18DE LAAN 46^^OUDTSHOORN^^6625^^H^BRIDGTON ~18DE LAAN 46^^OUDTSHOORN^^6625^^M^BRIDGTON|ZA|0442794905^PRN^PH||AF^Afrikaans|M^Married|21^Congregation Church|||||2^Coloured||||||SA^South African||0|||20200702234542
NK1||DYSSEL^BEAULA^|SPO^Spouse|18DE LAAN 46^^OUDTSHOORN^^6625^^^BRIDGTON
PV1||N|^^^|||||||||||||||||||||||||||||||||||||||||20200702234543";

        private readonly string invalidPIDValidHeaderMessage = @"MSH|^~\&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL
EVN|A08|20200702234543|||||
PID|-1||010-536-7189^^^DOH&www.health.gov.za&DNS^NH ~5812185170084^^^DHA^NNZAF||DYSSEL^JACOB^^^MR^^L||19581218|M|||18DE LAAN 46^^OUDTSHOORN^^6625^^H^BRIDGTON ~18DE LAAN 46^^OUDTSHOORN^^6625^^M^BRIDGTON|ZA|0442794905^PRN^PH||AF^Afrikaans|M^Married|21^Congregation Church|||||2^Coloured||||||SA^South African||0|||20200702234542
NK1||DYSSEL^BEAULA^|SPO^Spouse|18DE LAAN 46^^OUDTSHOORN^^6625^^^BRIDGTON
PV1||N|^^^|||||||||||||||||||||||||||||||||||||||||20200702234543";

        private readonly string invalidPIDInvalidHeaderMessage = @"MSH|^~\&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL
EVN|A08|20200702234543|||||
PID|-1||010-536-7189^^^DOH&www.health.gov.za&DNS^NH ~5812185170084^^^DHA^NNZAF||DYSSEL^JACOB^^^MR^^L||19581218|M|||18DE LAAN 46^^OUDTSHOORN^^6625^^H^BRIDGTON ~18DE LAAN 46^^OUDTSHOORN^^6625^^M^BRIDGTON|ZA|0442794905^PRN^PH||AF^Afrikaans|M^Married|21^Congregation Church|||||2^Coloured||||||SA^South African||0|||20200702234542
NK1||DYSSEL^BEAULA^|SPO^Spouse|18DE LAAN 46^^OUDTSHOORN^^6625^^^BRIDGTON
PV1||N|^^^|||||||||||||||||||||||||||||||||||||||||20200702234543";

        private readonly string invalidHeaderOnly = @"MSH|^~&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL";

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            hl7Config = fixture.Create<HL7Config>();

            hl7MessageProcessorUnderTest = new HL7MessageProcessor(Options.Create(hl7Config));
        }

        [Test]
        public async Task ParseAndReturnEncodedAck_WhenValid_ReturnsEncodedAck()
        {
            // Act
            var actualMessage = await hl7MessageProcessorUnderTest.ParseAndReturnEncodedAck(validHL7Message);

            // Assert
            Assert.That(actualMessage, Does.Contain("MSH|"));
            Assert.That(actualMessage, Does.Contain("MSA|AA|"));
            Assert.That(actualMessage, Does.Not.Contain("ERR|"));
        }

        [Test, Ignore("Struggling to induce a parse error for the MSH segment")]
        public async Task ParseAndReturnEncodedAck_GivenMessageHeaderOnlyAsInput_WhenInvalidAndWithParseError_ReturnsAppicationErrorAckWithPreviousError()
        {
            // Arrange
            var previousErrorMessage = "This is the previous error message";

            // Act
            var actualMessage = await hl7MessageProcessorUnderTest.ParseAndReturnEncodedAck(invalidHeaderOnly);

            // Assert
            Assert.That(actualMessage, Does.Contain("MSH|"));
            Assert.That(actualMessage, Does.Contain("MSA|AE|"));
            Assert.That(actualMessage, Does.Contain($"ERR|{previousErrorMessage}"));
        }

        [Test,Ignore("Struggling to induce a parse error for the MSH segment")]
        public void ParseAndReturnEncodedAck_GivenMessageHeaderOnlyAsInput_WhenInvalidAndWithoutParseError_ThrowsHL7Exception()
        {
            // Act
            Assert.ThrowsAsync<HL7Exception>(() => hl7MessageProcessorUnderTest.ParseAndReturnEncodedAck(invalidHeaderOnly));
        }

        /// <summary>
        /// This is an indirect test that depends on a recursive call to ParseAndReturnEncodedAck
        /// </summary>
        [Test, Ignore("This test succeeds locally but fails on the DevOps pipeline build. O_o")]
        public async Task ParseAndReturnEncodedAck_GivenFullMessageAsInput_WhenInvalidButWithValidHeader_ReturnsErrorAckWithNonEmptyErrSegment()
        {
            // Act
            var actualMessage = await hl7MessageProcessorUnderTest.ParseAndReturnEncodedAck(invalidPIDValidHeaderMessage);

            // Assert
            Assert.That(actualMessage, Does.Contain("MSH|"));
            Assert.That(actualMessage, Does.Contain("MSA|AE"));
            Assert.That(actualMessage, Does.Contain("ERR|"));
        }

        /// <summary>
        /// This is an indirect test that depends on a recursive call to ParseAndReturnEncodedAck
        /// </summary>
        [Test, Ignore("Struggling to induce a parse error for the MSH segment")]
        public void ParseAndReturnEncodedAck_GivenFullMessageAsInput_WhenInvalidWithInvalidHeader_ThrowsHL7Exception()
        {
            // Act
            Assert.ThrowsAsync<HL7Exception>(() => hl7MessageProcessorUnderTest.ParseAndReturnEncodedAck(invalidPIDInvalidHeaderMessage));
        }
    }
}
