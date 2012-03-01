using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.IdentityModel.Claims;

namespace Service
{
    public class Service : IService
    {
        public List<ViewClaim> GetClaims()
        {
            var identity = Thread.CurrentPrincipal.Identity as IClaimsIdentity;

            return (from c in identity.Claims
                    select new ViewClaim
                    {
                        ClaimType = c.ClaimType,
                        Value = c.Value,
                        Issuer = c.Issuer
                    })
                   .ToList();
        }
    }
}
