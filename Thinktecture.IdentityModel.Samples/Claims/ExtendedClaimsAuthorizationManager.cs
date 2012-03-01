using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Claims
{
    /// <summary>
    /// Claims authorization manager with support for message inspection and custom principals.
    /// </summary>
    public class ExtendedClaimsAuthorizationManager : ClaimsAuthorizationManager
    {
        /// <summary>
        /// Creates a custom principal.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>A custom IClaimsPrincipal or null</returns>
        public virtual IClaimsPrincipal GetCustomPrincipal(OperationContext operationContext, IClaimsPrincipal principal)
        {
            return null;
        }

        /// <summary>
        /// Inspects the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>The original or modified message</returns>
        public virtual bool CheckMessage(ref Message message, OperationContext operationContext, IClaimsPrincipal principal)
        {
            return true;
        }
    }
}
