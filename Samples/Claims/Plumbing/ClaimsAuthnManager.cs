using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.Samples
{
    class ClaimsAuthnManager : ClaimsAuthenticationManager
    {
        public override IClaimsPrincipal Authenticate(string endpointUri, IClaimsPrincipal incomingPrincipal)
        {
            Console.WriteLine("\n--ClaimsAuthenticationManager.Authenticate");

            string issuer = "ClaimsAuthenticationManager";
            var identity = incomingPrincipal.First();
            
            ClaimsIdentity id = new ClaimsIdentity(new List<Claim>
            {
                new Claim(WSIdentityConstants.ClaimTypes.Name, 
                    identity.Name, 
                    ClaimValueTypes.String, 
                    issuer),
                new Claim("http://www.thinktecture.com/claims/clearance", 
                    "Secret", 
                    ClaimValueTypes.String, 
                    issuer),
                identity.FindClaims(ClaimTypes.AuthenticationMethod).First(),
                identity.FindClaims(ClaimTypes.AuthenticationInstant).First()
            });

            return new ClaimsPrincipal(new List<IClaimsIdentity> { id });
        }
    }
}
