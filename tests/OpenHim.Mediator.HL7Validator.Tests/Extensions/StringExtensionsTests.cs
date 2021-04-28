using System;
using NUnit.Framework;
using OpenHim.Mediator.HL7Validator.Extensions;
using static OpenHim.Mediator.HL7Validator.Extensions.StringExtensions;

namespace OpenHim.Mediator.HL7Validator.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private readonly string hl7HeaderOnly = "MSH|^~&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL";
        private readonly string whitespace = "   ";

        [Test]
        public void IsHL7MessageHeaderOnly_WhenHeaderOnlyReturnsTrue()
        {
            // Assert
            Assert.That(hl7HeaderOnly.IsHL7MessageHeaderOnly(), Is.True);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenWhitespaceReturnsFalse()
        {
            // Assert
            Assert.That(whitespace.IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenNullStringReturnsFalse()
        {
            // Assert
            Assert.That(((string)null).IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenMultiLineReturnsFalse()
        {
            // Arrange
            var multiLine = @$"MSH|^~\&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}EVN|A08|20200702234543|||||";

            // Assert
            Assert.That(multiLine.IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenSingleLineButNotHL7MessageHeaderReturnsFalse()
        {
            // Arrange
            var nonHL7HeaderLine = "BLAH";

            // Assert
            Assert.That(nonHL7HeaderLine.IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void GetHL7MessageHeader_WhenHeaderExists_ReturnsHeader()
        {
            var multiLine = @$"MSH|^~&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}EVN|A08|20200702234543|||||";

            Assert.That(multiLine.GetHL7MessageHeader(), Is.EqualTo(hl7HeaderOnly));
        }

        [Test]
        public void GetHL7MessageHeader_WhenWhitespaceReturnsNull()
        {
            // Assert
            Assert.That(whitespace.GetHL7MessageHeader(), Is.Null);
        }

        [Test]
        public void GetHL7MessageHeader_WhenNullStringReturnsNull()
        {
            // Assert
            Assert.That(((string)null).GetHL7MessageHeader(), Is.Null);
        }

        [Test]
        public void GetHL7MessageHeader_WhenNoHeaderExists_ReturnsNull()
        {
            // Arrange
            var noHL7Header = $"FOO{Environment.NewLine}BAR";

            Assert.That(noHL7Header.GetHL7MessageHeader(), Is.Null);
        }

        [Test]
        public void IsHL7ApplicationAcceptAck_MessageAckLineWithApplicationExcep_ReturnsTrue()
        {
            var messageWithMSA_AA = @$"MSH|^~\\&|PDIPIXPDQ|WCPHDC|WCGPIXHPRS|WCGDOH|202104221202||ACK^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}MSA|AA|20200702234543";

            // Assert
            Assert.That(messageWithMSA_AA.IsHL7ApplicationAcceptAck(), Is.True);

        }

        [Test]
        public void IsHL7ApplicationAcceptAck_MessageAckLineWithApplicationError_ReturnsFalse()
        {
            var messageWithMSA_AE = $@"MSH|^~\\&|PDIPIXPDQ|WCPHDC|WCGPIXHPRS|WCGDOH|202104221202||ACK^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}MSA|AE|20200702234543";

            // Assert
            Assert.That(messageWithMSA_AE.IsHL7ApplicationAcceptAck(), Is.False);
        }

        [Test]
        public void IsHL7ApplicationAcceptAck_MessageAckLineWithApplicationReject_ReturnsFalse()
        {
            var messageWithMSA_AR = $@"MSH|^~\\&|PDIPIXPDQ|WCPHDC|WCGPIXHPRS|WCGDOH|202104221202||ACK^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}MSA|AR|20200702234543";

            // Assert
            Assert.That(messageWithMSA_AR.IsHL7ApplicationAcceptAck(), Is.False);
        }

        [Test]
        public void IsHL7ApplicationAcceptAck_NoMessageAck_ReturnsFalse()
        {
            var messageWithNoMSA = $@"MSH|^~\\&|PDIPIXPDQ|WCPHDC|WCGPIXHPRS|WCGDOH|202104221202||ACK^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL{nHapiHL7NewLine}EVN|A08|20200702234543|||||";

            // Assert
            Assert.That(messageWithNoMSA.IsHL7ApplicationAcceptAck(), Is.False);
        }

        [Test]
        public void IsHL7ApplicationAcceptAck_WhitespaceMessage_ReturnsFalse()
        {
            // Assert
            Assert.That(whitespace.IsHL7ApplicationAcceptAck(), Is.False);
        }

        [Test]
        public void IsHL7ApplicationAcceptAck_NullMessage_ReturnsFalse()
        {
            string nullMessage = null;

            // Assert
            Assert.That(nullMessage.IsHL7ApplicationAcceptAck(), Is.False);
        }
    }
}
