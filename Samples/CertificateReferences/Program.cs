using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenFromStore();
            StoreDeployed();
            FileDeployed();
            DownloadSsl();
        }

        private static void OpenFromStore()
        {
            var cert = X509Certificates.GetCertificateFromStore("CN=Service");
            X509Certificate2UI.DisplayCertificate(cert);
        }

        private static void DownloadSsl()
        {
            var cert = X509Certificates.DownloadSslCertificate("www.microsoft.com", 443);
            X509Certificate2UI.DisplayCertificate(cert);
        }

        private static void FileDeployed()
        {
            var cert = X509Certificates.GetCertificate("File");
            X509Certificate2UI.DisplayCertificate(cert);
        }

        private static void StoreDeployed()
        {
            var cert = X509Certificates.GetCertificate("Store");
            X509Certificate2UI.DisplayCertificate(cert);
        }
    }
}
