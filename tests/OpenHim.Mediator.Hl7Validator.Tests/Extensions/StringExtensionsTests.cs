using NUnit.Framework;
using OpenHim.Mediator.Hl7Validator.Extensions;
using System;

namespace OpenHim.Mediator.Hl7Validator.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private string hl7HeaderOnly = "MSH|^~&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL";

        [Test]
        public void IsHL7MessageHeaderOnly_WhenHeaderOnlyReturnsTrue()
        {
            // Assert
            Assert.That(hl7HeaderOnly.IsHL7MessageHeaderOnly(), Is.True);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenEmptyStringReturnsFalse()
        {
            // Assert
            Assert.That(string.Empty.IsHL7MessageHeaderOnly(), Is.False);
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
            var multiLine = @"MSH|^~\&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL
EVN|A08|20200702234543|||||";

            // Assert
            Assert.That(multiLine.IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void IsHL7MessageHeaderOnly_WhenSingleLineButNotHL7MessageHeaderReturnsFalse()
        {
            // Arrange
            var nonHl7HeaderLine = "BLAH";

            // Assert
            Assert.That(nonHl7HeaderLine.IsHL7MessageHeaderOnly(), Is.False);
        }

        [Test]
        public void GetHL7MessageHeader_WhenHeaderExists_ReturnsHeader()
        {
            var multiLine = @"MSH|^~&|WCGPIXHPRS|WCGDOH|HPRSPIXPDQ|CENTRAL|20200702234543.424||ADT^A08^ADT_A01|20200702234543|P|2.5.1|||AL|AL
EVN|A08|20200702234543|||||";

            Assert.That(multiLine.GetHL7MessageHeader(), Is.EqualTo(hl7HeaderOnly));
        }

        [Test]
        public void GetHL7MessageHeader_WhenEmptyStringReturnsNull()
        {
            // Assert
            Assert.That(string.Empty.GetHL7MessageHeader(), Is.Null);
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

    }
}
