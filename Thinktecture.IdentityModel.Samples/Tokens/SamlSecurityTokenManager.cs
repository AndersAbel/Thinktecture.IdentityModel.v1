/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// Security token manager for the SAML client credentials
    /// </summary>
    public class SamlSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        SamlClientCredentials _credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlSecurityTokenManager"/> class.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        public SamlSecurityTokenManager(SamlClientCredentials credentials)
            : base(credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }

            _credentials = credentials;
        }

        /// <summary>
        /// Creates a security token provider.
        /// </summary>
        /// <param name="tokenRequirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement"/>.</param>
        /// <returns>
        /// The <see cref="T:System.IdentityModel.Selectors.SecurityTokenProvider"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="tokenRequirement"/> is null.</exception>
        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement.TokenType == SecurityTokenTypes.Saml ||
                tokenRequirement.TokenType == "http://docs.oasis-open.org/wss/oasis-wss-saml-issuedToken-profile-1.1#SAMLV1.1" ||
                tokenRequirement.TokenType == "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1")
            {
                return new SamlSecurityTokenProvider(_credentials.SamlToken);
            }

            return base.CreateSecurityTokenProvider(tokenRequirement);
        }
    }
}