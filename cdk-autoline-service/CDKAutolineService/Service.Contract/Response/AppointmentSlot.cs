using System.Collections.Generic;

namespace Service.Contract.Response
{
    public class AppointmentSlot
    {
        public string Date { get; set; }
        public List<string> Slots { get; set; }
    }
}
