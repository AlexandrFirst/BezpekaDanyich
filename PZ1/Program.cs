using System;
using System.Collections.Generic;

namespace PZ1
{

    public abstract class Cipher<T>
    {
        protected readonly string alfabet;
        protected readonly string fullAlfabet;
        protected char[] fullAlfabetArray;
        protected int alphabetLength = 0;
        public Cipher(string locale)
        {
            if (locale == "EN")
            {
                alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            else
            {
                alfabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            }
            fullAlfabet = alfabet.ToLower() + alfabet;
            alphabetLength = fullAlfabet.Length;
            fullAlfabetArray = fullAlfabet.ToCharArray();
        }
        public abstract string Encrypt(string plainMessage, T key);
        public abstract string Decrypt(string encryptedMessage, T key);
    }

    public class CaesarCipher : Cipher<int>
    {
        public CaesarCipher(string locale) : base(locale)  { }
        private string CodeEncode(string text, int k)
        {
            var fullAlfabet = alfabet.ToLower() + alfabet;
            var q = fullAlfabet.Length;
            var fullAlfabetArray = fullAlfabet.ToCharArray();
            var retVal = "";
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var index = Array.IndexOf(fullAlfabetArray, c);
                if (index < 0)
                {
                    retVal += c.ToString();
                }
                else
                {
                    var codeIndex = (q + index + k) % q;
                    if (Char.IsUpper(c))
                        retVal += Char.ToUpper(fullAlfabet[codeIndex]);
                    else
                        retVal += Char.ToLower(fullAlfabet[codeIndex]);
                }
            }

            return retVal;
        }
        public override string Encrypt(string plainMessage, int key)
            => CodeEncode(plainMessage, key);
        public override string Decrypt(string encryptedMessage, int key)
            => CodeEncode(encryptedMessage, -key);
    }

    public class Token
    {
        public char TokenStr { get; set; }
        public int Position { get; set; }
    }

    public class VigenereCipher : Cipher<string>
    {

        public VigenereCipher(string local) : base(local) { }

        //генерация повторяющегося пароля
        private string GetRepeatKey(string s, int n)
        {
            var p = s;
            while (p.Length < n)
            {
                p += p;
            }

            return p.Substring(0, n);
        }

        private string Vigenere(string text, string password, bool encrypting = true)
        {
            var gamma = GetRepeatKey(password, text.Length);
            var retValue = "";
            var q = fullAlfabetArray.Length;

            for (int i = 0; i < text.Length; i++)
            {
                var letterIndex = fullAlfabet.IndexOf(text[i]);
                var codeIndex = fullAlfabet.IndexOf(gamma[i]);
                if (letterIndex < 0)
                {
                    retValue += text[i].ToString();
                }
                else
                {

                    var index = (q + letterIndex + ((encrypting ? 1 : -1) * codeIndex)) % q;
                    var letter = fullAlfabet[index].ToString();
                    retValue += Char.IsUpper(text[i]) ? letter.ToUpper() : letter.ToLower();
                }
            }

            return retValue;
        }


        private (string result, List<Token> tokens) clearFromTokens(string input)
        {
            List<Token> allTokens = new List<Token>();
            string clearedStr = "";
            char[] tokens = new char[] { ' ', ',', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
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

            return (clearedStr, allTokens);
        }

        private string putTokensBack(string str, List<Token> tokens)
        {
            var newStr = str;
            foreach (var t in tokens)
            {
                newStr = newStr.Insert(t.Position, t.TokenStr.ToString());
            }
            return newStr;
        }

        public override string Decrypt(string plainMessage, string password)
        {
            var clearedResult = clearFromTokens(plainMessage);

            var decrypted = Vigenere(clearedResult.result, password, false);

            decrypted = putTokensBack(decrypted, clearedResult.tokens);
            return decrypted;
        }

        public override string Encrypt(string plainMessage, string password)
        {
            var clearedResult = clearFromTokens(plainMessage);
            var encrypted = Vigenere(clearedResult.result, password);
            encrypted = putTokensBack(encrypted, clearedResult.tokens);
            return encrypted;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("~~Task 1");
            var caesar = new CaesarCipher("RU");
            var message = "Р гясисэс бахеюсэс, ида ая – гцьвцдямы ёъщъь. Дсьац шц щсфсхаияац умвсшцяъц эъзс,";
            var secretKey = 18;
            Console.WriteLine("Расшифрованное сообщение: {0}", caesar.Decrypt(message, secretKey));
            Console.WriteLine();
            Console.WriteLine("~Task 2");
            var visioner = new VigenereCipher("RU");
            var message2 = "прщуым ъй епръвцниф ёдой эюцмы уихумлфгя апс йамфщ у 1984 гопь. ан пьнхпоччшил, гыа есчс ты пъзуиллън воухажнъъдь иэшальучуатз к ьачръдве ъыьрыючфо кчжиа ишз ъли ычитонды адьнг алфъм, то иыа лидсэо бж ъэожщьп пръяцдуьь сутрцдиффусциф кгякъла смжъэа. дъффое нщцмя фмця шлхъра ъъдавлфссь нъцго чсйь кьигивът ьриыыагрлэъчеэуай гъфавоччюкох, ца в 2000 гъме, бллладаьз аднът ъзвръднох ьрзвфхастф к оллфшдичръьой цщъптълваффс, ъдей ьхалъън воыфатиюе у жиуцн.";
            var code2 = "алиса";

            Console.WriteLine("Расшифрованное сообщение: {0}", visioner.Decrypt(message2, code2));

            Console.WriteLine("~Task 3");
            var message3 = "зеэкле он ьаиоёнюби йыяв свнэу зммдеашъп шдх бреиэ к 1984 ужда. чю зрсмажлыпщд, чяч хйлх йл золкщдаюе тжзщччёоюым бсьчьфзыкркь о урпеюытэ ояубутыля глкар бмл сьб пыагжвзт рьрсъ рдиюд, гж эяч ьбшхфя щы юфяянаж аиогнфлра идкеъыщмичижби оъпгопч веыюфр. ьошляэ вэнэч ирнп ращсбш оюыръашивф вюнуж лхбм грмъщъоц уббпячуиабсзэсччъ ыошчтжлыхыжй, ъч т 2000 ыорь, сдапчфшрл чфёоц сшъеюыюжй азшъищчвки о ёьдиьыщпеюуяв кэсакопщрмих, сфэю амрдоюе тжпшчгбти к чбзъе.";
            var code3 = "Шамир";

            Console.WriteLine("Расшифрованное сообщение: {0}", visioner.Decrypt(message3, code3));
        }
    }
}
