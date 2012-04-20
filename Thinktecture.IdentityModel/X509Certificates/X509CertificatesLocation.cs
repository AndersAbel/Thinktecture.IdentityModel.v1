using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Thinktecture.IdentityModel.X509Certificates
{
    public class X509CertificatesLocation
    {
        StoreLocation _location;

        public X509CertificatesLocation(StoreLocation location)
        {
            _location = location;
        }

        public X509CertificatesName My
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.My);
            }
        }


        public X509CertificatesName AddressBook
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.AddressBook);
            }
        }
    }
}
