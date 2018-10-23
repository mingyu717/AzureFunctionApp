using SendGrid;
using SendGrid.Helpers.Mail;
using Service.Contract;
using System;
using System.Threading.Tasks;
using System.Net;
using Service.Contract.Exceptions;

namespace Service.Implementation
{
    public class EmailGatewayClient : IEmailGatewayClient
    {
        private readonly ISendGridClient _sendGridClient;

        public EmailGatewayClient(ISendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
        }

        public async Task<(bool, Exception)> SendHtmlEmail(string fromEmailAddress, string toEmailAddress, string subject, string htmlContent)
        {
            try
            {
                var fromEmail = new EmailAddress(fromEmailAddress);
                var toEmail = new EmailAddress(toEmailAddress);
                var emailMessage = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, string.Empty, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(emailMessage);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
                    return (true, null);
                return (false, new Exception(string.Format(ExceptionMessages.UnableToSendEmail, fromEmail.Email, toEmail.Email)));
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }
    }
}