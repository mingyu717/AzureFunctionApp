using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;

namespace Service.Implementation
{
    public class CDKVehicleMaintenanceService : BaseService, ICDKVehicleMaintenanceService
    {
        private readonly IRestApiClient _restApiClient;
        private readonly ITokenService _tokenService;
        private readonly TelemetryClient _telemetryClient;
        private readonly string _cdkAutolineUrl;
        private const string GetRecommendedServiceUrl = "GetRecommendedService";

        public CDKVehicleMaintenanceService(IRestApiClient restApiClient, string cdkAutolineUrl, ITokenService tokenService, TelemetryClient telemetryClient)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _telemetryClient = telemetryClient;
        }

        public async Task<GetRecommendedServicesResponse> GetRecommendedServices(GetRecommendedServicesRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var requestBody = UtilityHelper.SerializeObject(new
            {
                request.RooftopId,
                request.MakeCode,
                request.ModelCode,
                request.EstOdometer,
                request.EstVehicleAgeMonths
            });

            var cdkApiRequest = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl,
                    request.CommunityId), GetRecommendedServiceUrl),
                ContentType = Constants.ContentTypeJson
            };

            var appToken = await _tokenService.GetAppToken(request.CommunityId, request.RooftopId);

            AddRequestHeader(cdkApiRequest, $"{Constants.DataHubAppTokenHeaderValuePrefix}{appToken}");

            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var getRecommendedServiceApiResponse = await _restApiClient.Invoke<CDKRecommendedServiceResponse>(cdkApiRequest);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetRecommendedService",
                JsonConvert.SerializeObject(cdkApiRequest), startTime,
                timer.Elapsed,
                getRecommendedServiceApiResponse != null);
            _telemetryClient?.TrackTrace($"CDKAutolineService GetRecommendedService response: {JsonConvert.SerializeObject(getRecommendedServiceApiResponse)}");

            if (getRecommendedServiceApiResponse?.Result == null || !getRecommendedServiceApiResponse.Success
                || !(getRecommendedServiceApiResponse.Result is CDKRecommendedServiceResponse cdkRecommendedServiceResponse))
            {
                throw new CDKAutolineException(UtilityHelper.SerializeObject(getRecommendedServiceApiResponse?.Errors));
            }

            return cdkRecommendedServiceResponse.Results;
        }
    }
}
