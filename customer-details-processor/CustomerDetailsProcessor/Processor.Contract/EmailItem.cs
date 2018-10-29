using System.Collections.Generic;

namespace Processor.Contract
{
    public class EmailItem
    {
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public List<EmailAdditionalContent> EmailAdditionalContents { get; set; }
    }
}
