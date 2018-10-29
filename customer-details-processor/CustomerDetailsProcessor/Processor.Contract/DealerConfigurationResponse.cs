namespace Processor.Contract
{
    public class DealerConfigurationResponse
    {
        public int DealerId { get; set; }
        public string DealerName { get; set; }
        public string RooftopId { get; set; }
        public string CommunityId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int CommunicationMethod { get; set; }
        public string EmailAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AppThemeName { get; set; }
        public bool ShowTransportations { get; set; }
        public bool ShowAdvisors { get; set; }
        public bool ShowPrice { get; set; }
        public int MinimumFreeCapacity { get; set; }
        public int CsvSource { get; set; }
    }
}