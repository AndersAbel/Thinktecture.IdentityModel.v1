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
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Web.Old
{
    /// <summary>
    /// This class represents a Simple Web Token (SWT)
    /// </summary>
    [Obsolete]
    public class SimpleWebToken
    {
        private const string _issuerLabel = "Issuer";
        private const string _expiresLabel = "ExpiresOn";
        private const string _audienceLabel = "Audience";
        private const string _hmacSHA256Label = "HMACSHA256";

        string _token;
        string _claimPrefix = "http://swt/claims/";
        string _nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        string _roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the signing key.
        /// </summary>
        /// <value>The signing key.</value>
        public byte[] SigningKey { get; set; }

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public Dictionary<string, string> Claims { get; protected set; }

        /// <summary>
        /// Gets or sets the claim prefix.
        /// This prefix is used for the conversion to a ClaimsIdentity when incoming claims don't use the URI format.
        /// </summary>
        /// <value>The claim prefix.</value>
        public string ClaimPrefix
        {
            get { return _claimPrefix; }
            set { _claimPrefix = value; }
        }

        /// <summary>
        /// Gets or sets the type of the name claim.
        /// </summary>
        /// <value>The type of the name claim.</value>
        public string NameClaimType
        {
            get { return _nameClaimType; }
            set { _nameClaimType = value; }
        }

        /// <summary>
        /// Gets or sets the type of the role claim.
        /// </summary>
        /// <value>The type of the role claim.</value>
        public string RoleClaimType
        {
            get { return _roleClaimType; }
            set { _roleClaimType = value; }
        }

        /// <summary>
        /// Converts the SimpleWebToken to a ClaimsIdentity.
        /// </summary>
        /// <value>The claims identity.</value>
        public ClaimsIdentity ClaimsIdentity
        {
            get
            {
                return ToClaimsIdentity();
            }
        }

        #region Statics
        /// <summary>
        /// Retrieves the token from an HTTP header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>The token</returns>
        public static SimpleWebToken GetFromHeader(string header)
        {
            return new SimpleWebToken(GetTokenStringFromHeader(header));
        }

        /// <summary>
        /// Retrieves the token from a query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns>The token.</returns>
        public static SimpleWebToken GetFromQueryString(string queryString)
        {
            return new SimpleWebToken(GetTokenStringFromQueryString(queryString));
        }

        /// <summary>
        /// Retrieves and validates a token from an HTTP header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <returns>The token.</returns>
        public static SimpleWebToken GetAndValidateFromHeader(string header, string issuer, string audience, string signingKey)
        {
            return new SimpleWebToken(
                GetTokenStringFromHeader(header),
                issuer,
                audience,
                signingKey);
        }

        /// <summary>
        /// Retrieves and validates a token from a query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <returns>The token.</returns>
        public static SimpleWebToken GetAndValidateFromQueryString(string queryString, string issuer, string audience, string signingKey)
        {
            return new SimpleWebToken(
                GetTokenStringFromQueryString(queryString),
                issuer,
                audience,
                signingKey);
        }

        /// <summary>
        /// Tries to retrieve and validate a token from an HTTP header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="token">The token.</param>
        /// <returns>True if successful - otherwise false</returns>
        public static bool TryGetAndValidateFromHeader(string header, string issuer, string audience, string signingKey, out SimpleWebToken token)
        {
            token = null;

            try
            {
                token = new SimpleWebToken(
                    GetTokenStringFromHeader(header),
                    issuer,
                    audience,
                    signingKey);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to retrieve and validate a token from an HTTP header.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="token">The token.</param>
        /// <returns>True if successful - otherwise false</returns>
        public static bool TryGetAndValidateFromQueryString(string queryString, string issuer, string audience, string signingKey, out SimpleWebToken token)
        {
            token = null;

            try
            {
                token = new SimpleWebToken(
                    GetTokenStringFromQueryString(queryString),
                    issuer,
                    audience,
                    signingKey);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the token string from an HTTP header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>The token as a string.</returns>
        public static string GetTokenStringFromHeader(string header)
        {
            // check for empty header
            if (string.IsNullOrEmpty(header))
            {
                throw new SecurityTokenException("Authorization header is empty");
            }

            // check for right scheme
            if (!header.StartsWith("WRAP", StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Wrong authorization scheme");
            }

            // only one space allowed
            if (header.Split(' ').Length != 2)
            {
                throw new SecurityTokenException("Malformed header");
            }

            // split header and make sure quotes are in the right place - then strip them
            var token = header.Substring("WRAP access_token=".Length);
            if (token[0] != '\"' && token[token.Length - 1] != '\"')
            {
                throw new SecurityTokenException("Malformed token");
            }

            token = token.Substring(1);
            token = token.Substring(0, token.Length - 1);

            return token;
        }

        /// <summary>
        /// Retrieves the token string from a query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns>The token as a string.</returns>
        public static string GetTokenStringFromQueryString(string queryString)
        {
            queryString = queryString.Substring(1);
            var paramName = "wrap_access_token=";

            var parts = queryString.Split('&');
            var tokenString = (from p in parts
                               where p.StartsWith(paramName, StringComparison.OrdinalIgnoreCase)
                               select p)
                              .FirstOrDefault();

            if (tokenString == null)
            {
                throw new SecurityTokenException("Malformed token");
            }

            tokenString = tokenString.Substring(paramName.Length);
            return Uri.UnescapeDataString(tokenString);
        }

        /// <summary>
        /// Tries to validate a give token string.
        /// </summary>
        /// <param name="tokenString">The token string.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="token">The token.</param>
        /// <returns>True if successful - otherwise false</returns>
        public static bool TryValidate(string tokenString, string issuer, string audience, string signingKey, out SimpleWebToken token)
        {
            token = null;

            try
            {
                token = new SimpleWebToken(tokenString, issuer, audience, signingKey);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebToken"/> class.
        /// </summary>
        /// <param name="token">The token as a string.</param>
        public SimpleWebToken(string token)
        {
            _token = token;
            ParseToken();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebToken"/> class.
        /// </summary>
        /// <param name="token">The token as a string.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        public SimpleWebToken(string token, string issuer, string audience, string signingKey)
            : this(token)
        {
            Issuer = issuer;
            Audience = audience;
            SigningKey = Convert.FromBase64String(signingKey);

            ValidateToken(Issuer, Audience, SigningKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebToken"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        public SimpleWebToken(string issuer, string audience, byte[] signingKey)
            : this(issuer, audience, signingKey, (Dictionary<string, string>)null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebToken"/> class from a ClaimsIdentity.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="identity">The identity.</param>
        public SimpleWebToken(string issuer, string audience, byte[] signingKey, IClaimsIdentity identity)
            : this(issuer, audience, signingKey, ClaimsIdentityToClaims(identity))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebToken"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="signingKey">The signing key.</param>
        /// <param name="claims">The claims.</param>
        public SimpleWebToken(string issuer, string audience, byte[] signingKey, Dictionary<string, string> claims)
        {
            Issuer = issuer;
            Audience = audience;
            SigningKey = signingKey;

            if (claims != null)
            {
                Claims = claims;
            }
            else
            {
                Claims = new Dictionary<string, string>();
            }
        }
        #endregion

        /// <summary>
        /// Adds a claim.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        /// <param name="value">The value.</param>
        public void AddClaim(string claimType, string value)
        {
            AddClaimToDictionary(claimType, value, Claims);
        }

        /// <summary>
        /// Returns the serialized token.
        /// </summary>
        /// <returns>
        /// The serialized token.
        /// </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return _token;
            }
            else
            {
                return CreateToken();
            }
        }

        /// <summary>
        /// Converts the SimpleWebToken to a standard WRAP HTTP header.
        /// </summary>
        /// <returns>The SimpleWebToken to a standard WRAP authorization HTTP header.</returns>
        public string ToAuthorizationHeader()
        {
            return String.Format("WRAP access_token=\"{0}\"", this.ToString());
        }

        /// <summary>
        /// Converts the SimpleWebToken to a standard WRAP query string parameter.
        /// </summary>
        /// <returns></returns>
        public string ToQueryStringParameter()
        {
            return String.Format("wrap_access_token={0}", Uri.EscapeDataString(this.ToString()));
        }

        /// <summary>
        /// Converts the SimpleWebToken to a ClaimsIdentity
        /// </summary>
        /// <returns>A ClaimsIdentity</returns>
        public ClaimsIdentity ToClaimsIdentity()
        {
            return ToClaimsIdentity(ClaimPrefix, NameClaimType, RoleClaimType);
        }

        /// <summary>
        /// Converts the SimpleWebToken to a ClaimsIdentity
        /// </summary>
        /// <param name="claimPrefix">The claim prefix.</param>
        /// <param name="nameClaimType">Type of the name claim.</param>
        /// <param name="roleClaimType">Type of the role claim.</param>
        /// <returns>A ClaimsIdentity</returns>
        public ClaimsIdentity ToClaimsIdentity(string claimPrefix, string nameClaimType, string roleClaimType)
        {
            var claims = new List<Claim>();

            foreach (var item in Claims)
            {
                item.Value.Split(',').ToList().ForEach(v => claims.Add(new Claim(NormalizeClaimType(item.Key), v, ClaimValueTypes.String, Issuer)));
            }

            return new ClaimsIdentity(claims, "WRAP")
            {
                NameClaimType = nameClaimType,
                RoleClaimType = roleClaimType
            };
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="expectedIssuer">The expected issuer.</param>
        /// <param name="expectedAudience">The expected audience.</param>
        /// <param name="expectedSigningKey">The expected signing key.</param>
        public void Validate(string expectedIssuer, string expectedAudience, byte[] expectedSigningKey)
        {
            ValidateToken(expectedIssuer, expectedAudience, expectedSigningKey);
        }

        #region Private
        private string NormalizeClaimType(string claimType)
        {
            if (claimType.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return claimType;
            }
            else
            {
                return ClaimPrefix + claimType;
            }
        }

        private static Dictionary<string, string> ClaimsIdentityToClaims(IClaimsIdentity identity)
        {
            var claims = new Dictionary<string, string>();

            identity.Claims.ToList().ForEach(claim => AddClaimToDictionary(claim.ClaimType, claim.Value, claims));
            return claims;
        }

        private static void AddClaimToDictionary(string claimType, string value, Dictionary<string, string> claims)
        {
            string foundValue;
            if (!claims.TryGetValue(claimType, out foundValue))
            {
                claims.Add(claimType, value);
            }
            else
            {
                claims[claimType] = string.Format("{0},{1}", foundValue, value);
            }
        }

        private string CreateToken()
        {
            // build the claims string
            StringBuilder builder = new StringBuilder(64);
            foreach (KeyValuePair<string, string> entry in Claims)
            {
                builder.Append(entry.Key);
                builder.Append('=');
                builder.Append(entry.Value);
                builder.Append('&');
            }

            // add the issuer name
            builder.Append("Issuer=");
            builder.Append(Issuer);
            builder.Append('&');

            // add the Audience
            builder.Append("Audience=");
            builder.Append(Audience);
            builder.Append('&');

            // add the expires on date
            builder.Append("ExpiresOn=");
            builder.Append(GetExpiresOn(20));

            string signature = GenerateSignature(builder.ToString(), SigningKey);
            builder.Append("&HMACSHA256=");
            builder.Append(signature);

            return builder.ToString();
        }

        private static ulong GetExpiresOn(double minutesFromNow)
        {
            TimeSpan expiresOnTimeSpan = TimeSpan.FromMinutes(minutesFromNow);
            DateTime expiresDate = DateTime.UtcNow + expiresOnTimeSpan;
            TimeSpan ts = expiresDate - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return Convert.ToUInt64(ts.TotalSeconds);
        }

        private string GenerateSignature(string unsignedToken, byte[] signingKey)
        {
            using (HMACSHA256 hmac = new HMACSHA256(signingKey))
            {
                byte[] locallyGeneratedSignatureInBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken));
                string locallyGeneratedSignature = HttpUtility.UrlEncode(Convert.ToBase64String(locallyGeneratedSignatureInBytes));

                return locallyGeneratedSignature;
            }
        }

        private void ParseToken()
        {
            Claims = GetNameValues(_token);
            Issuer = Claims[_issuerLabel];
            Audience = Claims[_audienceLabel];
        }

        private Dictionary<string, string> GetNameValues(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException();
            }

            return
                token
                .Split('&')
                .Aggregate(
                    new Dictionary<string, string>(), (dict, rawNameValue) =>
                    {
                        if (rawNameValue == string.Empty)
                        {
                            return dict;
                        }

                        string[] nameValue = rawNameValue.Split('=');

                        if (nameValue.Length != 2)
                        {
                            throw new ArgumentException("Invalid formEncodedstring - contains a name/value pair missing an = character", "token");
                        }

                        if (dict.ContainsKey(nameValue[0]) == true)
                        {
                            throw new ArgumentException("Repeated name/value pair in form", "token");
                        }

                        dict.Add(HttpUtility.UrlDecode(nameValue[0]), HttpUtility.UrlDecode(nameValue[1]));
                        return dict;
                    });
        }

        private void ValidateToken(string expectedIssuer, string expectedAudience, byte[] exptectedSigningKey)
        {
            if (!IsHMACValid(_token, exptectedSigningKey))
            {
                throw new SecurityTokenException("Signature is invalid");
            }

            if (IsExpired())
            {
                throw new SecurityTokenException("Token is expired");
            }

            if (!IsIssuerTrusted(expectedIssuer))
            {
                throw new SecurityTokenException("Issuer not trusted");
            }

            if (!IsAudienceTrusted(expectedAudience))
            {
                throw new SecurityTokenException("Invalid audience");
            }
        }

        private bool IsAudienceTrusted(string expectedAudience)
        {
            string audienceValue;
            Claims.TryGetValue(_audienceLabel, out audienceValue);

            if (!string.IsNullOrEmpty(audienceValue))
            {
                return string.Equals(audienceValue, expectedAudience, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool IsIssuerTrusted(string expectedIssuer)
        {
            string issuerName;
            Claims.TryGetValue(_issuerLabel, out issuerName);

            if (!string.IsNullOrEmpty(issuerName))
            {
                return string.Equals(issuerName, expectedIssuer, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool IsHMACValid(string swt, byte[] sha256HMACKey)
        {
            string[] swtWithSignature = swt.Split(new string[] { String.Format("&{0}=", _hmacSHA256Label) }, StringSplitOptions.None);

            if ((swtWithSignature == null) || (swtWithSignature.Length != 2))
            {
                return false;
            }

            using (HMACSHA256 hmac = new HMACSHA256(sha256HMACKey))
            {
                byte[] locallyGeneratedSignatureInBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(swtWithSignature[0]));
                string locallyGeneratedSignature = HttpUtility.UrlEncode(Convert.ToBase64String(locallyGeneratedSignatureInBytes));

                return ObfuscatingComparer.IsEqual(locallyGeneratedSignature, swtWithSignature[1]);
            }
        }

        private bool IsExpired()
        {
            try
            {
                string expiresOnValue = Claims[_expiresLabel];
                ulong expiresOn = Convert.ToUInt64(expiresOnValue);
                ulong currentTime = Convert.ToUInt64(GenerateTimeStamp());

                if (currentTime > expiresOn)
                {
                    return true;
                }

                return false;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException();
            }
        }

        private static ulong GenerateTimeStamp()
        {
            // Default implementation of epoch time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(ts.TotalSeconds);
        }
#endregion
    }
}