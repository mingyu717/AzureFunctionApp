using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Processor.Contract;
using Processor.Implementation;

namespace Processor.Tests
{
    [TestFixture]
    public class EmailProcessorTests
    {
        private Mock<IMailService> _mailServiceMock;
        private IEmailProcessor _emailProcessor;

        [SetUp]
        public void SetUp()
        {
            _mailServiceMock = new Mock<IMailService>();
            _emailProcessor = new EmailProcessor(_mailServiceMock.Object);
        }

        [Test]
        public void EmailProcessor_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailProcessor(null));
        }

        [Test]
        public async Task ProcessEmailContent_Test()
        {
            List<EmailAdditionalContent> sampleEmailAdditionalContents = new List<EmailAdditionalContent>
            {
                new EmailAdditionalContent
                {
                    ContentName = "Sample.csv",
                    ContentSource = new byte[100]
                }
            };

            List<EmailAdditionalContent> mockEmailAdditionalContents = new List<EmailAdditionalContent>
            {
                new EmailAdditionalContent
                {
                    ContentName = "Mock.csv",
                    ContentSource = new byte[100]
                }
            };

            IEnumerable<EmailItem> emailItems = new List<EmailItem>
            {
                new EmailItem
                {
                    EmailAdditionalContents = sampleEmailAdditionalContents
                },
                new EmailItem
                {
                    EmailAdditionalContents = mockEmailAdditionalContents
                }
            };

            _mailServiceMock.Setup(m => m.ReadEmails()).Returns(Task.FromResult(emailItems));

            var fileContents = await _emailProcessor.ProcessEmailContent();

            Assert.AreEqual(2, fileContents.Count);
            Assert.AreEqual("Sample.csv", fileContents[0].FileName);
            Assert.AreEqual("Mock.csv", fileContents[1].FileName);
        }

        [Test]
        public void ProcessEmailContent_ThrowException_Test()
        {
            const string exceptionMessage = "This is a new exception";
            _mailServiceMock.Setup(m => m.ReadEmails()).Throws(new Exception(exceptionMessage));
            var exception = Assert.ThrowsAsync<GetEmailContentException>(() => _emailProcessor.ProcessEmailContent());
            Assert.AreEqual(exceptionMessage, exception.Message);
        }
    }
}