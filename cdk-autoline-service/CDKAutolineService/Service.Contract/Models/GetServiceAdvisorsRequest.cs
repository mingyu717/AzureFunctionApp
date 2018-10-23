using System;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class GetServiceAdvisorsRequest
    {
        [Required]
        public string CommunityId { get; set; }
        [Required]
        public int CustomerNo { get; set; }
        public string CustomerId { get; set; }
        [Required]
        public string RooftopId { get; set; }
        [Required]
        public string TransportMethod { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        [Required]
        public string DropOffTime { get; set; }
    }
}
