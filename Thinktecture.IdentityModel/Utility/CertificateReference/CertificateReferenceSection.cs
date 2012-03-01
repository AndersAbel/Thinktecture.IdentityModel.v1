/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Configuration;

namespace Thinktecture.IdentityModel.Utility
{
    /// <summary>
    /// Custom configuration section for certificate references
    /// </summary>
    public class CertificateReferenceSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the certificate references.
        /// </summary>
        /// <value>The certificate references.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public CertificateReferenceCollection CertificateReferences
        {
            get
            {
                return (CertificateReferenceCollection)base[""];
            }
        }
    }
}
