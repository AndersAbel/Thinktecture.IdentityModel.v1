/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.IdentityModel.Tokens;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;

namespace Thinktecture.IdentityModel.SecurityTokenService
{
    /// <summary>
    /// Base class for a simple security token service
    /// </summary>
    public abstract class SimpleRestfulTokenServiceBase : ISimpleRestfulTokenServiceContract
    {
        /// <summary>
        /// Retrieves the signing credentials.
        /// </summary>
        /// <returns>The signing credential</returns>
        protected abstract SigningCredentials GetSigningCredentials();
        
        /// <summary>
        /// Creates the output claims identity.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="username">The username.</param>
        /// <returns>An IClaimsIdentity describing the requesting entity</returns>
        protected abstract IClaimsIdentity GetOutputClaimsIdentity(string realm, string username);

        /// <summary>
        /// Retrieves the encrypting credentials.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>The encrypting credential</returns>
        protected abstract EncryptingCredentials GetEncryptingCredentials(string realm);
        
        /// <summary>
        /// Retrieves the name of the token issuer.
        /// </summary>
        /// <returns>The issuer name as an URI</returns>
        protected abstract Uri GetIssuerName();

        /// <summary>
        /// Issues a token for the specified realm.
        /// </summary>
        /// <param name="realm">The realm name.</param>
        /// <returns>A SecurityToken as XElement</returns>
        public XElement Issue(string realm)
        {
            if (String.IsNullOrEmpty(realm))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (ServiceSecurityContext.Current == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }

            var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            var handler = new Saml11SecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = realm,
                Lifetime = GetLifetime(realm, username),
                TokenIssuerName = GetIssuerName().AbsoluteUri,
                EncryptingCredentials = GetEncryptingCredentials(realm),
                SigningCredentials = GetSigningCredentials(),
                Subject = GetOutputClaimsIdentity(realm, username)
            };

            var token = handler.CreateToken(descriptor);
            
            StringBuilder sb = new StringBuilder();
            var writer = XmlWriter.Create(sb);
            handler.WriteToken(writer, token);

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
            return XElement.Parse(sb.ToString());
        }

        /// <summary>
        /// Gets the lifetime.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="userName">The username.</param>
        /// <returns>The life time</returns>
        protected virtual Lifetime GetLifetime(string realm, string userName)
        {
            return new Lifetime(DateTime.Now, DateTime.Now.AddHours(8));
        }
    }
}
