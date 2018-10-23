using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Service.Contract;

namespace Service.Implementation
{
    public class RestfulClient : IRestfulClient
    {
        private readonly ClientConfiguration _clientConfiguration;
        private readonly MediaTypeFormatter _requestFormatter = new JsonMediaTypeFormatter();

        public RestfulClient(ClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            return await SendAsync<TResponse>(client => client.GetAsync(BuildUrl(url)));
        }

        public async Task PostAsync<TRequest>(string url, TRequest request)
        {
            await SendAsync(client => client.PostAsync(BuildUrl(url), request, _requestFormatter));
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            return await SendAsync<TResponse>(client => client.PostAsync(BuildUrl(url), request, _requestFormatter));
        }

        protected virtual async Task<TResponse> SendAsync<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            using (var client = CreateHttpClient())
            {
                var response = await func(client);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: return await response.Content.ReadAsAsync<TResponse>();
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        return default(TResponse);
                    default:
                        var error = response.Content.ReadAsStringAsync().Result;
                        throw new Exception(error);
                }
            }
        }

        protected virtual async Task SendAsync(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            using (var client = CreateHttpClient())
            {
                var response = await func(client);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        return;
                    default:
                        var error = response.Content.ReadAsStringAsync().Result;
                        throw new Exception(error);
                }
            }
        }

        protected virtual HttpClient CreateHttpClient()
        {
            HttpMessageHandler clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            var client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-functions-key", _clientConfiguration.AccessKey);
            return client;
        }

        protected virtual string BuildUrl(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (url.StartsWith("/"))
            {
                url = url.TrimStart('/');
            }

            if (!_clientConfiguration.ServiceUrl.EndsWith("/"))
            {
                return _clientConfiguration.ServiceUrl + "/" + url;
            }

            return _clientConfiguration.ServiceUrl + url;
        }

    }
}