using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.RequestModel;
using Unity;

namespace AppointmentService
{
    public static class AppointmentService
    {
        [FunctionName("CreateAppointment")]
        public static async Task<HttpResponseMessage> CreateAppointment(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger create appointment");
                var container = UnityContainer.Instanace;
                var validateRequest = container.Resolve<IValidateRequest>();
                var appointmentService = container.Resolve<IAppointmentService>();

                // Validate incoming request is valid or not.
                var createAppointmentRequest = await req.Content.ReadAsAsync<CreateAppointmentRequest>();
                var validateMessage = validateRequest.ValidateRequestData(createAppointmentRequest);
                if (validateMessage != null) return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                log.Info($"Create appointment request: {JsonConvert.SerializeObject(createAppointmentRequest)}");

                await appointmentService.CreateAppointment(createAppointmentRequest);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while creating appointment", ex);
                throw;
            }
        }
    }
}
