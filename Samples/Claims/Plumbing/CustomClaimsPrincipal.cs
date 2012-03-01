using Microsoft.IdentityModel.Claims;

namespace Thinktecture.Samples
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        public CustomClaimsPrincipal(IClaimsPrincipal principal, string customValue)
            : base(principal)
        {
            CustomPropery = customValue;
        }

        public string CustomPropery { get; set; }
    }
}
