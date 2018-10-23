using System.Collections.Generic;

namespace Service.Contract.Models
{
    public class CdkRecommendedServicesResponse
    {
        public IEnumerable<CdkVehicleService> PriceListData { get; set; }
    }
}