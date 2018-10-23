using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class CreateServiceBookingRequest
    {
        [Required]
        public string RooftopId { get; set; }

        [Required]
        public string CommunityId { get; set; }

        [Required]
        public int CustomerNo { get; set; }

        [EmailAddressValidate]
        public string EmailAddress { get; set; }

        public string MobileTelNo { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public string VehicleRegistrationNo { get; set; }

        [Required]
        public string VehMakeCode { get; set; }

        [Required]
        public string VehModelCode { get; set; }

        [Required]
        public string VehVariantCode { get; set; }

        public int ActualMileage { get; set; }

        public List<JobData> Jobs { get; set; }

        [Required]
        public DateTime JobDate { get; set; }

        public string TransportMethod { get; set; }

        public string AdvisorId { get; set; }

        public string AdvisorDropOffTimeCode { get; set; }
    }
}
