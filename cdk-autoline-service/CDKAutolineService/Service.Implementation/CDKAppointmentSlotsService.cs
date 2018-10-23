using AutoMapper;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CDKAppointmentSlotsService : BaseService, ICDKAppointmentSlotsService
    {
        private readonly IRestApiClient _restApiClient;
        private readonly ICdkCustomerDAL _cdkCustomerDAL;
        private readonly ITokenService _tokenService;
        private readonly TelemetryClient _telemetryClient;
        private readonly IValidateRequest _validateRequest;

        private readonly IMapper _mapper;
        private readonly string _cdkAutolineUrl;
        private readonly string AppointmentSlotsDateFormat = "yyyy-MM-dd";
        private const string GetAppointmentSlotsUrl = "GetAppointmentSlots";


        public CDKAppointmentSlotsService(IRestApiClient restApiClient, ICdkCustomerDAL cdkCustomerDAL,
            ITokenService tokenService, TelemetryClient telemetryClient, IValidateRequest validateRequest, IMapper mapper, string cdkAutolineUrl)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _cdkCustomerDAL = cdkCustomerDAL ?? throw new ArgumentNullException(nameof(cdkCustomerDAL));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _telemetryClient = telemetryClient;
            _validateRequest = validateRequest ?? throw new ArgumentNullException(nameof(validateRequest));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
        }

        public async Task<GetAppointmentSlotsResponse> GetAppointmentSlots(GetAppointmentSlotsRequest getAppointmentSlotsRequest)
        {
            if (getAppointmentSlotsRequest == null) throw new ArgumentNullException(nameof(getAppointmentSlotsRequest));

            var validateMessages = _validateRequest.ValidateRequestData(getAppointmentSlotsRequest);
            if (validateMessages != null)  throw new InvalidRequestException(validateMessages.AsEnumerable().ToList());

            var customer = _cdkCustomerDAL.GetCdkCustomer(getAppointmentSlotsRequest.CommunityId, getAppointmentSlotsRequest.CustomerNo);
            if (customer == null) throw new InvalidCustomerException(ExceptionMessages.InvalidCustomer);

            // if token is null then requesting for new token.
            // Otherwise used the stored token in database.
            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var customerToken = await _tokenService.GetCustomerToken(customer, getAppointmentSlotsRequest.RooftopId);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetCustomerToken",
                customer.CustomerLoginId, startTime,
                timer.Elapsed,
                customerToken != null);

            startTime = DateTime.Now;
            timer = System.Diagnostics.Stopwatch.StartNew();
            var cdkGetAppointmentSlotsRequest = MapAppointmentSlots(getAppointmentSlotsRequest);
            var getServiceAdvisorsResponse = await RequestGetAppointmentSlots(cdkGetAppointmentSlotsRequest, getAppointmentSlotsRequest.CommunityId,
                customer.CustomerLoginId, customerToken);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetAppointmentSlots",
                JsonConvert.SerializeObject(cdkGetAppointmentSlotsRequest), startTime,
                timer.Elapsed,
                getServiceAdvisorsResponse != null);

            if (getServiceAdvisorsResponse == null || !getServiceAdvisorsResponse.Success)
            {
                var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(getServiceAdvisorsResponse?.Errors));
                _telemetryClient?.TrackException(cdkAutolineException);
                throw cdkAutolineException;
            }

            return (getServiceAdvisorsResponse.Result as GetAppointmentSlotsResponse);
        }

        internal async Task<ApiResponse> RequestGetAppointmentSlots(CDKGetAppointmentSlotsRequest serviceRequest, string communityId, string customerLoginid, string token)
        {
            IsoDateTimeConverter converter = new IsoDateTimeConverter { DateTimeFormat = AppointmentSlotsDateFormat };
            var requestBody = UtilityHelper.SerializeObject(serviceRequest, converter);

            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, communityId), GetAppointmentSlotsUrl),
                ContentType = Constants.ContentTypeJson,
                Accept = Constants.ContentTypeJson
            };
            AddRequestHeader(request,
                $"{Constants.DataHubTokenHeaderValuePrefix}{token}");
            return await _restApiClient.Invoke<GetAppointmentSlotsResponse>(request);
        }

        internal CDKGetAppointmentSlotsRequest MapAppointmentSlots(GetAppointmentSlotsRequest request)
        {
            return _mapper.Map<GetAppointmentSlotsRequest, CDKGetAppointmentSlotsRequest>(request);
        }
    }
}
