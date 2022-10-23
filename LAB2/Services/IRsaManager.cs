using System.Security.Cryptography;

namespace LAB2.Services
{
    public interface IRsaManager
    {
        public (string xmlFormat, RSAParameters rawFormat, string pemFormat) GetPublicKey();
        public (string xmlFormat, RSAParameters rawFormat, string pemFormat) GetPrivateKey();
        public void UpdateKeys();
    }
}
