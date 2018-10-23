using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IRestfulClient
    {
        Task<TResponse> GetAsync<TResponse>(string url);
        Task PostAsync<TRequest>(string url, TRequest request);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request);
    }
}
