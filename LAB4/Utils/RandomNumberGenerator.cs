using System;

namespace LAB4.Utils
{
    public class RandomNumberGenerator
    {
        Random random;

        private readonly Object _object = new object();

        public RandomNumberGenerator()
        {
            random = new Random();
        }

        public int GetRandomNumber(int maxValue) 
        {
            lock (_object)
            {
                return random.Next(1, maxValue);
            }
        }
    }
}
