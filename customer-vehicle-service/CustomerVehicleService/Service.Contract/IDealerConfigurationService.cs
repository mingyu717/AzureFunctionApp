using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IDealerConfigurationService
    {
        Task<DealerConfigurationResponse> GetDealerConfiguration(string roofTopId, string communityId);
        Task<DealerConfigurationResponse> GetDealerConfiguration(int dealerId);
        Task<DealerInvitationContentResponse> GetDealerInvitationContent(string roofTopId, string communityId);
    }
}
