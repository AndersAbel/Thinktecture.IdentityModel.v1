using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Thinktecture.IdentityModel.Claims;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            SetPrincipal();

            GetClaimValues();
            FindClaims();
            DemandClaims();
        }

        private static void GetClaimValues()
        {
            var principal = Thread.CurrentPrincipal.AsClaimsPrincipal();

            // search over all identities
            Console.WriteLine("Name: {0}", principal.GetClaimValue(WSIdentityConstants.ClaimTypes.Name));
            Console.WriteLine("1st Action Claim: {0}", principal.GetClaimValue(WSAuthorizationConstants.Action));

            // use a specific identity
            Console.WriteLine("Name: {0}", principal.First().GetClaimValue(WSIdentityConstants.ClaimTypes.Name));

            // GetClaimValue throws an exception when the specified claim is not found
            try
            {
                var claim = principal.GetClaimValue("http://nonexistent");
            }
            catch (ClaimNotFoundException)
            {
                Console.WriteLine("Claim not found (catch block)");
            }

            // TryGetClaimValue is more defensive
            string value = "";
            if (principal.TryGetClaimValue("http://nonexistent", out value))
            {
                Console.WriteLine("Claim found: {0}", value);
            }
            else
            {
                Console.WriteLine("Claim not found (if/else block)");
            }
        }

        private static void DemandClaims()
        {
            var principal = Thread.CurrentPrincipal.AsClaimsPrincipal();

            try
            {
                principal.DemandClaim(ClaimTypes.Role, "Sales");
                Console.WriteLine("\nMember of Sales role");

                principal.DemandClaim(WSAuthorizationConstants.Action, "DeleteCustomer");
                Console.WriteLine("Allowed to delete a Customer");
            }
            catch (SecurityException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void FindClaims()
        {
            var principal = Thread.CurrentPrincipal.AsClaimsPrincipal();

            Console.WriteLine("\nAll Role claims");
            var claims = principal.FindClaims(ClaimTypes.Role);
            ShowClaims(claims);

            Console.WriteLine("\nAll Action claims");
            claims = principal.FindClaims(WSAuthorizationConstants.Action);
            ShowClaims(claims);

            Console.WriteLine("\nAll Claims from Issuer: Idp");
            claims = principal.FindClaims(claim => claim.Issuer == "IdP");
            ShowClaims(claims);
        }

        private static void ShowClaims(IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                Console.WriteLine("{0}:\n {1}", claim.ClaimType, claim.Value);
            }
        }

        private static void SetPrincipal()
        {
            var idPclaims = new List<Claim>
            {
                new Claim(WSIdentityConstants.ClaimTypes.Name, "Alice", ClaimValueTypes.String, "IdP"),
                new Claim(WSIdentityConstants.ClaimTypes.Email, "alice@thinktecture.com", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Users", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Marketing", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Sales", ClaimValueTypes.String, "IdP"),
                new Claim("http://www.thinktecture.com/claims/location", "Heidelberg", ClaimValueTypes.String, "IdP")
            };

            var authZclaims = new List<Claim>
            {
                new Claim(WSAuthorizationConstants.Action, "AddCustomer", ClaimValueTypes.String, "RSTS"),
                new Claim(WSAuthorizationConstants.Action, "ChangeCustomer", ClaimValueTypes.String, "RSTS")
            };

            var idp = new ClaimsIdentity(idPclaims);
            var authZ = new ClaimsIdentity(authZclaims);

            var principal = new ClaimsPrincipal(new List<IClaimsIdentity> { idp, authZ });

            Thread.CurrentPrincipal = principal;
        }
    }
}
