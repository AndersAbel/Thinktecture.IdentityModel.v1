using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Thinktecture.Samples
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            byte[] bytes = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            var byteString = Convert.ToBase64String(bytes);

            Console.WriteLine(byteString);
            Clipboard.SetText(byteString);
        }
    }
}
