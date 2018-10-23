namespace Service.Implementation
{
    public class Constants
    {
        public class ExceptionMessages
        {
            public const string DealerIdRequired = "The DealerId field is required";
            public const string RoofTopIdRequired = "The RooftopId field is required.";
            public const string DealerNameRequired = "The DealerName field is required.";
            public const string CommunityIdRequired = "The communityid field is required";
            public const string InvalidRequest = "Invalid request.";
            public const string NotFound = "not found";

            public const string DealerConfigurationAlreadyExists =
                "Dealer with same rooftop id and community id already exist";

            public const string InvalidCommunicationMethod = "Invalid communication method";
            public const string InvalidCsvSource = "Invalid csv source";
        }
    }
}