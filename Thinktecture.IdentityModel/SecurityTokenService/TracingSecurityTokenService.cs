/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using WIF = Microsoft.IdentityModel.SecurityTokenService;

namespace Thinktecture.IdentityModel.SecurityTokenService
{
    /// <summary>
    /// A SecurityTokenService class with hooks for logging the RST, RSTR and issued token
    /// </summary>
    public abstract class TracingSecurityTokenService : WIF.SecurityTokenService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TracingSecurityTokenService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public TracingSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration)
        { }

        /// <summary>
        /// Logging extensibility point.
        /// </summary>
        /// <param name="rst">The RST.</param>
        /// <param name="rstr">The RSTR.</param>
        /// <param name="token">The token.</param>
        protected virtual void OnTrace(XElement rst, XElement rstr, XElement token) { }

        /// <summary>
        /// Creates the token response and invokes the logging callbacks.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="tokenDescriptor">The token descriptor.</param>
        /// <returns>A RequestSecurityTokenResponse</returns>
        protected override RequestSecurityTokenResponse GetResponse(RequestSecurityToken request, SecurityTokenDescriptor tokenDescriptor)
        {
            var response = base.GetResponse(request, tokenDescriptor);

            // see if token is encrypted
            EncryptedSecurityToken encryptedToken = tokenDescriptor.Token as EncryptedSecurityToken;
            SecurityToken token;

            if (encryptedToken != null)
            {
                // if so, use inner token
                token = encryptedToken.Token;
            }
            else
            {
                // if not, use the token directly
                token = tokenDescriptor.Token;
            }

            var sb = new StringBuilder(128);
            FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.WriteToken(XmlWriter.Create(new StringWriter(sb)), token);

            try
            {
                // do logging callback
                OnTrace(
                    XElement.Parse(SerializeRequest(request)),
                    XElement.Parse(SerializeResponse(response)),
                    XElement.Parse(sb.ToString()));
            }
            catch
            { }

            return response;
        }

        private string SerializeRequest(RequestSecurityToken request)
        {
            var serializer = new WSTrust13RequestSerializer();
            WSTrustSerializationContext context = new WSTrustSerializationContext();
            StringBuilder sb = new StringBuilder(128);

            using (var writer = new XmlTextWriter(new StringWriter(sb)))
            {
                serializer.WriteXml(request, writer, context);
                return sb.ToString();
            }
        }

        private string SerializeResponse(RequestSecurityTokenResponse response)
        {
            var serializer = new WSTrust13ResponseSerializer();
            WSTrustSerializationContext context = new WSTrustSerializationContext();
            StringBuilder sb = new StringBuilder(128);

            using (var writer = new XmlTextWriter(new StringWriter(sb)))
            {
                serializer.WriteXml(response, writer, context);
                return sb.ToString();
            }
        }
    }
}
