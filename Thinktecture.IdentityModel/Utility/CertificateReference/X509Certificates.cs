/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Web.Hosting;

namespace Thinktecture.IdentityModel.Utility
{
    /// <summary>
    /// Helper class to retrieve certificates from configuration
    /// </summary>
    public static class X509Certificates
    {
        /// <summary>
        /// Retrieves a named certificate reference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A CertificateReferenceElement containing the certificate reference information</returns>
        internal static CertificateReferenceElement GetReference(string name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<CertificateReferenceElement>() != null);


            CertificateReferenceSection section;

            try
            {
                section = (CertificateReferenceSection)ConfigurationManager.GetSection("certificateReferences");
            }
            catch
            {
                throw new ConfigurationErrorsException("certificateReferences section not found");
            }

            try
            {
                return section.CertificateReferences[name];
            }
            catch
            {
                throw new ConfigurationErrorsException(String.Format("Reference section {0} not found", name));
            }
        }

        /// <summary>
        /// Retrieves a named certificate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 GetCertificate(string name)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);


            return GetCertificate(name, null);
        }

        /// <summary>
        /// Retrieves a named certificate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="password">The password.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 GetCertificate(string name, string password)
        {
            Contract.Requires(!String.IsNullOrEmpty(name));
            Contract.Requires(!String.IsNullOrEmpty(password));
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);


            CertificateReferenceElement certref = GetReference(name);
            bool isHosted = HostingEnvironment.IsHosted;

            if (!string.IsNullOrEmpty(certref.Filename))
            {
                string filename = certref.Filename;
                if (isHosted)
                {
                    filename = string.Format("{0}\\{1}\\{2}",
                        HostingEnvironment.ApplicationPhysicalPath,
                        "App_Data\\certificates",
                        certref.Filename);
                }

                if (!string.IsNullOrEmpty(password))
                {
                    return new X509Certificate2(filename, password);
                }
                else
                {
                    return new X509Certificate2(filename);
                }
            }

            try
            {
                return GetCertificateFromStore(
                    certref.StoreLocation,
                    certref.StoreName,
                    certref.X509FindType,
                    certref.FindValue);
            }
            catch
            {
                throw new ConfigurationErrorsException(String.Format("Certificate for section {0} not found", name));
            }
        }

        /// <summary>
        /// Downloads the SSL certificate from an SSL site.
        /// </summary>
        /// <param name="machineName">Name of the machine.</param>
        /// <param name="port">The port.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 DownloadSslCertificate(string machineName, int port)
        {
            Contract.Requires(!String.IsNullOrEmpty(machineName));
            Contract.Requires(port != 0);
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);


            using (TcpClient client = new TcpClient())
            {
                client.Connect(machineName, port);

                using (SslStream ssl = new SslStream(client.GetStream()))
                {
                    ssl.AuthenticateAsClient(machineName);

                    return new X509Certificate2(ssl.RemoteCertificate);
                }
            }
        }

        /// <summary>
        /// Retrieves a certificate from the certificate store.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="name">The name.</param>
        /// <param name="findType">Type of the find.</param>
        /// <param name="value">The value.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 GetCertificateFromStore(StoreLocation location, StoreName name, X509FindType findType, object value)
        {
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);

            X509Store store = new X509Store(name, location);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                // work around possible bug in framework
                if (findType == X509FindType.FindByThumbprint)
                {
                    var thumbprint = value.ToString();
                    thumbprint = thumbprint.Trim();
                    thumbprint = thumbprint.Replace(" ", "");

                    foreach (var cert in store.Certificates)
                    {
                        if (string.Equals(cert.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(cert.Thumbprint, thumbprint, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return cert;
                        }
                    }
                }
                if (findType == X509FindType.FindBySerialNumber)
                {
                    var serial = value.ToString();
                    serial = serial.Trim();
                    serial = serial.Replace(" ", "");

                    foreach (var cert in store.Certificates)
                    {
                        if (string.Equals(cert.SerialNumber, serial, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(cert.SerialNumber, serial, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return cert;
                        }
                    }
                }

                var certs = store.Certificates.Find(findType, value, true);

                if (certs.Count != 1)
                {
                    throw new InvalidOperationException(String.Format("Certificate not found: {0}", value));
                }

                return certs[0];
            }
            finally
            {
                store.Close();
            }
        }

        //public static X509Certificate2 GetCertificateFromStore(StoreLocation location, StoreName name, X509FindType findType, object value)
        //{
        //    Contract.Requires(value != null);
        //    Contract.Ensures(Contract.Result<X509Certificate2>() != null);

        //    if (findType == X509FindType.FindByThumbprint ||
        //        findType == X509FindType.FindBySerialNumber)
        //    {
        //        value = value.ToString().ToUpperInvariant();
        //    }


        //    X509Store store = new X509Store(name, location);

        //    try
        //    {
        //        store.Open(OpenFlags.ReadOnly);
        //        var certs = store.Certificates.Find(findType, value, true);

        //        if (certs.Count != 1)
        //        {
        //            throw new InvalidOperationException(String.Format("Certificate not found: {0}", value));
        //        }

        //        return certs[0];
        //    }
        //    finally
        //    {
        //        store.Close();
        //    }
        //}

        /// <summary>
        /// Retrieves a certificate from the current user / personal certificate store.
        /// </summary>
        /// <param name="subjectDistinguishedName">The subject distinguished name of the certificate.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 GetCertificateFromStore(string subjectDistinguishedName)
        {
            Contract.Requires(!String.IsNullOrEmpty(subjectDistinguishedName));
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);

            
            return GetCertificateFromStore(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindBySubjectDistinguishedName,
                subjectDistinguishedName);
        }

        /// <summary>
        /// Retrieves a certificate from the specified personal certificate store.
        /// </summary>
        /// <param name="subjectDistinguishedName">The subject distinguished name of the certificate.</param>
        /// <param name="location">The store location.</param>
        /// <returns>A X509Certificate2</returns>
        public static X509Certificate2 GetCertificateFromStore(string subjectDistinguishedName, StoreLocation location)
        {
            Contract.Requires(!String.IsNullOrEmpty(subjectDistinguishedName));
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);


            return GetCertificateFromStore(
                location,
                StoreName.My,
                X509FindType.FindBySubjectDistinguishedName,
                subjectDistinguishedName);
        }
    }
}