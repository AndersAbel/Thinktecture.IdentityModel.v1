using Microsoft.IdentityModel.Claims;

namespace OData
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        // use this method to check requests/operations against your authorization policy
        public override bool CheckAccess(AuthorizationContext context)
        {
            return base.CheckAccess(context);
        }
    }
}
