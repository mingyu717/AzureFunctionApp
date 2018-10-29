using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IDealerConfigurationClient
    {
        Task<DealerConfigurationResponse> GetDealerConfiguration(string roofTopId, string communityId);
        Task<List<string>> GetDealerCsvSources();
        bool ValidateEmailOrSmsByCommunicationMethod(DealerConfigurationResponse dealerConfigurationResponse, string phoneNumber, string email);
    }
}
