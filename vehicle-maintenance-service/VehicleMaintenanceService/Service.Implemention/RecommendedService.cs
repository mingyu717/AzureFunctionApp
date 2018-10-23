using System;
using System.Threading.Tasks;
using Service.Contract;
using Service.Contract.Models;

namespace Service.Implemention
{
    public class RecommendedService : IRecommendedService
    {
        private readonly IDealerService _dealerService;
        private readonly ICDKAutolineService _cdkAutolineService;

        public RecommendedService(IDealerService dealerService, ICDKAutolineService cdkAutolineService)
        {
            _dealerService = dealerService ?? throw new ArgumentNullException(nameof(dealerService));
            _cdkAutolineService = cdkAutolineService ?? throw new ArgumentNullException(nameof(cdkAutolineService));
        }

        public async Task<GetRecommendedServicesResponse> Get(GetRecommendedServicesRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var dealer = await _dealerService.GetDealerConfiguration(request.DealerId);
            if (dealer == null)
            {
                return null;
            }

            return await _cdkAutolineService.GetRecommendedServices(request, dealer);
        }
    }
}