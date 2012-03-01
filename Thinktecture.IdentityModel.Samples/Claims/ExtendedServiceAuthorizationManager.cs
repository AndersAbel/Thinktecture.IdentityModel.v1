using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Claims
{
    /// <summary>
    /// This class extends the WIF service authorization manager to support the <see cref="T:Thinktecture.IdentityModel.Claims.ExtendedClaimsAuthorizationManager" />
    /// </summary>
    public class ExtendedServiceAuthorizationManager : IdentityModelServiceAuthorizationManager
    {
        /// <summary>
        /// Checks authorization for the given operation context when access to a message is required.
        /// </summary>
        /// <param name="operationContext">The <see cref="T:System.ServiceModel.OperationContext"/>.</param>
        /// <param name="message">The <see cref="T:System.ServiceModel.Channels.Message"/> to be examined to determine authorization.</param>
        /// <returns>
        /// true if access is granted; otherwise; otherwise false. The default is false.
        /// </returns>
        public override bool CheckAccess(OperationContext operationContext, ref Message message)
        {
            // set ServiceSecurityContext 
            SetSecurityContext(operationContext);

            // grab ICP
            var principal = GetPrincipal(operationContext);

            // create custom ICP
            var customPrincipal = GetCustomPrincipal(operationContext, principal);
            if (customPrincipal != null)
            {
                SetPrincipal(operationContext, customPrincipal);
                principal = customPrincipal;
            }

            // optionally inspect message
            bool result = CheckMessage(ref message, operationContext, principal);
            if (result == false)
            {
                return false;
            }

            // call authZ Manager
            return CheckClaimsAuthorizationManager(operationContext);
        }

        #region Implementation
        /// <summary>
        ///     This method checks if a ExtendedClaimsAuthorizationManager is registered.
        ///     If yes, it gets called to get a chance to inspect or modify message content.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>false to deny access, true to proceed with the (modified) message</returns>
        protected virtual bool CheckMessage(ref Message message, OperationContext operationContext, IClaimsPrincipal principal)
        {
            var federatedServiceCredentials = GetFederatedServiceCredentials();
            if (federatedServiceCredentials == null)
            {
                return true;
            }

            var manager = federatedServiceCredentials.ClaimsAuthorizationManager as ExtendedClaimsAuthorizationManager;
            if (manager != null)
            {
                return manager.CheckMessage(ref message, operationContext, principal);
            }

            return true;
        }

        /// <summary>
        ///     This method checks if a ExtendedClaimsAuthorizationManager is registered.
        ///     If yes it gets called for an opportunity to supply a custom IClaimsPrincipal implementation
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>A custom IClaimsPrincipal, or null.</returns>
        protected virtual IClaimsPrincipal GetCustomPrincipal(OperationContext operationContext, IClaimsPrincipal principal)
        {
            var federatedServiceCredentials = GetFederatedServiceCredentials();
            if (federatedServiceCredentials == null)
            {
                return null;
            }

            var manager = federatedServiceCredentials.ClaimsAuthorizationManager as ExtendedClaimsAuthorizationManager;
            if (manager != null)
            {
                return manager.GetCustomPrincipal(operationContext, principal);
            }

            return null;
        }
        
        private void SetSecurityContext(OperationContext operationContext)
        {
            var empty = new List<IAuthorizationPolicy>().AsReadOnly();

            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = this.GetAuthorizationPolicies(operationContext);
            operationContext.IncomingMessageProperties.Security.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies ?? empty);
        }

        private IClaimsPrincipal GetPrincipal(OperationContext operationContext)
        {
            var properties = operationContext.ServiceSecurityContext.AuthorizationContext.Properties;
            return properties["Principal"] as IClaimsPrincipal;
        }

        private void SetPrincipal(OperationContext operationContext, IClaimsPrincipal principal)
        {
            var properties = operationContext.ServiceSecurityContext.AuthorizationContext.Properties;
            properties["Principal"] = principal;
        }

        private bool CheckClaimsAuthorizationManager(OperationContext operationContext)
        {
            if (operationContext == null)
            {
                return false;
            }

            string action = operationContext.IncomingMessageHeaders.Action;
            Uri to = operationContext.IncomingMessageHeaders.To;
            FederatedServiceCredentials federatedServiceCredentials = GetFederatedServiceCredentials();

            if (((federatedServiceCredentials == null) || string.IsNullOrEmpty(action)) || (to == null))
            {
                return false;
            }

            IClaimsPrincipal subject = 
                operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as IClaimsPrincipal;

            return (((subject != null) && (subject.Identities != null)) && federatedServiceCredentials.ClaimsAuthorizationManager.CheckAccess(
                new Microsoft.IdentityModel.Claims.AuthorizationContext(subject, to.AbsoluteUri, action)));
        }

        private FederatedServiceCredentials GetFederatedServiceCredentials()
        {
            ServiceCredentials credentials;

            if (OperationContext.Current != null && OperationContext.Current.Host != null && OperationContext.Current.Host.Description != null && OperationContext.Current.Host.Description.Behaviors != null)
            {
                credentials = OperationContext.Current.Host.Description.Behaviors.Find<ServiceCredentials>();
            }
            else
            {
                credentials = null;
            }

            FederatedServiceCredentials credentials2 = credentials as FederatedServiceCredentials;
            if (credentials2 == null)
            {
                throw new Exception();
            }

            return credentials2;

        }
        #endregion
    }
}
