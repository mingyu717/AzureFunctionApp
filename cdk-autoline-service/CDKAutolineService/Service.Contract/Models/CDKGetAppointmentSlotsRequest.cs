using System;
using System.Collections.Generic;

namespace Service.Contract.Models
{
    public class CDKGetAppointmentSlotsRequest
    {
        public string RooftopId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> JobCode { get; set; }
    }
}
