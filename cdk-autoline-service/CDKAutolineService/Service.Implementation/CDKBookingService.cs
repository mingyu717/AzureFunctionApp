using Microsoft.ApplicationInsights;
using Newtonsoft.Json.Converters;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Service.Contract.DbModels;

namespace Service.Implementation
{
    public class CDKBookingService : BaseService, ICDKBookingService
    {
        private readonly IRestApiClient _restApiClient;
        private readonly ITokenService _tokenService;
        private readonly ICdkCustomerDAL _cdkCustomerDAL;
        private readonly TelemetryClient _telemetryClient;
        private readonly string _cdkAutolineUrl;
        private readonly string ServiceBookingDateFormat = "yyyy-MM-dd";
        private const string CreateServiceBookingUrl = "CreateServiceBooking";

        public CDKBookingService(IRestApiClient restApiClient, ITokenService tokenService,
            ICdkCustomerDAL cdkCustomerDAL, TelemetryClient telemetryClient, string cdkAutolineUrl)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _cdkCustomerDAL = cdkCustomerDAL ?? throw new ArgumentNullException(nameof(cdkCustomerDAL));
            _telemetryClient = telemetryClient;
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
        }

        public async Task<CreateServiceBookingResponse> CreateServiceBooking(CreateServiceBookingRequest createServiceBookingRequest)
        {
            if (createServiceBookingRequest == null) throw new ArgumentNullException(nameof(createServiceBookingRequest));

            var customer = _cdkCustomerDAL.GetCdkCustomer(createServiceBookingRequest.CommunityId, createServiceBookingRequest.CustomerNo);
            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);

            // if token is null then requesting for new token.
            // Otherwise used the stored token in database.
            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var customerToken = await _tokenService.GetCustomerToken(customer, createServiceBookingRequest.RooftopId);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetCustomerToken",
                customer.CustomerLoginId, startTime,
                timer.Elapsed,
                customerToken != null);

            startTime = DateTime.Now;
            timer = System.Diagnostics.Stopwatch.StartNew();

            var cdkCreateServiceBookingRequest = MapToCdkCreateServiceBookingRequest(createServiceBookingRequest, customer);

            var cdkCreateServiceBookingResponse = await RequestCreateServiceBooking(cdkCreateServiceBookingRequest, createServiceBookingRequest.CommunityId, customerToken);
            _telemetryClient?.TrackDependency("CDKAutolineService", "CreateServiceBooking",
                JsonConvert.SerializeObject(cdkCreateServiceBookingRequest), startTime,
                timer.Elapsed,
                cdkCreateServiceBookingResponse != null);

            if (cdkCreateServiceBookingResponse == null || !cdkCreateServiceBookingResponse.Success)
            {
                var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(cdkCreateServiceBookingResponse?.Errors));
                _telemetryClient?.TrackException(cdkAutolineException);
                throw cdkAutolineException;
            }

            return (cdkCreateServiceBookingResponse.Result as CreateServiceBookingResponse);
        }

        internal CdkCreateServiceBookingRequest MapToCdkCreateServiceBookingRequest(CreateServiceBookingRequest createServiceBookingRequest, CdkCustomer cdkCustomer)
        {
            var cdkCreateServiceBookingRequest =
                new CdkCreateServiceBookingRequest
                {
                    RooftopId = createServiceBookingRequest.RooftopId,
                    CustomerId = cdkCustomer.CustomerLoginId,
                    EmailAddress = createServiceBookingRequest.EmailAddress,
                    MobileTelNo = createServiceBookingRequest.MobileTelNo,
                    FirstName = createServiceBookingRequest.FirstName,
                    SurName = createServiceBookingRequest.SurName,
                    VehicleRegistrationNo = createServiceBookingRequest.VehicleRegistrationNo,
                    VehMakeCode = createServiceBookingRequest.VehMakeCode,
                    VehModelCode = createServiceBookingRequest.VehModelCode,
                    VehVariantCode = createServiceBookingRequest.VehVariantCode,
                    Jobs = createServiceBookingRequest.Jobs,
                    JobDate = createServiceBookingRequest.JobDate,
                    TransportMethod = createServiceBookingRequest.TransportMethod,
                    AdvisorID = createServiceBookingRequest.AdvisorId,
                    AdvisorDropOffTimeCode = createServiceBookingRequest.AdvisorDropOffTimeCode,
                    SendConfirmationMail = true
                };

            return cdkCreateServiceBookingRequest;
        }

        internal async Task<ApiResponse> RequestCreateServiceBooking(CdkCreateServiceBookingRequest serviceRequest, string communityId, string token)
        {
            IsoDateTimeConverter converter = new IsoDateTimeConverter { DateTimeFormat = ServiceBookingDateFormat };
            var requestBody = UtilityHelper.SerializeObject(serviceRequest, converter);

            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, communityId), CreateServiceBookingUrl),
                ContentType = Constants.ContentTypeJson,
                Accept = Constants.ContentTypeJson
            };
            AddRequestHeader(request,
                $"{Constants.DataHubTokenHeaderValuePrefix}{token}");
            return await _restApiClient.Invoke<CreateServiceBookingResponse>(request);
        }
    }
}
