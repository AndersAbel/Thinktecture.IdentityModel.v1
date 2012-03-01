using System.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Thinktecture.IdentityModel.Tokens;

namespace Thinktecture.Samples
{
    class ClientSaml11SecurityTokenHandler : ClientSaml11SecurityTokenHandlerBase
    {
        static string _passwordClaimType = "http://www.identity.thinktecture.com/claims/password";
        static string _customerIdClaimType = "http://www.identity.thinktecture.com/claims/customerId";

        // sample implementation - do not use for production ;)
        protected override bool ValidateUser(ClaimsIdentity id, out IClaimsIdentity newIdentity)
        {
            newIdentity = null;
            var usernameClaim = id.Claims.First(c => c.ClaimType == WSIdentityConstants.ClaimTypes.Name);
            var passwordClaim = id.Claims.First(c => c.ClaimType == _passwordClaimType);
            var customerIdClaim = id.Claims.First(c => c.ClaimType == _customerIdClaimType);

            if (usernameClaim.Value == passwordClaim.Value)
            {
                newIdentity = new ClaimsIdentity(new Claim[] 
                {
                    usernameClaim,
                    customerIdClaim
                }, "ClientSaml");

                return true;
            }

            return false;
        }
    }
}
