using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract.Models
{
    public class CdkCreateServiceBookingRequest
    {
        public string RooftopId { get; set; }

        public string CustomerId { get; set; }

        public string EmailAddress { get; set; }

        public string MobileTelNo { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string VehicleRegistrationNo { get; set; }

        public string VehMakeCode { get; set; }

        public string VehModelCode { get; set; }

        public string VehVariantCode { get; set; }

        public List<JobData> Jobs { get; set; }

        public DateTime JobDate { get; set; }

        public string TransportMethod { get; set; }

        public string AdvisorID { get; set; }

        public string AdvisorDropOffTimeCode { get; set; }

        public bool SendConfirmationMail { get; set; }
    }
}
