using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LAB4.Services
{
    public class Cypher : ICypher
    {
        private readonly string saltData;
        public Cypher(IConfiguration configuration)
        {
            saltData = configuration.GetSection("AES")["Salt"];
        }

        public byte[] EncryptMessage(byte[] publicKey, byte[] message)
        {
            
            byte[] encryptedMessage;
            byte[] salt = Encoding.Unicode.GetBytes(saltData);

            using (Aes encryptor = Aes.Create()) 
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(publicKey, salt, 10_000);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(message, 0, message.Length);
                        cs.Close();
                    }
                    encryptedMessage = ms.ToArray();
                }
            }

            return encryptedMessage;
        }

        public byte[] DecryptMessage(byte[] publicKey, byte[] message)
        {
            byte[] resultToReturn;

            byte[] encryptedText = message;
            byte[] salt = Encoding.Unicode.GetBytes(saltData);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(publicKey, salt, 10_000);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedText, 0, encryptedText.Length);
                        cs.Close();
                    }
                    resultToReturn = ms.ToArray();
                }
            }

            return resultToReturn;
        }
    }
}
