/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// This class represents a compressed security token
    /// </summary>
    public class CompressedSecurityToken : SecurityToken
    {
        SecurityToken _token;

        /// <summary>
        /// Token namespace and identifier
        /// </summary>
        public const string TokenTypeIdentifier = "http://www.thinktecture.com/tokens/compressedtoken";

        /// <summary>
        /// Name of the token (and XML element name)
        /// </summary>
        public const string TokenName = "CompressedToken";

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressedSecurityToken"/> class.
        /// </summary>
        /// <param name="token">The token to be compressed.</param>
        public CompressedSecurityToken(SecurityToken token)
        {
            Contract.Requires(token != null);


            _token = token;
        }

        /// <summary>
        /// Gets a unique identifier of the security token. Not implemented.
        /// </summary>
        /// <value></value>
        /// <returns>The unique identifier of the security token.</returns>
        public override string Id
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the cryptographic keys associated with the security token. Not implemented.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1"/> of type <see cref="T:System.IdentityModel.Tokens.SecurityKey"/> that contains the set of keys associated with the security token.</returns>
        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the first instant in time at which this security token is valid. Not implemented.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.DateTime"/> that represents the instant in time at which this security token is first valid.</returns>
        public override DateTime ValidFrom
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the last instant in time at which this security token is valid. Not implemented.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.DateTime"/> that represents the last instant in time at which this security token is valid.</returns>
        public override DateTime ValidTo
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the token to be compressed.
        /// </summary>
        /// <value>The token.</value>
        public SecurityToken Token
        {
            get { return _token; }
        }
    }
}
