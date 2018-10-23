using System.Threading.Tasks;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface IDealerService
    {
        Task<DealerConfigurationResponse> GetDealerConfiguration(int dealerId);
    }
}
