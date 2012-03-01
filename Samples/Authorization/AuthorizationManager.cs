using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            "AuthorizationManager.CheckAccess\n".ConsoleYellow();

            context.Principal.Identity.Name.ConsoleYellow();

            ShowClaimsCollection(context.Resource, "Resource(s)");
            ShowClaimsCollection(context.Action, "Action(s)");

            return true;
        }

        private void ShowClaimsCollection(Collection<Claim> collection, string heading)
        {
            ("\n" + heading + "\n").ConsoleRed();

            foreach (var claim in collection)
            {
                var output = string.Format("{0}\n {1}", claim.ClaimType, claim.Value);
                output.ConsoleRed();
            }
        }
    }
}
