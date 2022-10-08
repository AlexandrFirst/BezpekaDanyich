using System;

namespace LAB1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ObsoleteDES des = new ObsoleteDES();
            //key max 8 chars

            string key = "aaaaaaaa";
            var encryptedText = des.Encrypt("Some text to encrypt", key);
            
            Console.WriteLine(encryptedText);
            Console.WriteLine("=========================================================");
            var decryptedText = des.Decrypt(encryptedText, key);
            Console.WriteLine(decryptedText);
        }
    }
}
