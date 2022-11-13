using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LAB3
{
    public class HashGenerator
    {
        private readonly string[] pathes;

        private byte[] T8 = new byte[256];
        private byte[] T4 = new byte[32];
        private byte[] T2 = new byte[4];

        public HashGenerator(string[] pathes)
        {
            this.pathes = pathes;
        }
        public HashGenerator()
        {
   
        }

        public void Init(bool generateNew = false)
        {
            if (generateNew)
            {
                for (int i = 0; i < 256; i++)
                {
                    T8[i] = (byte)i;
                }
                Random random = new Random();
                T8 = T8.OrderBy(x => random.Next()).ToArray();
            }
            else
            {
                T8 = new byte[] {
                    98, 6, 85,150, 36, 23,112,164,135,207,169, 5, 26, 64,165,219, 
                    61, 20, 68, 89,130, 63, 52,102, 24,229,132,245, 80,216,195,115,
                    90,168,156,203,177,120, 2,190,188, 7,100,185,174,243,162, 10,
                    237, 18,253,225, 8,208,172,244,255,126,101, 79,145,235,228,121,
                    123,251, 67,250,161, 0,107, 97,241,111,181, 82,249, 33, 69, 55,
                    59,153, 29, 9,213,167, 84, 93, 30, 46, 94, 75,151,114, 73,222,
                    197, 96,210, 45, 16,227,248,202, 51,152,252,125, 81,206,215,186,
                    39,158,178,187,131,136, 1, 49, 50, 17,141, 91, 47,129, 60, 99,
                    154, 35, 86,171,105, 34, 38,200,147, 58, 77,118,173,246, 76,254,
                    133,232,196,144,198,124, 53, 4,108, 74,223,234,134,230,157,139,
                    189,205,199,128,176, 19,211,236,127,192,231, 70,233, 88,146, 44,
                    183,201, 22, 83, 13,214,116,109,159, 32, 95,226,140,220, 57, 12,
                    221, 31,209,182,143, 92,149,184,148, 62,113, 65, 37, 27,106,166,
                    3, 14,204, 72, 21, 41, 56, 66, 28,193, 40,217, 25, 54,179,117,
                    238, 87,240,155,180,170,242,212,191,163, 78,218,137,194,175,110,
                    43,119,224, 71,122,142, 42,160,104, 48,247,103, 15, 11,138,239
                };

                T4 = new byte[] {
                   10,31,21,27,16,23,30,11,
                    22,18,1,24,6,8,9,14,
                    29,17,13,19,26,20,5,0,
                    25,12,15,4,3,28,7,2
                };

                T2 = new byte[] { 2, 1, 3, 0 };
            }
        }
        //https://gist.github.com/CodesInChaos/4a399a26b98221155a92
        public void GenerateCollision() 
        {
            var m0 = new byte[] { 0 };
            var targetHash = pearsonHash(new byte[] { 1 });
            int maxAlphabet = 280; // limit the alphabet size to this
            byte[][] alphabetLetters1 = Enumerable.Range('A', 26).Select(i => new byte[] { (byte)i }).ToArray();
            byte[][] alphabetLetters2 = Combinations(alphabetLetters1, alphabetLetters1).ToArray(); // two letters

            var alphabet = alphabetLetters2.Take(maxAlphabet).ToArray();

            var h0 = pearsonHash(m0);
            Console.WriteLine("Target Hash: " + BitConverter.ToString( targetHash));

            Func<byte[], int, byte[], bool> check = (m, count, target) => pearsonHash(m0.Concat(m).ToArray()).Take(count).SequenceEqual(target.Take(count));

            var projections = alphabet;
            var fixedPoints = alphabet;


            for (int i = 1; i <= h0.Length; i++)
            {
                var fixedPoints2 = Combinations(fixedPoints, fixedPoints).Where(m => check(m, i, h0)).Take(maxAlphabet).ToArray();
                var projections2 = Combinations(fixedPoints, projections).Where(m => check(m, i, targetHash)).Take(maxAlphabet).ToArray();

                fixedPoints = fixedPoints2;
                projections = projections2;

                Console.WriteLine(i);
                Console.WriteLine(fixedPoints.Length + " " + projections.Length);
                Console.WriteLine(alphabet.First().Length);
                Console.WriteLine(BitConverter.ToString(pearsonHash(m0.Concat(projections.First()).ToArray())));
                Console.WriteLine();

            }

            var fullSuffix = projections.First();
            var combinedMessage = m0.Concat(fullSuffix).ToArray();
            Console.WriteLine(BitConverter.ToString(combinedMessage));
            
            var attackHash = pearsonHash(combinedMessage);
            var initHash = pearsonHash(new byte[] { 1 });

            Console.WriteLine("{0} | {1}", BitConverter.ToString(attackHash), BitConverter.ToString(initHash));

            Console.WriteLine("Suffix (hex): " + BitConverter.ToString(fullSuffix));
            Console.WriteLine("Suffix (text): " + Encoding.ASCII.GetString(fullSuffix));
            Console.WriteLine("Success: " + targetHash.SequenceEqual(attackHash));
        }

        private IEnumerable<byte[]> Combinations(ICollection<byte[]> xs, ICollection<byte[]> ys)
        {
            foreach (var x in xs)
            {
                foreach (var y in ys)
                {
                    yield return x.Concat(y).ToArray();
                }
            }
        }
        public (byte hash_b, string hash_s)[] DoHash8bit()
        {
            var result = pathes.Select(x => GenerateHash8bit(x)).ToArray();
            return result;
        }

        public (byte hash_b, string hash_s)[] DoHash4bit()
        {
            var result = pathes.Select(x => GenerateHash4bit(x)).ToArray();
            return result;
        }

        public (byte hash_b, string hash_s)[] DoHash2bit()
        {
            var result = pathes.Select(x => GenerateHash2bit(x)).ToArray();
            return result;
        }

        private (byte hash_b, string hash_s) GenerateHash8bit(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Invalid path to file");

            byte hash = 0;
            byte[] bytes = File.ReadAllBytes(path);

            foreach (var b in bytes)
            {
                hash = T8[(byte)(hash ^ b)];
            }

            string result = BitConverter.ToString(new byte[] { hash });

            return (hash_b: hash, hash_s: result);
        }

        private (byte hash_b, string hash_s) GenerateHash4bit(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Invalid path to file");

            byte hash = 0;
            byte[] bytes = File.ReadAllBytes(path);

            foreach (var b in bytes)
            {
                var b1 = (b & 0xF0) >> 4;
                var b2 = b & 0xF;

                hash = T4[(byte)(hash ^ b1)];
                hash = T4[(byte)(hash ^ b2)];
            }

            string result = BitConverter.ToString(new byte[] { hash });

            return (hash_b: hash, hash_s: result);
        }

        private (byte hash_b, string hash_s) GenerateHash2bit(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Invalid path to file");

            byte hash = 0;
            byte[] bytes = File.ReadAllBytes(path);

            foreach (var b in bytes)
            {
                var b1 = (b & 0xC0) >> 6;
                var b2 = (b & 0x30) >> 4;
                var b3 = (b & 0xC) >> 2;
                var b4 = b & 0x3;

                hash = T2[(byte)(hash ^ b1)];
                hash = T2[(byte)(hash ^ b2)];
                hash = T2[(byte)(hash ^ b3)];
                hash = T2[(byte)(hash ^ b4)];
            }

            string result = BitConverter.ToString(new byte[] { hash });

            return (hash_b: hash, hash_s: result);
        }

        private byte[] pearsonHash(byte[] input)
        {
            byte hash = 0;
            foreach (var b in input)
            {
                hash = T8[(byte)(hash ^ b)];
            }

            return new byte[] { hash };
        }
    }
}
