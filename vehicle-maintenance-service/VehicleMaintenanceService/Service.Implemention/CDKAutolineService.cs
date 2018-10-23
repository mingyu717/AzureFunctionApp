using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Models;

namespace Service.Implemention
{
    public class CDKAutolineService : ICDKAutolineService
    {
        private readonly IRestfulClient _restfulClient;
        private readonly IMapper _mapper;
        private const string GetRecommendedServicesUrl = "GetRecommendedServices";
        private const string DistanceBasedServicesDescriptionPattern = "^[0-9]{1,3}(\\w+)/[0-9]{1,2}(\\w+) service$";
        private const string AnnualServicesDescriptionPattern = "^(Annual|Generic){1}(\\s|\\s\\w+\\s)Service$";
        private const string MinEstVehicleAgeMonths = "6";
        private const int MaxDistanceBasedServices = 3;
        private const int MinVehicleAge = 0;
        private const int OldestModelYear = 1900;
        private readonly TelemetryClient _telemetryClient;

        public CDKAutolineService(IRestfulClient restfulClient, TelemetryClient telemetryClient)
        {
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient));
            _telemetryClient = telemetryClient;
            _mapper = AutoMapper.Instance;
        }

        public async Task<GetRecommendedServicesResponse> GetRecommendedServices(GetRecommendedServicesRequest request, DealerConfigurationResponse dealer)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (dealer == null) throw new ArgumentNullException(nameof(dealer));

            var cdkRecommendedServiceRequest = MapRecommendedServicesRequest(request, dealer);

            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var cdkRecommendedServiceResponse = await _restfulClient.PostAsync<GetCdkRecommendedServiceRequest, CdkRecommendedServicesResponse>(
                GetRecommendedServicesUrl, cdkRecommendedServiceRequest);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetRecommendedServices",
                JsonConvert.SerializeObject(request), startTime,
                timer.Elapsed,
                cdkRecommendedServiceResponse != null);

            return MapRecommendedServicesReponse(cdkRecommendedServiceResponse);
        }

        private GetRecommendedServicesResponse MapRecommendedServicesReponse(CdkRecommendedServicesResponse cdkRecommendedServicesResponse)
        {
            if (cdkRecommendedServicesResponse?.PriceListData == null)
            {
                return null;
            }

            var getRecommendedServicesResponse = new GetRecommendedServicesResponse
            {
                DistanceBasedServices = new List<ServiceResponse>(),
                AdditionalServices = new List<ServiceResponse>()
            };

            foreach (var cdkVehicleService in cdkRecommendedServicesResponse.PriceListData)
            {
                if (IsDistanceBasedServices(cdkVehicleService.JobDescription))
                {
                    if (getRecommendedServicesResponse.DistanceBasedServices.Count < MaxDistanceBasedServices)
                    {
                        getRecommendedServicesResponse.DistanceBasedServices.Add(_mapper.Map<CdkVehicleService, ServiceResponse>(cdkVehicleService));
                    }
                }
                else
                {
                    getRecommendedServicesResponse.AdditionalServices.Add(_mapper.Map<CdkVehicleService, ServiceResponse>(cdkVehicleService));
                }
            }

            return getRecommendedServicesResponse;
        }

        internal bool IsDistanceBasedServices(string jobDescription)
        {
            return !string.IsNullOrEmpty(jobDescription) && 
                   (Regex.Match(jobDescription, DistanceBasedServicesDescriptionPattern).Success || Regex.Match(jobDescription, AnnualServicesDescriptionPattern).Success);
        }

        internal GetCdkRecommendedServiceRequest MapRecommendedServicesRequest(GetRecommendedServicesRequest request, DealerConfigurationResponse dealer)
        {
            var getCdkRecommendedServiceRequest = _mapper.Map<GetRecommendedServicesRequest, GetCdkRecommendedServiceRequest>(request);

            getCdkRecommendedServiceRequest.EstVehicleAgeMonths = GetEstVehicleAgeMonths(request.ModelYear);

            return _mapper.Map(dealer, getCdkRecommendedServiceRequest);
        }

        internal string GetEstVehicleAgeMonths(string modelYearString)
        {
            if (Int32.TryParse(modelYearString, out var modelYear) && modelYear > OldestModelYear)
            {
                var vehicleAge = DateTime.Now.Year - modelYear;
                return vehicleAge > MinVehicleAge ? (vehicleAge * 12).ToString() : MinEstVehicleAgeMonths;
            }

            return "";
        }
    }
}