using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CadeauThea
{
    class TrialList
    {
        //use crypto RNG for more entropy == more variation
        RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider();

        private static void Shuffle<T>(this Random randomNumberGenerator, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = randomNumberGenerator.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }


    }
}
