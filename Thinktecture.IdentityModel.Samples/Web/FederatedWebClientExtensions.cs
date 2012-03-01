/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Data.Services.Client;
using System.Net;
using System.Text;

namespace Thinktecture.IdentityModel.Web.Old
{
    /// <summary>
    /// Extension methods to set tokens on a HTTP header
    /// </summary>
    [Obsolete]
    public static class FederatedWebClientExtensions
    {
        /// <summary>
        /// Sets the access token on a WebClient.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        public static void SetAccessToken(this WebClient client, string token, string type)
        {
            client.SetAccessToken(token, type, "Authorization");
        }

        /// <summary>
        /// Sets a basic authentication header on a WebClient.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">The User name.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthenticationHeader(this WebClient client, string userName, string password)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string credential = String.Format("{0}:{1}", userName, password);

            var encoded = Convert.ToBase64String(encoding.GetBytes(credential));

            client.Headers[HttpRequestHeader.Authorization] = String.Format("Basic {0}", encoded);
        }

        /// <summary>
        /// Sets the access token on a WebClient.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        /// <param name="headerName">Name of the header.</param>
        public static void SetAccessToken(this WebClient client, string token, string type, string headerName)
        {
            client.Headers[headerName] = GetHeader(token, type);
        }

        /// <summary>
        /// Sets the access token on a HttpWebRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        /// <param name="headerName">Name of the header.</param>
        public static void SetAccessToken(this HttpWebRequest request, string token, string type, string headerName)
        {
            request.Headers[headerName] = GetHeader(token, type);
        }

        /// <summary>
        /// Sets the access token on a HttpWebRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        public static void SetAccessToken(this HttpWebRequest request, string token, string type)
        {
            request.SetAccessToken(token, type, "Authorization");
        }

        /// <summary>
        /// Sets the access token on a WCF Data Services client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        public static void SetAccessToken(this DataServiceContext context, string token, string type)
        {
            context.SetAccessToken(token, type, "Authorization");
        }

        /// <summary>
        /// Sets the access token on a WCF Data Services client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        /// <param name="type">The type.</param>
        /// <param name="headerName">Name of the header.</param>
        public static void SetAccessToken(this DataServiceContext context, string token, string type, string headerName)
        {
            context.SendingRequest += (s, e) =>
            {
                e.RequestHeaders[headerName] = GetHeader(token, type);
            };
        }

        private static string GetHeader(string token, string type)
        {
            if (type == WebClientTokenSchemes.SWT)
            {
                return String.Format("{0} access_token=\"{1}\"", type, token);
            }
            else
            {
                return String.Format("{0} {1}", type, token);
            }
        }
    }
}
