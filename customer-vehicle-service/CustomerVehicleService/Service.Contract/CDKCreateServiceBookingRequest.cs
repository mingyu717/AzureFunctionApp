using System;
using System.Collections.Generic;

namespace Service.Contract
{
    public class CDKCreateServiceBookingRequest
    {
        public string RooftopId { get; set; }
        public string CommunityId { get; set; }
        public int CustomerNo { get; set; }
        public string EmailAddress { get; set; }
        public string MobileTelNo { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public int VehicleNo { get; set; }
        public string VehMakeCode { get; set; }
        public string VehModelCode { get; set; }
        public string VehVariantCode { get; set; }
        public int ActualMileage { get; set; }
        public List<JobData> Jobs { get; set; }
        public DateTime JobDate { get; set; }
        public string TransportMethod { get; set; }
        public string AdvisorId { get; set; }
        public string AdvisorDropOffTimeCode { get; set; }
    }
}