using AutoMapper;
using Service.Contract;
using Service.Contract.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class DealerConfigurationService : IDealerConfigurationService
    {
        private readonly IRestfulClient _restfulClient;
        private const string DealerUrl = "dealers";
        private const string DealerInvitationUrl = "dealers/invitationcontent";

        public DealerConfigurationService(IRestfulClient restfulClient)
        {
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient)); ;
        }

        public async Task<DealerConfigurationResponse> GetDealerConfiguration(string roofTopId, string communityId)
        {
            var url = $"{DealerUrl}/?roofTopId={roofTopId}&communityid={communityId}";
            return await _restfulClient.GetAsync<DealerConfigurationResponse>(url);
        }

        public async Task<DealerConfigurationResponse> GetDealerConfiguration(int dealerId)
        {
            var url = $"{ DealerUrl}/{dealerId}";
            return await _restfulClient.GetAsync<DealerConfigurationResponse>(url);
        }

        public async Task<DealerInvitationContentResponse> GetDealerInvitationContent(string roofTopId, string communityId)
        {
            var url = $"{DealerInvitationUrl}/?roofTopId={roofTopId}&communityid={communityId}";
            return await _restfulClient.GetAsync<DealerInvitationContentResponse>(url);
        }
    }
}
