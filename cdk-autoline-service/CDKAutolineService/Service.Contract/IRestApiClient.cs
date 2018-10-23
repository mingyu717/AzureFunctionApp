using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IRestApiClient
    {
        Task<ApiResponse> Invoke<T>(ApiRequest request);
    }
}
