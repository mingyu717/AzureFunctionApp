namespace Service.Contract
{
    public interface IEncryptionService
    {
        string EncryptString(string clearText);
        string DecryptString(string cipherText);
    }
}