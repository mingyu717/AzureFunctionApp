using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Service.Contract;

namespace Service.Implementation
{
    public class RijndaelEncryptionService : IEncryptionService
    {
        private readonly string _secretKey;

        public RijndaelEncryptionService(string secretKey)
        {
            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="clearText">The clear text.</param>
        /// <returns></returns>
        public string EncryptString(string clearText)
        {
            if (string.IsNullOrWhiteSpace(clearText)) throw new ArgumentNullException(nameof(clearText));

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            byte[] encryptedData;
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(_secretKey,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}))
            {
                encryptedData = EncryptString(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            }

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="clearText">The clear text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        private byte[] EncryptString(byte[] clearText, byte[] Key, byte[] IV)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearText, 0, clearText.Length);
                    }
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        public string DecryptString(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) throw new ArgumentNullException(nameof(cipherText));

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] decryptedData;
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(_secretKey,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}))
            {
                decryptedData = DecryptString(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            }

            return Encoding.Unicode.GetString(decryptedData);
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="cipherData">The cipher data.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        private byte[] DecryptString(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] decryptedData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    alg.Key = Key;
                    alg.IV = IV;
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                    }
                }

                decryptedData = ms.ToArray();
            }

            return decryptedData;
        }
    }
}