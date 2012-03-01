/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;

namespace Thinktecture.IdentityModel.Web.Old
{
    /// <summary>
    /// Specifies requirements for creating or validating a SimpleWebToken.
    /// </summary>
    [Obsolete]
    public class SimpleWebTokenRequirement
    {
        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the signing key.
        /// </summary>
        /// <value>The signing key.</value>
        public string SigningKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebTokenRequirement"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        public SimpleWebTokenRequirement(string issuer, string audience, string signingKey)
        {
            Issuer = issuer;
            Audience = audience;
            SigningKey = signingKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebTokenRequirement"/> class.
        /// </summary>
        public SimpleWebTokenRequirement()
        { }
    }
}
