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

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// A security token provider for the SAML client credentials.
    /// </summary>
    public class SamlSecurityTokenProvider : SecurityTokenProvider
    {
        SecurityToken _samlToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlSecurityTokenProvider"/> class.
        /// </summary>
        /// <param name="samlToken">The saml token.</param>
        internal SamlSecurityTokenProvider(SecurityToken samlToken)
        {
            if (samlToken == null)
            {
                throw new ArgumentNullException("samlToken");
            }

            _samlToken = samlToken;
        }

        /// <summary>
        /// Gets a security token.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that specifies the timeout value for the message that gets the security token.</param>
        /// <returns>
        /// The <see cref="T:System.IdentityModel.Tokens.SecurityToken"/> that represents the security token to get.
        /// </returns>
        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            return _samlToken;
        }
    }
}
