using System.ComponentModel.DataAnnotations;

namespace Service.Contract.RequestModel
{
    public class CreateAppointmentRequest
    {
        [Required, Range(1, int.MaxValue, ErrorMessage = "DealerID need be bigger than 0")]
        public int DealerId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "JobId need be bigger than 0")]
        public int JobId { get; set; }

        [Required]
        public string CustomerFirstName { get; set; }

        [Required]
        public string CustomerSurName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        public string VehicleRegistrationNumber { get; set; }

        [Required]
        public string JobDate { get; set; }

        public string TransportMethod { get; set; }

        public string DropOffTime { get; set; }
    }
}
