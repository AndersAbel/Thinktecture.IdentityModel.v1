/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// SecurityTokenHandler for AccessSecurityToken
    /// </summary>
    public class AccessSecurityTokenHandler : SecurityTokenHandler
    {
        XNamespace _ns = XNamespace.Get(AccessSecurityToken.TokenTypeIdentifier);

        /// <summary>
        /// Gets the token type identifiers.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { AccessSecurityToken.TokenTypeIdentifier };
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public override Type TokenType
        {
            get { return typeof(AccessSecurityToken); }
        }

        /// <summary>
        /// Determines whether this instance can read a AccessSecurityToken.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can read a AccessSecurityToken; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanReadToken(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return reader.IsStartElement(AccessSecurityToken.TokenName, AccessSecurityToken.TokenTypeIdentifier);
        }

        /// <summary>
        /// Gets a value indicating whether this instance can write a ntoken.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can write a token; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWriteToken
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can validate a token.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can validate a token; otherwise, <c>false</c>.
        /// </value>
        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="tokenDescriptor">The token descriptor.</param>
        /// <returns></returns>
        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            if (tokenDescriptor == null)
            {
                throw new ArgumentNullException("tokenDescriptor");
            }
            //Contract.Ensures(Contract.Result<SecurityToken>() != null);
            //Contract.EndContractBlock();
            

            AccessSecurityToken token = new AccessSecurityToken(
                tokenDescriptor.Subject.Name, 
                tokenDescriptor.AppliesToAddress, 
                tokenDescriptor.Lifetime, 
                ((X509SigningCredentials)tokenDescriptor.SigningCredentials).Certificate);

            return token;
        }

        /// <summary>
        /// Reads the token.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        /// <returns></returns>
        public override SecurityToken ReadToken(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("tokenDescriptor");
            }
            //Contract.Ensures(Contract.Result<SecurityToken>() != null);
            //Contract.EndContractBlock();

            var xml = XElement.ReadFrom(reader) as XElement;
            var issuerCertificate = VerifySignature(xml);

            var token = ReadTokenValues(xml, issuerCertificate);
            return token;
        }

        /// <summary>
        /// Writes the token.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="token">The token.</param>
        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            //Contract.EndContractBlock();

            var accToken = token as AccessSecurityToken;
            if (accToken == null)
            {
                throw new ArgumentException("token");
            }
            
            // create the token payload XML
            var xml = new XElement(_ns + AccessSecurityToken.TokenName, new XAttribute("id", accToken.Id),
                new XElement(_ns + "resource", accToken.Resource),
                new XElement(_ns + "subject", accToken.SubjectName),
                new XElement(_ns + "expires", accToken.ValidTo.ToString(DateTimeFormats.Generated, CultureInfo.InvariantCulture)));
 
            // create the digital signature of the token XML and append it to the token
            var sig = SignToken(xml, accToken.IssuerCertificate);
            xml.Add(sig);

            writer.WriteNode(XmlReader.Create(new StringReader(xml.ToString())), false);
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A collection with one identity.</returns>
        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            
            //Contract.Ensures(Contract.Result<ClaimsIdentityCollection>() != null);
            //Contract.EndContractBlock();

            var accToken = token as AccessSecurityToken;
            if (accToken == null)
            {
                throw new ArgumentException("token");
            }

            CheckExpiration(accToken);
            string issuer = ValidateIssuer(accToken.IssuerCertificate);
            return CreateClaims(accToken, issuer);
        }

        /// <summary>
        /// Checks the expiration.
        /// </summary>
        /// <param name="accToken">The acc token.</param>
        private void CheckExpiration(AccessSecurityToken accToken)
        {
            Contract.Requires(accToken != null);
            //Contract.Requires(accToken.ValidTo != null);


            if (accToken.ValidTo < DateTime.UtcNow)
            {
                throw new SecurityTokenValidationException("Token has expired");
            }
        }

        /// <summary>
        /// Creates the claims.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="issuer">The issuer.</param>
        /// <returns></returns>
        protected virtual ClaimsIdentityCollection CreateClaims(AccessSecurityToken token, string issuer)
        {
            Contract.Requires(token != null);
            Contract.Requires(!String.IsNullOrEmpty(token.SubjectName));
            Contract.Requires(!String.IsNullOrEmpty(token.Resource));
            //Contract.Requires(token.ValidTo != null);
            Contract.Ensures(Contract.Result<ClaimsIdentityCollection>() != null);
            

            var claims = new List<Claim>
            {
                new Claim(WSIdentityConstants.ClaimTypes.Name, token.SubjectName, ClaimValueTypes.String, issuer),
                new Claim(WSIdentityConstants.ClaimTypes.Uri, token.Resource, ClaimValueTypes.String, issuer),
                new Claim("http://www.thinktecture.com/claims/expires", XmlConvert.ToString(token.ValidTo, DateTimeFormats.Generated), ClaimValueTypes.Datetime, issuer)
            };

            return new ClaimsIdentityCollection(new List<IClaimsIdentity> { new ClaimsIdentity(claims) });
        }

        private string ValidateIssuer(X509Certificate2 issuerCertificate)
        {
            Contract.Requires(issuerCertificate != null);
            Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));


            if ((Configuration == null) || (Configuration.IssuerNameRegistry == null))
            {
                throw new SecurityTokenException("No IssuerNameRegistry configured");
            }

            string issuer = Configuration.IssuerNameRegistry.GetIssuerName(new X509SecurityToken(issuerCertificate));
            if (string.IsNullOrEmpty(issuer))
            {
                throw new SecurityTokenException("No issuer name found");
            }

            return issuer;
        }

        /// <summary>
        /// Reads the token values.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="issuerCertificate">The issuer certificate.</param>
        /// <returns></returns>
        protected virtual AccessSecurityToken ReadTokenValues(XElement xml, X509Certificate2 issuerCertificate)
        {
            Contract.Requires(xml != null);
            Contract.Requires(issuerCertificate != null);
            Contract.Ensures(Contract.Result<AccessSecurityToken>() != null);


            var id = xml.Attribute("id").Value;
            var subject = xml.Element(_ns + "subject").Value;
            var resource = xml.Element(_ns + "resource").Value;
            var expires = DateTime.ParseExact(xml.Element(_ns + "expires").Value, DateTimeFormats.Accepted, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();

            if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(subject) && !String.IsNullOrEmpty(resource) && expires != null)
            {
                return new AccessSecurityToken(id, subject, resource, expires, issuerCertificate);
            }
            else
            {
                throw new SecurityTokenException("Missing values in token");
            }
        }

        /// <summary>
        /// Verifies the signature.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>The issuer certificate</returns>
        protected virtual X509Certificate2 VerifySignature(XElement xml)
        {
            Contract.Requires(xml != null);
            Contract.Ensures(Contract.Result<X509Certificate2>() != null);

            if ((Configuration == null) || (Configuration.IssuerTokenResolver == null))
            {
                throw new SecurityTokenException("No issuer token resolver configured");
            }

            var xmlElement = xml.ToXmlElement();
            var signedXml = new SignedXml(xmlElement);
            
            // find signature
            XmlNodeList nodeList = xmlElement.GetElementsByTagName("Signature");

            // throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // throw an exception if more than one signature was found.
            if (nodeList.Count > 1)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // load the <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // resolve the issuer certificate
            byte[] thumbprint = Convert.FromBase64String(GetIssuerThumbprint(signedXml));
            var identifier = new X509ThumbprintKeyIdentifierClause(thumbprint);
            var issuerKey = Configuration.IssuerTokenResolver.ResolveToken(identifier) as X509SecurityToken;

            // check the signature
            if (!signedXml.CheckSignature(issuerKey.Certificate, true))
            {
                throw new CryptographicException("Signature verification failed");
            }

            if (issuerKey.Certificate != null)
            {
                return issuerKey.Certificate;
            }
            else
            {
                throw new CryptographicException("No issuer certificate found");
            }
        }

        /// <summary>
        /// Gets the issuer thumbprint.
        /// </summary>
        /// <param name="signedXml">The signed XML.</param>
        /// <returns>The thumbprint of the issuer certificate</returns>
        protected virtual string GetIssuerThumbprint(SignedXml signedXml)
        {
            Contract.Requires(signedXml != null);
            Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));


            var keyName = signedXml.Signature.KeyInfo.OfType<KeyInfoName>().FirstOrDefault();
            
            if (keyName == null)
            {
                throw new CryptographicException("No KeyName found");
            }

            return keyName.Value;
        }

        /// <summary>
        /// Signs the token.
        /// </summary>
        /// <param name="tokenXml">The token XML.</param>
        /// <param name="signer">The signer.</param>
        /// <returns>The signed token XML</returns>
        protected virtual XElement SignToken(XElement tokenXml, X509Certificate2 signer)
        {
            Contract.Requires(tokenXml != null);
            Contract.Requires(signer != null);
            Contract.Ensures(Contract.Result<XElement>() != null);

            // create SignedXml instance and set signer key
            var signedXml = new SignedXml(tokenXml.ToXmlElement());
            signedXml.SigningKey = signer.PrivateKey;

            // add an enveloped transformation to the reference.
            Reference reference = new Reference { Uri = "" };            
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            // add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // add a key info to the SignedXml object
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoName(Convert.ToBase64String(signer.GetCertHash())));
            signedXml.KeyInfo = keyInfo;

            // compute the signature.
            signedXml.ComputeSignature();

            // get the XML representation of the signature
            return signedXml.GetXml().ToXElement();
        }
    }
}
