using System;
using System.Security.Cryptography;
using System.Text;
using Service.Contract;
using Service.Contract.DbModels;

namespace Service.Implementation
{
    public class EncryptedTokenCodeService : IEncryptedTokenCodeService
    {
        private readonly string _unregisteredGuid;
        private readonly IEncryptionService _encryptionService;

        public EncryptedTokenCodeService(string unregisteredGuid, IEncryptionService encryptionService)
        {
            _unregisteredGuid = unregisteredGuid ?? throw new ArgumentNullException(nameof(unregisteredGuid));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public string GetEncryptedTokenCode(string token, CdkCustomer cdkCustomer, string partnerKey, bool addPassword = false)
        {
            //Partner key.
            byte[] partnerKeyInBytes = Encoding.UTF8.GetBytes(partnerKey);

            //Convert token into bytes.
            byte[] tokenInBytes = Encoding.UTF8.GetBytes(token);

            // Calculate Hash value using via defined HMAC- algorithm using previously get token from
            // services (/RequestToken) API.
            HMACSHA384 objHmacsha384 = new HMACSHA384(partnerKeyInBytes);

            var hashMessage = objHmacsha384.ComputeHash(tokenInBytes);

            //var hashCodeObj = Convert.ToBase64String(hashMessage);
            var hashCodeObj = UtilityHelper.ByteToString(hashMessage);

            var base64Format = addPassword
                ? cdkCustomer.CustomerLoginId + ":" + hashCodeObj + ":" + _encryptionService.DecryptString(cdkCustomer.Password)
                : _unregisteredGuid + ":" + hashCodeObj + ":";

            //Convert Hash value into Base64 string in format<{candidateId}:{hash}:>
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(base64Format));
        }
    }
}