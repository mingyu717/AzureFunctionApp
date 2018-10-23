using Service.Contract.DbModels;

namespace Service.Contract
{
    public interface IEncryptedTokenCodeService
    {
        string GetEncryptedTokenCode(string token, CdkCustomer objPasswordModel,string partnerKey, bool addPassword = false);
    }
}