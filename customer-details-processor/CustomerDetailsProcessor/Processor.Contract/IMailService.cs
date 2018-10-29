using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IMailService
    {
        //void ConnectMail();
        Task<IEnumerable<EmailItem>> ReadEmails();
    }
}
