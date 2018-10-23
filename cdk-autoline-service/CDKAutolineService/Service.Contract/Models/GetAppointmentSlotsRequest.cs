using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class GetAppointmentSlotsRequest
    {
        [Required]
        public string CommunityId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "CustomerNo need be bigger than 0")]
        public int CustomerNo { get; set; }
        [Required]
        public string RooftopId { get; set; }
        [Required]
        public DateTime InitialDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public List<string> JobCode { get; set; }
    }
}
