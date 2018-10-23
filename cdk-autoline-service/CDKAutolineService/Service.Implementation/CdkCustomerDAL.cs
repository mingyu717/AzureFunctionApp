using System;
using System.Linq;
using System.Threading.Tasks;
using Service.Contract;
using Service.Contract.DbModels;

namespace Service.Implementation
{
    public class CdkCustomerDAL : ICdkCustomerDAL
    {
        private readonly CDKAutolineContext _context;

        public CdkCustomerDAL(CDKAutolineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CdkCustomer GetCdkCustomer(string communityId, int customerNo)
        {
            if (communityId == null) throw new ArgumentNullException(nameof(communityId));
            if (customerNo < 0) throw new ArgumentOutOfRangeException(nameof(customerNo));

            return _context.CdkCustomers.FirstOrDefault(c => c.CommunityId == communityId && c.CustomerNo == customerNo);
        }

        public async Task<int> AddCustomer(CdkCustomer cdkCustomer)
        {
            if (cdkCustomer == null) throw new ArgumentNullException(nameof(cdkCustomer));
            if(cdkCustomer.CommunityId == null) throw new ArgumentNullException(nameof(cdkCustomer.CommunityId));
            if(cdkCustomer.CustomerNo < 0) throw new ArgumentOutOfRangeException(nameof(cdkCustomer.CustomerNo));

            _context.CdkCustomers.Add(cdkCustomer);
            return await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerToken(int id, Guid? token)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            var existingCdkCustomer = _context.CdkCustomers.FirstOrDefault(c => c.Id == id);
            if (existingCdkCustomer == null)
            {
                throw new Exception($"Customer does not existing with Id: {id}");
            }

            existingCdkCustomer.Token = token;
            await _context.SaveChangesAsync();
        }
    }
}