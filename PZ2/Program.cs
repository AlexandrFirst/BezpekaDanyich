using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PZ2
{
    public class Token
    {
        public char TokenStr { get; set; }
        public int Position { get; set; }
    }

    class Program
    {
        public readonly static IReadOnlyDictionary<char, string> val2Key;
        public readonly static IReadOnlyDictionary<string, char> key2Val;
        static Program()
        {
            val2Key = new Dictionary<char, string> {
                {'А',"1"},  {'Б',"81"}, {'В',"82"},  {'Г',"83"},  {'Д',"84"},   {'Е',"4"},
                {'Ж',"85"},   {'З',"86"}, {'И',"2"},  {'К',"87"},   {'Л',"88"},   {'М',"89"},
                {'Н',"6"},  {'О',"7"}, {'П',"80"},   {'Р',"91"}, {'С',"5"},  {'Т',"3"}, {'У',"92"},
                {'Ф',"93"}, {'Х',"94"},  {'Ц',"95"}, {'Ч',"96"},   {'Ш',"97"}, {'Щ',"98"},   {'Ъ',"99"},
                {'Ы',"90"},  {'Ь',"01"},{'Э',"02"},  {'Ю',"03"}, {'Я',"04"}, {' ',"00"},
                {'1',"711"},  {'2',"712"},{'3',"713"},  {'4',"714"}, {'5',"715"},{'6',"716"},
                {'7',"717"},{'8',"718"},  {'9',"719"}, {'0',"710"} };

            key2Val = val2Key.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        public static string EncodeByTable(string s)
        {
            return string.Concat(s.ToUpper().ToCharArray()
                .Where(c => val2Key.ContainsKey(c)).Select(c => val2Key[c]));
        }

        public static string DecodedTable(string s)
        {
            return string.Concat(Regex.Matches(s, "71.|8.|9.|0.|.").Cast<Match>()
                .Where(m => key2Val.ContainsKey(m.Value)).Select(m => key2Val[m.Value]));
        }

        public static (List<Token> tokens, string pureString) ClearText(string input)
        {
            char[] tokens = new char[] { ',', '.' };

            List<Token> allTokens = new List<Token>();
            string clearedStr = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (Array.Exists<char>(tokens, x => x == input[i]))
                {
                    allTokens.Add(new Token() { TokenStr = input[i], Position = i });
                }
                else
                {
                    clearedStr += input[i];
                }
            }
            return (allTokens, clearedStr);
        }

        public static string PutTokensBack(List<Token> tokens, string input)
        {
            var result = input;
            foreach (var token in tokens)
            {
                result = result.Insert(token.Position, token.TokenStr.ToString());
            }
            return result;
        }

        public static string GetRepeatKey(string s, int n)
        {
            var p = s;
            while (p.Length < n)
            {
                p += p;
            }

            return p.Substring(0, n);
        }


        private static string DecodeInput(string encodedInput, string key) 
        {
            
            var encodedText = encodedInput;
            var repeatedKey = GetRepeatKey(key, encodedText.Length);
            
            var decodedText = "";
            var encodedRepeatedKey = EncodeByTable(repeatedKey);
            for (int i = 0; i < encodedText.Length; i++)
            {
                var temp = int.Parse(encodedText[i].ToString()) - int.Parse(encodedRepeatedKey[i].ToString());
                if (temp < 0)
                {
                    temp = 10 + temp;
                }
                decodedText += temp.ToString();
            }
            var text = DecodedTable(decodedText);

            return text;
        }

        private static string EncodeInput(string originalText, string key) 
        {
            var encodedTextByTable = EncodeByTable(originalText);
            var encodedKeyByTable = EncodeByTable(key);
            var repeatedEncodedKey = GetRepeatKey(encodedKeyByTable, encodedTextByTable.Length);

            string encodedTextByKey = "";
            for (int i = 0; i < encodedTextByTable.Length; i++)
            {
                var temp = (int.Parse(encodedTextByTable[i].ToString()) + int.Parse(repeatedEncodedKey[i].ToString())) % 10;
                encodedTextByKey += temp.ToString();
            }
            return encodedTextByKey;
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine("~~~Task 1");
            var decodedTextPart1 = DecodeInput("6719882196864085864979275245", "лес");
            System.Console.WriteLine(decodedTextPart1);
            System.Console.WriteLine("~~~Task 2");
            
            string inputInfo = "Александр Логвнов 3114.2112.1.1234567890 1 2 3 4 5 6 7 8 9 0 10 77 171 717";
            string key = "олень";

            var clearedStringInfo = ClearText(inputInfo);
            var textToEncode = clearedStringInfo.pureString.ToUpper();

            var encodedText = EncodeInput(textToEncode, key);
            Console.WriteLine("EncodedText: " + encodedText);
            var decodedText = DecodeInput(encodedText, key);
            Console.WriteLine("DecodedText: " + decodedText);
        }
    }
}
