/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Protocols.WSTrust;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// Simple security token for access scenarios with expiration.
    /// </summary>
    public class AccessSecurityToken : SecurityToken
    {
        /// <summary>
        /// Token namespace and identifier
        /// </summary>
        public const string TokenTypeIdentifier = "http://www.thinktecture.com/tokens/accesstoken";

        /// <summary>
        /// Name of the token (and XML element name)
        /// </summary>
        public const string TokenName = "AccessToken";

        string _subjectName;
        string _id;
        DateTime? _validFrom;
        DateTime? _validTo;
        string _resource;
        X509Certificate2 _issuerCertificate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessSecurityToken"/> class.
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="resource">The resource identifier.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="issuerCertificate">The issuer certificate.</param>
        public AccessSecurityToken(string subjectName, string resource, Lifetime lifetime, X509Certificate2 issuerCertificate)
        {
            Contract.Requires(!String.IsNullOrEmpty(subjectName));
            Contract.Requires(!String.IsNullOrEmpty(resource));
            Contract.Requires(lifetime != null);
            Contract.Requires(lifetime.Created != null);
            Contract.Requires(lifetime.Expires != null);
            Contract.Requires(issuerCertificate != null);


            _subjectName = subjectName;
            _resource = resource;
            _issuerCertificate = issuerCertificate;
            
            _id = UniqueId.CreateRandomUri();
            _validFrom = lifetime.Created;
            _validTo = lifetime.Expires;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessSecurityToken"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="resource">The resource identifier.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="issuerCertificate">The issuer certificate.</param>
        public AccessSecurityToken(string id, string subjectName, string resource, DateTime expires, X509Certificate2 issuerCertificate)
        {
            Contract.Requires(!String.IsNullOrEmpty(id));
            Contract.Requires(!String.IsNullOrEmpty(subjectName));
            Contract.Requires(!String.IsNullOrEmpty(resource));
            Contract.Requires(expires != null);
            Contract.Requires(issuerCertificate != null);


            _id = id;
            _subjectName = subjectName;
            _resource = resource;
            _validFrom = DateTime.Now;
            _validTo = expires;
            _issuerCertificate = issuerCertificate;
        }

        /// <summary>
        /// Gets a unique identifier of the security token.
        /// </summary>
        /// <value></value>
        /// <returns>The unique identifier of the security token.</returns>
        public override string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the cryptographic keys associated with the security token.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1"/> of type <see cref="T:System.IdentityModel.Tokens.SecurityKey"/> that contains the set of keys associated with the security token.</returns>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return EmptyReadOnlyCollection<SecurityKey>.Instance; }
        }

        /// <summary>
        /// Gets the first instant in time at which this security token is valid.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.DateTime"/> that represents the instant in time at which this security token is first valid.</returns>
        public override DateTime ValidFrom
        {
            get { return _validFrom.Value; }
        }

        /// <summary>
        /// Gets the last instant in time at which this security token is valid.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.DateTime"/> that represents the last instant in time at which this security token is valid.</returns>
        public override DateTime ValidTo
        {
            get { return _validTo.Value; }
        }

        /// <summary>
        /// Gets the name of the subject.
        /// </summary>
        /// <value>The name of the subject.</value>
        public string SubjectName
        {
            get { return _subjectName; }
        }

        /// <summary>
        /// Gets the issuer certificate.
        /// </summary>
        /// <value>The issuer certificate.</value>
        public X509Certificate2 IssuerCertificate
        {
            get { return _issuerCertificate; }
        }

        /// <summary>
        /// Gets the resource identifier.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource
        {
            get { return _resource; }
        }
    }
}
