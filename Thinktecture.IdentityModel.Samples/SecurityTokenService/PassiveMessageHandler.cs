using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Security;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityModel.SecurityTokenService
{
    /// <summary>
    /// Helper class for WS-Federation message handling
    /// </summary>
    public static class PassiveMessageHandler
    {
        /// <summary>
        /// Processes a WS-Federation request.
        /// </summary>
        /// <param name="request">The WS-Federation request message.</param>
        /// <param name="principal">The client principal.</param>
        /// <param name="configuration">The token service configuration.</param>
        public static void ProcessRequest(WSFederationMessage request, IClaimsPrincipal principal, SecurityTokenServiceConfiguration configuration)
        {
            Contract.Requires(request != null);
            Contract.Requires(principal != null);
            Contract.Requires(configuration != null);


            HttpContext context = HttpContext.Current;

            if (request.Action == WSFederationConstants.Actions.SignIn)
            {
                var response = ProcessSignInRequest(
                    (SignInRequestMessage)request,
                    principal,
                    configuration);

                response.Write(context.Response.Output);
                context.Response.Flush();
                context.Response.End();
            }
            else if (request.Action == WSFederationConstants.Actions.SignOut)
            {
                var signOut = (SignOutRequestMessage)request;
                ProcessSignOutRequest();

                if (!String.IsNullOrEmpty(signOut.Reply))
                {
                    context.Response.Redirect(signOut.Reply);
                }
                else
                {
                    context.Response.Redirect("~/");
                }
            }
            else if (request.Action == WSFederationConstants.Actions.SignOutCleanup)
            {
                var signOut = (SignOutCleanupRequestMessage)request;
                ProcessSignOutRequest();

                if (!String.IsNullOrEmpty(signOut.Reply))
                {
                    context.Response.Redirect(signOut.Reply);
                }
                else
                {
                    context.Response.Redirect("~/");
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(
                             CultureInfo.InvariantCulture,
                             "Unsupported Action: {0}", request.Action));
            }
        }

        /// <summary>
        /// Processes a WS-Federation sign in request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="principal">The client principal.</param>
        /// <param name="configuration">The token service configuration.</param>
        /// <returns>A SignInResponseMessage</returns>
        public static SignInResponseMessage ProcessSignInRequest(SignInRequestMessage request, IClaimsPrincipal principal, SecurityTokenServiceConfiguration configuration)
        {
            Contract.Requires(request != null);
            Contract.Requires(principal != null);
            Contract.Requires(configuration != null);
            Contract.Ensures(Contract.Result<SignInResponseMessage>() != null);
            

            // create token service and serializers
            var sts = configuration.CreateSecurityTokenService();
            var context = new WSTrustSerializationContext(
                sts.SecurityTokenServiceConfiguration.SecurityTokenHandlerCollectionManager,
                sts.SecurityTokenServiceConfiguration.ServiceTokenResolver,
                sts.SecurityTokenServiceConfiguration.IssuerTokenResolver);
            var federationSerializer = new WSFederationSerializer(
                sts.SecurityTokenServiceConfiguration.WSTrust13RequestSerializer,
                sts.SecurityTokenServiceConfiguration.WSTrust13ResponseSerializer);

            // convert ws-fed message to RST and call issue pipeline
            var rst = federationSerializer.CreateRequest(request, context);
            var rstr = sts.Issue(principal, rst);

            // check ReplyTo
            Uri result = null;
            if (!Uri.TryCreate(rstr.ReplyTo, UriKind.Absolute, out result))
            {
                throw new InvalidOperationException("Invalid ReplyTo");
            }

            var response = new SignInResponseMessage(result, rstr, federationSerializer, context);

            // copy the incoming context data (as required by the WS-Federation spec)
            if (!String.IsNullOrEmpty(request.Context))
            {
                response.Context = request.Context;
            }

            return response;
        }

        /// <summary>
        /// Processes a WS-Federation sign out request. 
        /// This deletes all local FormsAuthentication and SessionAuthentication cookies.
        /// </summary>
        public static void ProcessSignOutRequest()
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                try
                {
                    FormsAuthentication.SignOut();
                }
                finally
                {
                    var sessionAuthenticationModule = FederatedAuthentication.SessionAuthenticationModule;
                    if (sessionAuthenticationModule != null)
                    {
                        sessionAuthenticationModule.DeleteSessionTokenCookie();
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to create a federation message from the current URI.
        /// </summary>
        /// <returns>A WSFederationMessage created from the current URI</returns>
        public static WSFederationMessage GetFederationMessage()
        {
            WSFederationMessage requestMessage;
            if (WSFederationMessage.TryCreateFromUri(HttpContext.Current.Request.Url, out requestMessage))
            {
                return requestMessage;
            }
            else
            {
                throw new InvalidRequestException("Invalid request");
            }
        }
    }
}
