using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityModel.Tokens;

namespace Thinktecture.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            StartHost();
            CallHost();
        }

        private static void CallHost()
        {
            // Windows credentials
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nPress Enter for Windows Credentials");
            Console.ResetColor();
            Console.ReadLine();
            var proxy = new ChannelFactory<IClaimsService>("Windows").CreateChannel();
            CallProxy(proxy);

            // Certificate credentials
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nPress Enter for Certificate Credentials");
            Console.ResetColor();
            Console.ReadLine();
            proxy = new ChannelFactory<IClaimsService>("Certificates").CreateChannel();
            CallProxy(proxy);

            // UserName credentials
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nPress Enter for UserName Credentials");
            Console.ResetColor();
            Console.ReadLine();

            var factory = new ChannelFactory<IClaimsService>("UserName");
            factory.Credentials.UserName.UserName = "dominick";
            factory.Credentials.UserName.Password = "dominick";
            proxy = factory.CreateChannel();
            CallProxy(proxy);
        }

        private static void StartHost()
        {
            Console.WriteLine("setting up host...");
            ServiceHost host = new ServiceHost(typeof(ClaimsService));

            var wifConfiguration = FederatedAuthentication.ServiceConfiguration;
            wifConfiguration.SecurityTokenHandlers.AddOrReplace(
                new GenericUserNameSecurityTokenHandler((username, password) => username == password));

            FederatedServiceCredentials.ConfigureServiceHost(host, FederatedAuthentication.ServiceConfiguration);
            host.Open();
            
            Console.WriteLine("host running...\n");
            host.Description.Endpoints.ToList().ForEach(ep => Console.WriteLine(ep.Address));
            Console.WriteLine();

            if (wifConfiguration.ClaimsAuthenticationManager != null)
            {
                Console.WriteLine("ClaimsAuthenticationManager: {0}", wifConfiguration.ClaimsAuthenticationManager.GetType().Name);
            }

            if (wifConfiguration.ClaimsAuthorizationManager != null)
            {
                Console.WriteLine("ClaimsAuthorizationManager : {0}", wifConfiguration.ClaimsAuthorizationManager.GetType().Name);
            }

            var auth = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            if (auth != null && auth.ServiceAuthorizationManager != null)
            {
                Console.WriteLine("ServiceAuthorizationManager: {0}", auth.ServiceAuthorizationManager.GetType().Name);
            }

            if (wifConfiguration.IssuerNameRegistry != null)
            {
                Console.WriteLine("IssuerNameRegistry         : {0}", wifConfiguration.IssuerNameRegistry.GetType().Name);
            }


            Console.WriteLine("\nSecurityTokenHandlers:");
            foreach (var handler in wifConfiguration.SecurityTokenHandlers)
            {
                Console.WriteLine("  " + handler);
            }
        }

        static void CallProxy(IClaimsService proxy)
        {
            var proxyChannel = proxy as IClientChannel;
            if (proxy != null)
            {
                proxyChannel.Open();
            }

            try
            {
                proxy.ShowClaims();
                proxy.DoSecret();
                proxy.DoTopSecret();

                ((IClientChannel)proxy).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
