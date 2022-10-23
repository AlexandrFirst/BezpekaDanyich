using LAB2.Utils;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LAB2.Services
{
    public class RsaManager : IRsaManager
    {
        private int rsaKeypairLength = 2048;

        private string privateKeyStr;
        private RSAParameters privateKey;


        private string publicKeyStr;
        private RSAParameters publicKey;

        private RSACryptoServiceProvider csp;

        public RsaManager()
        {
            initCryptoService();
        }

        private void initCryptoService() 
        {
            csp = new RSACryptoServiceProvider(rsaKeypairLength);
            privateKey = csp.ExportParameters(true);
            publicKey = csp.ExportParameters(false);


            privateKeyStr = getKeyString(privateKey);
            publicKeyStr = getKeyString(publicKey);
        }

        private string getKeyString(RSAParameters key) 
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, key);
            
            string pubKeyString = sw.ToString();
            return pubKeyString;
        }

        private string MakePem(byte[] ber, string header)
        {
            StringBuilder builder = new StringBuilder("-----BEGIN ");
            builder.Append(header);
            builder.AppendLine("-----");

            string base64 = Convert.ToBase64String(ber);
            int offset = 0;
            const int LineLength = 64;

            while (offset < base64.Length)
            {
                int lineEnd = Math.Min(offset + LineLength, base64.Length);
                builder.AppendLine(base64.Substring(offset, lineEnd - offset));
                offset = lineEnd;
            }

            builder.Append("-----END ");
            builder.Append(header);
            builder.AppendLine("-----");
            return builder.ToString();
        }

        public (string xmlFormat, RSAParameters rawFormat, string pemFormat) GetPrivateKey()
        {
            var pemFormat = MakePem(csp.ExportRSAPrivateKey(), "RSA PRIVATE KEY");

            return (privateKeyStr, privateKey, pemFormat);
        }

        public (string xmlFormat, RSAParameters rawFormat, string pemFormat) GetPublicKey()
        {
            var pemFormat = MakePem(csp.ExportSubjectPublicKeyInfo(), "PUBLIC KEY");


            Console.WriteLine("xmlFormat: \n" + publicKeyStr);
            Console.WriteLine("pemFormat: \n" + pemFormat);
            return (publicKeyStr, publicKey, pemFormat);
        }

        public void UpdateKeys()
        {
            initCryptoService();
        }
    }
}
