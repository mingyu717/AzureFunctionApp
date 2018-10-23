namespace Service.Contract.Response
{
    public class CreateServiceBookingResponse
    {
        public int AppointmentId { get; set; }
        public int WipNo { get; set; }
        public ErrorResponse Result { get; set; }
    }
}
