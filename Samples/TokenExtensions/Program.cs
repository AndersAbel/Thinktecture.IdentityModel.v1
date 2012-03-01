using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Utility;

namespace TokenExtensions
{
    class Program
    {
        static string idp = "https://roadie/idsrv/issue/wstrust/mixed/certificate";
        static string clearTextRP = "http://leastprivilege.accesscontrol.windows.net/";
        static string encryptedRP = "http://roadie/rp/";

        static X509Certificate2 signingCert;

        static void Main(string[] args)
        {
            signingCert = X509Certificates.GetCertificateFromStore(
                "CN=roadie, OU=Research, O=LeastPrivilege, L=Heidelberg, S=BaWue, C=DE", 
                StoreLocation.LocalMachine);

            BearerClearText();
            SymmetricEncrypted();
        }

        private static void BearerClearText()
        {
            var token = RequestBearerClearTextToken();

            var principal = token.ToClaimsPrincipal(signingCert);

            "\nBearerClearText\n".ConsoleRed();
            ClaimsViewer.ShowConsole(principal);
        }

        private static void SymmetricEncrypted()
        {
            var token = RequestSymmetricEncryptedToken(signingCert);

            var principal = token.ToClaimsPrincipal(signingCert);

            "\nSymmetric Encrypted\n".ConsoleRed();
            ClaimsViewer.ShowConsole(principal);
        }
        

        private static SecurityToken RequestBearerClearTextToken()
        {
            var factory = new WSTrustChannelFactory(
                new CertificateWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(idp));
            factory.Credentials.ClientCertificate.Certificate = X509Certificates.GetCertificateFromStore("CN=Client");

            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(clearTextRP),
                KeyType = KeyTypes.Bearer,
            };

            

            var genericToken = factory.CreateChannel().Issue(rst) as GenericXmlSecurityToken;
            var token = genericToken.ToSecurityToken();
            
            return token;
        }

        private static SecurityToken RequestSymmetricEncryptedToken(X509Certificate2 decryptionCert)
        {
            var factory = new WSTrustChannelFactory(
                new CertificateWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(idp));
            factory.Credentials.ClientCertificate.Certificate = X509Certificates.GetCertificateFromStore("CN=Client");

            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(encryptedRP),
                KeyType = KeyTypes.Symmetric
            };

            var genericToken = factory.CreateChannel().Issue(rst) as GenericXmlSecurityToken;
            var token = genericToken.ToSecurityToken(decryptionCert);

            return token;
        }
    }
}
