using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens;

namespace Thinktecture.Samples
{
    class Program
    {
        static string _passwordClaimType = "http://www.identity.thinktecture.com/claims/password";
        static string _customerIdClaimType = "http://www.identity.thinktecture.com/claims/customerId";

        static void Main(string[] args)
        {
            StartHost();
            CallService();
        }

        private static void StartHost()
        {
            ServiceHost host = new ServiceHost(typeof(Service));

            // add message security endpoint via code
            host.AddServiceEndpoint(
                typeof(IService),
                new ClientSamlHttpBinding(SecurityMode.Message),
                "Message");

            var config = new ServiceConfiguration();
            config.SecurityTokenHandlers.AddOrReplace(new ClientSaml11SecurityTokenHandler());

            FederatedServiceCredentials.ConfigureServiceHost(host, config);
            host.Open();

            host.Description.Endpoints.ToList().ForEach(ep => Console.WriteLine(ep.Address));
        }

        private static void CallService()
        {
            var identity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(WSIdentityConstants.ClaimTypes.Name, "Bob"),
                    new Claim(_passwordClaimType, "Bob"),
                    new Claim(_customerIdClaimType, "42")
                });


            var token = ClientSaml11SecurityTokenHandlerBase.CreateToken(identity);

            Console.WriteLine("\nMIXED MODE::::");
            CallMixedMode(token);

            Console.WriteLine("\nMESSAGE MODE::::");
            CallMessage(token);
        }

        private static void CallMessage(SamlSecurityToken token)
        {
            var factory = new ChannelFactory<IServiceClientChannel>(
                new ClientSamlHttpBinding(SecurityMode.Message),
                new EndpointAddress(
                    new Uri("http://roadie:9000/Services/ClientSaml/Message"),
                    EndpointIdentity.CreateDnsIdentity("Service")));

            factory.Credentials.ServiceCertificate.SetDefaultCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindBySubjectDistinguishedName,
                "CN=Service");

            factory.ConfigureChannelFactory<IServiceClientChannel>();
            var proxy = factory.CreateChannelWithIssuedToken<IServiceClientChannel>(token);

            proxy.Ping("foo");
            proxy.Close();
        }

        private static void CallMixedMode(SamlSecurityToken token)
        {
            var factory = new ChannelFactory<IServiceClientChannel>("*");

            factory.ConfigureChannelFactory<IServiceClientChannel>();
            var proxy = factory.CreateChannelWithIssuedToken<IServiceClientChannel>(token);

            proxy.Ping("foo");
            proxy.Close();
        }
    }
}
