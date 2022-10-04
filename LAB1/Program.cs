using System;

namespace LAB1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            DES des = new DES();
            //key max 8 chars
            string key = "ketToEnc";
            var encryptedText = des.Encrypt("Some text to encrypt", key);
            Console.WriteLine(encryptedText);
            Console.WriteLine("=========================================================");
            var decryptedText = des.Decrypt(encryptedText, key);
            Console.WriteLine(decryptedText);
        }
    }
}
