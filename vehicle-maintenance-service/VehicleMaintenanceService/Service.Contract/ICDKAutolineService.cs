using System.Threading.Tasks;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface ICDKAutolineService
    {
        Task<GetRecommendedServicesResponse> GetRecommendedServices(GetRecommendedServicesRequest request, DealerConfigurationResponse dealer);
    }
}