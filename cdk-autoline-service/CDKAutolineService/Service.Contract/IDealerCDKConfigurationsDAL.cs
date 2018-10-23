using Service.Contract.DbModels;

namespace Service.Contract
{
    public interface IDealerCDKConfigurationsDAL
    {
        DealerCDKConfiguration GetDealerCDKConfigurations(string roofTopId, string communityId);
    }
}
