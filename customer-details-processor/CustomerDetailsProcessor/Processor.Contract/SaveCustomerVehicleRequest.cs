namespace Processor.Contract
{
    public class SaveCustomerVehicleRequest
    {
        public int CustomerNo { get; set; }
        public string CustomerEmail { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string RooftopId { get; set; }
        public string CommunityId { get; set; }
        public int VehicleNo { get; set; }
        public string RegistrationNo { get; set; }
        public string VinNumber { get; set; }
        public string MakeCode { get; set; }
        public string ModelCode { get; set; }
        public string ModelYear { get; set; }
        public string ModelDescription { get; set; }
        public string VariantCode { get; set; }
        public int NextServiceMileage { get; set; }
    }
}