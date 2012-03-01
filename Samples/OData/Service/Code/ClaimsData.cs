using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.Samples
{
    public class ClaimsData
    {
        public IQueryable<ViewClaim> Claims
        {
            get { return GetClaims().AsQueryable(); }
        }

        private List<ViewClaim> GetClaims()
        {
            var claims = new List<ViewClaim>();
            var identity = Thread.CurrentPrincipal.Identity as IClaimsIdentity;

            int id = 0;
            identity.Claims.ToList().ForEach(claim =>
                {
                    claims.Add(new ViewClaim
                    {
                       Id = ++id,
                       ClaimType = claim.ClaimType,
                       Value = claim.Value,
                       Issuer = claim.Issuer
                    });
                });

            return claims;
        }
    }
}
