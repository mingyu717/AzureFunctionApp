using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Request;
using Service.Implementation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unity;

namespace ConfigurationManagerService
{
    public static class ConfigurationManagerService
    {
        #region "=== [ Get Requests ] ====================="

        /// <summary>
        /// Get dealer configuration details by giving dealer id.
        /// </summary>
        /// <param name="req">HTTP request message</param>
        /// <param name="id">Dealer id</param>
        /// <param name="log">Trace log.</param>
        /// <returns></returns>
        [FunctionName("GetDealerConfigurationById")]
        public static HttpResponseMessage GetDealerConfigurationById([HttpTrigger(AuthorizationLevel.Function, "get",
                Route = "dealers/{id:int}")]
            HttpRequestMessage req, int id, TraceWriter log)
        {
            try
            {
                log.Info($"Get Dealer by id:{id} HTTP trigger function processed a request.");
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                // Validate DealerId 
                var validateMessage = validateRequest.ValidateDealerIdRequest(id);
                if (!string.IsNullOrEmpty(validateMessage))
                {
                    log.Error($"Invalid get dealer request, id:{id}");
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);
                }

                var dealerResponse = dealerService.GetDealerConfigurationById(id);
                return dealerResponse == null
                    ? req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound)
                    : req.CreateResponse(HttpStatusCode.OK, dealerResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting dealer configuration details by id", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get dealer configuration detail's by rooftopid and community id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GetDealerConfigurationByRooftopIdAndCommunityId")]
        public static HttpResponseMessage GetDealerConfigurationByRooftopIdAndCommunityId([HttpTrigger(
                AuthorizationLevel.Function, "get",
                Route = "dealers")]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Get Dealer Configuration by rooftopid and communityId function processed a request.");

            try
            {
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                string roofTopId = req.GetQueryValue<string>("roofTopId");
                string communityId = req.GetQueryValue<string>("communityId");

                // Validate DealerBy RoofTopId and CommunityId Request
                var validateMessage =
                    validateRequest.ValidateDealerRoofTopIdAndCommunityIdRequest(roofTopId, communityId);
                if (validateMessage.Count > 0)
                {
                    log.Error($"Get Dealer Configuration by Rooftop Id and Community Id, bad request roofTopId: {roofTopId} and communityId: {communityId}");
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);
                }

                //Get Dealer configuration by roofTopId and CommunityId.
                var dealerResponse = dealerService.GetDealerConfigurationByRoofTopIdAndCommunityId(roofTopId, communityId);
                return dealerResponse == null
                    ? req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound)
                    : req.CreateResponse(HttpStatusCode.OK, dealerResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting dealer configuration details by rooftopid and communityid", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, false, null, ex.Message);
            }
        }

        [FunctionName("GetDealerInvitationContent")]
        public static HttpResponseMessage GetDealerInvitationContent([HttpTrigger(AuthorizationLevel.Function,"get",
                Route ="dealers/invitationcontent")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Get Dealer invitation content by rooftopid and communityId function processed a request.");
            try
            {
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                string roofTopId = req.GetQueryValue<string>("roofTopId");
                string communityId = req.GetQueryValue<string>("communityId");

                // Validate DealerBy RoofTopId and CommunityId Request
                var validateMessage =
                    validateRequest.ValidateDealerRoofTopIdAndCommunityIdRequest(roofTopId, communityId);
                if (validateMessage.Count > 0)
                {
                    log.Error($"Get Dealer invitation content by Rooftop Id and Community Id, bad request roofTopId: {roofTopId} and communityId: {communityId}");
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);
                }

                //Get Dealer invitation content by roofTopId and CommunityId.
                var dealerInvitationContentResponse = dealerService.GetInvitationContent(roofTopId, communityId);
                return dealerInvitationContentResponse == null
                    ? req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound)
                    : req.CreateResponse(HttpStatusCode.OK, dealerInvitationContentResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting dealer configuration details by rooftopid and communityid", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, false, null, ex.Message);
            }
        }

        [FunctionName("GetDealersCsvSources")]
        public static HttpResponseMessage GetDealersCsvSources(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dealers/csvsources")]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("Gel all dealers csv sources HTTP trigger function processed a request");
                var container = UnityContainer.Instance;
                var dealerService = container.Resolve<IDealerConfigurationService>();
                var dealersCsvSourcesResponse = dealerService.GetDealersCsvSources();
                return dealersCsvSourcesResponse == null
                    ? req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound)
                    : req.CreateResponse(HttpStatusCode.OK, dealersCsvSourcesResponse);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while getting all dealers csv sources", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "=== [ Post Requests ] ===================="

        /// <summary>
        /// Add dealer configuration detail(s)
        /// </summary>
        /// <param name="req">HttpRequest having "DealerConfiguration" object</param>
        /// <param name="log">Trace writer</param>
        /// <returns></returns>
        [FunctionName("AddDealerConfiguration")]
        public static async Task<HttpResponseMessage> AddDealerConfiguration([HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "dealer")]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Add Dealer Configuration detail function processed a request.");
            try
            {
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                // Validate Dealer Configuration Request
                var dealerConfigurationRequest = await req.Content.ReadAsAsync<DealerConfigurationCreateRequest>();
                var validateMessage = validateRequest.ValidateRequestData(dealerConfigurationRequest);

                if (validateMessage != null)
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                //Check if dealer configuration detail already exists in DB.
                var dealerResponse = dealerService.CheckDealerExist(
                    DealerSearchCriteria.SearchByRooftopAndCommunityId, default(int),
                    dealerConfigurationRequest.RooftopId, dealerConfigurationRequest.CommunityId);
                if (dealerResponse)
                {
                    log.Error($"Dealer with same rooftop id: {dealerConfigurationRequest.RooftopId} and community id: {dealerConfigurationRequest.CommunityId} already exist");
                    return req.CreateResponse(HttpStatusCode.BadRequest,
                        Constants.ExceptionMessages.DealerConfigurationAlreadyExists);
                }

                // Add dealer configuration details in DB.
                var dealerId = await dealerService.AddDealerConfiguration(dealerConfigurationRequest);
                return req.CreateResponse(HttpStatusCode.OK, dealerId);
            }
            catch (CommunicationMethodException ex)
            {
                log.Error("An error happened because invalid communication method", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, Constants.ExceptionMessages.InvalidCommunicationMethod);
            }
            catch (CsvSourceException ex)
            {
                log.Error("An error happened because invalid csv source", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, Constants.ExceptionMessages.InvalidCsvSource);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while saving dealer configuration details", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "=== [ Put Requests ] ====================="

        /// <summary>
        /// Edit Dealer configuration service.
        /// </summary>
        /// <param name="req">HTTP request message</param>
        /// <param name="id">Dealer id</param>
        /// <param name="log">Trace writer</param>
        /// <returns></returns>
        [FunctionName("EditDealerConfiguration")]
        public static async Task<HttpResponseMessage> EditDealerConfiguration([HttpTrigger(AuthorizationLevel.Function,
                "put", Route = "dealer/{id}")]
            HttpRequestMessage req, int id, TraceWriter log)
        {
            log.Info($"Edit Dealer Configuration detail function processed a request. id: {id}");
            try
            {
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                // Validate Dealer id Request
                var validateMessage = validateRequest.ValidateDealerIdRequest(id);
                if (!string.IsNullOrEmpty(validateMessage))
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);

                // Validate Dealer Configuration request
                var dealerConfigurationRequest = await req.Content.ReadAsAsync<DealerConfigurationUpdateRequest>();
                var dealerConfigurationValidationMessage =
                    validateRequest.ValidateRequestData(dealerConfigurationRequest);

                if (dealerConfigurationValidationMessage != null)
                {
                    log.Error($"Edit dealer configuration bad request, {JsonConvert.SerializeObject(dealerConfigurationRequest)}");
                    return req.CreateResponse(HttpStatusCode.BadRequest, dealerConfigurationValidationMessage);
                }

                // Edit dealer configuration details from DB.
                await dealerService.EditDealerConfiguration(dealerConfigurationRequest, id);
                return req.CreateResponse(HttpStatusCode.OK, id);
            }
            catch (CommunicationMethodException ex)
            {
                log.Error("An error happened because invalid communication method", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, Constants.ExceptionMessages.InvalidCommunicationMethod);
            }
            catch (CsvSourceException ex)
            {
                log.Error("An error happened because invalid csv source", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, Constants.ExceptionMessages.InvalidCsvSource);
            }
            catch (DealerNotExistException ex)
            {
                log.Error("An error happened because dealer not exists while editing", ex);
                return req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound);
            }
            catch (DealerAlreadyExistException ex)
            {
                log.Error($"An error happened because {Constants.ExceptionMessages.DealerConfigurationAlreadyExists} while editing", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    Constants.ExceptionMessages.DealerConfigurationAlreadyExists);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while edit dealer configuration detail", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "=== [ Delete Requests ] =================="

        /// <summary>
        /// Delete dealer configuration by id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("DeleteDealerConfiguration")]
        public static async Task<HttpResponseMessage> DeleteDealerConfiguration([HttpTrigger(
                AuthorizationLevel.Function,
                "delete", Route = "dealer/{id}")]
            HttpRequestMessage req, int id, TraceWriter log)
        {
            log.Info($"Delete Dealer Configuration detail function processed a request. id: {id}");
            try
            {
                var container = UnityContainer.Instance;
                var validateRequest = container.Resolve<IValidateRequest>();
                var dealerService = container.Resolve<IDealerConfigurationService>();

                // Validate Dealer id Request
                var validateMessage = validateRequest.ValidateDealerIdRequest(id);
                if (!string.IsNullOrEmpty(validateMessage))
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, validateMessage);
                }

                //Check if dealer configuration detail already exists in DB.
                var dealerExists = dealerService.CheckDealerExist(DealerSearchCriteria.SearchByDealerId, id,
                    string.Empty, string.Empty);
                if (!dealerExists)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound, Constants.ExceptionMessages.NotFound);
                }

                // Delete dealer configuration details from DB.
                await dealerService.DeleteDealerConfiguration(id);
                return req.CreateResponse(HttpStatusCode.OK, id);
            }
            catch (Exception ex)
            {
                log.Error("An error happened while deleting dealer configuration detail", ex);
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "=== [ Private Methods ] =================="

        private static T GetQueryValue<T>(this HttpRequestMessage req, string queryName)
        {
            var queryNamePair = req.GetQueryNameValuePairs();
            return UtilityHelper.Cast(UtilityHelper.GetQueryStringValue(queryNamePair, queryName), typeof(T));
        }

        #endregion
    }
}