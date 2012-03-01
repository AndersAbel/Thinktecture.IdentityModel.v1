using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Claims;

namespace Thinktecture.Samples
{
    class ClaimsAuthzManager : ExtendedClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            Console.WriteLine("\n--ClaimsAuthorizationManager.CheckAccess");

            string action = context.Action[0].Value;
            string to = context.Resource[0].Value;
            string client = context.Principal.Identity.Name;

            Console.WriteLine("\n----Action   : {0}", action);
            Console.WriteLine("----Resource : {0}", to);

            return true;
        }

        public override bool CheckMessage(ref Message message, OperationContext operationContext, IClaimsPrincipal principal)
        {
            Console.WriteLine("\n--ClaimsAuthorizationManager.CheckMessage");
            string msg = message.ToString();
            return true;
        }

        public override IClaimsPrincipal GetCustomPrincipal(OperationContext operationContext, IClaimsPrincipal principal)
        {
            Console.WriteLine("\n--ClaimsAuthorizationManager.GetCustomPrincipal");
            return new CustomClaimsPrincipal(principal, "some custom value");
        }
    }
}
