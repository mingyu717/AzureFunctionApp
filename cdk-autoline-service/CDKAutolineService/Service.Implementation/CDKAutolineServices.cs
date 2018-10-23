using Microsoft.ApplicationInsights;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using System;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CDKAutolineServices : ICDKAutolineServices
    {
        private readonly ICustomerService _customerService;
        private readonly ITokenService _tokenService;
        private readonly ICdkCustomerService _cdkCustomerService;
        private readonly TelemetryClient _telemetryClient;

        public CDKAutolineServices(ICustomerService customerService,
            ITokenService tokenService, ICdkCustomerService cdkCustomerService, TelemetryClient telemetryClient)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _cdkCustomerService = cdkCustomerService ?? throw new ArgumentNullException(nameof(cdkCustomerService));
            _telemetryClient = telemetryClient;
        }

        public async Task<string> RegisterCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest)
        {
            customerVehicleRegisterRequest.AppToken = await _tokenService.GetAppToken(customerVehicleRegisterRequest.CommunityId, customerVehicleRegisterRequest.RoofTopId);

            var cdkCustomer = _cdkCustomerService.MapCdkCustomer(customerVehicleRegisterRequest);
            // Send request to register customer on CDK auto line api
            var registerApiResponse = await _customerService.RegisterCustomer(customerVehicleRegisterRequest, cdkCustomer);
            if (registerApiResponse == null || !registerApiResponse.Success)
            {
                throw new CDKAutolineException(UtilityHelper.SerializeObject(registerApiResponse?.Errors));
            }

            await _cdkCustomerService.SaveCdkCustomer(cdkCustomer);
            return $"Customer {cdkCustomer.CustomerLoginId} registered successfully";
        }

        public async Task<VerifyPasswordResponse> VerifyCustomer(CustomerVerifyRequest customerVerifyRequest)
        {
            var cdkCustomer = _cdkCustomerService.GetCdkCustomer(customerVerifyRequest.CommunityId, customerVerifyRequest.CustomerNo);
            if (cdkCustomer == null)
            {
                throw new CustomerNotRegisterException($"Customer with CustomerNo {customerVerifyRequest.CustomerNo} has not been requested");
            }

            // Request Token from CDK Autoline service.
            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var token =
                await _tokenService.GetCustomerToken(cdkCustomer, customerVerifyRequest.RoofTopId);
            _telemetryClient?.TrackDependency("CDKAutolineService", "GetCustomerToken",
                $"CommunictyId: {customerVerifyRequest.CommunityId}, CustomerLoginId: {cdkCustomer.CustomerLoginId}", startTime,
                timer.Elapsed,
                token != null);
            
            return new VerifyPasswordResponse {CDKAutolineToken = token, CustomerLoginId = cdkCustomer.CustomerLoginId};
        }
    }
}