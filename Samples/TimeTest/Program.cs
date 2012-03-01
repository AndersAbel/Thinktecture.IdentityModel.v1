using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Thinktecture.IdentityModel;

namespace Thinktecture.Samples
{
    class Program
    {
        static int iterations = 10000;

        static void Main(string[] args)
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(bytes);

            var string1 = Convert.ToBase64String(bytes);
            var string2 = Convert.ToBase64String(bytes);

            bytes[0]++;
            var string3 = Convert.ToBase64String(bytes);

            Test(string1, string2, string3);
            TestConstant(string1, string2, string3);
        }

        private static void Test(string x, string y, string z)
        {
            var sw1 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var same = (x == y);
            }
            Console.WriteLine("Same String: {0}", sw1.ElapsedMilliseconds);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var same = (x == z);
            }
            Console.WriteLine("Different String: {0}", sw2.ElapsedMilliseconds);
            sw2.Stop();
        }

        private static void TestConstant(string x, string y, string z)
        {
            var sw1 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                bool same = ObfuscatingComparer.IsEqual(x, y);
            }
            Console.WriteLine("Same String: {0}", sw1.ElapsedMilliseconds);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                bool same = ObfuscatingComparer.IsEqual(x, z);
            }
            Console.WriteLine("Different String: {0}", sw2.ElapsedMilliseconds);
            sw2.Stop();
        }
    }
}
