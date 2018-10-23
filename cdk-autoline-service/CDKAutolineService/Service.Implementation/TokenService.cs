using Service.Contract;
using Service.Contract.Response;
using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Service.Contract.DbModels;
using Service.Contract.Exceptions;
using Service.Contract.Models;

namespace Service.Implementation
{
    /// <summary>
    /// Class TokenService, used for RestAPI to get the token from SOL service or Activate the token from SOL service.
    /// <seealso cref="https://sol-portal2-r3.dmsdigital.net"/>
    /// </summary>
    public class TokenService : BaseService, ITokenService
    {
        private readonly IRestApiClient _restApiClient;
        private readonly IEncryptedTokenCodeService _encryptedTokenCodeService;
        private readonly IAppTokenDAL _appTokenDal;
        private readonly TelemetryClient _telemetryClient;
        private readonly ICdkCustomerService _cdkCustomerService;
        private readonly ICustomerService _customerService;
        private readonly string _cdkAutolineUrl;
        private readonly string _unregisteredGuid;
        private readonly IDealerCDKConfigurationsDAL _dealerCDKConfigurationDAL;
        private const string ActivateTokenUrl = "ActivateToken";
        private const string RequestTokenUrl = "RequestToken";

        /// <summary>
        /// Constructor accepting input of type IRestApiClient , implementation of which will be resolved by Unity.
        /// </summary>
        /// <param name="restApiClient"></param>
        /// <param name="cdkAutolineUrl"></param>
        /// <param name="partnerId"></param>
        /// <param name="encryptedTokenCodeService"></param>
        /// <param name="partnerVersion"></param>
        /// <param name="unregisteredGuid"></param>
        /// <param name="appTokenDal"></param>
        /// <param name="telemetryClient"></param>
        /// <param name="cdkCustomerService"></param>
        /// <param name="customerService"></param>
        public TokenService(IRestApiClient restApiClient, IEncryptedTokenCodeService encryptedTokenCodeService,
            string cdkAutolineUrl, string unregisteredGuid,
            IAppTokenDAL appTokenDal, TelemetryClient telemetryClient, ICdkCustomerService cdkCustomerService,
            ICustomerService customerService, IDealerCDKConfigurationsDAL dealerCDKConfigurationDAL)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
            _encryptedTokenCodeService = encryptedTokenCodeService ?? throw new ArgumentNullException(nameof(_encryptedTokenCodeService));
            _unregisteredGuid = unregisteredGuid ?? throw new ArgumentNullException(nameof(_unregisteredGuid));
            _appTokenDal = appTokenDal ?? throw new ArgumentNullException(nameof(appTokenDal));
            _telemetryClient = telemetryClient;
            _cdkCustomerService = cdkCustomerService ?? throw new ArgumentNullException(nameof(cdkCustomerService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _dealerCDKConfigurationDAL = dealerCDKConfigurationDAL ?? throw new ArgumentNullException(nameof(dealerCDKConfigurationDAL));
        }

        public async Task<string> GetAppToken(string communityId, string roofTopId)
        {
            if (communityId == null) throw new ArgumentNullException(nameof(communityId));

            var appToken = _appTokenDal.GetAppToken(communityId);
            var isTokenFromDb = appToken != null;
            var dealerCDKConfig = _dealerCDKConfigurationDAL.GetDealerCDKConfigurations(roofTopId, communityId);
            if (dealerCDKConfig == null) throw new DealerCDKConfigurationException(ExceptionMessages.InvalidDealerCDKConfiguration);

            var activedToken = await RequestAndActivateAppToken(dealerCDKConfig, communityId, appToken, isTokenFromDb)
                               ?? await RequestAndActivateAppToken(dealerCDKConfig, communityId, null, false);

            await _appTokenDal.SaveAppToken(new AppToken
            {
                CommunityId = communityId,
                Token = new Guid(activedToken)
            });

            return activedToken;
        }

        public async Task<string> GetCustomerToken(CdkCustomer cdkCustomer, string roofTopId)
        {
            if (cdkCustomer == null) throw new ArgumentNullException(nameof(cdkCustomer));
            if (roofTopId == null) throw new ArgumentNullException(nameof(roofTopId));
            var dealerCDKConfig = _dealerCDKConfigurationDAL.GetDealerCDKConfigurations(roofTopId, cdkCustomer.CommunityId);
            if (dealerCDKConfig == null) throw new DealerCDKConfigurationException(ExceptionMessages.InvalidDealerCDKConfiguration);

            var customerToken = await RequestAndActivateCustomerToken(cdkCustomer, dealerCDKConfig, roofTopId, cdkCustomer.Token) ??
                                await RequestAndActivateCustomerToken(cdkCustomer, dealerCDKConfig, roofTopId, null);

            cdkCustomer.Token = new Guid(customerToken);

            await _cdkCustomerService.SaveCdkCustomer(cdkCustomer);
            return customerToken;
        }

        internal async Task<string> RequestAndActivateCustomerToken(CdkCustomer cdkCustomer, DealerCDKConfiguration dealerCDKConfig, string roofTopId, Guid? existingToken)
        {
            if (existingToken == null)
            {
                var requestApiResponse = await RequestToken(dealerCDKConfig, cdkCustomer.CommunityId, cdkCustomer.CustomerLoginId);
                if (requestApiResponse.Result == null || !requestApiResponse.Success
                                                      || !(requestApiResponse.Result is TokenResponse objRequestToken)
                                                      || string.IsNullOrWhiteSpace(objRequestToken.Token))
                {
                    var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(requestApiResponse.Errors));
                    _telemetryClient?.TrackException(cdkAutolineException);
                    throw cdkAutolineException;
                }

                cdkCustomer.Token = new Guid(objRequestToken.Token);
            }

            //Activate Token from generated token.
            var checkPasswordApiResponse = await _customerService.CheckPassword(new CustomerVerifyRequest
            {
                CommunityId = cdkCustomer.CommunityId,
                CustomerNo = cdkCustomer.CustomerNo,
                RoofTopId = roofTopId
            }, cdkCustomer, dealerCDKConfig.PartnerKey);

            if (checkPasswordApiResponse == null || !checkPasswordApiResponse.Success)
            {
                if (existingToken != null) return null;
                throw new CDKAutolineException(UtilityHelper.SerializeObject(checkPasswordApiResponse?.Errors));
            }

            await _cdkCustomerService.SaveCdkCustomer(cdkCustomer);
            return cdkCustomer.Token.ToString();
        }

