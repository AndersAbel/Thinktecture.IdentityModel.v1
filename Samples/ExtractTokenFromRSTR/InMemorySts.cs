using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;

namespace ExtractTokenFromRSTR
{
    class InMemorySts : SecurityTokenService
    {
        X509Certificate2 _encryptionkey;

        public InMemorySts(SecurityTokenServiceConfiguration configuration, X509Certificate2 encryptionKey)
            : base(configuration)
        {
            _encryptionkey = encryptionKey;
        }

        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            return new Scope(request.AppliesTo.Uri.AbsoluteUri, new X509EncryptingCredentials(_encryptionkey));
        }

        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            return principal.Identities.First();
        }
    }

    class InMemoryStsConfiguration : SecurityTokenServiceConfiguration
    {
        public InMemoryStsConfiguration(X509Certificate2 signingCert)
            : base("http://inmemoryissuer/", new X509SigningCredentials(signingCert))
        {
            DefaultTokenType = SecurityTokenTypes.Saml11TokenProfile11;
        }
    }
}
