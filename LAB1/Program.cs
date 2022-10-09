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
            var encryptedText = des.Encrypt("GEY SEX gey sex to gey sex to gey sex to gey sex to gey sex to sdsddddddddddddddddddddddddddddddddddddddddddddddddddddddd", key);
            
            Console.WriteLine(encryptedText);
            Console.WriteLine("=========================================================");
            var decryptedText = des.Decrypt(encryptedText, key);
            Console.WriteLine(decryptedText);
        }
    }
}
