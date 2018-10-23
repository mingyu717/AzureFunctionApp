using Service.Contract;
using System;

namespace Service.Implementation
{
    public abstract class BaseService
    {
        /// <summary>
        /// Add request header in request object.
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <param name="headerValue"></param>
        protected void AddRequestHeader(ApiRequest apiRequest, string headerValue)
        {
            apiRequest.Headers.Add(Constants.AuthorizationHeaderKey, headerValue);
        }

        /// <summary>
        /// Combine urls
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        protected string CombineUrl(string baseUrl, string relativeUri)
        {
            var baseUri = new Uri(baseUrl);
            return new Uri(baseUri, relativeUri).ToString();
        }

        protected string GetCdkAutolineUrl(string cdkAutolineUrlPlaceholder, string communityId)
        {
            return string.Format(cdkAutolineUrlPlaceholder, communityId);
        }
    }
}