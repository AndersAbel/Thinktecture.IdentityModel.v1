using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thinktecture.IdentityModel.X509Certificates;
using System.Security.Cryptography.X509Certificates;

namespace Certificates
{
    class Program
    {
        static void Main(string[] args)
        {
            var cert = X509.CurrentUser.My.Thumbprint.Find("abc");
        }
    }
}
