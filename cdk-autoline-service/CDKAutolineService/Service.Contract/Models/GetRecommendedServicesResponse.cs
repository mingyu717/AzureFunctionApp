using System.Collections.Generic;

namespace Service.Contract.Models
{
    public class GetRecommendedServicesResponse
    {
        public IEnumerable<VehicleService> PriceListData { get; set; }
    }
}
