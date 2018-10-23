using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class GetRecommendedServicesRequest
    {
        [Required, Range(0, int.MaxValue, ErrorMessage = "Dealer need be equal or larger than 0")]
        public int DealerId { get; set; }
        [Required]
        public string MakeCode { get; set; }
        [Required]
        public string ModelCode { get; set; }
        [Required]
        public string EstOdometer { get; set; }
        public string ModelYear { get; set; }
    }
}