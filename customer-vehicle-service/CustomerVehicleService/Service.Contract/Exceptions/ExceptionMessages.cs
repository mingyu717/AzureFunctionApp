namespace Service.Contract.Exceptions
{
    public static class ExceptionMessages
    {
        public static string InvitationExpired => "Invitation expired";
        public static string InvalidPhoneNumber => "Invalid phone number";
        public static string InvalidRequest => "Invalid request";
        public static string InvalidRoofTopAndCommunityId => "Invalid rooftopid and communityid";
        public static string UnableToSendEmail => "Unable to send the email from {0} to {1}";
        public static string InvalidCustomer => "invalid customer";
        public static string InvalidCustomerVehicle => "invalid customer vehicle";
        public const string InvalidDealerId = "The field DealerId is invalid";
        public const string InvalidCustomerNo = "The field CustomerNo is invalid";
        public const string InvalidVehicleNo = "The field VehicleNo is invalid";
    }
}