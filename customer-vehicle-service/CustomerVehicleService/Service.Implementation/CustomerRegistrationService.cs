using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Response;

namespace Service.Implementation
{
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly IMapper _mapper;
        private readonly IRestfulClient _restfulClient;
        private readonly TelemetryClient _telemetryClient;
        private const string RegisterCustomerUrl = "registercustomer";
        private const string VerifyCustomerUrl = "verifycustomer";

        public CustomerRegistrationService(IMapper mapper, IRestfulClient restfulClient, TelemetryClient telemetryClient)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient));
            _telemetryClient = telemetryClient;
        }

        public async Task Register(Customer customer, CustomerVehicle customerVehicle)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            if (customerVehicle == null) throw new ArgumentNullException(nameof(customerVehicle));

            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var request = GetRegisterCustomerRequest(customer, customerVehicle);

            await _restfulClient.PostAsync(RegisterCustomerUrl, request);

            _telemetryClient?.TrackDependency("CDKAutolineService", "RegisterCustomer",
                JsonConvert.SerializeObject(request), startTime,
                timer.Elapsed, true);
        }

        public async Task<VerifyCustomerResponse> Verify(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var request = GetVerifyCustomerRequest(customer);

            var verifyCustomerResponse =
                await _restfulClient.PostAsync<VerifyCustomerRequest, VerifyCustomerResponse>(VerifyCustomerUrl,
                    request);

            _telemetryClient?.TrackDependency("CDKAutolineService", "VerifyCustomer",
                JsonConvert.SerializeObject(request), startTime,
                timer.Elapsed,
                verifyCustomerResponse != null);

            return verifyCustomerResponse;
        }

        internal VerifyCustomerRequest GetVerifyCustomerRequest(Customer customer)
        {
            return  _mapper.Map<Customer, VerifyCustomerRequest>(customer);
        }

        internal RegisterCustomerRequest GetRegisterCustomerRequest(Customer customer, CustomerVehicle customerVehicle)
        {
            var customerMap = _mapper.Map<Customer, RegisterCustomerRequest>(customer);
            var request = _mapper.Map(customerVehicle, customerMap);

            return request;
        }
    }
}