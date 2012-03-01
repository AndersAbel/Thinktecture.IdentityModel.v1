using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Utility;

namespace ExtractTokenFromRSTR
{
    class Program
    {
        static string idp = "https://roadie/idsrv/issue/wstrust/mixed/certificate";
        static string encryptedRP = "http://roadie/rp/";

        static void Main(string[] args)
        {
            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(encryptedRP),
                KeyType = KeyTypes.Symmetric
            };

            var rstr = RequestToken(rst);
            var token = rstr.ToGenericXmlSecurityToken(rst);

            var rstr2 = RequestTokenInMemory(rst);
            var token2 = rstr2.ToGenericXmlSecurityToken(rst);
        }

        private static RequestSecurityTokenResponse RequestToken(RequestSecurityToken rst)
        {
            var factory = new WSTrustChannelFactory(
                new CertificateWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(idp));
            factory.Credentials.ClientCertificate.Certificate = X509Certificates.GetCertificateFromStore("CN=Client");

            RequestSecurityTokenResponse rstr;
            var token = factory.CreateChannel().Issue(rst, out rstr);

            return rstr;
        }

        private static RequestSecurityTokenResponse RequestTokenInMemory(RequestSecurityToken rst)
        {
            var signingCert = X509Certificates.GetCertificateFromStore("CN=STS", StoreLocation.LocalMachine);
            var encryptingCert = X509Certificates.GetCertificateFromStore("CN=Service", StoreLocation.LocalMachine);

            var config = new InMemoryStsConfiguration(signingCert);
            var sts = new InMemorySts(config, encryptingCert);

            var id = new ClaimsIdentity(new List<Claim> 
                {
                    new Claim(ClaimTypes.Name, "dominick")
                });

            return sts.Issue(ClaimsPrincipal.CreateFromIdentity(id), rst);
        }
    }
}
