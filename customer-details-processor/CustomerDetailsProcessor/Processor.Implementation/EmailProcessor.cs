using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Processor.Contract;

namespace Processor.Implementation
{
    /// <summary>
    /// Class EmailProcessor, used for working with ExchangeService for reading and processing email(s) ' attachment(s) as of now.
    /// </summary>
    public class EmailProcessor : IEmailProcessor
    {
        private readonly IMailService _mailService;

        /// <summary>
        /// Constructor accepting input of type IExchangeService , implementation of which will be resolved by Unity.
        /// </summary>
        /// <param name="mailService"></param>
        public EmailProcessor(IMailService mailService)
        {
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        /// <summary>
        /// Read attachment(s) from Exchange service
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileContent>> ProcessEmailContent()
        {
            try
            {
                //Read Emails, it can be multiple.
                var emailContentList = await _mailService.ReadEmails();

                //Read Attachments, it can be multiple per email

                return (from emailItem in emailContentList
                    from emailAdditionalContent in emailItem.EmailAdditionalContents
                    select new FileContent
                    {
                        FileName = emailAdditionalContent.ContentName,
                        Content = emailAdditionalContent.ContentSource
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new GetEmailContentException(ex.Message, ex.StackTrace);
            }
        }
    }
}