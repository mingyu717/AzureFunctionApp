using System.Collections.Generic;
using Service.Contract.Request;
using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IDealerConfigurationService
    {
        DealerConfigurationResponse GetDealerConfigurationById(int dealerId);
        DealerConfigurationResponse GetDealerConfigurationByRoofTopIdAndCommunityId(string roofTopId, string communityId);
        List<string> GetDealersCsvSources();
        Task<int> AddDealerConfiguration(DealerConfigurationCreateRequest objDealerConfiguration);
        Task EditDealerConfiguration(DealerConfigurationUpdateRequest objEditDealerConfiguration, int dealerId);
        Task DeleteDealerConfiguration(int dealerId);
        bool CheckDealerExist(DealerSearchCriteria searchCriteria, int id, string rooftopId, string communityid);
        DealerInvitationContentResponse GetInvitationContent(string roofTopId, string communityId);
    }
}
