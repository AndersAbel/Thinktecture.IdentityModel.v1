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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    ///     Security token handler that adds a compression aspect to the token handler pipeline.
    ///     Similar to the encryption aspect of EncryptedSecurityTokenHandler.
    /// </summary>
    public class CompressedSecurityTokenHandler : SecurityTokenHandler
    {
        /// <summary>
        /// Gets the token type identifiers.
        /// </summary>
        /// <returns>A list of token typed identifiers this handler can process</returns>
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { CompressedSecurityToken.TokenTypeIdentifier };
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token this token handler can process.</value>
        public override Type TokenType
        {
            get { return typeof(CompressedSecurityToken); }
        }

        /// <summary>
        /// Determines whether this instance can read a CompressedSecurityToken.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can read a CompressedSecurityToken; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanReadToken(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return reader.IsStartElement(CompressedSecurityToken.TokenName, CompressedSecurityToken.TokenTypeIdentifier);
        }

        /// <summary>
        /// Gets a value indicating whether this instance can write a token.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can write a token; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWriteToken
        {
            get { return true; }
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


            var compressedToken = token as CompressedSecurityToken;
            if (compressedToken == null)
            {
                throw new ArgumentException("token");
            }


            var sb = new StringBuilder();

            using (var innerWriter = new XmlTextWriter(new StringWriter(sb)))
            {
                var handler = ContainingCollection[compressedToken.Token.GetType()];
                if (handler == null)
                {
                    throw new SecurityTokenException(String.Format("No token handler found for: {0}", compressedToken.Token.GetType().Name));
                }

                handler.WriteToken(innerWriter, compressedToken.Token);
            }

            var compressedInnerToken = Compress(sb.ToString());

            writer.WriteElementString(
                CompressedSecurityToken.TokenName,
                CompressedSecurityToken.TokenTypeIdentifier,
                compressedInnerToken);
        }

        /// <summary>
        /// Reads the token.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The uncompressed token</returns>
        public override SecurityToken ReadToken(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var compressedTokenString = reader.ReadElementContentAsString();
            var uncompressedTokenString = Decompress(compressedTokenString);

            using (var uncompressedReader = new XmlTextReader(new StringReader(uncompressedTokenString)))
            {
                if (ContainingCollection == null)
                {
                    throw new SecurityTokenException("No containing token handlers configured");
                }

                foreach (SecurityTokenHandler handler in base.ContainingCollection)
                {
                    if (handler.CanReadToken(uncompressedReader))
                    {
                        return handler.ReadToken(uncompressedReader);
                    }
                }

                throw new SecurityTokenException(String.Format("New token handler found for: {0}", uncompressedReader.LocalName));
            }
        }

        /// <summary>
        /// Compresses the specified string.
        /// </summary>
        /// <param name="input">A string.</param>
        /// <returns>A compressed string</returns>
        protected virtual string Compress(string input)
        {
            Contract.Requires(!String.IsNullOrEmpty(input));
            Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));


            byte[] buffer = Encoding.UTF8.GetBytes(input);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                ms.Position = 0;
                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
                byte[] gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                return Convert.ToBase64String(gzBuffer);
            }
        }

        /// <summary>
        /// Decompresses the specified compressed string.
        /// </summary>
        /// <param name="input">The compressed string.</param>
        /// <returns>The uncompressed string</returns>
        protected virtual string Decompress(string input)
        {
            Contract.Requires(!String.IsNullOrEmpty(input));
            Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));


            byte[] gzBuffer = Convert.FromBase64String(input);

            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
                byte[] buffer = new byte[msgLength];
                ms.Position = 0;

                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
