/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Thinktecture.IdentityModel.Utility
{
    /// <summary>
    /// Represents a certificate reference
    /// </summary>
    public class CertificateReferenceElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        [ConfigurationProperty("filename")]
        public string Filename
        {
            get { return (string)base["filename"]; }
            set { base["filename"] = value; }
        }

        /// <summary>
        /// Gets or sets the find value.
        /// </summary>
        /// <value>The find value.</value>
        [ConfigurationProperty("findValue")]
        public string FindValue
        {
            get { return (string)base["findValue"]; }
            set { base["findValue"] = value; }
        }

        /// <summary>
        /// Gets or sets the search type.
        /// </summary>
        /// <value>The type of search.</value>
        [ConfigurationProperty("x509FindType")]
        public X509FindType X509FindType
        {
            get { return (X509FindType)base["x509FindType"]; }
            set { base["x509FindType"] = value; }
        }

        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        /// <value>The store location.</value>
        [ConfigurationProperty("storeLocation")]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)base["storeLocation"]; }
            set { base["storeLocation"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        /// <value>The name of the store.</value>
        [ConfigurationProperty("storeName")]
        public StoreName StoreName
        {
            get { return (StoreName)base["storeName"]; }
            set { base["storeName"] = value; }
        }
    }
}
