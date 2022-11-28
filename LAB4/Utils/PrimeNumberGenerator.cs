using System;
using System.Collections.Generic;

namespace LAB4.Utils
{
    public class PrimeNumberGenerator
    {
        //private readonly uint[] _matrix = new uint[624];
        //private int _index = 0;

        private readonly Object _object = new object();

        //public PrimeNumberGenerator() : this((uint)(0xFFFFFFFF & DateTime.Now.Ticks)) { }

        ///// <summary>
        ///// Initializes a new instance of the MersennePrimeRNG with a seed
        ///// </summary>
        ///// <param name="seed"></param>
        //public PrimeNumberGenerator(uint seed)
        //{
        //    _matrix[0] = seed;
        //    for (int i = 1; i < _matrix.Length; i++)
        //        _matrix[i] = (1812433253 * (_matrix[i - 1] ^ ((_matrix[i - 1]) >> 30) + 1));
        //}

        ///// <summary>
        ///// Generates a new matrix table
        ///// </summary>
        //private void Generate()
        //{
        //    for (int i = 0; i < _matrix.Length; i++)
        //    {
        //        uint y = (_matrix[i] >> 31) + ((_matrix[(i + 1) & 623]) << 1);
        //        _matrix[i] = _matrix[(i + 397) & 623] ^ (y >> 1);
        //        if (y % 2 != 0)
        //            _matrix[i] = (_matrix[i] ^ (2567483615));
        //    }
        //}

        ///// <summary>
        ///// Generates and returns a random number
        ///// </summary>
        ///// <returns></returns>
        //public int Next()
        //{
        //    lock (_object)
        //    {
        //        if (_index == 0)
        //            Generate();

        //        uint y = _matrix[_index];
        //        y = y ^ (y >> 11);
        //        y = (y ^ (y << 7) & (2636928640));
        //        y = (y ^ (y << 15) & (4022730752));
        //        y = (y ^ (y >> 18));

        //        _index = (_index + 1) % 623;
        //        return (int)(y % int.MaxValue);
        //    }
        //}

        ///// <summary>
        ///// Generates and returns a random number
        ///// </summary>
        ///// <param name="max">The highest value that can be returned</param>
        ///// <returns></returns>
     

        ///// <summary>
        ///// Generates and returns a random number
        ///// </summary>
        ///// <param name="min">The lowest value returned</param>
        ///// <param name="max">The highest value returned</param>
        ///// <returns></returns>
        //public int Next(int min, int max)
        //{
        //    lock (_object)
        //    {
        //        if (min > max)
        //            throw new ArgumentException("min cannot be greater than max", "min");
        //        return min + Next(min - max);
        //    }
        //}


        private List<int> primeNumberList;
        private Random _random = new Random();

        public PrimeNumberGenerator()
        {
            primeNumberList = GetAllPrimes(1_000_000);
        }

        public int Next()
        {
            lock (_object)
            {
                var randomValue = primeNumberList[_random.Next(primeNumberList.Count)];
                return randomValue;
            }
        }

        private List<int> GetAllPrimes(int n)
        {
            var result = new List<int> { 2 };
            for (int i = 3; i <= n; i += 2)
                if (IsPrime(i))
                    result.Add(i);
            return result;
        }

        private bool IsPrime(int n)
        {
            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
    }
}
