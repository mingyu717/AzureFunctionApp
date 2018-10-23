using System.ComponentModel.DataAnnotations;

namespace Service.Contract
{
    public class SaveCustomerVehicleRequest
    {
        [Required, Range(1, int.MaxValue, ErrorMessage = "CustomerNo need be bigger than 0")]
        public int CustomerNo { get; set; }

        [EmailAddressValidate(ErrorMessage = "The Customer Email field is not a valid e-mail address.")]
        public string CustomerEmail { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string CommunityId { get; set; }

        [Required]
        public string RooftopId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "VehicleNo need be bigger than 0")]
        public int VehicleNo { get; set; }

        [Required]
        public string RegistrationNo { get; set; }

        [Required]
        public string VinNumber { get; set; }
        
        [Required]
        public string MakeCode { get; set; }

        [Required]
        public string ModelCode { get; set; }

        [Required]
        public string VariantCode { get; set; }
        public string ModelYear { get; set; }
        public string ModelDescription { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
        public int LastKnownMileage { get; set; }
        public int NextServiceMileage { get; set; }
    }
}