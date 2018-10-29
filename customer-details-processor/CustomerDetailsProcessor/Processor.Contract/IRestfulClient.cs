using System.Net.Http;
using System.Threading.Tasks;

namespace Processor.Contract
{
    public interface IRestfulClient
    {
        Task<TResponse> GetAsync<TResponse>(string url);
        Task<HttpResponseMessage> PostAsync<TRequest>(string url, TRequest request);
    }
}