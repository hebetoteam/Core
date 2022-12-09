using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeCoreApp.Utilities.Helpers
{
    public class GameLib
    {
        static string ClientSeed = "000000000000000007a9a31ff7f07463d91af6b5454241d5faf282e5e0fe1b3a";

        const int nbits = 52;

        public static string GenerateHash(string serverSeed)
        {
            return ClsCrypto.ComputeSha256Hash(serverSeed);
        }

        public static long GetCrashPointFromHash(string serverSeed)
        {
            var hash  = GenerateHash(serverSeed);

            ClientSeed = hash;

            if (Divisible(hash, 101))
                return 0;

            // Use the most significant 52-bit from the hash to calculate the crash point
          
            var h = long.Parse(hash.Substring(0, 13), NumberStyles.HexNumber);
            var e = Math.Pow(2, nbits);

            return (long)Math.Floor((100 * e - h) / (e - h));

        }

        private static bool Divisible(string hash,int mod)
        {
            // We will read in 4 hex at a time, but the first chunk might be a bit smaller
            // So ABCDEFGHIJ should be chunked like  AB CDEF GHIJ
            var val = 0;

            var o = hash.Length % 4;
            for (var i = o > 0 ? o - 4 : 0; i < hash.Length; i += 4)
            {
                var a = (val << 16);
                var b = hash.Substring(i, 4);
                var c = int.Parse(b, NumberStyles.HexNumber);
                val = (a+c) % mod;

                //int v = int.Parse(hash.Substring(i, i + 4), NumberStyles.HexNumber);
                //int y = (int)((uint)val << 16);
                //val = (y + v) % mod;
            }

            return val == 0;
        }

        
    }
}
