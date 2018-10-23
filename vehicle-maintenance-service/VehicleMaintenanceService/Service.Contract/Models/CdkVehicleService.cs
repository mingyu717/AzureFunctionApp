namespace Service.Contract.Models
{
    public class CdkVehicleService
    {
        public string JobCode { get; set; }
        public string ProductCode { get; set; }
        public string JobDescription { get; set; }
        public string JobTime { get; set; }
        public string JobPrice { get; set; }
        public string JobTypeCode { get; set; }
        public string JobExtDescription { get; set; }
        public string ParentJobCode { get; set; }
    }
}
