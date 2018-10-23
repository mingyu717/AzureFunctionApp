using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IRestfulClient
    {
        Task<TResponse> GetAsync<TResponse>(string url);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request);
    }
}