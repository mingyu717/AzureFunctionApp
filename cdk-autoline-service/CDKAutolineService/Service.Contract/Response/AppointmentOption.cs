using System.Collections.Generic;

namespace Service.Contract.Response
{
    public class AppointmentOption
    {
        public string OptionID { get; set; }
        public string OptionDisplayName { get; set; }
        public string OptionDescription { get; set; }
        public double OptionPrice { get; set; }
        public string OptionAdvisor { get; set; }
        public string InitialAppFieldName { get; set; }
        public bool InitialAppTimeRequired { get; set; }
        public string SecondAppFieldName { get; set; }
        public bool SecondAppTimeRequired { get; set; }
        public bool RequestAddress { get; set; }
        public bool RequestPostCode { get; set; }
        public string FirstAddressLabel { get; set; }
        public bool RequestSecondAddress { get; set; }
        public bool RequestSecondPostCode { get; set; }
        public string SecondAddressLabel { get; set; }
        public bool AdvisorsPhotos { get; set; }
        public List<AppointmentSlot> Slots { get; set; }
        public AppointmentResource Resource { get; set; }
    }
}
