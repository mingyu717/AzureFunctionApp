using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unity;

namespace CDKAutoline.Service
{
    public static class CDKAutolineService
    {
        /// <summary>
        /// Register customer on CDK autoline, firstly send request token and activate previously recieved token from request token by appling HMACSHA384 algorithum
        /// and submit requested data with hascode to register customer on CDK autoline api.
        /// </summary>
        /// <param name="req">Http request parameter</param>
        /// <param name="log">log trace object</param>
        /// <returns></returns>
        [FunctionName("RegisterCustomer")]
        public static async Task<HttpResponseMessage> RegisterCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Register Customer HTTP trigger function processed a request.");
            try
            {
                var container = UnityContainer.Instance;
                var cdkAutolineService = container.Resolve<ICDKAutolineServices>();

                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate request customer vehicle data 
                var customerVehicleRegisterRequest = await req.Content.ReadAsAsync<CustomerVehicleRegisterRequest>();
                var validateMessages = validateRequest.ValidateRequestData(customerVehicleRegisterRequest);
                if (validateMessages != null && validateMessages.Any())
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessages);
                }

                var registerResponse = await cdkAutolineService.RegisterCustomer(customerVehicleRegisterRequest);
                return req.CreateResponse(HttpStatusCode.OK, registerResponse);
            }
            catch (DealerCDKConfigurationException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            catch (CDKAutolineException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Verify customer, By sending request token and then check password 
        /// </summary>
        /// <param name="req">Http request parameter</param>
        /// <param name="log">log trace object</param>
        /// <returns></returns>
        [FunctionName("VerifyCustomer")]
        public static async Task<HttpResponseMessage> VerifyCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Verify Customer HTTP trigger function processed a request.");
            try
            {
                // parse query parameter
                var container = UnityContainer.Instance;
                var cdkAutolineService = container.Resolve<ICDKAutolineServices>();
                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate check password request.
                var objCheckPassword = await req.Content.ReadAsAsync<CustomerVerifyRequest>();
                var validateMessages = validateRequest.ValidateRequestData(objCheckPassword);
                if (validateMessages != null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessages);
                }

                var verifyPasswordResponse = await cdkAutolineService.VerifyCustomer(objCheckPassword);
                return req.CreateResponse(HttpStatusCode.OK, verifyPasswordResponse);
            }
            catch (DealerCDKConfigurationException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            catch (CDKAutolineException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [FunctionName("GetRecommendedServices")]
        public static async Task<HttpResponseMessage> GetRecommendedServices(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Get Recommended Services HTTP trigger function processed a request.");
            try
            {
                var container = UnityContainer.Instance;
                var cdkVehicleMaintenanceServcie = container.Resolve<ICDKVehicleMaintenanceService>();

                var getRecommendedServicesRequest = await req.Content.ReadAsAsync<GetRecommendedServicesRequest>();

                log.Info($"Get recommended service request: {JsonConvert.SerializeObject(getRecommendedServicesRequest)}");

                var validateRequest = container.Resolve<IValidateRequest>();
                var validateMessages = validateRequest.ValidateRequestData(getRecommendedServicesRequest);
                if (validateMessages != null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessages);
                }

                var recommendedServices = await cdkVehicleMaintenanceServcie.GetRecommendedServices(getRecommendedServicesRequest);
                return req.CreateResponse(HttpStatusCode.OK, recommendedServices);
            }
            catch (DealerCDKConfigurationException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [FunctionName("CreateServiceBooking")]
        public static async Task<HttpResponseMessage> CreateServiceBooking(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                var container = UnityContainer.Instance;
                var cdkBookingService = container.Resolve<ICDKBookingService>();

                var createServiceBookingRequest = await req.Content.ReadAsAsync<CreateServiceBookingRequest>();
                log.Info($"Create service booking: {JsonConvert.SerializeObject(createServiceBookingRequest)}");

                var validateRequest = container.Resolve<IValidateRequest>();
                var validateMessages = validateRequest.ValidateRequestData(createServiceBookingRequest);
                if (validateMessages != null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessages);
                }
                var createServiceBookingResponse = await cdkBookingService.CreateServiceBooking(createServiceBookingRequest);
                return req.CreateResponse(HttpStatusCode.OK, createServiceBookingResponse);

            }
            catch (InvalidCustomerException ex)
            {
                log.Error("An exception happened due to ", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidCustomer);
            }
            catch (CDKAutolineException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while creating service booking ", ex);
                throw;
            }
        }

        [FunctionName("GetServiceAdvisors")]
        public static async Task<HttpResponseMessage> GetServiceAdvisors(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route =null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                var container = UnityContainer.Instance;
                var cdkServiceAdvisors = container.Resolve<ICDKServiceAdvisors>();

                var getServiceAdvisorsRequest = await req.Content.ReadAsAsync<GetServiceAdvisorsRequest>();
                log.Info($"Get service advisors: {JsonConvert.SerializeObject(getServiceAdvisorsRequest)}");

                var validateRequest = container.Resolve<IValidateRequest>();
                var validateMessages = validateRequest.ValidateRequestData(getServiceAdvisorsRequest);
                if (validateMessages != null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessages);
                }
                var getServiceAdviorsResponse = await cdkServiceAdvisors.GetServiceAdvisors(getServiceAdvisorsRequest);
                return req.CreateResponse(HttpStatusCode.OK, getServiceAdviorsResponse);
            }
            catch (InvalidCustomerException ex)
            {
                log.Error("An exception happened due to ", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidCustomer);
            }
            catch (CDKAutolineException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting service adviors ", ex);
                throw;
            }
        }

        [FunctionName("GetAppointmentSlots")]
        public static async Task<HttpResponseMessage> GetAppointmentSlots(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                var container = UnityContainer.Instance;
                var appointmentSlots = container.Resolve<ICDKAppointmentSlotsService>();

                var getAppointmentSlotsRequest = await req.Content.ReadAsAsync<GetAppointmentSlotsRequest>();
                log.Info($"Get appointment slots: {JsonConvert.SerializeObject(getAppointmentSlotsRequest)}");

                var getAppointmentSlotsResponse =
                    await appointmentSlots.GetAppointmentSlots(getAppointmentSlotsRequest);
                return req.CreateResponse(HttpStatusCode.OK, getAppointmentSlotsResponse);
            }
            catch (InvalidRequestException ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, ex.ValidateMessages);
            }
            catch (InvalidCustomerException ex)
            {
                log.Error("An exception happened due to ", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidCustomer);
            }
            catch (CDKAutolineException ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting appointment slots ", ex);
                throw;
            }
        }
    }
}