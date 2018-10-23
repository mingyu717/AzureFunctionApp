using System;
using Service.Contract.DbModels;
using System.Threading.Tasks;


namespace Service.Contract
{
    public interface ITokenService
    {
        Task<string> GetAppToken(string communityId, string roofTopId);
        Task<string> GetCustomerToken(CdkCustomer cdkCustomer, string roofTopId);
    }
}