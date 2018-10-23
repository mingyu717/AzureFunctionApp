using Service.Contract.Response;

namespace Service.Contract.Models
{
    public class CDKRecommendedServiceResponse
    {
        public GetRecommendedServicesResponse Results { get; set; }
        public ErrorResponse Result { get; set; }
    }
}
