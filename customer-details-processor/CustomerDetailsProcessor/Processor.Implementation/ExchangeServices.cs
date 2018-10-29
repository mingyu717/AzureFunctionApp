using Microsoft.Exchange.WebServices.Data;
using Processor.Contract;
using System;

namespace Processor.Implementation
{
    /// <summary>
    /// Class, It is responsible for making connection with (Microsoft) Exchange services
    /// and set search filter criteria and their item view property.
    /// </summary>
    public class ExchangeServices : IExchangeServices
    {
        public ExchangeService ExchangeService { get; private set; }
        public SearchFilter.IsEqualTo SearchFilter { get; private set; }
        public ItemView ItemView { get; private set; }

        /// <summary>
        /// Constructor, It is responsible for connect the exchange service, set search filter , set item view property.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="url"></param>
        public ExchangeServices(int version, string userName, string password, string url)
        {
            Connect(version, userName, password, url);
            SetSearchFilter();
            SetItemView();
        }


        private void Connect(int version, string userName, string password, string url)
        {
            ExchangeService = new ExchangeService((ExchangeVersion) version)
            {
                Credentials = new WebCredentials(userName, password),
                Url = new Uri(url)
            };
        }

        private void SetSearchFilter()
        {
            SearchFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead,false);
        }

        private void SetItemView()
        {
            PropertySet itemPropertySet =
                new PropertySet(BasePropertySet.FirstClassProperties) {RequestedBodyType = BodyType.Text};
            ItemView = new ItemView(1000) {PropertySet = itemPropertySet};
        }
    }
}