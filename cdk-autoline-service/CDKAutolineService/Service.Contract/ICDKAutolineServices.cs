using System.Threading.Tasks;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface ICDKAutolineServices
    {
        Task<string> RegisterCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest);
        Task<VerifyPasswordResponse> VerifyCustomer(CustomerVerifyRequest customerVerifyRequest);
    }
}