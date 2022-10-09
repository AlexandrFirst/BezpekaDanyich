using System;

namespace LAB1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ObsoleteDES des = new ObsoleteDES();
            //key max 8 chars

            string key = "superkey";
            var encryptedText = des.Encrypt("gey sex to gey sex to gey sex to gey sex to gey sex to gey sex to gey sex to gey sex to", key);
            
            Console.WriteLine(encryptedText);
            Console.WriteLine("=========================================================");
            var decryptedText = des.Decrypt(encryptedText, key);
            Console.WriteLine(decryptedText);
        }
    }
}
