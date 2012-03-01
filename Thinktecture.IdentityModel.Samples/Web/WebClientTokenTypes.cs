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
    /// Holds constants for the federated web token plumbing
    /// </summary>
    [Obsolete]
    public static class WebClientTokenSchemes
    {
        /// <summary>
        /// SAML token
        /// </summary>
        public const string SAML = "SAML";

        /// <summary>
        /// SimpleWebToken over WRAP
        /// </summary>
        public const string SWT = "WRAP";

        /// <summary>
        /// SimpleWebToken over OAuth 2
        /// </summary>
        public const string OAuth = "OAuth";
    }
}
