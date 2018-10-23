using System;
using System.Threading.Tasks;
using Service.Contract.DbModels;

namespace Service.Contract
{
    public interface ICdkCustomerDAL
    {
        CdkCustomer GetCdkCustomer(string communityId, int customerNo);
        Task<int> AddCustomer(CdkCustomer cdkCustomer);
        Task UpdateCustomerToken(int id, Guid? token);
    }
}