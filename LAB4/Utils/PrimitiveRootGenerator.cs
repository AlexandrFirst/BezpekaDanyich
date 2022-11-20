using System.Collections.Generic;

namespace LAB4.Utils
{
    public class PrimitiveRootGenerator
    {
        private int powmod(int a, int b, int p)
        {
            int res = 1;
            while (b != 0)
                if ((b & 1) != 0) {
                    res = (int)(res * 1L * a % p);
                    --b;
                }
                else
                {
                    a = (int)(a * 1L * a % p);
                    b >>= 1;
                }
            return res;
        }

        public int Generator(int p) 
        {
            List<int> fact = new List<int>();
            int phi =  p - 1;
            int n = phi;

            for (int i = 2; i * i <= n; ++i) 
            {
                if (n % i == 0)
                {
                    fact.Add(i);
                    while (n % i == 0)
                        n /= i;
                }
            }

            if (n > 1) 
            {
                fact.Add(n);
            }

            for (int res = 2; res <= p; ++res) 
            {
                bool ok = true;
                for (int i = 0; i < fact.Count && ok; ++i)
                    ok &= powmod(res, phi / fact[i], p) != 1;
                if (ok) return res;
            }
            return -1;
        }
    }
}
