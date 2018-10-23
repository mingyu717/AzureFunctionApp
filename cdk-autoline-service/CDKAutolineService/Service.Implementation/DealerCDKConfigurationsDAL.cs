using System;
using Service.Contract;
using Service.Contract.DbModels;
using System.Linq;

namespace Service.Implementation
{
    public class DealerCDKConfigurationsDAL : IDealerCDKConfigurationsDAL
    {
        private readonly CDKAutolineContext _context;
        public DealerCDKConfigurationsDAL(CDKAutolineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DealerCDKConfiguration GetDealerCDKConfigurations(string roofTopId, string communityId)
        {
            return _context.DealerCDKConfigurations.FirstOrDefault(x => x.RoofTopId == roofTopId && x.CommunityId == communityId);
        }
    }
}
