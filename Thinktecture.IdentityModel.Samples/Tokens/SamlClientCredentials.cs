/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Description;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// A specialized WCF client credentials implementation that allows direct setting of SAML tokens.
    /// </summary>
    public class SamlClientCredentials : ClientCredentials
    {
        /// <summary>
        /// Gets or sets the SAML token.
        /// </summary>
        /// <value>The saml token.</value>
        public SecurityToken SamlToken { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlClientCredentials"/> class.
        /// </summary>
        public SamlClientCredentials() : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamlClientCredentials"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public SamlClientCredentials(SamlClientCredentials other)
            : base(other)
        {
            SamlToken = other.SamlToken;
        }

        /// <summary>
        /// Creates a security token manager for this instance. This method is rarely called explicitly; it is primarily used in extensibility scenarios and is called by the system itself.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ClientCredentialsSecurityTokenManager"/> for this <see cref="T:System.ServiceModel.Description.ClientCredentials"/> instance.
        /// </returns>
        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new SamlSecurityTokenManager(this);
        }

        /// <summary>
        /// Creates a new copy of this <see cref="T:System.ServiceModel.Description.ClientCredentials"/> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.Description.ClientCredentials"/> instance.
        /// </returns>
        protected override ClientCredentials CloneCore()
        {
            return new SamlClientCredentials(this);
        }
    }
}
