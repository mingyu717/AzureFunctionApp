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
using PhoneNumbers;
using Service.Contract;
using Service.Contract.Exceptions;
using Unity;

namespace CustomerVehicleService
{
    public static class CustomerVehicleService
    {
        [FunctionName("SaveCustomerVehicle")]
        public static async Task<HttpResponseMessage> SaveCustomerVehicle(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger save customer vehicle function");

                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate incoming request is valid or not.
                var saveCustomerVehicleRequest = await req.Content.ReadAsAsync<SaveCustomerVehicleRequest>();
                var validateMessage = validateRequest.ValidateRequestData(saveCustomerVehicleRequest);
                if (validateMessage != null) return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                log.Info($"Save customer vehicle request: {JsonConvert.SerializeObject(saveCustomerVehicleRequest)}");
                
                //Check dealer configutation details available in dealercofiguration api.
                var dealerConfigurationService = container.Resolve<IDealerConfigurationService>();
                var dealerConfigResponse = await dealerConfigurationService.GetDealerConfiguration(
                    saveCustomerVehicleRequest.RooftopId, saveCustomerVehicleRequest.CommunityId);

                if (dealerConfigResponse == null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidRoofTopAndCommunityId);
                }

                // Save the customer vehicle details
                var customerVehicleService = container.Resolve<ICustomerVehicleService>();

                await customerVehicleService.SaveCustomerVehicle(saveCustomerVehicleRequest, dealerConfigResponse);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (DealerInvitationContentException ex)
            {
                log.Error("An error happern while fetching invitation content", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidRoofTopAndCommunityId);
            }
            catch (NumberParseException ex)
            {
                log.Error("An error happens while parsing phone number", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvalidPhoneNumber);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while saving customer vehicle details", ex);
                throw;
            }
        }

        [FunctionName("GetCustomerVehicle")]
        public static async Task<HttpResponseMessage> GetCustomerVehicle(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route =
                "dealers/{dealerid:int}/customers/{customerno:int}/vehicles/{vehicleno:int}")]
            HttpRequestMessage req, int dealerid, int customerno, int vehicleno, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger get customer vehicle function.",
                    $"dealerid: {dealerid}, customerno: {customerno}, vehicleno: {vehicleno}");

                var container = UnityContainer.Instance;

                var customerVehicleService = container.Resolve<ICustomerVehicleService>();
                var dealerConfigurationService = container.Resolve<IDealerConfigurationService>();

                var dealerConfigResponse = await dealerConfigurationService.GetDealerConfiguration(dealerid);
                if (dealerConfigResponse == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                var existingBooking = customerVehicleService.ExistingServiceBooking(customerno, vehicleno, dealerid);
                if (existingBooking != null)
                {
                    return req.CreateResponse(HttpStatusCode.OK, existingBooking);
                }

                var customerVehicle = await customerVehicleService.GetCustomerVehicle(customerno, vehicleno, dealerConfigResponse);

                if (customerVehicle == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                log.Info($"Get customer vehicle success: {JsonConvert.SerializeObject(customerVehicle)}");

                return req.CreateResponse(HttpStatusCode.OK, customerVehicle);
            }
            catch (InvitationExpiredException ex)
            {
                log.Error("Invitation expired exception happens while getting customer vehicle details", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, ExceptionMessages.InvitationExpired);
            }
            catch (Exception ex)
            {
                log.Error("An error happens while getting customer vehicle details", ex);
                throw;
            }
        }


        [FunctionName("DismissVehicleOwnership")]
        public static async Task<HttpResponseMessage> DismissVehicleOwnership(
            [HttpTrigger(AuthorizationLevel.Function, "post",Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger Dismiss vehicle ownership");

                var container = UnityContainer.Instance;

                var customerVehicleService = container.Resolve<ICustomerVehicleService>();
                var dealerConfigurationService = container.Resolve<IDealerConfigurationService>();
                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate incoming request is valid or not.
                var dismissVehicleOwnerShipRequest = await req.Content.ReadAsAsync<DismissVehicleOwnershipRequest>();
                var validateMessage = validateRequest.ValidateRequestData(dismissVehicleOwnerShipRequest);
                if (validateMessage != null) return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                log.Info($"Dismiss customer vehicle ownership request: {JsonConvert.SerializeObject(dismissVehicleOwnerShipRequest)}");

                var dealerConfigResponse = await dealerConfigurationService.GetDealerConfiguration(dismissVehicleOwnerShipRequest.DealerId);
                if (dealerConfigResponse == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                await customerVehicleService.DismissVehicleOwnership(dismissVehicleOwnerShipRequest, dealerConfigResponse);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (InvalidCustomerException ex)
            {
                log.Error("An exception happens due to ", ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error("An error happens while dismiss vehicle ownership", ex);
                throw;
            }
        }

        [FunctionName("UpdateCustomerContact")]
        public static async Task<HttpResponseMessage> UpdateCustomerContact(
            [HttpTrigger(AuthorizationLevel.Function,"post",Route =null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger update customer contact details");

                var container = UnityContainer.Instance;

                var customerVehicleService = container.Resolve<ICustomerVehicleService>();
                var dealerConfigurationService = container.Resolve<IDealerConfigurationService>();
                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate incoming request is valid or not.
                var updateCustomerContactRequest = await req.Content.ReadAsAsync<UpdateCustomerContactRequest>();
                var validateMessage = validateRequest.ValidateRequestData(updateCustomerContactRequest);
                if (validateMessage != null) return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                log.Info($"Update customer contact details request: {JsonConvert.SerializeObject(updateCustomerContactRequest)}");

                var dealerConfigResponse = await dealerConfigurationService.GetDealerConfiguration(updateCustomerContactRequest.DealerId);
                if (dealerConfigResponse == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                await customerVehicleService.UpdateCustomerContact(updateCustomerContactRequest, dealerConfigResponse);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (InvalidCustomerException ex)
            {
                log.Error("An exception happens due to invalid ", ex);
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                log.Error("An error happens while updating customer contact details", ex);
                throw;
            }
        }

        [FunctionName("CreateServiceBooking")]
        public static async Task<HttpResponseMessage> CreateServiceBooking(
            [HttpTrigger(AuthorizationLevel.Function,"post",Route =null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger update create service booking");

                var container = UnityContainer.Instance;

                var customerVehicleService = container.Resolve<ICustomerVehicleService>();
                var dealerConfigurationService = container.Resolve<IDealerConfigurationService>();
                var validateRequest = container.Resolve<IValidateRequest>();

                // Validate incoming request is valid or not.
                var createServiceBookingRequest = await req.Content.ReadAsAsync<CreateServiceBookingRequest>();
                var validationMessages = validateRequest.ValidateRequestData(createServiceBookingRequest);
                if (validationMessages != null) return req.CreateResponse(HttpStatusCode.BadRequest, validationMessages);

                log.Info($"Create service booking request: {JsonConvert.SerializeObject(createServiceBookingRequest)}");

                var dealerConfigResponse = await dealerConfigurationService.GetDealerConfiguration(createServiceBookingRequest.DealerId);
                if (dealerConfigResponse == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                var createServiceBookingResponse = await customerVehicleService.CreateServiceBooking(createServiceBookingRequest, dealerConfigResponse);
                return req.CreateResponse(HttpStatusCode.OK, createServiceBookingResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happens while create service booking ", ex);
                throw;
            }
        }
    }
}