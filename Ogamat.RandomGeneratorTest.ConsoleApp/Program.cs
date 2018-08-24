using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ogamat.RandomGeneratorTest.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> results = new List<int>();
            /// choose implementation
            //RandomNumberGenerator generator = new NaiveSystemRandomNumberGenerator();
            //RandomNumberGenerator generator = new StaticSystemRandomNumberGenerator();
            //RandomNumberGenerator generator = new ThreadSafeStaticSystemRandomNumberGenerator();
            RandomNumberGenerator generator = new RNGCryptoServiceProvider();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IEnumerable<Task> tasks = Enumerable.Range(0, 1000000).Select((x) => Task.Factory.StartNew(() =>
            {
                byte[] data = new byte[10];
                generator.GetBytes(data);
                results.Add(BitConverter.ToInt32(data, 0));
            }
            ));
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            Console.WriteLine($"Generated numbers: {String.Join(", ", results)}");
            Console.WriteLine($"Number of distinct values: {results.Distinct().Count()}");
            Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} [ms]");
            Console.ReadKey();

        }
    }

    

    public class NaiveSystemRandomNumberGenerator : RandomNumberGenerator
    {
        public override void GetBytes(byte[] data)
        {
            new Random().NextBytes(data);
        }
    }

    public class StaticSystemRandomNumberGenerator : RandomNumberGenerator
    {
        private static readonly Random instance;

        static StaticSystemRandomNumberGenerator()
        {
            instance = new Random();
        }

        public override void GetBytes(byte[] data)
        {
            StaticSystemRandomNumberGenerator.instance.NextBytes(data);
        }
    }

    public class ThreadSafeStaticSystemRandomNumberGenerator : RandomNumberGenerator
    {
        private static readonly Random instance;
        private static readonly object globalLock = new object();

        static ThreadSafeStaticSystemRandomNumberGenerator()
        {
            instance = new Random();
        }

        public override void GetBytes(byte[] data)
        {
            lock (globalLock)
            {
                ThreadSafeStaticSystemRandomNumberGenerator.instance.NextBytes(data);
            }
        }
    }
}
