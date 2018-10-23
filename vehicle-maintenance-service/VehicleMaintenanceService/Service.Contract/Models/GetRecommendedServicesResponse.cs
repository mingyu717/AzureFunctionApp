using System.Collections.Generic;

namespace Service.Contract.Models
{
    public class GetRecommendedServicesResponse
    {
        public List<ServiceResponse> DistanceBasedServices { get; set; }
        public List<ServiceResponse> AdditionalServices { get; set; }
    }
}