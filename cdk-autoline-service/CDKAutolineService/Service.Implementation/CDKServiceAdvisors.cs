using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using System;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CDKServiceAdvisors : BaseService, ICDKServiceAdvisors
    {
        private IRestApiClient _restApiClient;
        private ITokenService _tokenService;
        private ICdkCustomerDAL _cdkCustomerDAL;
        private TelemetryClient _telemetryClient;
        private string _cdkAutolineUrl;
        private readonly string ServiceAdvisorsDateFormat = "yyyy-MM-dd";
        private const string GetServiceAdvisorsUrl = "GetServiceAdvisors";

        public CDKServiceAdvisors(IRestApiClient restApiClient, ITokenService tokenService,
            ICdkCustomerDAL cdkCustomerDAL, TelemetryClient telemetryClient, string cdkAutolineUrl)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _cdkCustomerDAL = cdkCustomerDAL ?? throw new ArgumentNullException(nameof(cdkCustomerDAL));
            _telemetryClient = telemetryClient;
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
        }

        public async Task<GetServiceAdvisorsResponse> GetServiceAdvisors(GetServiceAdvisorsRequest getServiceBookingRequest)
        {
            if (getServiceBookingRequest == null) throw new ArgumentNullException(nameof(getServiceBookingRequest));

            var customer = _cdkCustomerDAL.GetCdkCustomer(getServiceBookingRequest.CommunityId, getServiceBookingRequest.CustomerNo);
            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);

            // if token is null then requesting for new token.
            // Otherwise used the stored token in database.
            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var customerToken = await _tokenService.GetCustomerToken(customer, getServiceBookingRequest.RooftopId);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetCustomerToken",
                customer.CustomerLoginId, startTime,
                timer.Elapsed,
                customerToken != null);

            startTime = DateTime.Now;
            timer = System.Diagnostics.Stopwatch.StartNew();
            var getServiceAdvisorsResponse = await RequestGetServiceAdvisors(getServiceBookingRequest, customer.CustomerLoginId, customerToken);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetServiceAdvisors",
                JsonConvert.SerializeObject(getServiceBookingRequest), startTime,
                timer.Elapsed,
                getServiceAdvisorsResponse != null);

            if (getServiceAdvisorsResponse == null || !getServiceAdvisorsResponse.Success)
            {
                var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(getServiceAdvisorsResponse?.Errors));
                _telemetryClient?.TrackException(cdkAutolineException);
                throw cdkAutolineException;
            }

            return (getServiceAdvisorsResponse.Result as GetServiceAdvisorsResponse);
        }

        internal async Task<ApiResponse> RequestGetServiceAdvisors(GetServiceAdvisorsRequest serviceRequest, string customerLoginid, string token)
        {
            IsoDateTimeConverter converter = new IsoDateTimeConverter { DateTimeFormat = ServiceAdvisorsDateFormat };
            serviceRequest.CustomerId = customerLoginid;
            var requestBody = UtilityHelper.SerializeObject(serviceRequest, converter);

            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, serviceRequest.CommunityId), GetServiceAdvisorsUrl),
                ContentType = Constants.ContentTypeJson,
                Accept = Constants.ContentTypeJson
            };
            AddRequestHeader(request,
                $"{Constants.DataHubTokenHeaderValuePrefix}{token}");
            return await _restApiClient.Invoke<GetServiceAdvisorsResponse>(request);
        }
    }
}
