using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Utility;

namespace Thinktecture.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new AccessSecurityTokenHandler() { Configuration = new SecurityTokenHandlerConfiguration() };
            ConfigureHandler(handler.Configuration);

            var token = CreateToken(handler);
            var tokenString = WriteToken(handler, token);

            Console.WriteLine(tokenString.ToString());
            Console.WriteLine("\nFull Length      : {0}", tokenString.Length);
            Console.WriteLine("Compressed Length: {0}\n", Compress(tokenString.ToString()).Length);

            var readToken = ReadToken(handler, tokenString.ToString());

            var identities = handler.ValidateToken(readToken);
            ClaimsViewer.ShowConsole(new ClaimsPrincipal(identities));
            
            TestHandlerCollection(tokenString);
            TestMalformedTokens();
            TestCompressedToken(token);
        }

        private static SecurityToken CreateToken(AccessSecurityTokenHandler handler)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = "http://tecteacher.thinktecture.com/videos/1",
                Lifetime = new Lifetime(DateTime.Now, DateTime.Now.AddMinutes(60)),
                SigningCredentials = GetSigningCredential(),
                TokenType = AccessSecurityToken.TokenTypeIdentifier,
                Subject = new ClaimsIdentity(new List<Claim> 
                    {
                        new Claim(WSIdentityConstants.ClaimTypes.Name, "bob")
                    })
            };

            var token = handler.CreateToken(descriptor);
            return token;
        }

        private static StringBuilder WriteToken(AccessSecurityTokenHandler handler, SecurityToken token)
        {
            var sb = new StringBuilder();
            using (var writer = new XmlTextWriter(new StringWriter(sb)))
            {
                handler.WriteToken(writer, token);
            }

            return sb;
        }

        private static SecurityToken ReadToken(AccessSecurityTokenHandler handler, string tokenString)
        {
            using (var reader = tokenString.AsXmlReader(true))
            {
                reader.MoveToContent();
                var readToken = handler.ReadToken(reader);
                return readToken;
            }
        }

        private static void ConfigureHandler(SecurityTokenHandlerConfiguration configuration)
        {
            var issuerTokens = new List<SecurityToken> { new X509SecurityToken(GetSigningCertificate()) }.AsReadOnly();
            configuration.IssuerTokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(
                issuerTokens, false);

            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer(GetSigningCertificate().Thumbprint, "TecTeacher");
            configuration.IssuerNameRegistry = registry;
        }

        private static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
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

        private static void TestCompressedToken(SecurityToken token)
        {
            var handlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            handlers.Add(new AccessSecurityTokenHandler());
            handlers.Add(new CompressedSecurityTokenHandler());

            ConfigureHandler(handlers.Configuration);

            var compressedToken = new CompressedSecurityToken(token);
            var sb = new StringBuilder();
            using (var writer = new XmlTextWriter(new StringWriter(sb)))
            {
                handlers.WriteToken(writer, compressedToken);
            }

            SecurityToken readToken;
            using (var reader = sb.ToString().AsXmlReader(true))
            {
                readToken = handlers.ReadToken(reader);
            }

            handlers.ValidateToken(readToken);
        }


        private static void TestMalformedTokens()
        {
            var expiredToken = File.ReadAllText("ExpiredToken.xml");
            var tamperedToken = File.ReadAllText("TamperedToken.xml");

            var handler = new AccessSecurityTokenHandler() { Configuration = new SecurityTokenHandlerConfiguration() };
            ConfigureHandler(handler.Configuration);

            try
            {
                using (var reader = expiredToken.AsXmlReader(true))
                {
                    Console.WriteLine("\nReading expired token");

                    var readToken = handler.ReadToken(reader);
                    handler.ValidateToken(readToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                using (var reader = tamperedToken.AsXmlReader(true))
                {
                    Console.WriteLine("\nReading tampered token");

                    var readToken = handler.ReadToken(reader);
                    handler.ValidateToken(readToken);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void TestHandlerCollection(StringBuilder tokenString)
        {
            var handlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            handlers.Add(new AccessSecurityTokenHandler());
            ConfigureHandler(handlers.Configuration);

            using (var reader = new XmlTextReader(new StringReader(tokenString.ToString())))
            {
                reader.MoveToContent();
                var readToken = handlers.ReadToken(reader);
                handlers.ValidateToken(readToken);
            }
        }

        private static SigningCredentials GetSigningCredential()
        {
            return new X509SigningCredentials(GetSigningCertificate());
        }

        private static X509Certificate2 GetSigningCertificate()
        {
            return X509Certificates.GetCertificateFromStore("CN=Service");
        }
    }
}
