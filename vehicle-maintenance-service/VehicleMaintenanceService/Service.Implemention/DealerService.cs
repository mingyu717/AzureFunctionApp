using System;
using System.Threading.Tasks;
using Service.Contract;
using Service.Contract.Models;

namespace Service.Implemention
{
    public class DealerService : IDealerService
    {
        private readonly IRestfulClient _restfulClient;
        private const string DealerUrl = "dealers";

        public DealerService(IRestfulClient restfulClient)
        {
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient));
        }

        public async Task<DealerConfigurationResponse> GetDealerConfiguration(int dealerId)
        {
            if (dealerId < 0) throw new ArgumentOutOfRangeException(nameof(dealerId));

            var url = $"{DealerUrl}/{dealerId}";
            return await _restfulClient.GetAsync<DealerConfigurationResponse>(url);
        }
    }
}
