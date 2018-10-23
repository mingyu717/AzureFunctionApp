using System;
using System.Threading.Tasks;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Models;

namespace Service.Implementation
{
    public class CdkCustomerService : ICdkCustomerService
    {
        private readonly IPasswordService _passwordService;
        private readonly ICdkCustomerDAL _cdkCustomerDal;
        private readonly IEncryptionService _encryptionService;

        public CdkCustomerService(IPasswordService passwordService, ICdkCustomerDAL cdkCustomerDal, IEncryptionService encryptionService)
        {
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _cdkCustomerDal = cdkCustomerDal ?? throw new ArgumentNullException(nameof(cdkCustomerDal));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public CdkCustomer MapCdkCustomer(CustomerVehicleRegisterRequest customerVehicleRegisterRequest)
        {
            if (customerVehicleRegisterRequest == null) throw new ArgumentNullException(nameof(customerVehicleRegisterRequest));

            return new CdkCustomer
            {
                CommunityId = customerVehicleRegisterRequest.CommunityId,
                CustomerNo = customerVehicleRegisterRequest.CustomerNo,
                CustomerLoginId = $"{customerVehicleRegisterRequest.CommunityId}{customerVehicleRegisterRequest.RoofTopId}{customerVehicleRegisterRequest.CustomerNo}",
                Password = _passwordService.GeneratePassword()
            };
        }
        
        public async Task SaveCdkCustomer(CdkCustomer cdkCustomer)
        {
            if (cdkCustomer == null) throw new ArgumentNullException(nameof(cdkCustomer));

            var existingCdkCustomer = _cdkCustomerDal.GetCdkCustomer(cdkCustomer.CommunityId, cdkCustomer.CustomerNo);
            if (existingCdkCustomer != null)
            {
                await _cdkCustomerDal.UpdateCustomerToken(existingCdkCustomer.Id, cdkCustomer.Token);
            }
            else
            {
                cdkCustomer.Password = _encryptionService.EncryptString(cdkCustomer.Password);
                await _cdkCustomerDal.AddCustomer(cdkCustomer);
            }
        }

        public CdkCustomer GetCdkCustomer(string communityId, int customerNo)
        {
            if (communityId == null) throw new ArgumentNullException(nameof(communityId));
            if (customerNo < 0) throw new ArgumentOutOfRangeException(nameof(customerNo));

            var cdkCustomer = _cdkCustomerDal.GetCdkCustomer(communityId, customerNo);

            return cdkCustomer;
        }
    }
}