        /// <summary>
        /// Activate token generated by API(/RequestToken).
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="appToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> ActivateToken(string communityId, string appToken, string partnerKey)
        {
            var request = new ApiRequest()
            {
                Body = string.Empty,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, communityId), ActivateTokenUrl),
                ContentType = Constants.ContentTypeJson
            };

            var hashCode = _encryptedTokenCodeService.GetEncryptedTokenCode(appToken, null, partnerKey);
            AddRequestHeader(request, $"{Constants.DataHubHashHeaderValuePrefix}{hashCode}");
            return await _restApiClient.Invoke<CustomerResponse>(request);
        }

        /// <summary>
        /// Request token for (InitToken for Unregistered/Registered customer)
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RequestToken(DealerCDKConfiguration dealerCDKConfig, string communityId, string customerId = null)
        {
            if (communityId == null) throw new ArgumentNullException(nameof(communityId));
            if (string.IsNullOrWhiteSpace(customerId)) customerId = _unregisteredGuid;

            var requestBody = UtilityHelper.SerializeObject(new
            {
                CustomerId = customerId,
                dealerCDKConfig.PartnerId,
                Version = dealerCDKConfig.PartnerVersion
            });
            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, communityId), RequestTokenUrl),
                ContentType = Constants.ContentTypeJson
            };
            return await _restApiClient.Invoke<TokenResponse>(request);
        }

        internal async Task<string> RequestAndActivateAppToken(DealerCDKConfiguration dealerCDKConfig, string communityId, AppToken appToken, bool isTokenFetchedFromDb)
        {
            string token;
            if (!isTokenFetchedFromDb)
            {
                var requestApiResponse = await RequestToken(dealerCDKConfig, communityId);
                if (requestApiResponse.Result == null || !requestApiResponse.Success || !(requestApiResponse.Result is TokenResponse objRequestToken))
                {
                    var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(requestApiResponse.Errors));
                    _telemetryClient?.TrackException(cdkAutolineException);
                    throw cdkAutolineException;
                }

                token = objRequestToken.Token;
            }
            else
            {
                token = appToken.Token.ToString();
            }

            //Activate Token from generated token.
            var activeApiTokenResponse = await ActivateToken(communityId, token, dealerCDKConfig.PartnerKey);
            if (activeApiTokenResponse == null || !activeApiTokenResponse.Success)
            {
                if (isTokenFetchedFromDb) return null;

                var cdkAutolineException = new CDKAutolineException(UtilityHelper.SerializeObject(activeApiTokenResponse?.Errors));
                _telemetryClient?.TrackException(cdkAutolineException);
                throw cdkAutolineException;
            }

            return token;
        }
    }
}