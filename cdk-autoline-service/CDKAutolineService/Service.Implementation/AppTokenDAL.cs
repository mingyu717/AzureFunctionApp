using System;
using Service.Contract;
using System.Linq;
using System.Threading.Tasks;
using Service.Contract.DbModels;

namespace Service.Implementation
{
    public class AppTokenDAL : IAppTokenDAL
    {
        private readonly CDKAutolineContext _context;

        public AppTokenDAL(CDKAutolineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get app token by communityid.
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public AppToken GetAppToken(string communityId)
        {
            return _context.AppTokens.FirstOrDefault(x => x.CommunityId == communityId);
        }

        /// <summary>
        /// Save app token details
        /// </summary>
        /// <param name="appToken"></param>
        /// <returns></returns>
        public async Task SaveAppToken(AppToken appToken)
        {
            var objAppToken = GetAppToken(appToken.CommunityId);
            if (objAppToken == null)
            {
                _context.AppTokens.Add(appToken);
            }
            else
            {
                appToken.Id = objAppToken.Id;
                objAppToken.Token = appToken.Token;
            }

            await _context.SaveChangesAsync();
        }
    }
}