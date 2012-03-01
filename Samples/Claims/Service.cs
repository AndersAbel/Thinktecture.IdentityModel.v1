using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    [ServiceBehavior(Namespace = "urn:tt", ConfigurationName = "ClaimsService")]
    class ClaimsService : IClaimsService
    {
        public void ShowClaims()
        {
            Console.WriteLine("\n--ShowClaims called");
            ClaimsViewer.ShowConsole(Thread.CurrentPrincipal.AsClaimsPrincipal());
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "Use", Resource = "Secret")]
        public void DoSecret()
        {
            Console.WriteLine("\n--DoSecret called");
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "Use", Resource = "TopSecret")]
        public void DoTopSecret()
        {
            Console.WriteLine("\n--DoTopSecret called");
        }
    }
}
