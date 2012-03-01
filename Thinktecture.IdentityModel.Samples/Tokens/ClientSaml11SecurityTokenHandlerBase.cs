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
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// Base class for a security token handler for client generated SAML tokens.
    /// </summary>
    public abstract class ClientSaml11SecurityTokenHandlerBase : Saml11SecurityTokenHandler
    {
        /// <summary>
        /// // extensibility point for authentication and claims filtering.
        /// </summary>
        /// <param name="id">The incoming identity.</param>
        /// <param name="newIdentity">The application identity.</param>
        /// <returns>Returns true when user validation succeeded - otherwise false.</returns>
        protected abstract bool ValidateUser(ClaimsIdentity id, out IClaimsIdentity newIdentity);

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A collection of ClaimsIdentity that represents the identity in the security token.</returns>
        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }        

            var samlToken = token as SamlSecurityToken;
            if (samlToken == null)
            {
                throw new ArgumentException("token");
            }
            if (samlToken.Assertion == null)
            {
                throw new ArgumentException("token");
            }
            
            var assertion = samlToken.Assertion as Saml11Assertion;
            this.ValidateConditions(samlToken.Assertion.Conditions, false);

            // extract claims from token
            var identity = new ClaimsIdentity("ClientSaml");
            ProcessStatement(assertion.Statements, identity, "Client");
            
            // call authentication and filtering logic
            IClaimsIdentity newIdentity;

            try
            {
                if (ValidateUser(identity, out newIdentity))
                {
                    return new ClaimsIdentityCollection(new IClaimsIdentity[] { newIdentity });
                }
                else
                {
                    throw new SecurityTokenValidationException("Authentication failed");
                }
            }
            catch (Exception ex)
            {
                throw new SecurityTokenValidationException("Security token validation failed", ex);
            }
        }

        /// <summary>
        /// Creates a bearer SAML security token from an IClaimsIdentity
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>A self-generated SAML bearer token</returns>
        public static SamlSecurityToken CreateToken(IClaimsIdentity identity)
        {
            var description = new SecurityTokenDescriptor
            {
                Subject = identity,
                TokenIssuerName = "http://self"
            };

            var handler = new Saml11SecurityTokenHandler();
            return (SamlSecurityToken)handler.CreateToken(description);
        }
    }
}
