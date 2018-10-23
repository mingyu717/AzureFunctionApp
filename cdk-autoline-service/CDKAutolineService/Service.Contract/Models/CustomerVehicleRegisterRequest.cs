using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class CustomerVehicleRegisterRequest
    {
        [Required]
        public string RoofTopId { get; set; }

        [Required]
        public int CustomerNo { get; set; }

        [Required]
        public string CommunityId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string AppToken { get; set; }
    }
}