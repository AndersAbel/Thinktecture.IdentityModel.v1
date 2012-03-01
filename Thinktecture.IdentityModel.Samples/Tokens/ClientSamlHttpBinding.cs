/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Tokens
{
    /// <summary>
    /// Custom binding for client generated bearer SAML tokens
    /// </summary>
    public class ClientSamlHttpBinding : CustomBinding
    {
        SecurityMode _mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSamlHttpBinding"/> class.
        /// </summary>
        /// <param name="mode">Either TransportWithMessageCredential or Message.</param>
        public ClientSamlHttpBinding(SecurityMode mode)
        {
            _mode = mode;
        }

        /// <summary>
        /// Gets the URI scheme for transport used by the custom binding.
        /// </summary>
        /// <value></value>
        /// <returns>The URI scheme for transport used by the custom binding; or an empty string if there is no transport (<see cref="T:System.ServiceModel.Channels.TransportBindingElement"/> is null).</returns>
        public override string Scheme
        {
            get
            {
                if (_mode == SecurityMode.TransportWithMessageCredential)
                {
                    return "https";
                }
                else if (_mode == SecurityMode.Message)
                {
                    return "http";
                }

                throw new InvalidOperationException("Only Message or TransportWithMessageCredential allowed");
            }
        }

        /// <summary>
        /// Returns a generic collection of the binding elements from the custom binding.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> object of type <see cref="T:System.ServiceModel.Channels.BindingElement"/> that contains the binding elements from the custom binding.
        /// </returns>
        public override BindingElementCollection CreateBindingElements()
        {
            if (_mode == SecurityMode.Message)
            {
                return new BindingElementCollection
                {
                    CreateClientSamlForCertificateBindingElement(),
                    new TextMessageEncodingBindingElement(),
                    new HttpTransportBindingElement()
                };
            }
            else
            {
                return new BindingElementCollection
                {
                    CreateClientSamlOverTransportBindingElement(),
                    new TextMessageEncodingBindingElement(),
                    new HttpsTransportBindingElement()
                };
            }
        }

        /// <summary>
        /// Creates a Message security version of the binding.
        /// </summary>
        /// <returns></returns>
        public static SecurityBindingElement CreateClientSamlForCertificateBindingElement()
        {
            // protection token
            var element = new SymmetricSecurityBindingElement(
                new X509SecurityTokenParameters(
                    X509KeyIdentifierClauseType.Thumbprint,
                    SecurityTokenInclusionMode.Never));

            // client token
            element.EndpointSupportingTokenParameters.SignedEncrypted.Add(CreateParameters());
            
            element.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            return element;
        }

        /// <summary>
        /// Creates a mixed mode (TransportWithMessageCredentials) version of the binding.
        /// </summary>
        /// <returns></returns>
        public static SecurityBindingElement CreateClientSamlOverTransportBindingElement()
        {
            return SecurityBindingElement.CreateIssuedTokenOverTransportBindingElement(CreateParameters());
        }

        private static IssuedSecurityTokenParameters CreateParameters()
        {
            var parameters = new IssuedSecurityTokenParameters(
                 SecurityTokenTypes.OasisWssSaml11TokenProfile11,
                 new EndpointAddress("http://self"),
                 new BasicHttpBinding());
            
            parameters.KeyType = System.IdentityModel.Tokens.SecurityKeyType.BearerKey;
            parameters.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;

            return parameters;
        }
    }
}
