using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LAB3
{
    internal class Program
    {
        public enum HashType { Bit8, Bit4, Bit2 };

        static void Main(string[] args)
        {
            string file1 = "Resources/test1.cpp";
            string file2 = "Resources/test2.jpg";
            string file3 = "Resources/test3.txt";

            string m_file1 = "Resources/test1_1.cpp";
            string m_file2 = "Resources/test2_2.jpg";
            string m_file3 = "Resources/test3_3.txt";

            Console.WriteLine("Original files:");
            var bit_array1 = hashRun(new string[] { file1, file2, file3 }).ToArray();

            Console.WriteLine("Modified files:");
            var bit_array2 = hashRun(new string[] { m_file1, m_file2, m_file3 }).ToArray();

            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
            Console.WriteLine(new string('-', 80));

            if (bit_array1.Count() != bit_array2.Count())
            {
                Console.WriteLine("Unable to comapre bit array");
                return;
            }

            for (int i = 0; i < bit_array1.Count(); i++)
            {
                var res1 = bit_array1[i];
                var res2 = bit_array2[i];
                Console.WriteLine(res1.hashType.ToString());

                var bit_arr1 = res1.bitArray;
                var bit_arr2 = res2.bitArray;

                for (int j = 0; j < bit_arr1.Length; j++)
                {
                    Console.WriteLine("File {0} report:", j);

                    double totalBitsToComapre = 0;
                    double diffBits = 0;

                    switch (res1.hashType)
                    {
                        case HashType.Bit8:
                            totalBitsToComapre = 8;
                            break;
                        case HashType.Bit4:
                            totalBitsToComapre = 4;
                            break;
                        case HashType.Bit2:
                            totalBitsToComapre = 2;
                            break;
                        default:
                            break;
                    }

                    for (int k = 0; k < 8; k++)
                    {
                        diffBits += Math.Abs((bit_arr1[j][k] ? 1 : 0) - (bit_arr2[j][k] ? 1 : 0));
                    }

                    double diffRatio = diffBits / totalBitsToComapre * 100;
                    Console.WriteLine("Diff ratio is: " + diffRatio);
                }
                Console.WriteLine(new string('=', 80));
            }

            Console.WriteLine(new string('*', 80));

            HashGenerator hashGenerator = new HashGenerator();
            hashGenerator.Init();
            hashGenerator.GenerateCollision();

        }

        private static IEnumerable<(BitArray[] bitArray, HashType hashType)> hashRun(string[] filesToHash)
        {
            HashGenerator hashGenerator = new HashGenerator(filesToHash);
            hashGenerator.Init();

            List<(Func<(byte hash_b, string hash_s)[]> func, HashType type)> doHashFunc =
                new List<(Func<(byte hash_b, string hash_s)[]> func, HashType type)>() {
                (hashGenerator.DoHash8bit, HashType.Bit8),
                (hashGenerator.DoHash4bit, HashType.Bit4),
                (hashGenerator.DoHash2bit, HashType.Bit2),
            };

            foreach (var hashFunc in doHashFunc)
            {
                Console.WriteLine(hashFunc.type.ToString());

                var resultsBit = hashFunc.func();
                displayHashResult(resultsBit);

                yield return (bitArray: resultsBit
                    .Select(x => new BitArray(new byte[] { x.hash_b })).ToArray(),
                    hashFunc.type);
            }
        }

        private static void displayHashResult((byte hash_b, string hash_s)[] results)
        {
            Array.ForEach(results, x =>
            {
                Console.WriteLine(new string('~', 80));
                Console.WriteLine("hex format: {0}", x.hash_s);
                Console.WriteLine("int format: {0}", x.hash_b);
                Console.WriteLine();
            });
        }
    }
}
