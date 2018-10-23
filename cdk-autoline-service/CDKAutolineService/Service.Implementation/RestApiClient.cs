using Newtonsoft.Json;
using Service.Contract;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    /// <summary>
    /// Concrete implementation of IRestApiClient
    /// </summary>
    public class RestApiClient : IRestApiClient
    {
        /// <summary>
        /// Invoke client api(s) to serailize the response content in particular T object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse> Invoke<T>(ApiRequest apiRequest)
        {
            var response = await GetResponseAsync(apiRequest);
            var content = await response.Content.ReadAsStringAsync();
            var errorCode = GetResponseCode(content);
            var apiResponse = new ApiResponse();
            var isEmptyResponseCode = string.IsNullOrWhiteSpace(errorCode);

            if (errorCode == Convert.ToString(0))
            {
                apiResponse.Result = JsonConvert.DeserializeObject<T>(content);
                apiResponse.Success = true;
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.Result = default(T);
                apiResponse.AddErrors(isEmptyResponseCode
                    ? content
                    : UtilityHelper.GetEnumDescription((ErrorResponseCode) int.Parse(errorCode)));
            }

            apiResponse.ResponseCode = isEmptyResponseCode
                ? response.StatusCode.ToString()
                : Enum.Parse(typeof(ErrorResponseCode), errorCode).ToString();


            return apiResponse;
        }

        /// <summary>
        /// Validate the property name is present 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool HasProperty(dynamic obj, string propertyName)
        {
            foreach (var item in obj)
            {
                if (item.Name == propertyName)
                    return true;
            }

            return false;
        }

        private string GetResponseCode(string response)
        {
            dynamic result = UtilityHelper.DeserializeObject(response);
            if (result == null) return string.Empty;
            if (HasProperty(result, "ErrorCode"))
            {
                return result.ErrorCode;
            }

            return result.Result == null
                ? string.Empty
                : (string) GetResponseCode(UtilityHelper.SerializeObject((result.Result)));
        }

        private async Task<HttpResponseMessage> GetResponseAsync(ApiRequest apiRequest)
        {
            var client = new HttpClient();
            {
                client.BaseAddress = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(apiRequest.ContentType));

                foreach (var header in apiRequest.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                return await GetHttpResponseAsync(client, apiRequest);
            }
        }

        /// <summary>
        /// Get Http response from third party API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetHttpResponseAsync(HttpClient client, ApiRequest apiRequest)
        {
            Enum.TryParse(apiRequest.Method.ToUpper(), out HttpVerbs methodUpper);
            switch (methodUpper)
            {
                default:
                    return await client.GetAsync(apiRequest.Url);

                case HttpVerbs.POST:
                case HttpVerbs.PUT:
                    var request = new StringContent(apiRequest.Body, Encoding.UTF8, apiRequest.ContentType);
                    if (methodUpper == HttpVerbs.POST)
                    {
                        return await client.PostAsync(apiRequest.Url, request);
                    }

                    return await client.PutAsync(apiRequest.Url, request);

                case HttpVerbs.DELETE:
                    return await client.DeleteAsync(apiRequest.Url);
            }
        }
    }
}