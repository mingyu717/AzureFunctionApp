namespace Service.Contract.Response
{
    public class DealerInvitationContentResponse
    {
        public int DealerId { get; set; }
        public string EmailContent { get; set; }
        public string EmailSubject { get; set; }
        public string SmsContent { get; set; }
    }
}
