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
    /// Represents a collection of certificate references
    /// </summary>
    [ConfigurationCollection(typeof(CertificateReferenceElement))]
    public class CertificateReferenceCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CertificateReferenceElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            CertificateReferenceElement reference = (CertificateReferenceElement)element;
            return reference.Name;
        }

        /// <summary>
        /// Gets or sets the <see cref="Thinktecture.IdentityModel.Utility.CertificateReferenceElement"/> at the specified index.
        /// </summary>
        /// <value>A certificate reference</value>
        public CertificateReferenceElement this[int index]
        {
            get
            {
                return (CertificateReferenceElement)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="Thinktecture.IdentityModel.Utility.CertificateReferenceElement"/> with the specified name.
        /// </summary>
        /// <value>A certificate reference</value>
        public CertificateReferenceElement this[string name]
        {
            get
            {
                return (CertificateReferenceElement)base.BaseGet(name);
            }
        }

    }
}
