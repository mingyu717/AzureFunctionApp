using System.Collections.Generic;

namespace Service.Contract.Response
{
    public class GetAppointmentSlotsResponse
    {
        public List<string> WorksDiary { get; set; }
        public List<WorksDiaryDetail> WorksDiaryDetails { get; set; }
        public List<string> NonWorkingDates { get; set; }
        public List<AppointmentOption> Option { get; set; }
        public ErrorResponse Result { get; set; }
    }
}
