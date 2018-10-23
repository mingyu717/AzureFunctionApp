using Service.Contract;
using Service.Contract.Response;
using System;
using System.Threading.Tasks;
using Service.Contract.DbModels;
using Service.Contract.Models;

namespace Service.Implementation
{
    /// <summary>
    /// Class Customer Service, used for RestAPI to register customer, activate customer and check password.
    /// <seealso cref="https://sol-portal2-r3.dmsdigital.net"/>
    /// </summary>
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly IRestApiClient _restApiClient;
        private readonly IEncryptedTokenCodeService _encryptedTokenCodeService;
        private readonly string _cdkAutolineUrl;
        private const string RegisterCustomerUrl = "RegisterCustomer";
        private const string CheckPasswordUrl = "CheckPassword";

        /// <summary>
        /// Constructor accepting input of type IRestApiClient , implementation of which will be resolved by Unity.
        /// </summary>
        /// <param name="restApiClient"></param>
        /// <param name="cdkAutolineUrl"></param>
        /// <param name="encryptedTokenCodeService"></param>
        public CustomerService(IRestApiClient restApiClient, string cdkAutolineUrl,
            IEncryptedTokenCodeService encryptedTokenCodeService)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
            _cdkAutolineUrl = cdkAutolineUrl ?? throw new ArgumentNullException(nameof(cdkAutolineUrl));
            _encryptedTokenCodeService = encryptedTokenCodeService ?? throw new ArgumentNullException(nameof(encryptedTokenCodeService));
        }

        /// <summary>
        /// Check password from SOL api.
        /// </summary>
        /// <param name="customerVerifyRequest"></param>
        /// <param name="cdkCustomer"></param>
        /// <returns></returns>
        public async Task<ApiResponse> CheckPassword(CustomerVerifyRequest customerVerifyRequest, CdkCustomer cdkCustomer, string partnerKey)
        {
            if (customerVerifyRequest == null) throw new ArgumentNullException(nameof(customerVerifyRequest));
            if (cdkCustomer == null) throw new ArgumentNullException(nameof(cdkCustomer));
            var requestBody = UtilityHelper.SerializeObject(new
            {
                RooftopId = customerVerifyRequest.RoofTopId
            });
            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl, customerVerifyRequest.CommunityId), CheckPasswordUrl),
                ContentType = Constants.ContentTypeJson
            };

            var hashCode =
                _encryptedTokenCodeService.GetEncryptedTokenCode(cdkCustomer.Token.ToString(), cdkCustomer, partnerKey, true);
            AddRequestHeader(request, $"{Constants.DataHubHashHeaderValuePrefix}{hashCode}");
            return await _restApiClient.Invoke<CustomerResponse>(request);
        }

        /// <summary>
        /// Registered customer in CDK Autoline services.
        /// </summary>
        /// <param name="customerVehicleRegisterRequest"></param>
        /// <param name="cdkCustomer"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RegisterCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest, CdkCustomer cdkCustomer)
        {
            if (customerVehicleRegisterRequest == null) throw new ArgumentNullException(nameof(customerVehicleRegisterRequest));
            if (cdkCustomer == null) throw new ArgumentNullException(nameof(cdkCustomer));

            var requestBody = UtilityHelper.SerializeObject(new
            {
                customerVehicleRegisterRequest.CommunityId,
                customerVehicleRegisterRequest.RoofTopId,
                CustomerId = cdkCustomer.CustomerLoginId,
                CustomerIdType = CustomerIdType.Other,
                cdkCustomer.Password,
                customerVehicleRegisterRequest.FirstName,
                SurName = customerVehicleRegisterRequest.Surname,
                customerVehicleRegisterRequest.EmailAddress,
                MobileTelNo = customerVehicleRegisterRequest.PhoneNumber
            });

            var request = new ApiRequest
            {
                Body = requestBody,
                Method = HttpVerbs.POST.ToString(),
                Url = CombineUrl(GetCdkAutolineUrl(_cdkAutolineUrl,
                    customerVehicleRegisterRequest.CommunityId), RegisterCustomerUrl),
                ContentType = Constants.ContentTypeJson
            };

            AddRequestHeader(request,
                $"{Constants.DataHubAppTokenHeaderValuePrefix}{customerVehicleRegisterRequest.AppToken}");
            return await _restApiClient.Invoke<CustomerResponse>(request);
        }
    }
}