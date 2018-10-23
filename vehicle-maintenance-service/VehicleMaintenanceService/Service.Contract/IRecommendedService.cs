using System.Threading.Tasks;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface IRecommendedService
    {
        Task<GetRecommendedServicesResponse> Get(GetRecommendedServicesRequest request);
    }
}
