using System.Threading.Tasks;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface ICDKVehicleMaintenanceService
    {
        Task<GetRecommendedServicesResponse> GetRecommendedServices(GetRecommendedServicesRequest request);
    }
}
