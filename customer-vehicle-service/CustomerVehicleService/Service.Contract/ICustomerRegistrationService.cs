using Service.Contract.Response;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface ICustomerRegistrationService
    {
        Task Register(Customer customer, CustomerVehicle customerVehicle);
        Task<VerifyCustomerResponse> Verify(Customer customer);
    }
}