using Service.Contract.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Service.Contract
{
    public class CreateServiceBookingRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = ExceptionMessages.InvalidDealerId)]
        public int DealerId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = ExceptionMessages.InvalidCustomerNo)]
        public int CustomerNo { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = ExceptionMessages.InvalidVehicleNo)]
        public int VehicleNo { get; set; }

        public int ActualMileage { get; set; }

        public List<JobData> Jobs { get; set; }

        [Required]
        public DateTime JobDate { get; set; }

        public string TransportMethod { get; set; }

        public string AdvisorID { get; set; }

        public string AdvisorDropOffTimeCode { get; set; }
    }
}