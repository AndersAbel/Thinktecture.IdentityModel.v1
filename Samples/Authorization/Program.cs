using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Thinktecture.IdentityModel.Claims;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    class Program
    {
        private const string CustomResourceType = "http://my/custom/type";
        private const string CustomActionType = "http://my/custom/type";

        static void Main(string[] args)
        {
            SetPrincipal();

            Imperative();
            ImperativeUsingPermission();
            StandardTypeAttribute();
            CustomTypeAttribute();
        }

        private static void ImperativeUsingPermission()
        {
            ClaimPermission.CheckAccess(
                "ImperativeAction", 
                "ImperativeResource", 
                new Claim("http://additionalClaim", "AdditionalResource"));
        }

        [ClaimPermission(SecurityAction.Demand,
            Operation = "StandardAction",
            Resource = "StandardResource")]
        private static void StandardTypeAttribute()
        {
            "\nStandardTypeAttribute\n".ConsoleGreen();
        }

        [ClaimPermission(SecurityAction.Demand,
            OperationType = CustomActionType,
            Operation = "CustomAction",
            ResourceType = CustomResourceType,
            Resource = "CustomResource")]
        private static void CustomTypeAttribute()
        {
            "\nCustomTypeAttribute\n".ConsoleGreen();
        }


        private static void Imperative()
        {
            ClaimsAuthorization.CheckAccess("ImperativeResource", "ImperativeAction");

            "\nImperativeStrings\n".ConsoleGreen();
        }

        private static void SetPrincipal()
        {
            var idPclaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Alice", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Email, "alice@thinktecture.com", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Users", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Marketing", ClaimValueTypes.String, "IdP"),
                new Claim(ClaimTypes.Role, "Sales", ClaimValueTypes.String, "IdP"),
                new Claim("http://www.thinktecture.com/claims/location", "Heidelberg", ClaimValueTypes.String, "IdP")
            };

            var authZclaims = new List<Claim>
            {
                new Claim(WSAuthorizationConstants.Action, "AddCustomer", ClaimValueTypes.String, "RSTS"),
                new Claim(WSAuthorizationConstants.Action, "ChangeCustomer", ClaimValueTypes.String, "RSTS")
            };

            var idp = new ClaimsIdentity(idPclaims);
            var authZ = new ClaimsIdentity(authZclaims);

            var principal = new ClaimsPrincipal(new List<IClaimsIdentity> { idp, authZ });

            Thread.CurrentPrincipal = principal;
        }
    }
}
