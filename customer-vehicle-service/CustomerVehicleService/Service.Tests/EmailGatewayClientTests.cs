using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;
using Service.Contract.Exceptions;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture()]
    public class EmailGatewayClientTests
    {
        private EmailGatewayClient _emailGatewayClient;
        private Mock<ISendGridClient> _sendGridClientMock;
        private const string FromEmail = "fromMail@mail.com";
        private const string ToEmail = "toMail@mail.com";

        [SetUp]
        public void SetUp()
        {
            _sendGridClientMock = new Mock<ISendGridClient>();
            _emailGatewayClient = new EmailGatewayClient(_sendGridClientMock.Object);
        }

        [Test]
        public void Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailGatewayClient(null));
        }

        [Test]
        public async Task SendEmail_ResponseNotOk_ShouldReturnFalse()
        {
            var errorResponse = new Response(HttpStatusCode.InternalServerError, new StringContent("error"), null);
            _sendGridClientMock.Setup(m => m.SendEmailAsync(It.IsAny<SendGridMessage>(), default(CancellationToken))).Returns(Task.FromResult(errorResponse));

            var result = await _emailGatewayClient.SendHtmlEmail(FromEmail, ToEmail, "test email", "this is a test email");
            Assert.IsFalse(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.AreEqual(string.Format(ExceptionMessages.UnableToSendEmail, FromEmail, ToEmail), result.Item2.Message);
        }

        [Test]
        public async Task SendEmail_ResponseOk_ShouldReturnTalse()
        {
            var errorResponse = new Response(HttpStatusCode.OK, new StringContent("ok"), null);
            _sendGridClientMock.Setup(m => m.SendEmailAsync(It.IsAny<SendGridMessage>(), default(CancellationToken))).Returns(Task.FromResult(errorResponse));

            var result = await _emailGatewayClient.SendHtmlEmail(FromEmail, ToEmail, "test email", "this is a test email");
            Assert.IsTrue(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public async Task SendEmail_ResponseAccepted_ShouldReturnTalse()
        {
            var errorResponse = new Response(HttpStatusCode.Accepted, new StringContent("accepted"), null);
            _sendGridClientMock.Setup(m => m.SendEmailAsync(It.IsAny<SendGridMessage>(), default(CancellationToken))).Returns(Task.FromResult(errorResponse));

            var result = await _emailGatewayClient.SendHtmlEmail(FromEmail, ToEmail, "test email", "this is a test email");
            Assert.IsTrue(result.Item1);
            Assert.IsNull(result.Item2);
        }

        [Test]
        public async Task SendEmail_ThrowsException_ShouldReturnTalse()
        {
            _sendGridClientMock.Setup(m => m.SendEmailAsync(It.IsAny<SendGridMessage>(), default(CancellationToken))).Throws(new Exception("this is an exception"));

            var result = await _emailGatewayClient.SendHtmlEmail(FromEmail, ToEmail, "test email", "this is a test email");
            Assert.IsFalse(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.AreEqual("this is an exception", result.Item2.Message);
        }
    }
}