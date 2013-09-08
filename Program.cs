using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BlowFishCS;

namespace cryptool.net
{
    class Program
    {
        private Thread workerThread;

        private SynchronizationContext context;

        public event EventHandler SomethingHappened;
        private static long keyCount =0;
        private static DateTime start= DateTime.Now;

        static void Main(string[] args)
        {
            BlowFish b = new BlowFish("8C56905D8B");
            string plainText = "Hello";
            string cipherText = b.Encrypt_ECB(plainText);
            cipherText = "57E5B6A79048A19B";
            Console.WriteLine(cipherText);            
            plainText = b.Decrypt_ECB(cipherText);
            Console.WriteLine(plainText);

            int keyLength = 5; //bytes
            Crack(cipherText, keyLength, new byte[] { });
            Console.WriteLine("done");
        }

        static void Crack(string cipherText, int keylength, byte[] currentKey)
        {
            
            if (keylength == currentKey.Length)
            {
                BlowFish b = new BlowFish(currentKey);
                string plainText = b.Decrypt_ECB(cipherText);
                if (plainText.StartsWith("Hello"))
                    Console.WriteLine("plaintext:" + plainText+", key:"+BitConverter.ToString(currentKey));
                keyCount++;
                if (keyCount % 10240 == 0)
                {
                    TimeSpan elapsed = DateTime.Now.Subtract(start);
                    long keyrate = 0;
                    if (elapsed.Seconds > 0) keyrate = (keyCount / elapsed.Seconds);
                    Console.WriteLine((keyCount / 1099511627776).ToString() + "%, seconds:" + elapsed.Seconds.ToString() + ", " + keyrate.ToString() + "keys/s ->" + BitConverter.ToString(currentKey));
                }
            }
            else
            {
                byte[] key = new byte[currentKey.Length+1];
                Array.Copy(currentKey,key,currentKey.Length);
                for (int i = 0; i < 256; i++)
                {
                    key[key.Length-1] = (byte)i;
                    Crack(cipherText, keylength, key);
                }
            }
        }

    }
}
