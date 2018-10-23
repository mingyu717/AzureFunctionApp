using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Models;
using Unity;

namespace VehicleMaintenanceService
{
    public static class VehicleMaintenanceService
    {
        [FunctionName("GetRecommendedServices")]
        public static async Task<HttpResponseMessage> GetRecommendedServices([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Get Recommended Services HTTP trigger function processed a request.");

            try
            {
                var container = UnityContainer.Instance;

                var getRecommendedServicesRequest = await req.Content.ReadAsAsync<GetRecommendedServicesRequest>();

                log.Info($"Get recommended service request: {JsonConvert.SerializeObject(getRecommendedServicesRequest)}");

                var validationContext = new ValidationContext(getRecommendedServicesRequest, null, null);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(getRecommendedServicesRequest, validationContext, results, true))
                {
                    var message = results.Select(x => x.ErrorMessage);
                    return req.CreateResponse(HttpStatusCode.BadRequest, message);
                }

                var recommendedService = container.Resolve<IRecommendedService>();

                var recommendedServiceResponse = await recommendedService.Get(getRecommendedServicesRequest);

                return req.CreateResponse(HttpStatusCode.OK, recommendedServiceResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting recommended services", ex);
                throw;
            }
        }
    }
}