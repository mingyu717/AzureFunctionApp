using Processor.Contract;
using Processor.Contract.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    public class DealerConfigurationClient : IDealerConfigurationClient
    {
        private readonly IRestfulClient _restfulClient;
        private const string DealerUrl = "dealers";

        public DealerConfigurationClient(IRestfulClient restfulClient)
        {
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient)); ;
        }

        public async Task<DealerConfigurationResponse> GetDealerConfiguration(string roofTopId, string communityId)
        {
            var url = $"{DealerUrl}/?roofTopId={roofTopId}&communityid={communityId}";
            return await _restfulClient.GetAsync<DealerConfigurationResponse>(url);
        }

        public async Task<List<string>> GetDealerCsvSources()
        {
            var url = $"{DealerUrl}/csvsources";
            return await _restfulClient.GetAsync<List<string>>(url);
        }

        public bool ValidateEmailOrSmsByCommunicationMethod(DealerConfigurationResponse response, string phoneNumber, string email)
        {
            var method = GetCommunitcationMethod(response.CommunicationMethod);

            switch (method)
            {
                case CommunicationMethod.Sms:
                    {
                        if (string.IsNullOrEmpty(phoneNumber))
                            throw new PhoneOrEmailNullException("phone number is mandatory to process the request");
                        return true;
                    }
                case CommunicationMethod.Email:
                    {
                        if (string.IsNullOrEmpty(email))
                            throw new PhoneOrEmailNullException("email address is mandatory to process the request");
                        return true;
                    }
                case CommunicationMethod.EmailOrSms:
                case CommunicationMethod.SmsOrEmail:
                    {
                        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phoneNumber))
                            throw new PhoneOrEmailNullException("email or phone number is mandatory to process the request");
                        return true;
                    }
                default:
                    return true;
            }
        }

        internal CommunicationMethod GetCommunitcationMethod(int methodId)
        {
            return (CommunicationMethod)Enum.Parse(typeof(CommunicationMethod), methodId.ToString());
        }
    }
}
