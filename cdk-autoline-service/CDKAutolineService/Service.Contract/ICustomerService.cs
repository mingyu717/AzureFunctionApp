using System.Threading.Tasks;
using Service.Contract.DbModels;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface ICustomerService
    {
        Task<ApiResponse> RegisterCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest, CdkCustomer cdkCustomer);
        Task<ApiResponse> CheckPassword(CustomerVerifyRequest customerVerifyRequest, CdkCustomer cdkCustomer, string partnerKey);
    }
}