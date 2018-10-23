using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class CustomerVerifyRequest
    {
        [Required]
        public int CustomerNo { get; set; }

        [Required]
        public string CommunityId { get; set; }

        [Required]
        public string RoofTopId { get; set; }
    }
}