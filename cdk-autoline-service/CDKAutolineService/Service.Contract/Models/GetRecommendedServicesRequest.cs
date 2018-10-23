using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class GetRecommendedServicesRequest
    {
        [Required]
        public string CommunityId { get; set; }
        [Required]
        public string RooftopId { get; set; }
        [Required]
        public string MakeCode { get; set; }
        [Required]
        public string ModelCode { get; set; }
        [Required]
        public string EstOdometer { get; set; }
        public string EstVehicleAgeMonths { get; set; }
    }
}
