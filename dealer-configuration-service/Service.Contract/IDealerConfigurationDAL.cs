using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IDealerConfigurationDAL
    {
        DealerConfiguration GetDealerConfigurationById(int dealerId);
        DealerConfiguration GetDealerConfigurationByRoofTopIdAndCommunityId(string roofTopId, string communityId);
        List<int> GetDealersCsvSources();
        Task<int> AddDealerConfiguration(DealerConfiguration objDealerConfiguration);
        Task EditDealerConfiguration(DealerConfiguration objDealerConfiguration, int dealerId);
        Task DeleteDealerConfiguration(int dealerId);
    }
}
