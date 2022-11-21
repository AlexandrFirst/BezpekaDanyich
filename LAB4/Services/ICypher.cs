namespace LAB4.Services
{
    public interface ICypher
    {
        byte[] DecryptMessage(byte[] publicKey, byte[] message);
        byte[] EncryptMessage(byte[] publicKey, byte[] message);
    }
}
