using System;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IEmailGatewayClient
    {
        Task<(bool, Exception)> SendHtmlEmail(string fromEmailAddress, string toEmailAddress, string subject, string htmlContent);
    }
}
