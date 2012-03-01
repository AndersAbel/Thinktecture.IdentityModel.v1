using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Client.ServiceReference;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Thinktecture.IdentityModel.Utility;
using Thinktecture.IdentityModel.Web.Old;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            "\nSAML\n".ConsoleYellow();
            CallService(RequestSamlToken(), WebClientTokenSchemes.SAML);

            "\nSWT\n".ConsoleYellow();
            CallService(CreateSwtToken(), WebClientTokenSchemes.SWT);
        }

        static void CallService(string token, string type)
        {
            var data = new ClaimsData(new Uri("http://localhost:9999/odata.svc/"));
            data.SetAccessToken(token, type);

            data.Claims.ToList().ForEach(c =>
                Console.WriteLine("{0}\n {1}\n ({2})\n", c.ClaimType, c.Value, c.Issuer));
        }

        private static string RequestSamlToken()
        {
            string idpAddress = "https://roadie/stsce/users/issue.svc/mixed/username";

            var credentials = new ClientCredentials();
            credentials.UserName.UserName = "dominick";
            credentials.UserName.Password = "abc!123";

            var token = WSTrustClient.Issue(
                new EndpointAddress(idpAddress),
                new EndpointAddress("http://websample"),
                new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                credentials);

            return (token as GenericXmlSecurityToken).TokenXml.OuterXml;
        }

        private static string CreateSwtToken()
        {
            var signingKey = "wAVkldQiFypTQ+kdNdGWCYCHRcee8XmXxOvgmak8vSY=";
            var audience = "http://websample";
            var issuer = "http://self";

            var token = new SimpleWebToken(issuer, audience, Convert.FromBase64String(signingKey));

            token.AddClaim(ClaimTypes.Name, "dominick");
            token.AddClaim(ClaimTypes.Email, "dominick.baier@thinktecture.com");
            token.AddClaim(ClaimTypes.Role, "Users");
            token.AddClaim(ClaimTypes.Role, "Administrators");
            token.AddClaim("simple", "test");

            return token.ToString();
        }
    }
}
