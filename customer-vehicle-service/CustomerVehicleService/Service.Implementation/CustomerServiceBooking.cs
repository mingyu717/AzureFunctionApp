using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Response;
using System;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CustomerServiceBooking : ICustomerServiceBooking
    {
        private readonly IRestfulClient _restfulClient;
        private readonly TelemetryClient _telemetryClient;
        private const string CreateServiceBookingUrl = "createservicebooking";
        public CustomerServiceBooking(IRestfulClient restfulClient, TelemetryClient telemetryClient)
        {
            _restfulClient = restfulClient ?? throw new ArgumentNullException(nameof(restfulClient));
            _telemetryClient = telemetryClient;
        }

        public async Task<CreateServiceBookingResponse> CreateServiceBooking(CDKCreateServiceBookingRequest request)
        {
            var startTime = DateTime.Now;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var createServiceBookingResponse =
                await _restfulClient.PostAsync<CDKCreateServiceBookingRequest, CreateServiceBookingResponse>(CreateServiceBookingUrl,
                    request);

            _telemetryClient?.TrackDependency("CDKAutolineService", "CreateServiceBooking",
                JsonConvert.SerializeObject(request), startTime,
                timer.Elapsed,
                createServiceBookingResponse != null);

            return createServiceBookingResponse;
        }
    }
}
