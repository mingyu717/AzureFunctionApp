using System.Threading.Tasks;
using Service.Contract.DbModels;
using Service.Contract.Models;

namespace Service.Contract
{
    public interface ICdkCustomerService
    {
        CdkCustomer MapCdkCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest);
        Task SaveCdkCustomer(CdkCustomer cdkCustomer);
        CdkCustomer GetCdkCustomer(string communityId, int customerNo);
    }
}