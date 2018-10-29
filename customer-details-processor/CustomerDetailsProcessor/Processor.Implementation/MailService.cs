using System;
using Microsoft.Exchange.WebServices.Data;
using Processor.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    /// <summary>
    /// Class used to Read all email and there attachmenet from Exchange service (Microsoft)
    /// </summary>
    public class MailService : IMailService
    {
        private readonly IExchangeServices _exchangeServices;

        /// <summary>
        /// Constructor, it is responsible for resolving the instance of exchange service.
        /// </summary>
        /// <param name="exchangeServices"></param>
        public MailService(IExchangeServices exchangeServices)
        {
            _exchangeServices = exchangeServices ?? throw new ArgumentNullException(nameof(exchangeServices));
        }


        #region "=== [ Methods ] =========================="

        /// <summary>
        /// Read Emails from exchange services.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EmailItem>> ReadEmails()
        {
            List<EmailItem> emailItems = new List<EmailItem>();
            var findResults = await ReadEmailItems();
            if (findResults == null) return emailItems;
            foreach (Item item in findResults)
            {
                var emailItem = await ReadEmailAttachment(item);
                emailItems.Add(emailItem);
            }

            return emailItems;
        }

        /// <summary>
        /// Read email items by providing search filter based on date.
        /// </summary>
        /// <returns></returns>
        private async Task<FindItemsResults<Item>> ReadEmailItems()
        {
            if (_exchangeServices.ExchangeService != null)
            {
                return await _exchangeServices.ExchangeService.FindItems(WellKnownFolderName.Inbox,
                    _exchangeServices.SearchFilter, _exchangeServices.ItemView);
            }

            return null;
        }

        /// <summary>
        /// Read email attachments from email message by partucular item id
        /// </summary>
        /// <param name="emailItem"></param>
        /// <returns></returns>
        private async Task<EmailItem> ReadEmailAttachment(Item emailItem)
        {
            List<EmailAdditionalContent> emailAdditionalContents = new List<EmailAdditionalContent>();
            EmailMessage message = await EmailMessage.Bind(_exchangeServices.ExchangeService, emailItem.Id,
                new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments));
            foreach (Attachment attachment in message.Attachments)
            {
                if (attachment is FileAttachment fileAttachment)
                {
                    await fileAttachment.Load();
                    emailAdditionalContents.Add(new EmailAdditionalContent
                    {
                        ContentType = fileAttachment.ContentType,
                        ContentSource = fileAttachment.Content,
                        ContentName = fileAttachment.Name
                    });
                }
            }

            await emailItem.Load();
            message.IsRead = true;
            await message.Update(ConflictResolutionMode.AlwaysOverwrite);

            return new EmailItem
            {
                EmailBody = emailItem.Body.Text,
                EmailSubject = emailItem.Subject,
                EmailAdditionalContents = emailAdditionalContents
            };
        }

        #endregion
    }
}