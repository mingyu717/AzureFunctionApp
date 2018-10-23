using System.Threading.Tasks;
using Service.Contract.DbModels;

namespace Service.Contract
{
    public interface IAppTokenDAL
    {
        AppToken GetAppToken(string communityId);
        Task SaveAppToken(AppToken appToken);
    }
}
