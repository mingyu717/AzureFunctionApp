using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public enum CommunicationMethod
    {
        None = 0,
        Sms = 1,
        Email = 2,
        SmsOrEmail = 3,
        EmailOrSms = 4
    }
}
