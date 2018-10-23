namespace Service.Contract.Models
{
    public class GetCdkRecommendedServiceRequest
    {
        public string CommunityId { get; set; }
        public string RooftopId { get; set; }
        public string MakeCode { get; set; }
        public string ModelCode { get; set; }
        public string EstOdometer { get; set; }
        public string EstVehicleAgeMonths { get; set; }
    }
}
