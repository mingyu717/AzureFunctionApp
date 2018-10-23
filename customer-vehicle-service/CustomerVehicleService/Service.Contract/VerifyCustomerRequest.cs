namespace Service.Contract
{
    public class VerifyCustomerRequest
    {
        public int CustomerNo { get; set; }
        public string RoofTopId { get; set; }
        public string CommunityId { get; set; }
    }
}