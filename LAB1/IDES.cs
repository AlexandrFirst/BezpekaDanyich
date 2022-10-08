using System;
using System.Collections.Generic;
using System.Text;

namespace LAB1
{
    public interface IDES
    {
        string Encrypt(string plainText, string key);
        string Decrypt(string encodedText, string key);
        string ConvertBitsToText(int[] sentarray, int len);

        public string[] GetkeyForRounds { get;}
        public int[][] GetEntropiaPerRound { get; }
    }
}
