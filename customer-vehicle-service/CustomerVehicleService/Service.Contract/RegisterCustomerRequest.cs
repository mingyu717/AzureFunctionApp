namespace Service.Contract
{
    public class RegisterCustomerRequest
    {
        public string RoofTopId { get; set; }
        public int CustomerNo { get; set; }
        public string CommunityId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleId { get; set; }
        public string MakeCode { get; set; }
        public string ModelCode { get; set; }
        public string VariantCode { get; set; }
        public string RegistrationNumber { get; set; }
    }
}