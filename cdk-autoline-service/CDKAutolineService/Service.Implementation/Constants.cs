namespace Service.Implementation
{
    public class Constants
    {
        public const string ContentTypeJson = "application/json";
        public const string AuthorizationHeaderKey = "Authorization";
        public const string DataHubHashHeaderValuePrefix = "DataHub-Hash ";
        public const string DataHubAppTokenHeaderValuePrefix = "DataHub-AppToken ";
        public const string DataHubTokenHeaderValuePrefix = "DataHub-Token ";

        public class ExceptionMessages
        {
            public const string InvalidRequest = "Invalid request.";
        }
    }
}