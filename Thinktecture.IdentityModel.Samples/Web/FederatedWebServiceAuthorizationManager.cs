/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityModel.Web.Old
{
    /// <summary>
    /// This class enables WIF and token support in WebServiceHost derived classes.
    /// </summary>
    [Obsolete]
    public class FederatedWebServiceAuthorizationManager : ServiceAuthorizationManager
    {
        SimpleWebTokenRequirement _swtRequirement;
        ServiceConfiguration _configuration = FederatedAuthentication.ServiceConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FederatedWebServiceAuthorizationManager"/> class.
        /// </summary>
        public FederatedWebServiceAuthorizationManager()
        {
            TryLoadSimpleWebTokenRequirement();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FederatedWebServiceAuthorizationManager"/> class.
        /// </summary>
        /// <param name="requirement">The validation requirements for a SWT token.</param>
        public FederatedWebServiceAuthorizationManager(SimpleWebTokenRequirement requirement)
        {
            _swtRequirement = requirement;
        }

        /// <summary>
        /// Checks authorization for the given operation context based on default policy evaluation.
        /// </summary>
        /// <param name="operationContext">The <see cref="T:System.ServiceModel.OperationContext"/> for the current authorization request.</param>
        /// <returns>
        /// true if access is granted; otherwise, false. The default is true.
        /// </returns>
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var properties = operationContext.ServiceSecurityContext.AuthorizationContext.Properties;
            var to = operationContext.IncomingMessageHeaders.To.AbsoluteUri;

            IClaimsIdentity identity;
            if (TryGetIdentity(out identity))
            {
                // set the IClaimsPrincipal
                var principal = _configuration.ClaimsAuthenticationManager.Authenticate(
                    to, ClaimsPrincipal.CreateFromIdentity(identity));
                properties["Principal"] = principal;

                return CallClaimsAuthorization(principal, operationContext);
            }
            else
            {
                SetUnauthorizedResponse();
                return false;
            }
        }

        private bool CallClaimsAuthorization(IClaimsPrincipal principal, OperationContext operationContext)
        {
            string action = string.Empty;
        
            var property = operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            if (property != null)
            {
                action = property.Method;
            }
        
            Uri to = operationContext.IncomingMessageHeaders.To;

            if (to == null || string.IsNullOrEmpty(action))
            {
                return false;
            }

            return FederatedAuthentication.ServiceConfiguration.ClaimsAuthorizationManager.CheckAccess(
                new AuthorizationContext(principal, to.AbsoluteUri, action));
        }

        /// <summary>
        /// Tries to retrieve the clients ClaimsIdentity from the current request context.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>True when a valid identity was found - otherwise false.</returns>
        protected virtual bool TryGetIdentity(out IClaimsIdentity identity)
        {
            identity = null;

            // check header first - authorization and x-authorization
            var headers = WebOperationContext.Current.IncomingRequest.Headers;
            if (headers != null)
            {
                var authZheader = headers[HttpRequestHeader.Authorization] ?? headers["X-Authorization"];
                if (!string.IsNullOrEmpty(authZheader))
                {
                    try
                    {
                        if (authZheader.StartsWith(WebClientTokenSchemes.SAML, StringComparison.OrdinalIgnoreCase))
                        {
                            identity = GetSamlIdentityFromHeader(authZheader);
                        }
                        else
                        {
                            identity = SimpleWebToken.GetAndValidateFromHeader(
                                authZheader,
                                _swtRequirement.Issuer,
                                _swtRequirement.Audience,
                                _swtRequirement.SigningKey).ClaimsIdentity;
                        }
                        
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            // then check query string
            if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null)
            {
                var queryString = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query;
                if (!string.IsNullOrEmpty(queryString))
                {
                    try
                    {
                        identity = SimpleWebToken.GetAndValidateFromQueryString(
                            queryString,
                            _swtRequirement.Issuer,
                            _swtRequirement.Audience,
                            _swtRequirement.SigningKey).ClaimsIdentity;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private IClaimsIdentity GetSamlIdentityFromHeader(string header)
        {
            //var token = header.Substring("SAML ".Length);

            var token = header.Substring(WebClientTokenSchemes.SAML.Length + 1);

            var samlToken = _configuration.SecurityTokenHandlers.ReadToken(new XmlTextReader(new StringReader(token)));
            return _configuration.SecurityTokenHandlers.ValidateToken(samlToken).First();
        }

        private static void SetUnauthorizedResponse()
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
            WebOperationContext.Current.OutgoingResponse.Headers[HttpResponseHeader.WwwAuthenticate] = "IssuedToken";
        }
        
        private void TryLoadSimpleWebTokenRequirement()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SwtIssuer"]) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SwtAudience"]) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SwtSigningKey"]))
            {
                _swtRequirement = new SimpleWebTokenRequirement(
                    ConfigurationManager.AppSettings["SwtIssuer"],
                    ConfigurationManager.AppSettings["SwtAudience"],
                    ConfigurationManager.AppSettings["SwtSigningKey"]);
            }
        }
    }
}
