using System;

namespace LAB1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ObsoleteDES des = new ObsoleteDES();
            //key max 8 chars

            string key = "aaa";
            var encryptedText = des.Encrypt("Hello world12ahfbdsvfbvsdhfgdgdggdgd some interesting tet gay se", key);
            
            Console.WriteLine(encryptedText);
            Console.WriteLine("=========================================================");
            var decryptedText = des.Decrypt(encryptedText, key);
            Console.WriteLine(decryptedText);
        }
    }
}
