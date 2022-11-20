namespace LAB4.Services
{
    public interface ICypher
    {
        string DecryptMessage(byte[] publicKey, string message);
        string EncryptMessage(byte[] publicKey, string message);
    }
}
