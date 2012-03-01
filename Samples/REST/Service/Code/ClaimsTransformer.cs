using System;
using System.Linq;
using Microsoft.IdentityModel.Claims;

namespace Service
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal.Identity.IsAuthenticated)
            {
                incomingPrincipal.Identities.First().Claims.Add(new Claim("http://claims/localtest", DateTime.Now.ToLongTimeString()));
            }

            return incomingPrincipal;
        }
    }
}
